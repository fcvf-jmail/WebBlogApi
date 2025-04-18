using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogService.API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime RegistrationDate { get; set; }

        // Навигационные свойства
        public virtual List<Blog> Blogs { get; set; } = new List<Blog>(); // Изменено на коллекцию
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
    }
}