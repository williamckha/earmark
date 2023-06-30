using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;

namespace Earmark.Helpers
{
    public static class WindowExtensions
    {
        /// <summary>
        /// Get the adjustment multiplier for mapping scaled client coordinates to screen coordinates.
        /// </summary>
        /// <returns>The scale adjustment.</returns>
        public static double GetScaleAdjustment(this Window window)
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(window);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            var hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            if (NativeMethods.GetDpiForMonitor(hMonitor, NativeMethods.MONITOR_DPI_TYPE.MDT_DEFAULT, out uint dpiX, out uint _) != 0)
            {
                throw new Exception("Could not get DPI for monitor.");
            }

            var scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorPercent / 100.0;
        }

        /// <summary>
        /// Applies the appropiate color palette to the AppWindow title bar based on the
        /// specified UI theme.
        /// </summary>
        /// <param name="theme">The UI theme to use.</param>
        public static void SetTitleBarColors(this AppWindow appWindow, ElementTheme theme)
        {
            if (!AppWindowTitleBar.IsCustomizationSupported()) return;

            switch (theme)
            {
                case ElementTheme.Light:
                    appWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                    appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                    appWindow.TitleBar.ButtonHoverForegroundColor = Colors.Black;
                    appWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 180, 180, 180);
                    appWindow.TitleBar.ButtonPressedForegroundColor = Colors.Black;
                    appWindow.TitleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 150, 150, 150);

                    appWindow.TitleBar.InactiveForegroundColor = Colors.DimGray;
                    appWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
                    appWindow.TitleBar.ButtonInactiveForegroundColor = Colors.DimGray;
                    appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    break;

                case ElementTheme.Dark:
                    appWindow.TitleBar.ButtonForegroundColor = Colors.White;
                    appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                    appWindow.TitleBar.ButtonHoverForegroundColor = Colors.White;
                    appWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 90, 90, 90);
                    appWindow.TitleBar.ButtonPressedForegroundColor = Colors.White;
                    appWindow.TitleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 120, 120, 120);

                    appWindow.TitleBar.InactiveForegroundColor = Colors.Gray;
                    appWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
                    appWindow.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                    appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    break;

                case ElementTheme.Default:
                    var requestedTheme = App.Current.RequestedTheme;
                    if (requestedTheme == ApplicationTheme.Light) goto case ElementTheme.Light;
                    if (requestedTheme == ApplicationTheme.Dark) goto case ElementTheme.Dark;
                    break;
            }
        }
    }
}
