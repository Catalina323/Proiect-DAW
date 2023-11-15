<<<<<<< HEAD
﻿namespace CollectiveKnowledgePlatform.Models
{
    public class Role
    {
=======
﻿using System.ComponentModel.DataAnnotations;

namespace CollectiveKnowledgePlatform.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }

>>>>>>> 384e93d5196630a580801c840971148713e54fd8
    }
}
