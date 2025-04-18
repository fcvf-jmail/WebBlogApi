using System.Collections.Generic;
using System.Threading.Tasks;
using BlogService.API.DTOs;

namespace BlogService.API.Services
{
    public interface IPostService
    {
        Task<PostDto> CreatePostAsync(int blogId, int userId, CreatePostDto createPostDto);
        Task<PostDto> UpdatePostAsync(int postId, int userId, UpdatePostDto updatePostDto);
        Task DeletePostAsync(int postId, int userId);
        Task<PostDto> GetPostByIdAsync(int postId);
        Task<IEnumerable<PostDto>> GetPostsByBlogIdAsync(int blogId);
        Task<IEnumerable<PostDto>> GetAllPostsAsync();
    }
}