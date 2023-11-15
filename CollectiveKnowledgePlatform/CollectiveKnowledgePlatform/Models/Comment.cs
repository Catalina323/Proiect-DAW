using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Comentariul nu are continut")]
        public string Continut {  get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int TopicId {  get; set; }
        public virtual Topic Topic { get; set; }

    }
}
