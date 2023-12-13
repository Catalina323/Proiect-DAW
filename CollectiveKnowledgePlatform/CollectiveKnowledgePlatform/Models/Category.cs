using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class Category
    {
        //atributele din tabel
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        //chestii pentru legatura dintre tabele

        [Required]
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Topic>? Topics { get; set; }
    }
}
