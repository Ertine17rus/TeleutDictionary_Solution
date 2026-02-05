namespace TeleutDictionary.API.Models.DTOs
{
    public class WordDto
    {
        public int Id { get; set; }
        public string Russian { get; set; } = string.Empty;
        public string Teleut { get; set; } = string.Empty;
        public string? PartOfSpeech { get; set; }
        public string? Example { get; set; }
        public string? Transcription { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}