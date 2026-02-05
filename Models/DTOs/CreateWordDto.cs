using System.ComponentModel.DataAnnotations;

namespace TeleutDictionary.API.Models.DTOs
{
    public class CreateWordDto
    {
        [Required]
        public string Russian { get; set; } = string.Empty;

        [Required]
        public string Teleut { get; set; } = string.Empty;

        public string? PartOfSpeech { get; set; }
        public string? Example { get; set; }
        public string? Transcription { get; set; }
        public string? Description { get; set; }
    }
}