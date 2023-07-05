using Microsoft.UI.Xaml;

namespace Earmark.Converters
{
    public static class VisibilityConverter
    {
        /// <summary>
        /// Converts the given bool to a visibility.
        /// </summary>
        public static Visibility ToVisibility(this bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Inverts the given bool to a visibility.
        /// </summary>
        public static Visibility InvertBool(this bool value)
        {
            return InvertVisibility(value ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// Inverts the given visibility.
        /// </summary>
        public static Visibility InvertVisibility(this Visibility value)
        {
            return value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Returns visible if string is not empty. 
        /// </summary>
        public static Visibility IfStringNotEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Returns visible if the integer is zero.
        /// </summary>
        public static Visibility IfZero(this int value)
        {
            return value == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Returns visible if the integer is not zero.
        /// </summary>
        public static Visibility IfNotZero(this int value)
        {
            return value == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}