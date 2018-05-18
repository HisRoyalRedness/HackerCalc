using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class OtherBasesVM : NotifyBase
    {
        public ILiteralToken Token
        {
            get { return _token; }
            set
            {
                if (SetProperty(ref _token, value))
                    UpdateNumbers(value);
            }
        }
        ILiteralToken _token = null;

        public BigInteger? IntegerValue
        {
            get { return _integerValue; }
            private set { SetProperty(ref _integerValue, value); }
        }
        BigInteger? _integerValue;

        public string Hexadecimal
        {
            get => _hexValue;
            private set { SetProperty(ref _hexValue, value); }
        }
        string _hexValue = "";

        public string Decimal
        {
            get => _decValue;
            private set { SetProperty(ref _decValue, value); }
        }
        string _decValue = "";

        public string Octal
        {
            get => _octValue;
            private set { SetProperty(ref _octValue, value); }
        }
        string _octValue = "";

        public string Binary
        {
            get => _binValue;
            private set { SetProperty(ref _binValue, value); }
        }
        string _binValue = "";

        void UpdateNumbers(ILiteralToken token)
        {
            switch (token?.GetType()?.Name)
            {
                case nameof(LimitedIntegerToken):
                    IntegerValue = ((LimitedIntegerToken)token).TypedValue;
                    break;

                case nameof(UnlimitedIntegerToken):
                    IntegerValue = ((UnlimitedIntegerToken)token).TypedValue;
                    break;

                default:
                    IntegerValue = null;
                    break;
            }

            Hexadecimal = token?.ToHex() ?? string.Empty;
            Decimal = token?.ToDec() ?? string.Empty;
            Octal = token?.ToOct() ?? string.Empty;
            Binary = token?.ToBin() ?? string.Empty;



        }
    }
}
