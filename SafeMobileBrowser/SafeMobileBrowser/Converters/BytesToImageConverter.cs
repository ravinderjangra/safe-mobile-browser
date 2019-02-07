using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace SafeMobileBrowser.Converters
{
    public class BytesToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return ImageSource.FromStream(() => new MemoryStream((byte[])value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
