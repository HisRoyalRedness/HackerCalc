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

        protected override TNewType InternalCastTo<TNewType>()
        {
            if (typeof(TNewType).IsAssignableFrom(GetType()))
                return this as TNewType;

            //switch (typeof(TToken).Name)
            //{
            //    case nameof(OldFloatToken):
            //        return new OldFloatToken((double)TypedValue) as TToken;

            //    case nameof(OldUnlimitedIntegerToken):
            //        return new OldUnlimitedIntegerToken(TypedValue) as TToken;

            //    case nameof(OldTimespanToken):
            //        return new OldTimespanToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

            //    case nameof(OldTimeToken):
            //        return new OldTimeToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

            //    case nameof(OldDateToken):
            //        return new OldDateToken(DateTime.Now.Date + TimeSpan.FromSeconds((double)TypedValue)) as TToken;

            //    default:
            //        return null;
            //}
        }    
    }
