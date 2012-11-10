using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Globalization;

namespace KeyboardMapper
{
    #region Converters
    [ValueConversion(typeof(Int32), typeof(String))]
    public class KeyVkCodeNameConverter : IValueConverter
    {
        /// <summary>
        /// convert from key virtual code to key name
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Key k = KeyInterop.KeyFromVirtualKey((int)value);
            return k.ToString();
        }
        /// <summary>
        /// convert from key name to virtual code
        /// </summary>
        /// <param name="value">a String containing key name</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Key k = (Key)Enum.Parse(typeof(Key), (string)value);
            return KeyInterop.VirtualKeyFromKey(k);

        }
    }

    #endregion
}