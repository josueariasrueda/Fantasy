using Fantasy.Backend.Data;
using Fantasy.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ApplicationDataContext _context;

        public CountriesController(ApplicationDataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Countries.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return Ok(country);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Country country)
        {
            _context.Add(country);
            await _context.SaveChangesAsync();
            return Ok(country);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(Country country)
        {
            var currentcountry = await _context.Countries.FindAsync(country.Id);
            if (currentcountry==null)
            {
                return NotFound();
            }

            currentcountry.Name = country.Name;
            currentcountry.Code = country.Code;
            currentcountry.CallingCode = country.CallingCode;

            _context.Update(currentcountry);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var currentcountry = await _context.Countries.FindAsync(id);
            if (currentcountry == null)
            {
                return NotFound();
            }

            _context.Remove(currentcountry);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}