namespace Shop.Web.Data
{
    using Entities;
    using Microsoft.AspNetCore.Identity;
    using Shop.Web.Helpers;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class SeedDb
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;
        private readonly Random random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            this.context = context;
            this.userHelper = userHelper;
            random = new Random();
        }

        public async Task SeedAsync()
        {
            await context.Database.EnsureCreatedAsync();

            User user = await userHelper.GetUserByEmailAsync("peru3dblender@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Rodrigo",
                    LastName = "Leite",
                    Email = "peru3dblender@gmail.com",
                    UserName = "peru3dblender@gmail.com",
                    PhoneNumber = "+51993099557"
                };

                IdentityResult result = await userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
            }


            if (!context.Products.Any())
            {
                AddProduct("iPhone X", user);
                AddProduct("Magic Mouse", user);
                AddProduct("iWatch Series 4", user);
                await context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, User user)
        {
            context.Products.Add(new Product
            {
                Name = name,
                Price = random.Next(100),
                IsAvailabe = true,
                Stock = random.Next(100),
                User = user
            });
        }
    }

}
