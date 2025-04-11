using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.FreeDeliverys;
using OutbornE_commerce.DAL.Data;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreeDeliveryController : ControllerBase
    {
        private readonly FreeDeliveryRepo freeDeliveryRepo;

        public FreeDeliveryController(FreeDeliveryRepo freeDeliveryRepo)
        {
            this.freeDeliveryRepo = freeDeliveryRepo;
        }

        // GET: api/FreeDelivery
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<FreeDelivery>>> GetFreeDeliveries()
        //{
        //    return await repository.Include(fd => fd.Country).ToListAsync();
        //}

        // GET: api/FreeDelivery/{id}
        [HttpGet]
        public async Task<IActionResult> GetFreeDelivery()
        {
            var freeDelivery = await freeDeliveryRepo.GetFreeDelivery();
            if (freeDelivery == null)
                return NotFound();

            return Ok(freeDelivery);
        }

        // POST: api/FreeDelivery
        //[HttpPost]
        //public async Task<ActionResult<FreeDelivery>> CreateFreeDelivery(FreeDelivery freeDelivery)
        //{
        //    // Ensure the related country exists
        //    var countryExists = await _context.Countries.AnyAsync(c => c.Id == freeDelivery.CountryId);
        //    if (!countryExists)
        //    {
        //        return BadRequest("Invalid Country ID.");
        //    }

        //    _context.FreeDeliveries.Add(freeDelivery);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetFreeDelivery), new { id = freeDelivery.Id }, freeDelivery);
        //}

        // PUT: api/FreeDelivery/{id}
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateFreeDelivery(FreeDeliveryDto freeDelivery, CancellationToken cancellationToken)
        {
            // Ensure the related country exists
            var countryExists = await freeDeliveryRepo.UpdateFreeDelivery(freeDelivery, cancellationToken);

            return Ok(countryExists);
        }

        //// DELETE: api/FreeDelivery/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteFreeDelivery(Guid id)
        //{
        //    var freeDelivery = await _context.FreeDeliveries.FindAsync(id);
        //    if (freeDelivery == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.FreeDeliveries.Remove(freeDelivery);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool FreeDeliveryExists(Guid id)
        //{
        //    return _context.FreeDeliveries.Any(e => e.Id == id);
        //}
    }
}