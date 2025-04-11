using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.FlashDealRepo
{
    public interface IFlashDealRepo :IBaseRepository<FlashDeal>
    {
        Task<FlashDeal> GetRandomActiveFlashDealAsync();
    }
}
