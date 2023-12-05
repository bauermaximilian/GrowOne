/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using BR = GrowOne.Resources.WebResources.BinaryResources;

namespace GrowOne.Resources
{
    internal class WebResourceResolver
    {
        /// <summary>
        /// Resolves an <paramref name="url"/> to an embedded GUI file resource.
        /// </summary>
        /// <param name="url">
        /// The URI of the requested resource. Case-insensitive.
        /// </param>
        /// <returns>
        /// The name of the resource, or <see cref="default"/> if the resource wasn't found.
        /// </returns>
        public static bool TryResolveResourceName(string? url, out BR resourceName)
        {
            // Add new files to WebResources folder and this project, link it in WebResources.resx
            // (there, set item property "FileType" to "Binary") and finally add a new mapping
            // from lowercase URL to "BR" (WebResources.BinaryResources) here.
            resourceName = url?.ToLower().Trim('/', ' ') switch
            {
                "index.html" => BR.index_html,
                "logo.svg" => BR.logo_svg,
                _ => default,
            };
            return resourceName != default;
        }
    }
}
