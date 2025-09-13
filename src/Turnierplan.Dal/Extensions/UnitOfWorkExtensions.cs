using Turnierplan.Core.SeedWork;

namespace Turnierplan.Dal.Extensions;

public static class UnitOfWorkExtensions
{
    public sealed class TransactionWrapper : IAsyncDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionWrapper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public bool ShouldCommit { private get; set; }

        public async ValueTask DisposeAsync()
        {
            if (ShouldCommit)
            {
                await _unitOfWork.CommitTransactionAsync();
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync();
            }
        }
    }

    public static async Task<TransactionWrapper> WrapTransactionAsync(this IUnitOfWork unitOfWork)
    {
        await unitOfWork.BeginTransactionAsync();

        return new TransactionWrapper(unitOfWork);
    }
}
