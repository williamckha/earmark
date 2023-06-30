using Earmark.Helpers;
using Earmark.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace Earmark.Views
{
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                this.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                this.AppWindow.SetTitleBarColors((this.Content as FrameworkElement).ActualTheme);
                this.AppTitleBar.Loaded += AppTitleBar_Loaded;
                this.AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            }
        }

        private void SetDragRegionForTitleBar()
        {
            if (AppWindowTitleBar.IsCustomizationSupported() && this.AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                double scaleAdjustment = this.GetScaleAdjustment();

                RightPaddingColumn.Width = new GridLength(this.AppWindow.TitleBar.RightInset / scaleAdjustment);

                List<Windows.Graphics.RectInt32> dragRectsList = new();

                Windows.Graphics.RectInt32 dragRect;
                dragRect.X = (int)((LeftPaddingColumn.ActualWidth) * scaleAdjustment);
                dragRect.Y = 0;
                dragRect.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
                dragRect.Width = (int)((IconColumn.ActualWidth
                                       + TitleColumn.ActualWidth
                                       + DragColumn.ActualWidth) * scaleAdjustment);
                dragRectsList.Add(dragRect);
                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();
                this.AppWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            SetDragRegionForTitleBar();
        }

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetDragRegionForTitleBar();
        }
    }
}
