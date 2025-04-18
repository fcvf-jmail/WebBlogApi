using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogService.API.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}

