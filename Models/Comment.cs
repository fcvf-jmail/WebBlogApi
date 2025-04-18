using System;
using System.ComponentModel.DataAnnotations;

namespace BlogService.API.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}