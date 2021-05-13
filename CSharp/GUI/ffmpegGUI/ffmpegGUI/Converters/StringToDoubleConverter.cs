using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ffmpegGUI.Converters
{
    /// <summary>
    /// string to double
    /// </summary>
    class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value.ToString();
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return (double)0;

            if (double.TryParse((value as string), out var num)) return num;
            else return 0;
        }
    }
}
