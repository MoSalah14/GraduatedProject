using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
namespace OutbornE_commerce.BAL.Repositories.Cart
{
    public class CartRepo : BaseRepository<DAL.Models.Cart>, ICartRepo
    {
        public CartRepo(ApplicationDbContext context):base(context) { }
    }
}
