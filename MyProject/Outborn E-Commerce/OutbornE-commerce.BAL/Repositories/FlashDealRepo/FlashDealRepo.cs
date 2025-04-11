using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.OrderItemRepo;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.FlashDealRepo
{
    public class FlashDealRepo : BaseRepository<FlashDeal>,IFlashDealRepo
    {
        public FlashDealRepo(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<FlashDeal> GetRandomActiveFlashDealAsync()
        {
            
            var activeFlashDeals = (await FindByCondition(x => x.IsActive)).ToList();

           
            if (activeFlashDeals.Any())
            {
                var random = new Random();
                int randomIndex = random.Next(activeFlashDeals.Count);
                return activeFlashDeals[randomIndex];
            }

            return null;
        }
    }
}
