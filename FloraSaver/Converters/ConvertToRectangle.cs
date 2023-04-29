using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Converters
{
    public class ConvertToRectangle : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null)
            {
                return new Rect(0d, (double)values[0], (double)values[1], (double)values[2]);
            } else
            {
                return new Rect(0d, 0d, 0d, 0d);
            }
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
