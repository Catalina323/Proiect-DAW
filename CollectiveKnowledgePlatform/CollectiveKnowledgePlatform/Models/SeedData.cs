    using CollectiveKnowledgePlatform.Data;
    using CollectiveKnowledgePlatform.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    namespace CollectiveKnowledgePlatform.Models
    {
        public static class SeedData
        {
            public static void Initialize(IServiceProvider serviceProvider)
            {
                using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
                {
                    if (context.Roles.Any())
                    {
                        return; 
                    }

                    //Adaug roluri in aplicatie
                    context.Roles.AddRange(

                    new IdentityRole
                    {
                        Id = "c6139664-edd9-4856-a0ba-e8a73d388ba0",
                        Name = "Administrator",
                        NormalizedName = "Administrator".ToUpper()
                    },


                    new IdentityRole
                    {
                        Id = "c6139664-edd9-4856-a0ba-e8a73d388ba1",
                        Name = "Moderator",
                        NormalizedName = "Moderator".ToUpper()
                    },


                    new IdentityRole
                    {
                        Id = "c6139664-edd9-4856-a0ba-e8a73d388ba2",
                        Name = "User",
                        NormalizedName = "User".ToUpper()
                    }

                    );

                    var hasher = new PasswordHasher<ApplicationUser>();

                    //Adaug useri in aplicatie
                    context.Users.AddRange(
                    new ApplicationUser
                    {
                        Id = "d6e23997-6f83-4f8c-ba9c-81f2189fe900",
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")
                    },

                    new ApplicationUser
                    {
                        Id = "d6e23997-6f83-4f8c-ba9c-81f2189fe901",
                        UserName = "moderator@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "MODERATOR@TEST.COM",
                        Email = "moderator@test.com",
                        NormalizedUserName = "MODERATOR@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Moderator1!")
                    },

                    new ApplicationUser
                    {
                        Id = "d6e23997-6f83-4f8c-ba9c-81f2189fe902",
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    },

                    new ApplicationUser
                    {
                        Id = "d6e23997-6f83-4f8c-ba9c-81f2189fe903",
                        UserName = "user_neconfirmat@test.com",
                        EmailConfirmed = false,
                        NormalizedEmail = "USER_NECONFIRMAT@TEST.COM",
                        Email = "user_neconfirmat@test.com",
                        NormalizedUserName = "USER_NECONFIRMAT@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User2!")
                    }

                    );

                    //Atribui roluri userilor din aplicatie
                    context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "c6139664-edd9-4856-a0ba-e8a73d388ba0",//admin
                        UserId = "d6e23997-6f83-4f8c-ba9c-81f2189fe900"
                    },

                    new IdentityUserRole<string>
                    {
                        RoleId = "c6139664-edd9-4856-a0ba-e8a73d388ba1",//moderator
                        UserId = "d6e23997-6f83-4f8c-ba9c-81f2189fe901"
                    },

                    new IdentityUserRole<string>
                    {
                        RoleId = "c6139664-edd9-4856-a0ba-e8a73d388ba2",//userul confirmat
                        UserId = "d6e23997-6f83-4f8c-ba9c-81f2189fe902"
                    },

                    new IdentityUserRole<string>
                    {
                        RoleId = "c6139664-edd9-4856-a0ba-e8a73d388ba2",//userul neconfirmat
                        UserId = "d6e23997-6f83-4f8c-ba9c-81f2189fe903"
                    }

                    );
                    context.SaveChanges();
                }
            }
        }
    }
