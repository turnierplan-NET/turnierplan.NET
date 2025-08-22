namespace Turnierplan.App.Models;

public sealed record PaginationResultDto<T>
{
    public required T[] Items { get; init; }

    public required int CurrentPage { get; init; }

    public required int ItemsPerPage { get; init; }

    public required int TotalItems { get; init; }

    public required int TotalPages { get; init; }

    public required bool HasNextPage { get; init; }
}
