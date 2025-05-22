using Turnierplan.Core.SeedWork;

namespace Turnierplan.Dal.Extensions;

public static class UnitOfWorkExtensions
{
    public sealed class TransactionWrapper : IDisposable, IAsyncDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionWrapper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public bool ShouldCommit { private get; set; }

        public void Dispose()
        {
            throw new NotSupportedException($"Only use '{nameof(UnitOfWorkExtensions)}.{nameof(WrapTransactionAsync)}()' with 'await using ...;'");
        }

        public async ValueTask DisposeAsync()
        {
            if (ShouldCommit)
            {
                await _unitOfWork.CommitTransactionAsync().ConfigureAwait(false);
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync().ConfigureAwait(false);
            }
        }
    }

    public static async Task<TransactionWrapper> WrapTransactionAsync(this IUnitOfWork unitOfWork)
    {
        await unitOfWork.BeginTransactionAsync().ConfigureAwait(false);

        return new TransactionWrapper(unitOfWork);
    }
}
