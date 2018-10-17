using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class DateType : DataTypeBase<DateTime, DateType>
    {
        public DateType(DateTime value)
            : base(value, DataType.Date)
        { }
    }
}
