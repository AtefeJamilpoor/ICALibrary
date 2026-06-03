using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;

namespace ServiceLayer.DTO
{
    public class BookInfoPartial
    {
        public Book Book { get; set; }
        public bool IsBorrowed {  get; set; }
        public bool IsReserver {  get; set; }
        public bool HasLate {  get; set; }
        public DateTime? returnDate { get; set; }
        public DateTime? accessDate { get; set; }
        public DateTime? lastAccessDate { get; private set; }

    }
}
