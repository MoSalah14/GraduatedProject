﻿using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ShippingPriceRepo
{
    public interface IShippingPriceRepo : IBaseRepository<ShippingPrice>
    {
        Task<decimal> GetShippingPriceBasedOnWeightAndCountryId(Guid? CountryId, double OrderWeight);
    }
}