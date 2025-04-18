using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogService.API.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Внешний ключ
        public int BlogId { get; set; }

        // Навигационные свойства
        public virtual Blog Blog { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}