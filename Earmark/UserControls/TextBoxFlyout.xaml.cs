using CommunityToolkit.WinUI.UI;
using Earmark.Helpers.Validation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;
using Windows.System;

namespace Earmark.UserControls
{
    public partial class TextBoxFlyout : UserControl
    {
        public static readonly DependencyProperty TextBoxPlaceholderTextProperty =
            DependencyProperty.Register(nameof(TextBoxPlaceholderText), typeof(string), typeof(TextBoxFlyout), new PropertyMetadata(string.Empty));

        public string TextBoxPlaceholderText
        {
            get => (string)GetValue(TextBoxPlaceholderTextProperty);
            set => SetValue(TextBoxPlaceholderTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(TextBoxFlyout), new PropertyMetadata(string.Empty));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty InputConfirmedCommandProperty =
            DependencyProperty.Register(nameof(InputConfirmedCommand), typeof(ICommand), typeof(TextBoxFlyout), new PropertyMetadata(null));

        public ICommand InputConfirmedCommand
        {
            get => (ICommand)GetValue(InputConfirmedCommandProperty);
            set => SetValue(InputConfirmedCommandProperty, value);
        }

        public static readonly DependencyProperty InputValidatorProperty =
            DependencyProperty.Register(nameof(InputValidator), typeof(IDataValidator<string>), typeof(TextBoxFlyout), new PropertyMetadata(null));

        public IDataValidator<string> InputValidator
        {
            get => (IDataValidator<string>)GetValue(InputValidatorProperty);
            set => SetValue(InputValidatorProperty, value);
        }

        public TextBoxFlyout()
        {
            this.InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                InputConfirmed();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InputConfirmed();
        }

        private void InputConfirmed()
        {
            if (string.IsNullOrEmpty(TextBox.Text)) return;

            if (InputValidator.Validate(TextBox.Text, out string errorMessage))
            {
                var flyoutPresenter = this.FindParent<FlyoutPresenter>();
                if (flyoutPresenter?.Parent is Popup popup)
                {
                    popup.IsOpen = false;
                }

                InputConfirmedCommand?.Execute(TextBox.Text);
                TextBox.Text = string.Empty;

                ErrorInfoBar.Message = string.Empty;
                ErrorInfoBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                ErrorInfoBar.Message = errorMessage;
                ErrorInfoBar.Visibility = Visibility.Visible;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ErrorInfoBar.Visibility == Visibility.Visible)
            {
                ErrorInfoBar.Message = string.Empty;
                ErrorInfoBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
