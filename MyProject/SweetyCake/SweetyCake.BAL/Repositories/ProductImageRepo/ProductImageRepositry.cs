﻿using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ProductImageRepo
{
    public class ProductImageRepositry : BaseRepository<ProductImage>,IProductImageRepositry
    {
        public ProductImageRepositry(ApplicationDbContext context) : base(context)
        {
        }
    }
   
}
