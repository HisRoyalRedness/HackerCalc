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
    }

    public class Configuration : IConfiguration
    {
        public static Configuration Default { get; } = new Configuration()
        {
            IgnoreLimitedIntegerMaxMinRange = false,
            AllowMultidayTimes = false
        };


        public bool IgnoreLimitedIntegerMaxMinRange { get; set; }
        public bool AllowMultidayTimes { get; set; }
    }
}
