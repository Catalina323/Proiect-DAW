using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectiveKnowledgePlatform.Models
{
    public class Topic
    {
        //atribute tabel
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Textul este obligatoriu")]
        public string Text { get; set; }

        //chestii pt legarea tabelelor

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<TopicLike>? TopicLikes { get; set; }

        //folosim asta pt ca in TopicsController in New
        //avem nevoie de o lista cu categoriile pt a adauga in nou topic
        //gasesti codul decomentat daca am folosit asta in aplicatie
        //daca il gasesti comentat lasa-l asa acolo (nu e folosit dar il sterg eu)
        //pwp cata <3

        //[NotMapped]
        //public IEnumerable<SelectListItem>? Categ { get; set; }

    }
}
