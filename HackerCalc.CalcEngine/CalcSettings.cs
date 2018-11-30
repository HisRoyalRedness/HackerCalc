using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class CalcSettings : NotifyBase, ICalcSettings
    {
        public bool AllowOverOrUnderflow
        {
            get => _allowOverOrUnderflow;
            set => SetProperty(ref _allowOverOrUnderflow, value);
        }
        bool _allowOverOrUnderflow = true;

        public bool AllowSignChange
        {
            get => _allowSignChange;
            set => SetProperty(ref _allowSignChange, value);
        }
        bool _allowSignChange = true;
    }
}
