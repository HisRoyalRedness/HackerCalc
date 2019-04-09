using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface ICalcState
    {
        bool OverOrUnderflowOccurred { get; }
        bool SignChangedOccurred { get; }
        void Reset();
    }

    public class CalcState : ICalcState
    {
        public void Reset()
        {
            OverOrUnderflowOccurred = false;
            SignChangedOccurred = false;
        }

        public bool OverOrUnderflowOccurred { get; set; } = false;
        public bool SignChangedOccurred { get; set; } = false;

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
