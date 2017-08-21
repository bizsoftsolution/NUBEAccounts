﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using NUBEAccounts.Common;
namespace NUBEAccounts.Pl.Conversion
{
    public class CurrencyInWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var n = value as decimal?;
                return n.ToCurrencyInWords();
            }
            catch (Exception ex) { }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
