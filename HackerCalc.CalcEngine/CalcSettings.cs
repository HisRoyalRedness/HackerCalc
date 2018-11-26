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
        public bool AllowOverflow
        {
            get => _allowOverflow;
            set => SetProperty(ref _allowOverflow, value);
        }
        bool _allowOverflow = true;

        public bool AllowUnderflow
        {
            get => _allowUnderflow;
            set => SetProperty(ref _allowUnderflow, value);
        }
        bool _allowUnderflow = true;

        public bool AllowSignChange
        {
            get => _allowSignChange;
            set => SetProperty(ref _allowSignChange, value);
        }
        bool _allowSignChange = true;
    }
}
