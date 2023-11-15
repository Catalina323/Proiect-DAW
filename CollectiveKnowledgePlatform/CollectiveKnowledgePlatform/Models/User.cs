using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public virtual Role Role { get; set; }


        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<TopicLike> TopicLikes { get; set; }

    }
}
