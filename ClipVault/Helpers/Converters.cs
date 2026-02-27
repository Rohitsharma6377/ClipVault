using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using System.Threading.Tasks;

namespace ClipVault.Helpers
{
    public class BooleanToVisibilityInvertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
            {
                return b ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
            {
                return b ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
            {
                return !b;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
            {
                return !b;
            }
            return false;
        }
    }

    public class BooleanToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b && b)
            {
                return "Pinned Clips";
            }
            return "Clipboard History";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PremiumStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b && b)
            {
                return "Premium Active";
            }
            return "Free Version";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is byte[] bytes && bytes.Length > 0)
            {
                var image = new BitmapImage();
                using (var stream = new InMemoryRandomAccessStream())
                {
                    stream.WriteAsync(bytes.AsBuffer()).AsTask().Wait();
                    stream.Seek(0);
                    image.SetSource(stream);
                }
                return image;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StringComparisonToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string s && parameter is string p)
            {
                return s == p ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
