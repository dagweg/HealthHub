using System.Text.Json;
using System.Text.Json.Nodes;
using ChapaNET;
using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces.Payments;
using HealthHub.Source.Models.Interfaces.Payments.Chapa;
using HealthHub.Source.Services.PaymentProviders;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xunit.Sdk;

public class ChapaPaymentProvider(AppConfig appConfig, ILogger<ChapaPaymentProvider> logger)
  : IPaymentProvider
{
  public PaymentProvider PaymentProvider => PaymentProvider.Chapa;

  public Task<decimal> CheckBalanceAsync(string email)
  {
    return Task.FromResult(decimal.MaxValue);
  }

  public async Task<TransferResponseInner> TransferAsync(TransferRequestDto transferRequestDto)
  {
    try
    {
      if (appConfig.ChapaSecretKey is null)
      {
        throw new Exception("Chapa Secret Key is not set");
      }

      var tx_rf = PaymentHelper.GetTransactionReference();

      var restClient = new RestClient();
      var request = new RestRequest(
        $"{appConfig.ChapaApiOrigin}/v1/transaction/initialize",
        Method.Post
      );
      request.AddHeader("Content-Type", "application/json");
      request.AddHeader("Authorization", $"Bearer {appConfig.ChapaSecretKey}");

      request.AddJsonBody(
        new
        {
          email = transferRequestDto.SenderEmail,
          amount = transferRequestDto.Amount,
          first_name = transferRequestDto.SenderName,
          tx_ref = tx_rf,
          currency = "ETB",
          callback_url = appConfig.ApiOrigin,
          phone_number = transferRequestDto.PhoneNumber
        }
      );

      var response = await restClient.ExecuteAsync(request);
      var data = JsonSerializer.Deserialize<JsonElement>(
        response.Content ?? "null",
        new JsonSerializerOptions { WriteIndented = true }
      );

      if (!response.IsSuccessStatusCode)
      {
        return new TransferResponseInner
        {
          IsSuccessful = false,
          Message = JToken.Parse(data.GetProperty("message").ToString() ?? "No content"),
          TransactionReference = "null"
        };
      }

      var status = data.GetProperty("status").GetString();

      return new TransferResponseInner
      {
        IsSuccessful = status == "success",
        Message = JToken.Parse(data.ToString() ?? "No content"),
        TransactionReference = tx_rf
      };
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error transferring funds");
      throw;
    }
  }

  public async Task<IChargeResponse> ChargeAsync(ICharge charge)
  {
    try
    {
      if (charge is not ChapaCharge chapaCharge)
        throw new InvalidOperationException("Charge type must be of type ChapaCharge");

      var restClient = new RestClient();
      restClient.AddDefaultHeader("Authorization", $"Bearer {appConfig.ChapaSecretKey}");

      string txRf = PaymentHelper.GetTransactionReference();

      var restRequest = new RestRequest(
        $"{appConfig.ChapaApiOrigin}/v1/charges?type={chapaCharge.PaymentMethod.GetDisplayName()}"
      )
      {
        Method = Method.Post
      };
      restRequest.AddHeader("Content-Type", "application/json");
      restRequest.AddBody(
        new
        {
          amount = chapaCharge.Amount,
          currency = chapaCharge.Currency.ToString(),
          mobile = chapaCharge.PhoneNumber,
          tx_rf = txRf
        }
      );

      var response = await restClient.ExecuteAsync(restRequest);

      if (!response.IsSuccessStatusCode)
      {
        throw new Exception(response.ErrorMessage);
      }
      var content = JsonSerializer.Deserialize<JsonElement>(response.Content ?? "");
      if (
        content.TryGetProperty("data", out var dataElement)
        && dataElement.TryGetProperty("meta", out var meta)
      )
      {
        return new ChargeResponse
        {
          Message = meta.TryGetProperty("message", out var messageProp)
            ? messageProp.GetString() ?? ""
            : "Message not available",
          RefId = meta.TryGetProperty("ref_id", out var refId)
            ? refId.GetString() ?? ""
            : "RefId not available",
          Status = meta.TryGetProperty("status", out var statusProp)
            ? statusProp.GetString() == "success"
            : false
        };
      }
      else
      {
        throw new InvalidOperationException("Data or Meta object is missing in the response");
      }
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured tryingto charge in chapa");
      throw;
    }
  }

  public async Task<IVerifyResponse> VerifyAsync(IVerifyRequest verifyRequest)
  {
    try
    {
      // https://api.chapa.co/v1/transaction/verify/chewatatest-6669
      var restClient = new RestClient();
      restClient.AddDefaultHeader("Authorization", $"Bearer {appConfig.ChapaSecretKey}");

      var restRequest = new RestRequest(
        $"{appConfig.ChapaApiOrigin}/v1/transaction/verify/{verifyRequest.TransactionReference}"
      )
      {
        Method = Method.Get
      };

      var response = await restClient.ExecuteAsync(restRequest);

      if (!response.IsSuccessStatusCode)
      {
        throw new Exception(response.ErrorMessage);
      }

      var content = JsonSerializer.Deserialize<JsonElement>(response.Content ?? "");

      return new VerifyResponse
      {
        Success = true,
        FirstName =
          content.TryGetProperty("data", out var data)
          && data.TryGetProperty("first_name", out var firstName)
            ? firstName.ToString()
            : "",
        LastName =
          !data.IsNull() && data.TryGetProperty("last_name", out var lastName)
            ? lastName.ToString()
            : "",
        Email =
          !data.IsNull() && data.TryGetProperty("last_name", out var email) ? email.ToString() : "",
      };
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to verify in chapa");
      throw;
    }
  }
}
