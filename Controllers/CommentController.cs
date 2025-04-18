using System.Collections.Generic;
using System.Threading.Tasks;
using BlogService.API.DTOs;
using BlogService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("post/{postId}")]
        [Authorize]
        public async Task<ActionResult<CommentDto>> CreateComment(int postId, CreateCommentDto createCommentDto)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value);
            var comment = await _commentService.CreateCommentAsync(postId, userId, createCommentDto);

            if (comment == null)
                return BadRequest("Failed to create comment");

            return Ok(comment);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<CommentDto>> UpdateComment(int id, UpdateCommentDto updateCommentDto)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value);
            var comment = await _commentService.UpdateCommentAsync(id, userId, updateCommentDto);

            if (comment == null)
                return NotFound();

            return Ok(comment);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value);
            await _commentService.DeleteCommentAsync(id, userId);
            return NoContent();
        }

        [HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByPostId(int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId, page, pageSize);
            return Ok(comments);
        }
    }
}