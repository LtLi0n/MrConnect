namespace MrConnect
{
    public static class StringExtensions
    {
        public static bool IsNumber(this string str)
        {
            foreach (char ch in str)
            {
                if (ch > '9' || ch < '0')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
