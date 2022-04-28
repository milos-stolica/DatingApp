using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers.Pagination
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int) Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreatePagedList(IQueryable<T> query, int currentPage, int pageSize)
        {
            var count = await query.CountAsync();

            var items = await query
                                .Skip((currentPage - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return new PagedList<T>(items, count, currentPage, pageSize);
        }
    }
}
