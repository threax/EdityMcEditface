using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    /// <summary>
    /// Options for remote publishing.
    /// </summary>
    public class RemotePublishOptions
    {
        /// <summary>
        /// The host url to publish to.
        /// </summary>
        public String Host { get; set; }

        /// <summary>
        /// The method to use when publishing, defaults to POST.
        /// </summary>
        public String Method { get; set; } = "POST";

        /// <summary>
        /// The username. If present it is sent as base64(User:Password) in the Authorization header.
        /// Password is required if you set this.
        /// </summary>
        public String User { get; set; }

        /// <summary>
        /// The password.
        /// </summary>
        public String Password { get; set; }
    }
}
