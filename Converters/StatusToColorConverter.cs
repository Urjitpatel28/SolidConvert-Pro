using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SolidConvert_Pro.Models;

namespace SolidConvert_Pro.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileStatus status)
            {
                switch (status)
                {
                    case FileStatus.Pending:
                        return new SolidColorBrush(Colors.Gray);
                    case FileStatus.Processing:
                        return new SolidColorBrush(Colors.Blue);
                    case FileStatus.Completed:
                        return new SolidColorBrush(Colors.Green);
                    case FileStatus.Error:
                        return new SolidColorBrush(Colors.Red);
                    default:
                        return new SolidColorBrush(Colors.Gray);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


