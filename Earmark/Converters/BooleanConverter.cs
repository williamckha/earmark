namespace Earmark.Converters
{
    public static class BooleanConverter
    {
        /// <summary>
        /// Inverts the boolean.
        /// </summary>
        public static bool Invert(this bool value) => !value;

        /// <summary>
        /// Returns true if the string is not empty. 
        /// </summary>
        public static bool IfStringNotEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Returns true if the integer is not zero.
        /// </summary>
        public static bool IfNotZero(this int value) => value != 0;
    }
}
