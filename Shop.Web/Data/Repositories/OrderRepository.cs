namespace Shop.Web.Data.Repositories
{
    using Entities;
    using Helpers;
    using Microsoft.EntityFrameworkCore;
    using Shop.Web.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;

        public OrderRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            this.context = context;
            this.userHelper = userHelper;
        }

        public async Task<IQueryable<Order>> GetOrdersAsync(string userName)
        {
            User user = await userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            if (await userHelper.IsUserInRoleAsync(user, "Admin"))
            {
                return context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .OrderByDescending(o => o.OrderDate);
            }

            return context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.User == user)
                .OrderByDescending(o => o.OrderDate);
        }
        public async Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string userName)
        {
            User user = await userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            return context.OrderDetailTemps
                .Include(o => o.Product)
                .Where(o => o.User == user)
                .OrderBy(o => o.Product.Name);
        }

        public async Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            User user = await userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return;
            }

            Product product = await context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return;
            }

            OrderDetailTemp orderDetailTemp = await context.OrderDetailTemps
                .Where(odt => odt.User == user && odt.Product == product)
                .FirstOrDefaultAsync();
            if (orderDetailTemp == null)
            {
                orderDetailTemp = new OrderDetailTemp
                {
                    Price = product.Price,
                    Product = product,
                    Quantity = model.Quantity,
                    User = user,
                };

                context.OrderDetailTemps.Add(orderDetailTemp);
            }
            else
            {
                orderDetailTemp.Quantity += model.Quantity;
                context.OrderDetailTemps.Update(orderDetailTemp);
            }

            await context.SaveChangesAsync();
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            OrderDetailTemp orderDetailTemp = await context.OrderDetailTemps.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }

            orderDetailTemp.Quantity += quantity;
            if (orderDetailTemp.Quantity > 0)
            {
                context.OrderDetailTemps.Update(orderDetailTemp);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            OrderDetailTemp orderDetailTemp = await context.OrderDetailTemps.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }

            context.OrderDetailTemps.Remove(orderDetailTemp);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmOrderAsync(string userName)
        {
            User user = await userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return false;
            }

            List<OrderDetailTemp> orderTmps = await context.OrderDetailTemps
                .Include(o => o.Product)
                .Where(o => o.User == user)
                .ToListAsync();

            if (orderTmps == null || orderTmps.Count == 0)
            {
                return false;
            }

            List<OrderDetail> details = orderTmps.Select(o => new OrderDetail
            {
                Price = o.Price,
                Product = o.Product,
                Quantity = o.Quantity
            }).ToList();

            Order order = new Order
            {
                OrderDate = DateTime.UtcNow,
                User = user,
                Items = details,
            };

            context.Orders.Add(order);
            context.OrderDetailTemps.RemoveRange(orderTmps);
            await context.SaveChangesAsync();
            return true;
        }

    }

}
