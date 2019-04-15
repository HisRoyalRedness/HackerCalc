using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for DataTypeControl.xaml
    /// </summary>
    public partial class DataTypeControl : UserControl
    {
        public DataTypeControl()
        {
            //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //    DataType = new FloatType(1.23);

            InitializeComponent();
            FontSize = 40;
            //DataContext = this;
        }

        public IDataType<DataType> DataType
        {
            get { return (IDataType<DataType>)GetValue(DataTypeProperty); }
            set { SetValue(DataTypeProperty, value); }
        }

        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register(
            nameof(DataType), 
            typeof(IDataType<DataType>), 
            typeof(DataTypeControl), 
            new PropertyMetadata(new FloatType(1.23), (o,e) =>
            {
                Console.WriteLine();
            }));




    }
}
