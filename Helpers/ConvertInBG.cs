using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Helpers
{
    public class ConvertInBG
    {
        public static string ConvertToTXT(decimal number)
        {
            return ConvertTXT(number) + "đồng";
        }
        private static string ConvertTXT(decimal number)
        {
            string[] units = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín", "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười lăm", "mười sáu", "mười bảy", "mười tám", "mười chín" };
            string[] tens = { "", "", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };

            if (number == 0)
                return "không";

            if (number < 0)
                return "âm " + ConvertTXT(Math.Abs(number));

            string words = "";

            if ((int)(number / 1000000) > 0)
            {
                words += ConvertTXT(number / 1000000) + " triệu ";
                number %= 1000000;
            }

            if ((int)(number / 1000) > 0)
            {
                words += ConvertTXT(number / 1000) + " nghìn ";
                number %= 1000;
            }

            if ((int)(number / 100) > 0)
            {
                words += ConvertTXT(number / 100) + " trăm ";
                number %= 100;
            }

            if (number > 0)
            {

                if (number < 20)
                    words += units[(int)number];
                else
                {
                    words += tens[(int)(number / 10)];
                    if ((number % 10) > 0)
                        words += " " + units[(int)(number % 10)];
                }
            }

            return words;
        }
    }
}