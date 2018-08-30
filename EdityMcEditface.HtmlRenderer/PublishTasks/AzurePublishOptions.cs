using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.PublishTasks
{
    /// <summary>
    /// Options for remote publishing.
    /// </summary>
    public class AzurePublishOptions
    {
        /// <summary>
        /// The name of the site to publish to. This is your part of the site.azurewebsites.net url.
        /// The full url used will be https://{site}.scm.azurewebsites.net/api/zipdeploy?isAsync=true
        /// </summary>
        public String SiteName { get; set; }

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
