namespace CloudflareZTClient.Converters
{
    using System;
    using System.Globalization;
    using Xamarin.Forms;

    /// <summary>
    /// Inverting boolean variable.
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Inversing the boolean based on the input.
        /// </summary>
        /// <returns>Converted boolean.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}