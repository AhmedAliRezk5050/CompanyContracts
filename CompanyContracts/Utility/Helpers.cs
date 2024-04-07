using System.Globalization;

namespace CompanyContracts.Utility;

public static class Helpers
{
    public static bool HasElementsAfter<T>(List<T> list, T element)
    {
        var index = list.IndexOf(element);
        
        if (index == -1)
        {
            return false;
        }

        return index < list.Count - 1;
    }
    
    public static string FormatWithCommas<T>(T number) where T : struct, IFormattable
    {
        return number.ToString("N2", CultureInfo.InvariantCulture);
    }
}