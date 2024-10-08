using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services.BlogService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/blogs")]
public class BlogController(IBlogService blogService, ILogger<BlogController> logger)
  : ControllerBase
{
  /// <summary>
  /// Get all blogs
  /// </summary>
  /// <returns></returns>
  [HttpGet("all")]
  public async Task<IActionResult> GetAllBlogs()
  {
    try
    {
      var result = await blogService.GetAllBlogsAsync();
      return Ok(new ApiResponse<List<BlogDto>>(true, "Blogs retrieved successfully", result));
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to get all the blogs.");
      throw;
    }
  }

  /// <summary>
  /// Get blog by id
  /// </summary>
  /// <param name="blogId"></param>
  /// <returns></returns>
  [HttpGet("{blogId}")]
  public async Task<IActionResult> GetBlogById([FromRoute] [Required] [Guid] Guid blogId)
  {
    try
    {
      var result = await blogService.GetBlogAsync(blogId);
      return Ok(new ApiResponse<BlogDto>(true, "Blog retrieved successfully", result));
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to get blog by id.");
      throw;
    }
  }

  /// <summary>
  /// Create a new blog
  /// </summary>
  /// <param name="createBlogDto"></param>
  /// <returns></returns>
  [HttpPost]
  public async Task<IActionResult> CreateBlog([FromBody] CreateBlogDto createBlogDto)
  {
    try
    {
      var result = await blogService.CreateBlogAsync(createBlogDto);
      return Ok(new ApiResponse<BlogDto>(true, "Blog created successfully", result));
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to create blog");
      throw;
    }
  }

  /// <summary>
  /// Update a blog
  /// </summary>
  /// <param name="blogId"></param>
  /// <param name="editBlogDto"></param>
  /// <returns></returns>
  [HttpPut("{blogId}")]
  public async Task<IActionResult> UpdateBlog(
    [FromRoute] Guid blogId,
    [FromBody] EditBlogDto editBlogDto
  )
  {
    try
    {
      var result = await blogService.UpdateBlogAsync(blogId, editBlogDto);
      return Ok(new ApiResponse<BlogDto>(true, "Blog updated successfully", result));
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to update blog");
      throw;
    }
  }

  /// <summary>
  /// Delete all blogs (Only for Testing Purpose)
  /// </summary>
  /// <returns></returns>
  [HttpDelete("all")]
  public IActionResult DeleteAllBlogs()
  {
    try
    {
      blogService.DeleteAllBlogs();
      return NoContent();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to update blog");
      throw;
    }
  }

  [HttpPost("comment")]
  public async Task<IActionResult> PostCommentOnBlog(
    [FromBody] CreateBlogCommentDto createBlogCommentDto
  )
  {
    try
    {
      var result = await blogService.CreateBlogCommentAsync(createBlogCommentDto);
      return Ok(new ApiResponse<BlogCommentDto>(true, "Comment posted successfully", result));
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to post a  blog comment");
      throw;
    }
  }

  [HttpGet("{blogId}/comments")]
  public async Task<IActionResult> GetBlogComments([FromRoute] [Required] [Guid] Guid blogId)
  {
    try
    {
      var result = await blogService.GetBlogCommentsAsync(blogId);
      return Ok(
        new ApiResponse<ICollection<BlogCommentDto>>(
          true,
          "Blog comments retrieved successfully",
          result
        )
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to get blog comments.");
      throw;
    }
  }

  [HttpPost("like")]
  public async Task<IActionResult> LikeBlog([FromBody] CreateBlogLikeDto createBlogLikeDto)
  {
    try
    {
      var result = await blogService.CreateBlogLikeAsync(createBlogLikeDto);
      return Ok(new ApiResponse<BlogLikeDto>(true, result.IsNull() ? "Unliked" : "Liked", result));
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to like a blog.");
      throw;
    }
  }
}
