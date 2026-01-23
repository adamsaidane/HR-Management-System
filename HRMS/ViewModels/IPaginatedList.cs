namespace HRMS.ViewModels;

public interface IPaginatedList
{
    int PageIndex { get; }
    int TotalPages { get; }
    int PageSize { get; }
    int TotalCount { get; }

    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}
