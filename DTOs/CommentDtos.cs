using System;
using System.ComponentModel.DataAnnotations;

namespace BlogService.API.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string AuthorUsername { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCommentDto
    {
        [Required]
        public string Content { get; set; }
    }

    public class UpdateCommentDto
    {
        [Required]
        public string Content { get; set; }
    }
}