using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Earmark.Helpers
{
    public static class FilePickerHelper
    {
        public static async Task<StorageFile> PickSingleFile(IEnumerable<string> fileTypes)
        {
            var openPicker = new FileOpenPicker();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Current.Window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            foreach (var fileType in fileTypes)
            {
                openPicker.FileTypeFilter.Add(fileType);
            }

            return await openPicker.PickSingleFileAsync();
        }
    }
}
