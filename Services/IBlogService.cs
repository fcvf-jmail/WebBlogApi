using System.Collections.Generic;
using System.Threading.Tasks;
using BlogService.API.DTOs;

namespace BlogService.API.Services
{
    public interface IBlogService
    {
        Task<BlogDto> CreateBlogAsync(int userId, CreateBlogDto createBlogDto);
        Task<BlogDto> UpdateBlogAsync(int blogId, int userId, UpdateBlogDto updateBlogDto);
        Task<BlogDto> GetBlogByIdAsync(int blogId);
        Task<IEnumerable<BlogDto>> GetBlogsByUserIdAsync(int userId);
        Task<IEnumerable<BlogDto>> GetAllBlogsAsync(int page, int pageSize);
    }
}