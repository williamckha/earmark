using System;

namespace Earmark.Data.Navigation
{
    public class NavigationItem : INavigationItem
    {
        public string Name { get; init; }

        public string IconGlyph { get; init; }

        public Type TargetViewModel { get; init; }

        public object Parameter => null;
    }
}
