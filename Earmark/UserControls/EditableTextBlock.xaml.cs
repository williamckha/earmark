using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;

namespace Earmark.UserControls
{
    public sealed partial class EditableTextBlock : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(EditableTextBlock), new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(EditableTextBlock), new PropertyMetadata(string.Empty));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public EditableTextBlock()
        {
            this.InitializeComponent();

            this.IsTabStop = true;
            this.GotFocus += EditableTextBlock_GotFocus;
        }

        private void StartEditing()
        {
            this.IsTabStop = false;
            this.GotFocus -= EditableTextBlock_GotFocus;

            TextBlock.Visibility = Visibility.Collapsed;
            TextBox.Visibility = Visibility.Visible;

            TextBox.Focus(FocusState.Pointer);
            TextBox.LostFocus += TextBox_LostFocus;
            TextBox.KeyDown += TextBox_KeyDown;
        }

        private void EndEditing()
        {
            this.IsTabStop = true;
            this.GotFocus += EditableTextBlock_GotFocus;

            TextBlock.Visibility = Visibility.Visible;
            TextBox.Visibility = Visibility.Collapsed;

            TextBox.LostFocus -= TextBox_LostFocus;
            TextBox.KeyDown -= TextBox_KeyDown;
        }

        private string GetTextBlockText(string text)
        {
            return string.IsNullOrEmpty(text) ? PlaceholderText : text;
        }

        private SolidColorBrush GetTextBlockForegroundBrush(string text)
        {
            return string.IsNullOrEmpty(text) ?
                App.Current.Resources["TextFillColorTertiaryBrush"] as SolidColorBrush :
                App.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
        }

        private void EditableTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            StartEditing();
        }

        private void TextBlock_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            StartEditing();
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox.Visibility = Visibility.Collapsed;
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape || e.Key == VirtualKey.Enter)
            {
                TextBox.LostFocus -= TextBox_LostFocus;
                EndEditing();
                e.Handled = true;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // This check allows the user to use the number box context menu without ending the edit.
            if (!(FocusManager.GetFocusedElement(XamlRoot) is AppBarButton or Popup))
            {
                EndEditing();
            }
        }
    }
}
