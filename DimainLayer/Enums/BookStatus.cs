using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimainLayer.Enums
{
    public  enum BookStatus
    {
        Available,   // موجود و قابل امانت
        Borrowed,    // در حال امانت
        Lost,        // مفقود یا خارج از چرخه
    }
}
