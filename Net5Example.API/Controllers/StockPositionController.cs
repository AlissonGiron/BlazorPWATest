using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net5Example.Data;
using Net5Example.ViewModels;

namespace Net5Example.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockPositionController : ControllerBase
    {
        private readonly Context _context;

        public StockPositionController(Context context)
        {
            _context = context;
        }

        // GET: api/StockPosition
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockPosition>>> GetStockPositions()
        {
            return await _context.StockPositions.Include(s => s.Product).ToListAsync();
        }

        [HttpPost("Seed")]
        public IActionResult Seed()
        {
            _context.AddRange(new List<StockPosition>()
            {
                new StockPosition() { Column = 0, Row = 0, Quantity = 10, Product = new Product() { Name = "Válvula" } },
                new StockPosition() { Column = 1, Row = 0, Quantity = 15, Product = new Product() { Name = "Fresa" } },
                new StockPosition() { Column = 2, Row = 0, Quantity = 5, Product = new Product() { Name = "Parafuso" } },
                new StockPosition() { Column = 0, Row = 1, Quantity = 50, Product = new Product() { Name = "Lâmina" } },
                new StockPosition() { Column = 1, Row = 1, Quantity = 3, Product = new Product() { Name = "AG013" } },
                new StockPosition() { Column = 2, Row = 1, Quantity = 18, Product = new Product() { Name = "Fresa MD" } },
                new StockPosition() { Column = 0, Row = 2, Quantity = 10, Product = new Product() { Name = "Pino" } },
                new StockPosition() { Column = 1, Row = 2, Quantity = 2, Product = new Product() { Name = "Alicate" } },
                new StockPosition() { Column = 2, Row = 2, Quantity = 5, Product = new Product() { Name = "Prensa" } },
            });

            _context.SaveChanges();

            return Ok("Dados criados com sucesso");
        }

        // GET: api/StockPosition/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockPosition>> GetStockPosition(int id)
        {
            var stockPosition = await _context.StockPositions.FindAsync(id);

            if (stockPosition == null)
            {
                return NotFound();
            }

            return stockPosition;
        }

        // PUT: api/StockPosition/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStockPosition(int id, StockPosition stockPosition)
        {
            if (id != stockPosition.StockPositionId)
            {
                return BadRequest();
            }

            _context.Entry(stockPosition).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockPositionExists(id))
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

        // POST: api/StockPosition
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StockPosition>> PostStockPosition(StockPosition stockPosition)
        {
            _context.StockPositions.Add(stockPosition);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStockPosition", new { id = stockPosition.StockPositionId }, stockPosition);
        }

        // DELETE: api/StockPosition/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockPosition(int id)
        {
            var stockPosition = await _context.StockPositions.FindAsync(id);
            if (stockPosition == null)
            {
                return NotFound();
            }

            _context.StockPositions.Remove(stockPosition);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StockPositionExists(int id)
        {
            return _context.StockPositions.Any(e => e.StockPositionId == id);
        }
    }
}
