using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PDFReader.Converters
{
    public class StateToImageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return null;

            bool isInitial = values[0] is bool b1 && b1;
            bool isNotFound = values[1] is bool b2 && b2;

            string imagePath;

            if (isInitial)
                imagePath = "/Resources/home.png";
            else if (isNotFound)
                imagePath = "/Resources/not-found.png";
            else
                return null;

            return new BitmapImage(new Uri(imagePath, UriKind.Relative));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}