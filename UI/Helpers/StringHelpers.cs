namespace FlazorTemplate.Helpers
{
    public static class StringHelpers
    {
        public static string GetCategoryLabel(this object value)
        {
            if (value == null)
            {
                return "";
            }

            var stringValue = (string)value;

            if (stringValue.Length <= 30)
            {
                return stringValue;
            }
            else
            {
                return stringValue.Substring(0, 30) + "...";
            }
        }
    }
}
