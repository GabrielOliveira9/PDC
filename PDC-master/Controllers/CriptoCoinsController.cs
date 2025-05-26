using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS_CriptoCoinRest.Model;

namespace CS_CriptoCoinRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriptoCoinsController : ControllerBase
    {
        private readonly CriptoCoinContext _context;

        public CriptoCoinsController(CriptoCoinContext context)
        {
            _context = context;
        }

        // GET: api/CriptoCoins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CriptoCoin>>> GetCriptoCoinItens()
        {
            if (_context.CriptoCoinItens == null)
            {
                return NotFound();
            }
            return await _context.CriptoCoinItens.ToListAsync();
        }

        // GET: api/CriptoCoins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CriptoCoin>> GetCriptoCoin(int id)
        {
            if (_context.CriptoCoinItens == null)
            {
                return NotFound();
            }
            var criptoCoin = await _context.CriptoCoinItens.FindAsync(id);

            return criptoCoin;
        }

        // PUT: api/CriptoCoins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCriptoCoin(int id, CriptoCoin criptoCoin)
        {
            if (id != criptoCoin.Id)
            {
                return BadRequest();
            }

            _context.Entry(criptoCoin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CriptoCoinExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CriptoCoins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CriptoCoin>> PostCriptoCoin(CriptoCoin criptoCoin)
        {
            _context.CriptoCoinItens.Add(criptoCoin);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCriptoCoin", new { id = criptoCoin.Id }, criptoCoin);
        }

        // DELETE: api/CriptoCoins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCriptoCoin(int id)
        {
            if (_context.CriptoCoinItens == null)
            {
                return NotFound();
            }
            var criptoCoin = await _context.CriptoCoinItens.FindAsync(id);
            if(criptoCoin == null)
            {
                return NotFound();
            }

            _context.CriptoCoinItens.Remove(criptoCoin);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CriptoCoinExists(int id)
        {
            return _context.CriptoCoinItens.Any(e => e.Id == id);
        }
    }
}
