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
                "logo.svg" => BR.logo,
                //"app.html" => BR.app_html,
                //"app.js" => BR.app_js,
                //"views/actions-view.js" => BR.views_actions_view_js,
                //"views/login-view.js" => BR.views_login_view_js,
                //"views/settings-view.js" => BR.views_settings_view_js,
                //"views/statistics-view.js" => BR.views_statistics_view_js,
                //"components/measurement-card.js" => BR.components_measurement_card_js,
                //"components/modal.js" => BR.components_modal_js,
                //"components/navbar.js" => BR.components_navbar_js,
                //"common/api-client.js" => BR.common_api_client_js,
                //"common/utils.js" => BR.common_utils_js,
                //"common/lib/pico.min.css" => BR.common_lib_pico_min_css,
                //"common/lib/preact.htm.module.js" => BR.common_lib_preact_htm_module_js,
                _ => default,
            };
            return resourceName != default;
        }
    }
}
