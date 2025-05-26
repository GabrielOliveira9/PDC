namespace CS_CriptoCoinRest.Model
{
    public class Wallet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<WalletItem> Items { get; set; } = new List<WalletItem>();
    }

    public class WalletItem
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public Wallet Wallet { get; set; } = null!;
    }
    public class WalletItemDto
    {
        public string Currency { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class WalletDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<WalletItemDto> Items { get; set; } = new();
    }

}
