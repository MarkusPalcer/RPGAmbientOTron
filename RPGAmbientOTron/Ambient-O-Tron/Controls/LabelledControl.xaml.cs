
using System.Windows;
using System.Windows.Markup;

namespace AmbientOTron.Controls
{
    [ContentProperty("Control")]
    public partial class LabelledControl 
    {

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(LabelledControl), new PropertyMetadata(default(string)));

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty ControlProperty = DependencyProperty.Register("Control", typeof(UIElement), typeof(LabelledControl), new PropertyMetadata(default(UIElement)));

        public UIElement Control
        {
            get { return (UIElement) GetValue(ControlProperty); }
            set { SetValue(ControlProperty, value); }
        }

        public LabelledControl()
        {
            InitializeComponent();
        }
    }
}
