﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Cart
{
    public class CreateCartDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
