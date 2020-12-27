
namespace Shop.Web.Data
{
    using Entities;
    public class ContryRepository: GenericRepository<Contry>, IContryRepository
    {
        public ContryRepository(DataContext context) : base(context)
    {
    }
}
}
