using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Earmark.Helpers
{
    public class ObservableGroupingCollection<T> : ObservableCollection<T> where T : IObservableGrouping
    {
        public event EventHandler GroupingChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.NewItems is not null)
            {
                foreach (var grouping in e.NewItems.Cast<T>())
                {
                    grouping.GroupingChanged += Grouping_GroupingChanged;
                }
            }

            if (e.OldItems is not null)
            {
                foreach (var grouping in e.OldItems.Cast<T>())
                {
                    grouping.GroupingChanged -= Grouping_GroupingChanged;
                }
            }
        }

        private void Grouping_GroupingChanged(object sender, EventArgs e)
        {
            GroupingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public interface IObservableGrouping
    {
        event EventHandler GroupingChanged;
    }
}
