using Earmark.Data.Suggestion;
using Earmark.Data.Suggestion.SuggestionProviders;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System.Linq;

namespace Earmark.UserControls
{
    public sealed partial class SuggestionBox : UserControl
    {
        private ISuggestion _selectedSuggestion;

        public static readonly DependencyProperty SuggestionProviderProperty =
            DependencyProperty.Register(nameof(SuggestionProvider), typeof(ISuggestionProvider), typeof(SuggestionBox), new PropertyMetadata(null));

        public ISuggestionProvider SuggestionProvider
        {
            get => (ISuggestionProvider)GetValue(SuggestionProviderProperty);
            set => SetValue(SuggestionProviderProperty, value);
        }

        public static readonly DependencyProperty SuggestionProviderPredicateArgProperty =
            DependencyProperty.Register(nameof(SuggestionProviderPredicateArg), typeof(object), typeof(SuggestionBox), new PropertyMetadata(null));

        public object SuggestionProviderPredicateArg
        {
            get => (object)GetValue(SuggestionProviderPredicateArgProperty);
            set => SetValue(SuggestionProviderPredicateArgProperty, value);
        }

        public static readonly DependencyProperty ChosenSuggestionProperty =
            DependencyProperty.Register(nameof(ChosenSuggestion), typeof(ISuggestion), typeof(SuggestionBox), new PropertyMetadata(null));

        public ISuggestion ChosenSuggestion
        {
            get => (ISuggestion)GetValue(ChosenSuggestionProperty);
            set => SetValue(ChosenSuggestionProperty, value);
        }

        public static readonly DependencyProperty SuggestionTemplateProperty =
            DependencyProperty.Register(nameof(SuggestionTemplate), typeof(DataTemplate), typeof(SuggestionBox), new PropertyMetadata(null));

        public DataTemplate SuggestionTemplate
        {
            get => (DataTemplate)GetValue(SuggestionTemplateProperty);
            set => SetValue(SuggestionTemplateProperty, value);
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(SuggestionBox), new PropertyMetadata(string.Empty));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public SuggestionBox()
        {
            this.InitializeComponent();

            this.IsTabStop = true;
            this.GotFocus += SuggestionBox_GotFocus;
        }

        private void StartEditing()
        {
            this.IsTabStop = false;
            this.GotFocus -= SuggestionBox_GotFocus;

            SuggestionProvider.PrepareForQuery();

            TextBlock.Visibility = Visibility.Collapsed;
            AutoSuggestBox.Visibility = Visibility.Visible;

            AutoSuggestBox.Focus(FocusState.Pointer);
            AutoSuggestBox.Text = ChosenSuggestion.QueryableName;
            AutoSuggestBox.TextChanged += AutoSuggestBox_TextChanged;
            AutoSuggestBox.SuggestionChosen += AutoSuggestBox_SuggestionChosen;
            AutoSuggestBox.QuerySubmitted += AutoSuggestBox_QuerySubmitted;
            AutoSuggestBox.LostFocus += AutoSuggestBox_LostFocus;
        }

        private void EndEditing()
        {
            this.IsTabStop = true;
            this.GotFocus += SuggestionBox_GotFocus;

            TextBlock.Visibility = Visibility.Visible;
            AutoSuggestBox.Visibility = Visibility.Collapsed;

            AutoSuggestBox.IsSuggestionListOpen = false;
            AutoSuggestBox.ItemsSource = null;
            AutoSuggestBox.TextChanged -= AutoSuggestBox_TextChanged;
            AutoSuggestBox.SuggestionChosen -= AutoSuggestBox_SuggestionChosen;
            AutoSuggestBox.QuerySubmitted -= AutoSuggestBox_QuerySubmitted;
            AutoSuggestBox.LostFocus -= AutoSuggestBox_LostFocus;
        }

        private void QuerySubmitted(ISuggestion chosenSuggestion)
        {
            if (chosenSuggestion is null)
            {
                var suggestions = SuggestionProvider.GetSuggestionsByQuery(AutoSuggestBox.Text, SuggestionProviderPredicateArg);
                var topSuggestion = suggestions.FirstOrDefault();
                if (topSuggestion is not null)
                {
                    chosenSuggestion = topSuggestion;
                }
            }

            if (chosenSuggestion is CreateSuggestion createSuggestion)
            {
                chosenSuggestion = SuggestionProvider.InvokeCreateSuggestionCallback(createSuggestion);
            }

            if (chosenSuggestion is not null &&
                chosenSuggestion.Id != ChosenSuggestion.Id)
            {
                ChosenSuggestion = chosenSuggestion;
            }

            EndEditing();
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

        private void SuggestionBox_GotFocus(object sender, RoutedEventArgs e)
        {
            StartEditing();
        }

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            StartEditing();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                _selectedSuggestion = null;
                sender.ItemsSource = SuggestionProvider.GetSuggestionsByQuery(sender.Text, SuggestionProviderPredicateArg);
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            _selectedSuggestion = args.SelectedItem as ISuggestion;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var chosenSuggestion = args.ChosenSuggestion as ISuggestion;
            QuerySubmitted(chosenSuggestion);
        }

        private void AutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // This check allows the user to use the AutoSuggestBox context menu without ending the edit.
            if (!(FocusManager.GetFocusedElement(XamlRoot) is AppBarButton or Popup))
            {
                QuerySubmitted(_selectedSuggestion);
            }
        }

        private void AutoSuggestBox_Loaded(object sender, RoutedEventArgs e)
        {
            AutoSuggestBox.Visibility = Visibility.Collapsed;
        }
    }
}
