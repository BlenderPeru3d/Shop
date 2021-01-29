
namespace Shop.Web.Controllers
{
    using Data;
    using Data.Entities;
    using Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;


    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;

        private readonly IUserHelper userHelper;

        public ProductsController(IProductRepository productRepository, IUserHelper userHelper)
        {
            this.productRepository = productRepository;
            this.userHelper = userHelper;
        }

        public IActionResult Index()
        {
            return View(productRepository.GetAll().OrderBy(p => p.Name));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            Product product = await productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel view)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if (view.ImageFile != null && view.ImageFile.Length > 0)
                {
                    string guid = Guid.NewGuid().ToString();
                    string file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(), 
                        "wwwroot\\images\\Products", 
                        file);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await view.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/Products/{file}";
                }
                Product product = ToProduct(view, path);
                product.User = await userHelper.GetUserByEmailAsync(User.Identity.Name);
                await productRepository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(view);
        }

        private Product ToProduct(ProductViewModel view, string path)
        {
            return new Product
            {
                Id = view.Id,
                ImageUrl = path,
                IsAvailabe = view.IsAvailabe,
                LastPurchase = view.LastPurchase,
                LastSale = view.LastSale,
                Name = view.Name,
                Price = view.Price,
                Stock = view.Stock,
                User = view.User
            };

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            Product product = await productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }
            object view = ToProductViewModel(product);
            return View(view);
        }

        private object ToProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                IsAvailabe = product.IsAvailabe,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel view)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string path = view.ImageUrl;

                    if (view.ImageFile != null && view.ImageFile.Length > 0)
                    {
                        string guid = Guid.NewGuid().ToString();
                        string file = $"{guid}.jpg";

                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\Products",
                            file);

                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            await view.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/Products/{file}";
                    }
                    Product product = ToProduct(view, path);
                    // TODO: Pending to change to: this.User.Identity.Name
                    product.User = await userHelper.GetUserByEmailAsync(User.Identity.Name);
                    await productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await productRepository.ExistAsync(view.Id))
                    {
                        return new NotFoundViewResult("ProductNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(view);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            Product product = await productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product product = await productRepository.GetByIdAsync(id);
            await productRepository.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ProductNotFound()
        {
            return this.View();
        }

    }

}