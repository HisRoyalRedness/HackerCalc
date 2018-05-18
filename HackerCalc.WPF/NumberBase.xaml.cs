using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HisRoyalRedness.com
{
    /// <summary>
    /// Interaction logic for NumberBase.xaml
    /// </summary>
    public partial class NumberBase : UserControl
    {
        public NumberBase()
        {
            InitializeComponent();
        }

        public ILiteralToken Token
        {
            get { return (ILiteralToken)GetValue(TokenProperty); }
            set { SetValue(TokenProperty, value); }
        }

        public static readonly DependencyProperty TokenProperty = DependencyProperty.Register(
            nameof(Token), 
            typeof(ILiteralToken), 
            typeof(NumberBase), 
            new PropertyMetadata(
                null, 
                (o, e) => (o as NumberBase)?.Recalc()));

        public int Base
        {
            get { return (int)GetValue(BaseProperty); }
            set { SetValue(BaseProperty, value); }
        }

        public static readonly DependencyProperty BaseProperty = DependencyProperty.Register(
            nameof(Base), 
            typeof(int), 
            typeof(NumberBase), 
            new PropertyMetadata(
                10, 
                (o, e) => (o as NumberBase)?.Recalc()),
                ValidateBase);

        static bool ValidateBase(object baseNum)
        {
            switch (baseNum as int?)
            {
                case 2:
                case 8:
                case 10:
                case 16:
                    return true;
                default:
                    return false;
            }
        }

        void Recalc()
        {
            switch (Token?.GetType()?.Name)
            {
                case nameof(LimitedIntegerToken):
                    ConvertToString((LimitedIntegerToken)Token, Base);
                    break;

                case nameof(UnlimitedIntegerToken):
                    ConvertToString((UnlimitedIntegerToken)Token, Base);
                    break;

                default:
                    numRun.Text = "";
                    baseRun.Text = "";
                    return;
            }
        }

        void ConvertToString(ILiteralToken token, int numBase)
        {
            var baseStr = numBase.ToString();
            var numStr = "";
            switch (numBase)
            {
                case 16:
                    numStr = token.ToHex();
                    break;
                case 10:
                    numStr = token.ToDec();
                    break;
                case 8:
                    numStr = token.ToOct();
                    break;
                case 2:
                    numStr = token.ToBin();
                    break;
                default:
                    baseStr = "";
                    return;
            }

            numRun.Text = numStr;
            baseRun.Text = baseStr;
        }
    }
}
