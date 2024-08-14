using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace Erp.BackOffice.Filters
{
    public class FormatNumber
    {
        public static string FormatNumberv(object value, int SoSauDauPhay = 2)
        {
            bool isNumber = IsNumeric(value);
            decimal GT = 0;
            if (isNumber)
            {
                GT = Convert.ToDecimal(value);
            }
            string str = "";
            string thapPhan = "";
            for (int i = 0; i < SoSauDauPhay; i++)
            {
                thapPhan += "#";
            }
            if (thapPhan.Length > 0) thapPhan = "." + thapPhan;
            string snumformat = string.Format("0:#,##0{0}", thapPhan);
            str = String.Format("{" + snumformat + "}", GT);

            return str;
        }
        private static bool IsNumeric(object value)
        {
            return value is sbyte
                       || value is byte
                       || value is short
                       || value is ushort
                       || value is int
                       || value is uint
                       || value is long
                       || value is ulong
                       || value is float
                       || value is double
                       || value is decimal;
        }
        public static string HtmlRate(int rate)
        {
            var str = "";
            if (rate == 1)
            {
                str = "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                    "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>" +
                    "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>" +
                    "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>" +
                    "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            else
            {
                if (rate == 2)
                {
                    str = "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                        "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                        "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>" +
                        "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>" +
                        "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
                }
                else
                {
                    if (rate == 3)
                    {
                        str = "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                            "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                            "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                            "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>" +
                            "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
                    }
                    else
                    {
                        if (rate == 4)
                        {
                            str = "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                                "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                                "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                                "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                                "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
                        }
                        else
                        {
                            if (rate == 5)
                            {
                                str = "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                                    "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                                    "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                                    "<li><i class='fa fa-star' aria-hidden='true'></i></li>" +
                                    "<li><i class='fa fa-star' aria-hidden='true'></i></li>";
                            }
                        }
                    }
                }
            }
            return str;
        }

        public static int GetWeekNumber(DateTime dt)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                dt = dt.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public static DateTime GetFirstDayOfWeek(int weekNumber, int year)
        {
            CultureInfo ci = CultureInfo.CurrentCulture;
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = ci.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekNumber;
            DateTime result = firstThursday;
            while (cal.GetWeekOfYear(result, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) != weekNum)
            {
                result = result.AddDays(-7);
            }
            return result.Date;
        }

        public static DateTime GetLastDayOfWeek(int weekNumber, int year)
        {
            return GetFirstDayOfWeek(weekNumber, year).AddDays(6);
        }
        private static Regex _phonePattern = new Regex(@"^(3|5|7|8|9)[0-9]{8}$");
        private static Regex _phonePattern0 = new Regex(@"^(03|05|07|08|09)[0-9]{8}$");
        public static string AdjustPhoneNumber(string input)
        {
            if (_phonePattern.IsMatch(input))
            {
                return "0" + input;
            }
            else if (_phonePattern0.IsMatch(input))
            {
                return input;
            }

            return null;
        }
    }
}