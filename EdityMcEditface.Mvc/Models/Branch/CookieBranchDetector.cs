using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Branch
{
    public class CookieBranchDetector : IBranchDetector
    {
        private String cookieName;
        private JsonSerializer jsonSerializer;
        private IHttpContextAccessor context;
        private String prepublishBranchName;

        public CookieBranchDetector(String cookieName, String prepublishBranchName, JsonSerializer jsonSerializer, IHttpContextAccessor context)
        {
            this.cookieName = cookieName;
            this.prepublishBranchName = prepublishBranchName;
            this.jsonSerializer = jsonSerializer;
            this.context = context;
        }

        public string RequestedBranch
        {
            get
            {
                var cookie = GetBranchCookie();
                return cookie.CurrentBranch;
            }
            set
            {
                var cookie = GetBranchCookie();
                cookie.CurrentBranch = value;
                using(var writer = new StringWriter())
                {
                    jsonSerializer.Serialize(writer, cookie);
                    context.HttpContext.Response.Cookies.Append(cookieName, writer.ToString());
                }
            }
        }

        private BranchCookie GetBranchCookie()
        {
            var cookie = context.HttpContext.Request.Cookies[cookieName];
            if (cookie != null)
            {
                using (JsonTextReader reader = new JsonTextReader(new StringReader(cookie)))
                {
                    var branchCookie = jsonSerializer.Deserialize<BranchCookie>(reader);
                    return branchCookie;
                }
            }
            return new BranchCookie();
        }

        public bool IsPrepublishBranch
        {
            get
            {
                return this.RequestedBranch == prepublishBranchName;
            }
        }

        public string PrepublishedBranchName
        {
            get
            {
                return prepublishBranchName;
            }
        }
    }
}
