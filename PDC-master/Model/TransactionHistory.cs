namespace CS_CriptoCoinRest.Model
{
    public class TransactionHistory
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}