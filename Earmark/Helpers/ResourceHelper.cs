using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Earmark.Helpers
{
    public static class ResourceHelper
    {
        private static readonly ResourceMap resourcesTree = new ResourceManager().MainResourceMap.TryGetSubtree("Resources");
        private static readonly ConcurrentDictionary<string, string> cachedResources = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Retrieves the provided resource for the given key.
        /// </summary>
        /// <param name="resourceKey">The resource key to retrieve.</param>
        /// <returns>String value for given resource or empty string if not found.</returns>
        public static string GetLocalizedResource(this string resourceKey)
        {
            if (cachedResources.TryGetValue(resourceKey, out var value))
            {
                return value;
            }
            value = resourcesTree?.TryGetValue(resourceKey)?.ValueAsString;
            return cachedResources[resourceKey] = value ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the text content of an app resource. 
        /// </summary>
        /// <param name="uri">The URI of the app resource.</param>
        /// <returns>The text content of the app resource.</returns>
        public static async Task<string> GetAppResourceContent(Uri uri)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            return File.ReadAllText(file.Path, Encoding.UTF8);
        }
    }
}