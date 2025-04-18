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
    public class BlogServiceImpl : IBlogService
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<User> _userRepository;

        public BlogServiceImpl(IRepository<Blog> blogRepository, IRepository<User> userRepository)
        {
            _blogRepository = blogRepository;
            _userRepository = userRepository;
        }

        public async Task<BlogDto> CreateBlogAsync(int userId, CreateBlogDto createBlogDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            var blog = new Blog
            {
                Title = createBlogDto.Title,
                Description = createBlogDto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _blogRepository.AddAsync(blog);
            await _blogRepository.SaveChangesAsync();

            return new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Owner = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                },
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt
            };
        }

        public async Task<BlogDto> UpdateBlogAsync(int blogId, int userId, UpdateBlogDto updateBlogDto)
        {
            var blog = await _blogRepository.SingleOrDefaultAsync(b => b.Id == blogId && b.UserId == userId);
            if (blog == null)
                return null;

            if (updateBlogDto.Title != null)
                blog.Title = updateBlogDto.Title;

            if (updateBlogDto.Description != null)
                blog.Description = updateBlogDto.Description;
            
            blog.UpdatedAt = DateTime.UtcNow;

            _blogRepository.Update(blog);
            await _blogRepository.SaveChangesAsync();

            var user = await _userRepository.GetByIdAsync(userId);

            return new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Owner = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                },
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt
            };
        }
        public async Task<BlogDto> GetBlogByIdAsync(int blogId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog == null)
                return null;

            var user = await _userRepository.GetByIdAsync(blog.UserId);
            if (user == null)
                return null;

            return new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Owner = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                },
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt
            };
        }

        public async Task<IEnumerable<BlogDto>> GetBlogsByUserIdAsync(int userId)
        {
            var blogs = await _blogRepository.GetAllAsync(b => b.UserId == userId);
            if (!blogs.Any())
                return Enumerable.Empty<BlogDto>();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Enumerable.Empty<BlogDto>();

            return blogs.Select(blog => new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Owner = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                },
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt
            });
        }

        public async Task<IEnumerable<BlogDto>> GetAllBlogsAsync(int page, int pageSize)
        {
            var blogs = await _blogRepository.GetAllAsync();
            var pagedBlogs = blogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var blogDtos = new List<BlogDto>();

            foreach (var blog in pagedBlogs)
            {
                var user = await _userRepository.GetByIdAsync(blog.UserId);
                if (user == null)
                    continue;

                blogDtos.Add(new BlogDto
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    Description = blog.Description,
                    Owner = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email
                    },
                    CreatedAt = blog.CreatedAt,
                    UpdatedAt = blog.UpdatedAt
                });
            }

            return blogDtos;
        }
    }
}