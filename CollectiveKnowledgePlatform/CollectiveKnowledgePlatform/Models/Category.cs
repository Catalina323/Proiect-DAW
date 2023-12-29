using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class Category
    {
        //atributele din tabel
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Numele categoriei este obligatoriu")]
        [StringLength(30, ErrorMessage ="Numele categoriei este prea mare")]
        [MinLength(2, ErrorMessage ="Numele categoriei este prea mic")]
        public string Name { get; set; }

        public string Description { get; set; }

        //chestii pentru legatura dintre tabele
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Topic>? Topics { get; set; }
    }
}
