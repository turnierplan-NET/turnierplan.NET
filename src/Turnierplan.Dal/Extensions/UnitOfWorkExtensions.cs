using Turnierplan.Dal.UnitOfWork;

namespace Turnierplan.Dal.Extensions;

public static class TurnierplanContextExtensions
{
    extension(IUnitOfWork unitOfWork)
    {
        public async Task<TransactionWrapper> WrapTransactionAsync()
        {
            await unitOfWork.BeginTransactionAsync();

            return new TransactionWrapper(unitOfWork);
        }
    }

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
}
