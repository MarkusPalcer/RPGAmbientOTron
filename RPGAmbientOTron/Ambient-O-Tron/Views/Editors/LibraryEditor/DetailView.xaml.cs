using System.ComponentModel.Composition;
using System.Windows;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    [Export]
    public partial class DetailView 
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached(
                                                                "Header",
                                                                typeof(string),
                                                                typeof(DetailView),
                                                                new PropertyMetadata(default(string)));

        public static void SetHeader(DependencyObject element, string value)
        {
            element.SetValue(HeaderProperty, value);
        }

        public static string GetHeader(DependencyObject element)
        {
            return (string) element.GetValue(HeaderProperty);
        }

        [ImportingConstructor]
        public DetailView(DetailViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}

