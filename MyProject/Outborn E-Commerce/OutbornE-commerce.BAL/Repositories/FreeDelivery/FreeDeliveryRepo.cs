using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.FlashDealRepo;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.FreeDeliverys
{
    public class FreeDeliveryRepo : BaseRepository<FreeDelivery>
    {
        public FreeDeliveryRepo(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<decimal?> GetFreeDelivery(Guid? countryId = null)
        {
            var UAE_ID = "b393e0e8-ec9c-467a-b894-9923b3743580";
            if (Guid.Parse(UAE_ID) != countryId)
                return null;

            var freeDelivery = await Find(fd => fd.CountryId == Guid.Parse(UAE_ID));
            if (freeDelivery == null)
                return null;

            return freeDelivery.FreeDeliveryBasedOnOrderPrice;
        }

        public async Task<Response<Guid>> UpdateFreeDelivery(FreeDeliveryDto freeDelivery, CancellationToken cancellationToken)
        {
            // Ensure the related country exists
            var countryExists = await Find(c => c.CountryId == freeDelivery.CountryId, true);

            countryExists.FreeDeliveryBasedOnOrderPrice = freeDelivery.FreeDeliveryBasedOnOrderPrice;

            await SaveAsync(cancellationToken);
            return new Response<Guid>
            {
                IsError = true,
                Message = "Updated Successfully",
                MessageAr = "تم التحديث بنجاح",
                Status = (int)StatusCodeEnum.Ok,
            };
        }
    }
}