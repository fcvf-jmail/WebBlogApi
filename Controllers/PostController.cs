using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogService.API.DTOs;
using BlogService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IBlogService _blogService;

        public PostController(IPostService postService, IBlogService blogService)
        {
            _postService = postService;
            _blogService = blogService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto createPostDto)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
            {
                return Unauthorized("Invalid token: 'sub' claim missing.");
            }

            if (!int.TryParse(subClaim.Value, out var userId))
            {
                return BadRequest("Invalid user ID in token.");
            }

            var blog = await _blogService.GetBlogByIdAsync(createPostDto.BlogId);
            if (blog == null || blog.Owner.Id != userId)
            {
                return BadRequest("Blog not found or does not belong to user.");
            }

            var post = await _postService.CreatePostAsync(createPostDto.BlogId, userId, createPostDto);
            if (post == null)
            {
                return BadRequest("Failed to create post.");
            }

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<PostDto>> UpdatePost(int id, UpdatePostDto updatePostDto)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
            {
                return Unauthorized("Invalid token: 'sub' claim missing.");
            }

            if (!int.TryParse(subClaim.Value, out var userId))
            {
                return BadRequest("Invalid user ID in token.");
            }

            var post = await _postService.UpdatePostAsync(id, userId, updatePostDto);
            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeletePost(int id)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
            {
                return Unauthorized("Invalid token: 'sub' claim missing.");
            }

            if (!int.TryParse(subClaim.Value, out var userId))
            {
                return BadRequest("Invalid user ID in token.");
            }

            await _postService.DeletePostAsync(id, userId);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpGet("blog/{blogId}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsByBlogId(int blogId)
        {
            var posts = await _postService.GetPostsByBlogIdAsync(blogId);
            return Ok(posts);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPosts()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }
    }
}