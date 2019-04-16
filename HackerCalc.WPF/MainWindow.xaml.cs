using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var vm = (DataContext as AppVM);
            if (vm == null)
                return;

            TextInput += (o, e) =>
            {
                foreach (var chr in e.Text.Replace("\r\n","\n").Replace("\r", "\n"))
                    switch (chr)
                    {
                        case '\n':
                        case '=':
                            (DataContext as AppVM)?.Enter();
                            break;

                        default:
                            (DataContext as AppVM)?.AddChar(chr);
                            break;
                    }
            };

            PreviewKeyDown += (o, e) =>
            {
                switch(e.Key)
                {
                    case Key.Escape:
                    case Key.Delete:
                        e.Handled = true;
                        (DataContext as AppVM)?.Clear();
                        break;

                    case Key.Back:
                        e.Handled = true;
                        (DataContext as AppVM)?.Back();
                        break;

                    default:
                        e.Handled = false;
                        break;
                }
            };
        }
    }
}
