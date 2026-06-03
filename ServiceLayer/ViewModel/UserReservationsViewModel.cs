using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;
using ServiceLayer.DTO;

namespace ServiceLayer.ViewModel
{
    public class UserReservationsViewModel
    {
        public List<BookInfoPartial> BorrowedBooks {  get; set; }
        public List<BookInfoPartial> ReservedBooks { get; set; }
        public List<BookInfoPartial> OverdueBooks { get; set; }
        public  BookInfoPartial bookInfo { get; set; }
        public bool hasAvalableCard { get;set; }
    }
}
