namespace TeleutDictionary.API.Models.DTOs
{
    public class UpdateWordDto
    {
        public string? Russian { get; set; }
        public string? Teleut { get; set; }
        public string? PartOfSpeech { get; set; }
        public string? Example { get; set; }
        public string? Transcription { get; set; }
        public string? Description { get; set; }
    }
}