using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace AmbientOTron.ValueConverters
{
  [ContentProperty("Value")]
  public class EnumToAnyConversion
  {
    public string EnumValue { get; set; }  
    public object Value { get; set; }
  }

  [ContentProperty("Rules")]
  public class EnumToAnyConverter : IValueConverter
  {
    public List<EnumToAnyConversion> Rules { get; } = new List<EnumToAnyConversion>();

    public object DefaultValue { get; set; } = null;

    #region Implementation of IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!value.GetType().IsEnum)
        return DefaultValue;

      var stringValue = value.ToString();

      return Rules.FirstOrDefault(r => r.EnumValue == stringValue)?.Value ?? DefaultValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }

    #endregion
  }
}