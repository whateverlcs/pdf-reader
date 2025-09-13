using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Windows.Data;

namespace PDFReader.Converters
{
    public class SidebarIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? PackIconKind.Backburger : PackIconKind.Menu;
            }
            return PackIconKind.Menu;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}