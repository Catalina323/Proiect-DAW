using Microsoft.AspNetCore.Identity;

namespace CollectiveKnowledgePlatform.Models
{
    public class ApplicationUser: IdentityUser
    {
        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<TopicLike> TopicLikes { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}
