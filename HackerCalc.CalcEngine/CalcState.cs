using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class CalcState : NotifyBase, ICalcState
    {
        public void Reset()
        {
            OverflowOccurred = false;
            UnderflowOccurred = false;
        }

        public bool OverflowOccurred
        {
            get => _overflowOccurred;
            set => SetProperty(ref _overflowOccurred, value);
        }
        bool _overflowOccurred = true;

        public bool UnderflowOccurred
        {
            get => _underflowOccurred;
            set => SetProperty(ref _underflowOccurred, value);
        }
        bool _underflowOccurred = true;

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (UnderflowOccurred)
                sb.AppendLine("Underflow");
            if (OverflowOccurred)
                sb.AppendLine("Overflow");
            return sb.ToString();
        }
    }
}
