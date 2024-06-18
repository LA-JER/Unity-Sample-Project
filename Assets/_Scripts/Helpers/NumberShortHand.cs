
public static class NumberShorthand
{
    public static string FormatNumber(float num)
    {
        return FormatNumber((double)num);
    }

    public static string FormatNumber(int num)
    {
        return FormatNumber((double)num);
    }

    private static string FormatNumber(double num)
    {
        if (num >= 1000000000000) // Trillion
        {
            return (num / 1000000000000D).ToString("0.##") + "T";
        }
        else if (num >= 1000000000) // Billion
        {
            return (num / 1000000000D).ToString("0.##") + "B";
        }
        else if (num >= 1000000) // Million
        {
            return (num / 1000000D).ToString("0.##") + "M";
        }
        else if (num >= 1000) // Thousand
        {
            return (num / 1000D).ToString("0.##") + "k";
        }
        else
        {
            return ((int)num).ToString();
        }
    }
}
