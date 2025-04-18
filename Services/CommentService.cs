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
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<User> _userRepository;

        public CommentService(
            IRepository<Comment> commentRepository,
            IRepository<Post> postRepository,
            IRepository<User> userRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        public async Task<CommentDto> CreateCommentAsync(int postId, int userId, CreateCommentDto createCommentDto)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                return null;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            var comment = new Comment
            {
                Content = createCommentDto.Content,
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                AuthorUsername = user.Username,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<CommentDto> UpdateCommentAsync(int commentId, int userId, UpdateCommentDto updateCommentDto)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
                return null;

            comment.Content = updateCommentDto.Content;

            _commentRepository.Update(comment);
            await _commentRepository.SaveChangesAsync();

            var user = await _userRepository.GetByIdAsync(userId);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                AuthorUsername = user.Username,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
                return;

            _commentRepository.Delete(comment);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId, int page, int pageSize)
        {
            var comments = await _commentRepository.GetAllAsync(c => c.PostId == postId);
            var pagedComments = comments
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var commentDtos = new List<CommentDto>();

            foreach (var comment in pagedComments)
            {
                var user = await _userRepository.GetByIdAsync(comment.UserId);
                if (user == null)
                    continue;

                commentDtos.Add(new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    AuthorUsername = user.Username,
                    CreatedAt = comment.CreatedAt
                });
            }

            return commentDtos;
        }
    }
}