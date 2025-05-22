using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MZ.Domain.Enums;

namespace MZ.Domain.Entities
{
    [Table("User")]
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Username must be 4 characters long")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [NotMapped]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Password must be 4 characters long")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;

        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
