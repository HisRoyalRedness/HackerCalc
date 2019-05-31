using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for GroupedDigitControl.xaml
    /// </summary>
    public partial class GroupedDigitControl : UserControl
    {
        public GroupedDigitControl()
        {
            InitializeComponent();
        }

        public string Number
        {
            get { return (string)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
            nameof(Number), 
            typeof(string), 
            typeof(GroupedDigitControl),
            new PropertyMetadata("", new PropertyChangedCallback((o, e) => CreateGroups(o, (string)e.NewValue))));

        public uint GroupSize
        {
            get { return (uint)GetValue(GroupSizeProperty); }
            set { SetValue(GroupSizeProperty, value); }
        }

        public static readonly DependencyProperty GroupSizeProperty = DependencyProperty.Register(
            nameof(GroupSize),
            typeof(uint),
            typeof(GroupedDigitControl),
            new PropertyMetadata((uint)0, new PropertyChangedCallback((o, e) => CreateGroups(o, (uint)e.NewValue))));

        public string Sign
        {
            get { return (string)GetValue(SignProperty); }
            private set { SetValue(SignProperty, value); }
        }

        public static readonly DependencyProperty SignProperty = DependencyProperty.Register(
            nameof(Sign), 
            typeof(string), 
            typeof(GroupedDigitControl), 
            new PropertyMetadata(""));

        static void CreateGroups(DependencyObject o, uint groupSize)
        {
            if (o is GroupedDigitControl gdc)
                gdc.CreateGroups(gdc.Number, groupSize);
        }

        static void CreateGroups(DependencyObject o, string number)
        {
            if (o is GroupedDigitControl gdc)
                gdc.CreateGroups(number, gdc.GroupSize);
        }

        void CreateGroups(string number, uint groupSize)
        {
            Groups.Clear();

            var num = number?.Trim() ?? string.Empty;
            Sign = num.StartsWith("-") ? "-" : string.Empty;
            num = num.TrimStart(new[] { '-', ' ' });

            var leadingChar = num.Length > 0 ? num[0] : '\0';

            if (groupSize <= 1)
            {
                Groups.Add(new NumberGroup(num, leadingChar));
                return;
            }

            var pad = (int)(num.Length % GroupSize);
            if (pad > 0)
                num = num.PadLeft(num.Length + ((int)GroupSize - pad), '0');

            foreach (var grp in num.Batch((int)GroupSize))
            {
                var numGroup = new NumberGroup(new string(grp.ToArray()), leadingChar);
                if (leadingChar != '\0' && !numGroup.AllLeadingsChars)
                    leadingChar = '\0';
                Groups.Add(numGroup);
            }
        }

        public ObservableCollection<NumberGroup> Groups { get; } = new ObservableCollection<NumberGroup>();

        public class NumberGroup : ViewModelBase
        {
            const byte LEADING_COLOUR_BYTE = 175;
            static readonly Color LEADING_COLOUR = Color.FromRgb(LEADING_COLOUR_BYTE, LEADING_COLOUR_BYTE, LEADING_COLOUR_BYTE);

            public NumberGroup(string value)
                : this(value, '\0')
            { }

            public NumberGroup(string value, char leadingChar)
            {
                var para = new Paragraph();
                Document.Blocks.Add(para);
                Width = value.Length == 8 ? 65.0 : 40.0;

                Value = value;
                AllLeadingsChars = leadingChar != '\0' && Value.All(c => c == leadingChar);
                var lead = leadingChar == '\0' ? "" : new string(Value.TakeWhile(c => c == leadingChar).ToArray());

                if (leadingChar == '\0' || lead.Length == 0)
                {
                    para.Inlines.Add(new Run(Value));
                    return;
                }

                para.Inlines.Add(new Run(lead)
                {
                    Foreground = new SolidColorBrush(LEADING_COLOUR)
                });

                if (lead.Length < Value.Length)
                    para.Inlines.Add(new Run(Value.Substring(lead.Length)));
            }

            public bool AllLeadingsChars { get; }

            public string Value { get; }

            public FlowDocument Document { get; } = new FlowDocument();

            public double Width { get; }
        }
    }
}
