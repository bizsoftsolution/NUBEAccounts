using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NUBEAccounts.Common
{
   public static class AppLib
    {
        public static string CurrencyToWordPrefix;
        public static string CurrencyToWordSuffix;

        public static string DecimalToWordSuffix;
        public static string DecimalToWordPrefix;

        public static string CurrencyPositiveSymbolPrefix;
        public static string CurrencyPositiveSymbolSuffix;

        public static string CurrencyNegativeSymbolPrefix;
        public static string CurrencyNegativeSymbolSuffix;
        public static string DecimalSymbol;
        public static string DigitGroupingSymbol;

        public static bool IsDisplayWithOnlyOnSuffix;

        public static int NoOfDigitAfterDecimal;
        public static int DigitGroupingBy;
        public static int CurrencyCaseSensitive;

        public static T toCopy<T>(this object objSource, T objDestination)
        {
            try
            {

                var l1 = objSource.GetType().GetProperties().Where(x => x.PropertyType.Namespace != "System.Collections.Generic").ToList();

                foreach (var pFrom in l1)
                {
                    try
                    {
                        var pTo = objDestination.GetType().GetProperties().Where(x => x.Name == pFrom.Name).FirstOrDefault();
                        pTo.SetValue(objDestination, pFrom.GetValue(objSource));
                    }
                    catch (Exception ex) { }

                }
            }
            catch (Exception ex)
            {

            }
            return objDestination;
        }

        public static void MutateVerbose<TField>(this INotifyPropertyChanged instance, ref TField field, TField newValue, Action<PropertyChangedEventArgs> raise, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue)) return;
            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
        }
        public static void WriteLog(String str)
        {

            using (StreamWriter writer = new StreamWriter(Path.GetTempPath() + "NUBEAccounts_log.txt", true))
            {
                writer.WriteLine(string.Format("{0:dd/MM/yyyy hh:mm:ss} => {1}", DateTime.Now, str));
            }
        }
        #region NumberToWords
        public static string ToCurrencyInWords(this decimal Number)
        {
            String words = "";
            try
            {
                if (Number == 0) return "";
                string[] Nums = string.Format("{0:0.00}", Number).Split('.');

                int number1 = int.Parse(Nums[0]);
                int number2 = int.Parse(Nums[1]);

                words = string.Format("{0}{1}{2}", CurrencyToWordPrefix, number1.ToWords(), CurrencyToWordSuffix);

                if (number2 > 0) words = string.Format("{0} AND {1}{2}{3}", words, DecimalToWordPrefix ?? "", number2.ToWords(), DecimalToWordSuffix ?? "");

                if (IsDisplayWithOnlyOnSuffix)
                {
                    words = string.Format("{0} Only", words);
                }


                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;

                if (CurrencyCaseSensitive == 0)
                {
                    return textInfo.ToLower(words);
                }
                else if (CurrencyCaseSensitive == 1)
                {
                    return textInfo.ToUpper(words);
                }
                else if (CurrencyCaseSensitive == 2)
                {
                    return textInfo.ToTitleCase(words.ToLower());
                }
                else
                {
                    return words;
                }

            }

            catch (Exception ex)
            {

            }
            return words;
        }
        public static string ToCurrencyInWords(this decimal? Number)
        {
            if (Number == null) return "";
            return Number.Value.ToCurrencyInWords();
        }

        public static string ToCurrencySymbol(this decimal? Number)
        {
            return Number == null ? "" : Number.Value.ToCurrencySymbol();
        }

        public static string ToCurrencySymbol(this decimal Number)
        {
            try
            {
                return string.Format("{0}{1}{2}", Number >= 0 ? CurrencyPositiveSymbolPrefix : CurrencyNegativeSymbolPrefix, Math.Abs(Number), Number >= 0 ? CurrencyPositiveSymbolSuffix : CurrencyNegativeSymbolSuffix);
            }
            catch (Exception ex) { }
            return "";
        }

        public static string ToNumberFormat(this decimal? Number)
        {
            return Number == null ? "" : Number.Value.ToNumberFormat();

        }

        public static string ToNumberFormat(this decimal Number)
        {
            return string.Format("{0} {1:0.00}", CurrencyPositiveSymbolPrefix, Number);
        }

        public static string ToDateFormat(this DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToDateFormat();
        }

        public static string ToDateFormat(this DateTime dt)
        {

            return string.Format("{0:yyyy}", dt);
        }




        public static string ToWords(this int number1)
        {
            string words = "";

            try
            {
                if (number1 == 0)
                    return "Zero";

                if (number1 < 0)
                    return "minus " + ToWords(Math.Abs(number1));

                if ((number1 / 1000000) > 0)
                {
                    words += ToWords(number1 / 1000000) + " Million ";
                    number1 %= 1000000;
                }

                if ((number1 / 1000) > 0)
                {
                    words += ToWords(number1 / 1000) + " Thousand ";
                    number1 %= 1000;
                }

                if ((number1 / 100) > 0)
                {
                    words += ToWords(number1 / 100) + " Hundred ";
                    number1 %= 100;
                }

                if (number1 > 0)
                {
                    if (words != "")
                        words += "and ";

                    var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                    var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                    if (number1 < 20)
                        words += unitsMap[number1];
                    else
                    {
                        words += tensMap[number1 / 10];
                        if ((number1 % 10) > 0)
                            // words += "-" + unitsMap[number1 % 10];
                            words += " " + unitsMap[number1 % 10];
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return words.ToUpper();
        }

        #endregion

        public static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9.]");
            return reg.IsMatch(str);

        }

        public static string NumericOnly(string str)
        {
            String newText = String.Empty;

            int DotCount = 0;
            foreach (Char c in str.ToCharArray())
            {
                if (Char.IsDigit(c) || Char.IsControl(c) || (c == '.' && DotCount == 0))
                {
                    newText += c;
                    if (c == '.') DotCount += 1;
                }
            }
            return newText;
        }

        public static string NumericQtyOnly(string str)
        {
            String newText = String.Empty;

            int DotCount = 0;
            foreach (Char c in str.ToCharArray())
            {
                if (Char.IsDigit(c) || Char.IsControl(c) || (c == '.' && DotCount == 0))
                {
                    newText += c;
                    if (c == '.') DotCount += 1;
                }
            }
            return newText;
        }
        public static bool IsValidEmailAddress(this string s)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(s);
        }

        #region Print


        #endregion

        public static byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            return imageData;
        }
        public static BitmapImage ViewImage(byte[] bytes)
        {
            BitmapImage img = new BitmapImage();

            if (bytes != null)
            {
                MemoryStream stream = new System.IO.MemoryStream(bytes);

                img.BeginInit();
                img.StreamSource = stream;
                img.EndInit();
            }

            return img;
        }
    }
}
