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

        //[Required]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<TopicLike>? TopicLikes { get; set; }


    }
}
