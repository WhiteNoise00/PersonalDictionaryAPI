using System.Text.Json.Serialization;

namespace PersonalDictionaryAPI.Models
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public virtual ICollection<Note>? Notes { get; set; }
    }
}
