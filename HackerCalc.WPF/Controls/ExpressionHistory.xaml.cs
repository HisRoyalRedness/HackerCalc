using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for ExpressionHistory.xaml
    /// </summary>
    public partial class ExpressionHistory : UserControl
    {
        public ExpressionHistory()
        {
            InitializeComponent();
        }

        private void ListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is ExpressionM expression)
                AppVM.Instance.SetExpression(expression.Input);
        }
    }
}
