using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class MoneyFormat
{
    public static Dictionary<int, string> Scales = new Dictionary<int, string>
    {
        {0, "hundred"},
        {3, "thousand"},
        {6, "million"},
        {9, "billion"},
        {12, "trillion"},
        {15, "quadrillion"},
        {18, "quintillion"},
        {21, "sextillion"},
        {24, "septillion"},
        {27, "octillion"},
        {30, "nonillion"},
        {33, "decillion"},
        {36, "undecillion"},
        {39, "duodecillion"},
        {42, "tredecillion"},
        {45, "quattuordecillion"},
        {48, "quindecillion"},
        {51, "sexdecillion"},
        {54, "septendecillion"},
        {57, "octodecillion"},
        {60, "novemdecillion"},
        {63, "vigintillion"},
        {66, "unvigintillion"},
        {69, "duovigintillion"},
        {72, "tresvigintillion"},
        {75, "quattuorvigintillion"},
        {78, "quinvigintillion"},
        {81, "sexvigintillion"},
        {84, "septenvigintillion"},
        {87, "octovigintillion"},
        {90, "novemvigintillion"},
        {93, "trigintillion"},
        {96, "untrigintillion"},
        {99, "duotrigintillion"},
        {102, "tretrigintillion"},
        {105, "quattuortrigintillion"},
        {108, "quintrigintillion"},
        {111, "sextrigintillion"},
        {114, "septentrigintillion"},
        {117, "octotrigintillion"},
        {120, "novemtrigintillion"},
        {123, "quadragintillion"},
        {126, "unquadragintillion"},
        {129, "duoquadragintillion"},
        {132, "trequadragintillion"},
        {135, "quattuorquadragintillion"},
        {138, "quinquadragintillion"},
        {141, "sexquadragintillion"},
        {144, "septquadragintillion"},
        {147, "octoquadragintillion"},
        {150, "novemquadragintillion"},
        {153, "quinquagintillion"},
        {156, "unquinquagintillion"},
        {159, "duoquinquagintillion"},
        {162, "trequinquagintillion"},
        {165, "quattuorquinquagintillion"},
        {168, "quinquinquagintillion"},
        {171, "sexquinquagintillion"},
        {174, "septquinquagintillion"},
        {177, "octoquinquagintillion"},
        {180, "novemquinquagintillion"},
        {183, "sexagintillion"},
        {186, "unsexagintillion"},
        {189, "duosexagintillion"},
        {192, "tresexagintillion"},
        {195, "quattuorsexagintillion"},
        {198, "quinsexagintillion"},
        {201, "sexsexagintillion"},
        {204, "septsexagintillion"},
        {207, "octosexagintillion"},
        {210, "novemsexagintillion"},
        {213, "septuagintillion"},
        {216, "unseptuagintillion"},
        {219, "duoseptuagintillion"},
        {222, "treseptuagintillion"},
        {225, "quattuorseptuagintillion"},
        {228, "quinseptuagintillion"},
        {231, "sexseptuagintillion"},
        {234, "septseptuagintillion"},
        {237, "octoseptuagintillion"},
        {240, "novemseptuagintillion"},
        {243, "octogintillion"},
        {246, "unoctogintillion"},
        {249, "duooctogintillion"},
        {252, "treoctogintillion"},
        {255, "quattuoroctogintillion"},
        {258, "quinoctogintillion"},
        {261, "sexoctogintillion"},
        {264, "septoctogintillion"},
        {267, "octooctogintillion"},
        {270, "novemoctogintillion"},
        {273, "nonagintillion"},
        {276, "unnonagintillion"},
        {279, "duononagintillion"},
        {282, "trenonagintillion"},
        {285, "quattuornonagintillion"},
        {288, "quinnonagintillion"},
        {291, "sexnonagintillion"},
        {294, "septnonagintillion"},
        {297, "octononagintillion"},
        {300, "novemnonagintillion"},
        {303, "centillion"},
        {306, "uncentillion"}
    };

    private static int lowestScale = 6;

    public static MoneyDetail GetNumberDetails(string money)
    {
        return GetNumberDetails(double.Parse(money));
    }

    public static MoneyDetail GetNumberDetails(double money)
    {
        string text = money.ToString();
        int num = DigitCount(text) - 1;
        if (num < lowestScale)
        {
            if (num < 3)
            {
                string text2 = money.ToString("#,##0.00");
                if (text2.Substring(text2.IndexOf(".") + 1, 2).Equals("00"))
                {
                    return new MoneyDetail(money, money.ToString("#,##0"), string.Empty);
                }

                return new MoneyDetail(money, text2, string.Empty);
            }

            return new MoneyDetail(money, money.ToString("#,##0"), string.Empty);
        }

        int num2 = num % 3;
        int key = num - num2;
        string scale = "Bad: " + num.ToString();
        Scales.TryGetValue(key, out scale);
        string empty = string.Empty;
        if (text.IndexOf("E") > 0)
        {
            string text3 = text.Substring(0, text.IndexOf("E"));
            if (text3.Length < num2 + 4)
            {
                text3 += new string('0', num2 + 4 - text3.Length);
            }

            empty = text3.Replace(".", string.Empty).Substring(0, num2 + 1);
            empty += ".";
            empty += text3.Replace(".", string.Empty).Substring(num2 + 1, 2);
            if (empty.IndexOf('.') > 0 && empty.Substring(empty.IndexOf('.'), 3).Equals(".00"))
            {
                empty = empty.Substring(0, empty.IndexOf('.'));
            }
        }
        else if (num2 == 0)
        {
            empty = text.Substring(0, 1) + "." + text.Substring(1, 2);
        }
        else
        {
            string text4 = "." + text.Substring(num2 + 1, 2);
            empty = text.Substring(0, num2 + 1);
            if (!text4.Equals(".00"))
            {
                empty += text4;
            }
        }

        return new MoneyDetail(money, empty, scale);
    }

    private static int DigitCount(string sNum)
    {
        int num = sNum.IndexOf("+");
        if (num > 0)
        {
            string s = sNum.Substring(num, sNum.Length - num);
            return int.Parse(s) + 1;
        }

        int num2 = sNum.IndexOf(".");
        if (num2 > 0)
        {
            sNum = sNum.Substring(0, num2);
        }

        return sNum.Length;
    }

    public static string Default(double unformatted)
    {
        var money = GetNumberDetails(unformatted);
        return "$ " + money.FormattedNumber + " " + money.Scale;
    }

    public static MoneyDetail GetMoney(double unformatted)
    {
        return GetNumberDetails(unformatted);
    }
}