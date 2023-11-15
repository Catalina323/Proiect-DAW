using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class TopicLike
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User {  get; set; }

        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }

        public int type { get; set; }

    }
}
