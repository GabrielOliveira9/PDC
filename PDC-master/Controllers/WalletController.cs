using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS_CriptoCoinRest.Model;

namespace CS_CriptoCoinRest.Controllers
{
    public class WalletItemDto { public string Currency { get; set; } = string.Empty; public decimal Amount { get; set; } }
    public class WalletDto { public int Id { get; set; } public string Name { get; set; } = string.Empty; public int ItemCount { get; set; } public decimal TotalAmount { get; set; } public ICollection<WalletItemDto> Items { get; set; } = new List<WalletItemDto>(); }
    public class AddWalletItemRequest { public string Currency { get; set; } = string.Empty; public decimal Amount { get; set; } }
    public class TransactionHistoryDto { public string Currency { get; set; } = string.Empty; public decimal Amount { get; set; } public string Timestamp { get; set; } = string.Empty; }
    public class TransferRequest { public int DestinationWalletId { get; set; } public string Currency { get; set; } = string.Empty; public decimal Amount { get; set; } }

    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly CriptoCoinContext _context;
        public WalletsController(CriptoCoinContext context) => _context = context;

        // GET: Lista todas as carteiras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WalletDto>>> GetWallets()
        {
            var wallets = await _context.Wallets
                .Include(w => w.Items)
                .Select(w => new WalletDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    ItemCount = w.Items.Count,
                    TotalAmount = w.Items.Sum(i => i.Amount),
                    Items = w.Items.Select(i => new WalletItemDto { Currency = i.Currency, Amount = i.Amount }).ToList()
                })
                .ToListAsync();

            return Ok(wallets);
        }


        // GET: Descreve a carteira desejada
        [HttpGet("{id}")]
        public async Task<ActionResult<WalletDto>> GetWallet(int id)
        {
            var dto = await _context.Wallets
                .Include(w => w.Items)
                .Where(w => w.Id == id)
                .Select(w => new WalletDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    ItemCount = w.Items.Count,
                    TotalAmount = w.Items.Sum(i => i.Amount),
                    Items = w.Items.Select(i => new WalletItemDto { Currency = i.Currency, Amount = i.Amount }).ToList()
                })
                .FirstOrDefaultAsync();
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        //POST: Cria uma nova carteira
        [HttpPost]
        public async Task<ActionResult<WalletDto>> CreateWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            var dto = new WalletDto { Id = wallet.Id, Name = wallet.Name, ItemCount = 0, TotalAmount = 0, Items = new List<WalletItemDto>() };
            return CreatedAtAction(nameof(GetWallet), new { id = dto.Id }, dto);
        }

        //PUT: Atualiza uma carteira
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWallet(int id, Wallet wallet)
        {
            if (id != wallet.Id) return BadRequest();
            _context.Entry(wallet).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
        //DELETE: Deleta a carteira selecionada
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null) return NotFound();
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Wallets/{id}/items
        [HttpPost("{id}/items")]
        public async Task<ActionResult<WalletItemDto>> AddItem(int id, AddWalletItemRequest request)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null) return NotFound();

            var existing = await _context.WalletItems.FirstOrDefaultAsync(i => i.WalletId == id && i.Currency == request.Currency);
            if (existing != null)
            {
                existing.Amount += request.Amount;
                // registra histórico
                _context.TransactionHistories.Add(new TransactionHistory
                {
                    WalletId = id,
                    Currency = request.Currency,
                    Amount = request.Amount,
                    Timestamp = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();

                return Ok(new WalletItemDto { Currency = existing.Currency, Amount = existing.Amount });
            }

            var entity = new WalletItem { WalletId = id, Currency = request.Currency, Amount = request.Amount };
            _context.WalletItems.Add(entity);
            // registra histórico
            _context.TransactionHistories.Add(new TransactionHistory
            {
                WalletId = id,
                Currency = request.Currency,
                Amount = request.Amount,
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWallet), new { id }, new WalletItemDto { Currency = entity.Currency, Amount = entity.Amount });
        }

        // POST: api/Wallets/{id}/transfer
        [HttpPost("{id}/transfer")]
        public async Task<IActionResult> Transfer(int id, TransferRequest req)
        {
            var source = await _context.WalletItems.FirstOrDefaultAsync(i => i.WalletId == id && i.Currency == req.Currency);
            if (source == null || source.Amount < req.Amount) return BadRequest("Insufficient balance");

            var dest = await _context.WalletItems.FirstOrDefaultAsync(i => i.WalletId == req.DestinationWalletId && i.Currency == req.Currency);
            if (dest == null)
            {
                dest = new WalletItem { WalletId = req.DestinationWalletId, Currency = req.Currency, Amount = 0 };
                _context.WalletItems.Add(dest);
            }

            source.Amount -= req.Amount;
            dest.Amount += req.Amount;

            // registra histórico de débito
            _context.TransactionHistories.Add(new TransactionHistory
            {
                WalletId = id,
                Currency = req.Currency,
                Amount = -req.Amount,
                Timestamp = DateTime.UtcNow
            });
            // registra histórico de crédito
            _context.TransactionHistories.Add(new TransactionHistory
            {
                WalletId = req.DestinationWalletId,
                Currency = req.Currency,
                Amount = req.Amount,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET histórico de transações de uma carteira
        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<TransactionHistoryDto>>> GetTransactionHistory(int id)
        {
            var exists = await _context.Wallets.AnyAsync(w => w.Id == id);
            if (!exists) return NotFound();

            var history = await _context.TransactionHistories
                .Where(t => t.WalletId == id)
                .OrderBy(t => t.Timestamp)
                .Select(t => new TransactionHistoryDto
                {
                    Currency = t.Currency,
                    Amount = t.Amount,
                    Timestamp = t.Timestamp.ToString("o")
                }).ToListAsync();

            return Ok(history);
        }
    }
}