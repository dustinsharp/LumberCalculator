using System;
using System.Globalization;
using System.Windows.Data;

namespace LumberCalculator
{
    public class RectangleDimensionConverter : IValueConverter
    {
        private decimal _multiplier = 5.5m; //3.5m;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) * _multiplier;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDecimal(value) / _multiplier;
        }
    }
}