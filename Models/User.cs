using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TeleutDictionary.API.Models;

namespace TeleutDictionary.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public List<Word> Words { get; set; } = new();
    }
}