﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Cart
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductNameEn { get; set; }
        public string ProductNameAr { get; set; }
        public string ImageUrl { get; set; }
        public decimal? ItemPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class DisplayCartItemDto
    {
        public decimal TotalPrice { get; set; }
        public List<CartItemDto> cartItemDtos { get; set; } = new List<CartItemDto>();
    }
}