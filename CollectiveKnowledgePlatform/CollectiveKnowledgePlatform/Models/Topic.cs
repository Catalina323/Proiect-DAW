using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Textul este obligatoriu")]
        public string Text { get; set; }

        public int CategoryId {  get; set; }

        public virtual Category Category { get; set; }


        [Required]
        public int UserId {  get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<TopicLike> TopicLikes { get; set; }  
        

    }
}
