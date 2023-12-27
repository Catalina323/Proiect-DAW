using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class Comment
    {
        //atribute tabel
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Comentariul nu are continut")]
        public string Continut {  get; set; }

        public DateTime Date { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? TopicId { get; set; }
        public virtual Topic? Topic { get; set; }
    }
}
