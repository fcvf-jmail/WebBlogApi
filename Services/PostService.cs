using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogService.API.Data.Repositories;
using BlogService.API.DTOs;
using BlogService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogService.API.Services
{
    public class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Blog> _blogRepository;

        public PostService(
            IRepository<Post> postRepository,
            IRepository<User> userRepository,
            IRepository<Blog> blogRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _blogRepository = blogRepository;
        }

        public async Task<PostDto> CreatePostAsync(int blogId, int userId, CreatePostDto createPostDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog == null || blog.UserId != userId)
                return null;

            var post = new Post
            {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                ImageUrl = string.IsNullOrWhiteSpace(createPostDto.ImageUrl) ? null : createPostDto.ImageUrl,
                BlogId = blogId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Comments = []
            };

            await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                BlogId = post.BlogId,
                BlogTitle = blog.Title,
                AuthorUsername = user.Username,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
        }

        public async Task<PostDto> UpdatePostAsync(int postId, int userId, UpdatePostDto updatePostDto)
        {
            var post = await _postRepository
                .Query()
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.Blog)
                .ThenInclude(b => b.User)
                .SingleOrDefaultAsync(p => p.Id == postId && p.Blog.UserId == userId);

            if (post == null)
                return null;

            if (updatePostDto.Title != null)
                post.Title = updatePostDto.Title;

            if (updatePostDto.Content != null)
                post.Content = updatePostDto.Content;

            if (updatePostDto.ImageUrl != null)
                post.ImageUrl = string.IsNullOrWhiteSpace(updatePostDto.ImageUrl) ? null : updatePostDto.ImageUrl;

            post.UpdatedAt = DateTime.UtcNow;

            _postRepository.Update(post);
            await _postRepository.SaveChangesAsync();

            var blog = await _blogRepository.GetByIdAsync(post.BlogId);
            var user = await _userRepository.GetByIdAsync(userId);

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                BlogId = post.BlogId,
                BlogTitle = blog.Title,
                AuthorUsername = user.Username,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Comments = post.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorUsername = c.User.Username,
                    CreatedAt = c.CreatedAt
                }).ToList() ?? [],
                CommentsCount = post.Comments.Count
            };
        }

        public async Task DeletePostAsync(int postId, int userId)
        {
            var post = await _postRepository.SingleOrDefaultAsync(p => p.Id == postId && p.Blog.UserId == userId);
            if (post == null)
                return;

            _postRepository.Delete(post);
            await _postRepository.SaveChangesAsync();
        }

        public async Task<PostDto> GetPostByIdAsync(int postId)
        {
            var post = await _postRepository
                .Query()
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.Blog)
                .ThenInclude(b => b.User)
                .SingleOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return null;

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                BlogId = post.BlogId,
                BlogTitle = post.Blog.Title,
                AuthorUsername = post.Blog.User.Username,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Comments = post.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorUsername = c.User.Username,
                    CreatedAt = c.CreatedAt
                })?.ToList() ?? [],
                CommentsCount = post.Comments.Count
            };
        }

        public async Task<IEnumerable<PostDto>> GetPostsByBlogIdAsync(int blogId)
        {

            var posts = await _postRepository
            .Query()
            .Where(p => p.BlogId == blogId)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .Include(p => p.Blog)
            .ThenInclude(b => b.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

            return posts.Select(post => new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                BlogId = post.BlogId,
                BlogTitle = post.Blog.Title,
                AuthorUsername = post.Blog.User.Username,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Comments = post.Comments?.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorUsername = c.User?.Username ?? "Unknown",
                    CreatedAt = c.CreatedAt
                }).ToList() ?? [],
                CommentsCount = post.Comments?.Count ?? 0
            }).ToList();
        }

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
        {
            var posts = await _postRepository
                .Query()
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.Blog)
                .ThenInclude(b => b.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return posts.Select(post => new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                BlogId = post.BlogId,
                BlogTitle = post.Blog.Title,
                AuthorUsername = post.Blog.User.Username,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Comments = post.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorUsername = c.User.Username,
                    CreatedAt = c.CreatedAt
                }).ToList(),
                CommentsCount = post.Comments.Count
            }).ToList();
        }
    }
}