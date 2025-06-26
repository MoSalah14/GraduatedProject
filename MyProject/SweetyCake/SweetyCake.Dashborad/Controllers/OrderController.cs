using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Repositories;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.BAL.Services.OrderService;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System.Drawing.Printing;

namespace SweetyCake.Dashborad.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
     

        public OrderController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }
        public async Task<IActionResult> Index()
        {
            string[] includes = new string[] { "OrderItems.Product" };

            try
            {
                var data = await this.orderRepository.FindAllAsync(includes);
                var OrderDtos = data.Adapt<List<OrderDto>>();

                return View(OrderDtos);

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
