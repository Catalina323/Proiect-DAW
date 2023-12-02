using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Comentariul nu are continut")]
        public string Continut {  get; set; }

        [Required]
        public virtual String UserId { get; set; }

        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
