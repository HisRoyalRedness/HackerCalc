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
            OverOrUnderflowOccurred = false;
        }

        public bool OverOrUnderflowOccurred
        {
            get => _overOrUnderflowOccurred;
            set => SetProperty(ref _overOrUnderflowOccurred, value);
        }
        bool _overOrUnderflowOccurred = true;

        public bool SignChangedOccurred
        {
            get => _signChangeOccurred;
            set => SetProperty(ref _signChangeOccurred, value);
        }
        bool _signChangeOccurred = true;

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (OverOrUnderflowOccurred)
                sb.AppendLine("Overflow or Underflow");
            if (SignChangedOccurred)
                sb.AppendLine("Sign change");
            return sb.ToString();
        }
    }
}
