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
    public partial class DataTypeControl : UserControl
    {
        public DataTypeControl()
        {
            InitializeComponent();
            FontSize = 40;
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
            new PropertyMetadata(new RationalNumberType(0)));
    }
}
