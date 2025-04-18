using System.Collections.Generic;
using System.Threading.Tasks;
using BlogService.API.DTOs;

namespace BlogService.API.Services
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(int postId, int userId, CreateCommentDto createCommentDto);
        Task<CommentDto> UpdateCommentAsync(int commentId, int userId, UpdateCommentDto updateCommentDto);
        Task DeleteCommentAsync(int commentId, int userId);
        Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(int postId, int page, int pageSize);
    }
}