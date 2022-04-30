namespace DatingApp.API.Helpers.Pagination
{
    public class PaginationParams
    {
        private int maxPageSize = 50;
        private int pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}