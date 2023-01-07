

using System.Text.Json.Serialization;

namespace PersonalDictionaryAPI.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Translation { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int? CategoryId { get; set; }
        public int? LanguageId { get; set; }
        public Category? Category { get; set; }
        public Language? Language { get; set; }       
    }
}
