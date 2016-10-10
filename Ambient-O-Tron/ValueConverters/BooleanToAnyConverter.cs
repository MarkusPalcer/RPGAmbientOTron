using System;
using System.Globalization;
using System.Windows.Data;

namespace AmbientOTron.ValueConverters
{
    public class BooleanToAnyConverter: IValueConverter
    {
        public object TrueValue { get; set; }

        public object FalseValue { get; set; }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            if (!boolValue.HasValue)
                return null;

            return boolValue.Value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == TrueValue)
                return true;

            if (value == FalseValue)
                return false;

            return null;
        }

        #endregion
    }
}