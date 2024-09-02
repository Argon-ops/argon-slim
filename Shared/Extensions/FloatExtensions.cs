namespace DuksGames.Argon.Shared
{
    public static class FloatExtensions
    {
        public static string ToString2N(this float value)
        {
            return value.ToString("0.00");
        }

        public static string ToString3N(this float value)
        {
            return value.ToString("0.000");
        }
    }
}