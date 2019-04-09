using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface IConfiguration
    {
        bool AllowMultidayTimes { get; }
        bool IgnoreLimitedIntegerMaxMinRange { get; }
        bool AllowOverOrUnderflow { get; }
        bool AllowSignChange { get; }

        ICalcState State { get; }
    }

    public class Configuration : IConfiguration
    {
        public bool IgnoreLimitedIntegerMaxMinRange { get; set; } = false;
        public bool AllowMultidayTimes { get; set; } = false;
        public bool AllowOverOrUnderflow { get; set; } = true;
        public bool AllowSignChange { get; set; } = true;

        public ICalcState State { get; } = new CalcState();
    }
}
