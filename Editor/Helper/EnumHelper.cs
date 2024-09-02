
namespace DuksGames.Tools
{
    public static class EnumHelper
    {
        public static dynamic GetValue(System.Type enumType, int index)
        {
            var vals = System.Enum.GetValues(enumType);
            return vals.GetValue(index);
        }
    }
}