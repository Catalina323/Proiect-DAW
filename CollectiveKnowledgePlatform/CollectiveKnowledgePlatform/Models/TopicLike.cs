﻿using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class TopicLike
    {
        [Key]
        public int Id { get; set; }
        public int Type { get; set; }

        [Required]
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public int? TopicId { get; set; }
        public virtual Topic? Topic { get; set; }

    }
}
