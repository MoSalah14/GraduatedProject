using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Dto.WishList;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.Brands;
using OutbornE_commerce.BAL.Repositories.Categories;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.BAL.Repositories.Tickets;
using OutbornE_commerce.BAL.Repositories.WishList;
using OutbornE_commerce.FilesManager;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly UserManager<User> _user;
        private readonly IOrderRepository _orderRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWishListRepo wishListRepo;
        private readonly ITicketRepository ticketRepository;

        private readonly IFilesManager filesManager;

        public ReportController(IFilesManager filesManager, IWishListRepo wishListRepo, ITicketRepository ticketRepository, IOrderRepository OrderRepository, IProductRepository ProductRepository, IBrandRepository BrandRepository, UserManager<User> User, ICategoryRepository categoryRepository)
        {
            _orderRepository = OrderRepository;
            _productRepository = ProductRepository;
            _brandRepository = BrandRepository;
            _user = User;
            _categoryRepository = categoryRepository;
            this.wishListRepo = wishListRepo;
            this.ticketRepository = ticketRepository;
            this.filesManager = filesManager;
        }

        [HttpGet("GetAllTotalProduct")]
        public async Task<IActionResult> GetAllTotalProduct()
        {
            var totalProducts = await _productRepository.CountAsync();

            if (totalProducts == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No products found",
                    MessageAr = "لا يوجد منتجات",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = totalProducts,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTotalBrands")]
        public async Task<IActionResult> GetAllTotalBrands()
        {
            var TotalBrands = await _brandRepository.CountAsync();

            if (TotalBrands == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Brands found",
                    MessageAr = "لا يوجد اي براند",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = TotalBrands,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTotalOrder")]
        public async Task<IActionResult> GetAllTotalOrder()
        {
            var TotalOrder = await _orderRepository.CountAsync();

            if (TotalOrder == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Orders found",
                    MessageAr = "لم يتم العثور علي اي طلبات",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = TotalOrder,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTotalUser")]
        public async Task<IActionResult> GetAllTotalUser()
        {
            var usersInRole = await _user.GetUsersInRoleAsync(AccountTypeEnum.User.ToString());

            var totalUsers = usersInRole.Count();

            if (totalUsers == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No users found",
                    MessageAr = "لم يتم العثور علي مستخدمين",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = totalUsers,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTotalCategory")]
        public async Task<IActionResult> GetAllTotalCategory()
        {
            var totalCategory = await _categoryRepository.CountAsync();

            if (totalCategory == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Categories found",
                    MessageAr = "لم يتم العثور علي اي فئات",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = totalCategory,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTotalPendingOrders")]
        public async Task<IActionResult> GetAllTotalPendingOrders()
        {
            var totalPendingOrder = await _orderRepository.CountAsync(o => o.OrderStatus == OrderStatus.Pending);

            if (totalPendingOrder == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No PendingOrders found",
                    MessageAr = "لم يتم العثور علي اي طلبات معلقة",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = totalPendingOrder,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTotalConfirmedOrders")]
        public async Task<IActionResult> GetAllTotalConfirmedOrders()
        {
            var totalPendingOrder = await _orderRepository.CountAsync(o => o.OrderStatus == OrderStatus.Confirmed);

            if (totalPendingOrder == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Confirmed Orders found",
                    MessageAr = "لم يتم العثور علي اي طلبات منتهية",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = totalPendingOrder,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTotalPlacedOrders")]
        public async Task<IActionResult> GetAllTotalPlacedOrders()
        {
            var totalPlacedOrder = await _orderRepository.CountAsync(o => o.ShippedStatus == ShippedStatus.Delivered);

            if (totalPlacedOrder == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Placed Orders found",
                    MessageAr = "لم يتم العثور علي اي طلبات مستلمة",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = totalPlacedOrder,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllWishListForSpecificProduct")]
        public async Task<IActionResult> GetAllWishListForSpecificProduct(Guid productId)
        {
            var wishListReport = await _productRepository.GetWishListByProduct(productId);

            if (wishListReport == null)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No wish lists found for the specified product.",
                    MessageAr = "لم يتم العثور على قوائم الرغبات للمنتج المحدد",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = wishListReport,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        //[HttpGet("GetProductReportByCategory/{categoryId}")]
        //public async Task<IActionResult> GetProductReportByCategory(Guid categoryId)
        //{
        //    var productReports = await _productRepository.GetProductsByCategory(categoryId);

        //    if (productReports == null || !productReports.Any())
        //    {
        //        return NotFound(new Response<List<ProductReport>>
        //        {
        //            Data = null,
        //            Message = "No ProductList found for the specified Category",
        //            MessageAr = "لم يتم العثور على قائمة المنتجات للفئة المحددة",
        //            IsError = true,
        //            Status = (int)StatusCodeEnum.NotFound
        //        });
        //    }

        //    return Ok(new Response<List<ProductReport>>
        //    {
        //        Data = productReports,
        //        IsError = false,
        //        Status = (int)StatusCodeEnum.Ok
        //    });
        //}

        [HttpGet("ExportSalesReport")]
        public async Task<IActionResult> GetSalesReport([FromQuery] SalesReportSearch salesReport)
        {
            var productReports = await _productRepository.GetSalesReportAsync(salesReport);

            if (productReports == null || !productReports.Any())
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    Message = "No Product List found for the specified Category",
                    MessageAr = "لم يتم العثور علي منتجات للفئة المحددة",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var excelData = filesManager.ExportSalesReportToExcelAsync(productReports);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesReport.xlsx");
        }

        [HttpGet("GetAllTotalTickets")]
        public async Task<IActionResult> GetAllTotalTickets()
        {
            var total = await ticketRepository.FindAllAsync(null, true);

            if (total == null)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Tickets found",
                    MessageAr = "لم يتم العثور علي اي تذاكر",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = new
                {
                    PenddingCount = total.Where(x => x.Status == TicketStatus.Pending).Count(),
                    SolvedCount = total.Where(x => x.Status == TicketStatus.Sloved).Count(),
                    WorkOnCount = total.Where(x => x.Status == TicketStatus.WorkingOn).Count(),
                    ClosededCount = total.Where(x => x.Status == TicketStatus.Closed).Count(),
                },
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTotalSolvedTickets")]
        public async Task<IActionResult> GetAllTotalSolvedTickets()
        {
            var total = await ticketRepository.CountAsync(o => o.Status == TicketStatus.Sloved);

            if (total == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Solved Tickets found",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = total,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpGet("GetAllTotalWorkingOnTickets")]
        public async Task<IActionResult> GetAllTotalWorkingOnTickets()
        {
            var total = await ticketRepository.CountAsync(o => o.Status == TicketStatus.WorkingOn);

            if (total == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No WorkingOn Ticket found",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = total,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTotalClosedTickets")]
        public async Task<IActionResult> GetAllTotalClosedTickets()
        {
            var total = await ticketRepository.CountAsync(o => o.Status == TicketStatus.Closed);

            if (total == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Closed Tickets found",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = total,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpGet("GetWishListReport")]
        public async Task<IActionResult> GetWishListReport(CancellationToken cancellationToken)
        {
            try
            {
                var report = await _productRepository.GetWishListProductAsync(cancellationToken);

                return Ok(new Response<List<WishListReportDto>>
                {
                    Data = report,
                    IsError = false,
                    Message = "WishList report generated successfully",
                    MessageAr = "تم إنشاء تقرير قائمة الرغبات بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<object>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while generating the wishlist report",
                    MessageAr = "حدث خطأ أثناء إنشاء تقرير قائمة الرغبات",
                });
            }
        }

        [HttpGet("GetAllTotalClosedTicketsCount")]
        public async Task<IActionResult> GetAllTotalClosedTicketsCount()
        {
            var total = await ticketRepository.CountAsync(o => o.Status == TicketStatus.Closed);

            if (total == 0)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Closed Tickets found",
                    MessageAr = "لم يتم العثور علي تذامر مغلقة",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<object>
            {
                Data = total,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}