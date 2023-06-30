using Earmark.Data.Navigation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Earmark.Helpers.TemplateSelectors
{
    public class NavigationItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }

        public DataTemplate AccountGroupTemplate { get; set; }

        public DataTemplate AccountTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item switch
            {
                NavigationItem => NormalTemplate,
                AccountGroupNavigationItem => AccountGroupTemplate,
                AccountNavigationItem => AccountTemplate,
                _ => throw new ArgumentException(nameof(item))
            };
        }
    }
}
