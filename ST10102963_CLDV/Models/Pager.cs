namespace ST10102963_CLDV.Models
{
    /* Code reference: https://youtu.be/O57nsLyZubc?si=dFWp2kaoQykl0Aqm */

    public class Pager
    {
        public int TotalProducts { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager()
        {

        }

        public Pager(int totalProducts, int page, int pageSize = 10)
        {
          int totalPages =  (int)Math.Ceiling((decimal)totalProducts / (decimal)pageSize);
            int currenPage = page;

            int startPage = CurrentPage - 5;
            int endPage = CurrentPage + 4;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;

                if(endPage > totalPages)
                {
                    endPage = totalPages;
                    if(endPage > 10)
                    {
                        startPage = endPage - 9;
                    }
                }

                TotalProducts = totalProducts;
                CurrentPage = currenPage;
                PageSize = pageSize;
                TotalPages = totalPages;
                StartPage = startPage;
                EndPage = endPage;
            }
        }
    }
}
