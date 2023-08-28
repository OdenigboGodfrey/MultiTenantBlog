using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenantBlogTest.src.Shared.ViewModels
{
    public class Page<T>
    {
        public long PageSize { get; }
        public long PageNumber { get; }
        public long TotalSize { get; }
        public T[] Items { get; set; }

        public Page(T[] items, long totalSize, long pageNumber, long pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            TotalSize = totalSize;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Items = items;
        }
    }

}
