using System;

namespace Earmark.Data.Navigation
{
    public interface INavigationItem
    {
        string Name { get; }

        string IconGlyph { get; }
    
        Type TargetViewModel { get; }

        object Parameter { get; }
    }
}
