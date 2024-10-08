using HealthHub.Source.Models.Enums;
using HealthHub.Source.Models.Interfaces.Payments;

namespace HealthHub.Source.Services.PaymentProviders;

/// <summary>
/// This interface will contain the methods that the payment provider will implement.
/// The payment provider will be a class that implements this interface.
/// All the methods in this interface are mandatory for any payment provider
/// </summary>
public interface IPaymentProvider
{
  PaymentProvider PaymentProvider { get; }
  Task<decimal> CheckBalanceAsync(string email);
  Task<TransferResponseInner> TransferAsync(TransferRequestDto transferRequestDto);
  Task<IChargeResponse> ChargeAsync(ICharge charge);
  Task<IVerifyResponse> VerifyAsync(IVerifyRequest verifyRequest);
}
