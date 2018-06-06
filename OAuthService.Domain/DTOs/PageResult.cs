using System.Collections.Generic;

namespace OAuthService.Domain.DTOs
{
    public class PageResult<T>
    {
        public PageResult(int totalRecord, IEnumerable<T> items)
        {
            TotalRecord = totalRecord;
            Items = items;
        }

        public PageResult(int totalRecord, int totalPage, IEnumerable<T> items)
        {
            TotalRecord = totalRecord;
            TotalPage = totalPage;
            Items = items;
        }

        public int TotalRecord { get; set; }

        public int TotalPage { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
