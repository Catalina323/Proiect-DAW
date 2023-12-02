using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public virtual String UserId { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
    }
}
