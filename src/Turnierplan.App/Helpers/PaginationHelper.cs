using Turnierplan.App.Mapping;
using Turnierplan.App.Models;

namespace Turnierplan.App.Helpers;

internal static class PaginationHelper
{
    public static PaginationResultDto<TDto> Process<TDomain, TDto>(IEnumerable<TDomain> source, int page, int pageSize, IMapper mapper, string languageCode = IMapper.DefaultLanguageCode)
        where TDomain : class
        where TDto : class
    {
        var itemsList = source.ToList();
        var pageOffset = page * pageSize;
        var totalItems = itemsList.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (float)pageSize);

        if (pageOffset >= totalItems)
        {
            return new PaginationResultDto<TDto>
            {
                Items = [],
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasNextPage = false
            };
        }

        // Remove all items from the list start until the beginning of the first page
        itemsList.RemoveRange(0, pageOffset);

        var hasNextPage = itemsList.Count > pageSize;

        if (hasNextPage)
        {
            // Remove all items after the target page until the end of the list
            var surplusCount = itemsList.Count - pageSize;
            itemsList.RemoveRange(pageSize, surplusCount);
        }

        return new PaginationResultDto<TDto>
        {
            Items = mapper.MapCollection<TDto>(itemsList, languageCode).ToArray(),
            CurrentPage = page,
            ItemsPerPage = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = hasNextPage
        };
    }
}
