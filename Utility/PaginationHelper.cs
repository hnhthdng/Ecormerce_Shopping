using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class PaginationHelper<T>
    {
        private int currentPage;
        private int itemsPerPage;

        public PaginationHelper(int itemsPerPage)
        {
            this.currentPage = 1; // Bắt đầu từ trang 1
            this.itemsPerPage = itemsPerPage;
        }

        public int CurrentPage
        {
            get { return currentPage; }
        }

        public int TotalPages(IEnumerable<T> list)
        {
            return (int)Math.Ceiling((double)list.Count() / itemsPerPage);
        }

        public IEnumerable<T> GetPagedData(IEnumerable<T> list)
        {
            return list.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
        }

        public void NextPage(IEnumerable<T> list)
        {
            if (currentPage < TotalPages(list))
            {
                currentPage++;
            }
        }

        public void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
            }
        }

        public void GoToPage(int page, IEnumerable<T> list)
        {
            if (page >= 1 && page <= TotalPages(list))
            {
                currentPage = page;
            }
        }
    }

}
