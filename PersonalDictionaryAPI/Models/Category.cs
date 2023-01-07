using System.Text.Json.Serialization;

namespace PersonalDictionaryAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Note>? Notes { get; set; }
    }
}
