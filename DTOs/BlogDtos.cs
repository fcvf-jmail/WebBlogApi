using System;
using System.ComponentModel.DataAnnotations;

namespace BlogService.API.DTOs
{
    public class BlogDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UserDto Owner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateBlogDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }

    public class UpdateBlogDto
    {
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}