using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TeleutDictionary.API.Models
{
    public class Word
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Russian { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Teleut { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PartOfSpeech { get; set; }

        [StringLength(500)]
        public string? Example { get; set; }

        [StringLength(200)]
        public string? Transcription { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}