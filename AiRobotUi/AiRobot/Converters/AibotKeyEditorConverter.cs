using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Nodify;

namespace Aibot
{
    public class AibotKeyEditorConverter : MarkupExtension, IMultiValueConverter
    {
        public bool CanChangeInputType { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is ICollection<AibotKeyViewModel> availableKeys && values[1] is AibotKeyViewModel target)
            {
                return new
                {
                    AvailableKeys = availableKeys,
                    Target = target,
                    IsEditing = values.Length >= 3 && values[2] is bool b && b,
                    CanChangeInputType = CanChangeInputType && (target.Type != AibotKeyType.Object || target.CanChangeType),
                    CanChangeKeyType = target.CanChangeType
                };
            }

            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    /// <summary>
    /// 获取窗口高度 减去 value 后的值
    /// </summary>
    public class SubtractValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height && parameter is string subtractValue && double.TryParse(subtractValue, out double subtract))
            {
                return height - subtract;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // 根据需要格式化日期时间
                return dateTime.ToString("yyyy/MM/dd/ HH:mm:ss");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果需要双向绑定，可以实现从字符串回到DateTime的转换逻辑
            throw new NotImplementedException();
        }
    }


}
