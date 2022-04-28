namespace DatingApp.API.Helpers.Pagination
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }
    }
}
