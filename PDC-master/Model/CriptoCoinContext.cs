using Microsoft.EntityFrameworkCore;

namespace CS_CriptoCoinRest.Model
{
    public class CriptoCoinContext : DbContext
    {
        public CriptoCoinContext(DbContextOptions<CriptoCoinContext> options)
            : base(options)
        {
        }

        public DbSet<CriptoCoin> CriptoCoinItens { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletItem> WalletItems { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }
    }
}
