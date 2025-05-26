using CS_CriptoCoinRest.Grpc;
using CS_CriptoCoinRest.Model;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace CS_CriptoCoinRest.Services
{
    public class WalletGrpcService : WalletService.WalletServiceBase
    {
        private readonly CriptoCoinContext _context;

        public WalletGrpcService(CriptoCoinContext context)
        {
            _context = context;
        }

        public override async Task<GetHistoryReply> GetTransactionHistory(GetHistoryRequest request, ServerCallContext context)
        {
            var entries = await _context.Set<CS_CriptoCoinRest.Model.TransactionHistory>()
                .Where(t => t.WalletId == request.WalletId)
                .OrderBy(t => t.Timestamp)
                .ToListAsync();

            var reply = new GetHistoryReply();
            foreach (var t in entries)
            {
                reply.Entries.Add(new TransactionEntry
                {
                    Currency = t.Currency,
                    Amount = (double)t.Amount,
                    Timestamp = t.Timestamp.ToString("o")
                });
            }
            return reply;
        }
    }
}