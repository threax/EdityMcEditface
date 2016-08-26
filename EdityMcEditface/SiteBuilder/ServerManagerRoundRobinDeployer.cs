﻿using EdityMcEditface.HtmlRenderer.SiteBuilder;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface
{
    public class ServerManagerRoundRobinDeployer : RoundRobinDeployer
    {
        public String siteName;

        public ServerManagerRoundRobinDeployer(String siteName)
        {
            this.siteName = siteName;
        }

        public bool Deploy(String outputFolder)
        {
            //This works, but have to be admin
            using (ServerManager serverManager = String.IsNullOrWhiteSpace(AppHostConfigPath) ? new ServerManager() : new ServerManager(AppHostConfigPath))
            {
                var site = serverManager.Sites[siteName];
                if (site != null)
                {
                    var application = site.Applications.FirstOrDefault(); //Assumes there is only one app per site
                    if (application != null)
                    {
                        var vDir = application.VirtualDirectories.FirstOrDefault();
                        if (vDir != null)
                        {
                            vDir.PhysicalPath = outputFolder;
                            serverManager.CommitChanges();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public String AppHostConfigPath { get; set; }
    }
}
