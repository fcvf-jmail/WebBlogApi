using System;
using System.ComponentModel.DataAnnotations;
using BlogService.API.Models;

namespace BlogService.API.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public int BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string AuthorUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>(); // Изменено на CommentDto
        public int CommentsCount { get; set; }
    }

    public class CreatePostDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int BlogId { get; set; }
    }

    public class UpdatePostDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? ImageUrl { get; set; }
    }
}