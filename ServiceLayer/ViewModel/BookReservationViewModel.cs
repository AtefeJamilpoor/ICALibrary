using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;

namespace ServiceLayer.ViewModel
{
    public class BookReservationViewModel
    {
        //public Book Book { get; set; }
        //public Reservation Reservation { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Barcode { get; set; }
        public string ReserveDate { get; set; }
        public string DueDate { get; set; }
        public string? ReturnDate { get; set; }

    }
}
