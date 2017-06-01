using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Branch
{
    public class CookiePhaseDetector : IPhaseDetector
    {
        private String cookieName;
        private JsonSerializer jsonSerializer;
        private IHttpContextAccessor context;

        public CookiePhaseDetector(String cookieName, JsonSerializer jsonSerializer, IHttpContextAccessor context)
        {
            this.cookieName = cookieName;
            this.jsonSerializer = jsonSerializer;
            this.context = context;
        }

        public Phases Phase
        {
            get
            {
                var cookie = GetBranchCookie();
                return cookie.Current;
            }
            set
            {
                var cookie = GetBranchCookie();
                cookie.Current = value;
                using(var writer = new StringWriter())
                {
                    jsonSerializer.Serialize(writer, cookie);
                    context.HttpContext.Response.Cookies.Append(cookieName, writer.ToString());
                }
            }
        }

        private PhaseCookie GetBranchCookie()
        {
            var cookie = context.HttpContext.Request.Cookies[cookieName];
            if (cookie != null)
            {
                using (JsonTextReader reader = new JsonTextReader(new StringReader(cookie)))
                {
                    var branchCookie = jsonSerializer.Deserialize<PhaseCookie>(reader);
                    return branchCookie;
                }
            }
            return new PhaseCookie();
        }
    }
}
