using Earmark.Converters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;

namespace Earmark.UserControls
{
    public sealed partial class EditableNumberBlock : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(EditableNumberBlock), new PropertyMetadata(0));
        
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty IsForegroundConditionalProperty =
            DependencyProperty.Register(nameof(IsForegroundConditional), typeof(bool), typeof(EditableNumberBlock), new PropertyMetadata(false));

        public bool IsForegroundConditional
        {
            get => (bool)GetValue(IsForegroundConditionalProperty);
            set => SetValue(IsForegroundConditionalProperty, value);
        }

        public EditableNumberBlock()
        {
            this.InitializeComponent();

            this.IsTabStop = true;
            this.GotFocus += EditableNumberBlock_GotFocus;
        }

        private void StartEditing()
        {
            this.IsTabStop = false;
            this.GotFocus -= EditableNumberBlock_GotFocus;

            TextBlock.Visibility = Visibility.Collapsed;
            NumberBox.Visibility = Visibility.Visible;

            NumberBox.Focus(FocusState.Pointer);
            NumberBox.LostFocus += NumberBox_LostFocus;
            NumberBox.KeyDown += NumberBox_KeyDown;
        }

        private void EndEditing()
        {
            this.IsTabStop = true;
            this.GotFocus += EditableNumberBlock_GotFocus;

            TextBlock.Visibility = Visibility.Visible;
            NumberBox.Visibility = Visibility.Collapsed;

            NumberBox.LostFocus -= NumberBox_LostFocus;
            NumberBox.KeyDown -= NumberBox_KeyDown;
        }

        private SolidColorBrush GetForegroundBrush(int value)
        {
            if (IsForegroundConditional)
            {
                return IntegerToBrushConverter.GetConditionalForegroundBrush(value);
            }
            return App.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
        }

        private void EditableNumberBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            StartEditing();
        }

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            StartEditing();
        }

        private void NumberBox_Loaded(object sender, RoutedEventArgs e)
        {
            NumberBox.NumberFormatter = CurrencyFormatConverter.Formatter;
            NumberBox.Visibility = Visibility.Collapsed;
        }

        private void NumberBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape || e.Key == VirtualKey.Enter)
            {
                NumberBox.LostFocus -= NumberBox_LostFocus;
                EndEditing();
                e.Handled = true;
            }
        }

        private void NumberBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // This check allows the user to use the number box context menu without ending the edit.
            if (!(FocusManager.GetFocusedElement(XamlRoot) is AppBarButton or Popup))
            {
                EndEditing();
            }
        }
    }
}
