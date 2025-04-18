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
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BlogDto>> CreateBlog(CreateBlogDto createBlogDto)
        {
            Console.WriteLine("Available claims in CreateBlog:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
            {
                return Unauthorized("Invalid token: 'sub' claim missing.");
            }

            if (!int.TryParse(subClaim.Value, out var userId))
            {
                return BadRequest("Invalid user ID in token.");
            }

            var blog = await _blogService.CreateBlogAsync(userId, createBlogDto);
            if (blog == null)
            {
                return BadRequest("Failed to create blog.");
            }

            return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, blog);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<BlogDto>> UpdateBlog(int id, UpdateBlogDto updateBlogDto)
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

            var blog = await _blogService.UpdateBlogAsync(id, userId, updateBlogDto);
            if (blog == null)
                return NotFound();
            return Ok(blog);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetBlog(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null)
                return NotFound();

            return Ok(blog);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetBlogsByUserId(int userId)
        {
            Console.WriteLine("Available claims in GetBlogsByUserId:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var blogs = await _blogService.GetBlogsByUserIdAsync(userId);
            if (!blogs.Any())
                return NotFound();
            return Ok(blogs);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetAllBlogs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var blogs = await _blogService.GetAllBlogsAsync(page, pageSize);
            return Ok(blogs);
        }
    }
}