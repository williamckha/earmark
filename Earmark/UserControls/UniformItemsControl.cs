using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Windows.Input;

namespace Earmark.UserControls
{
    public class UniformItemsControl : ItemsControl
    {
        public static readonly DependencyProperty RefreshItemsSourceCommandProperty =
            DependencyProperty.Register(nameof(RefreshItemsSourceCommand), typeof(ICommand), typeof(UniformItemsControl), new PropertyMetadata(null));

        public ICommand RefreshItemsSourceCommand
        {
            get => (ICommand)GetValue(RefreshItemsSourceCommandProperty);
            set => SetValue(RefreshItemsSourceCommandProperty, value);
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(UniformItemsControl), new PropertyMetadata(null));

        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        public UniformItemsControl()
        {
            this.SizeChanged += UniformItemsControl_SizeChanged;

            // Prevents issues with higher DPIs and underlying panel.
            this.UseLayoutRounding = false;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject obj, object item)
        {
            base.PrepareContainerForItemOverride(obj, item);
            if (obj is FrameworkElement element)
            {
                element.SetBinding(WidthProperty, new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(nameof(ItemWidth)),
                    Mode = BindingMode.TwoWay
                });
            }
        }

        private void UniformItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize != e.NewSize)
            {
                const double MinItemWidth = 300;
                var numberOfItems = Math.Max((int)(e.NewSize.Width / MinItemWidth), 1);
                var remainingWidth = Math.Max(e.NewSize.Width - (MinItemWidth * numberOfItems), 0);
                var extraWidth = remainingWidth / numberOfItems;
                ItemWidth = MinItemWidth + extraWidth;

                RefreshItemsSourceCommand?.Execute(numberOfItems);
            }
        }
    }
}
