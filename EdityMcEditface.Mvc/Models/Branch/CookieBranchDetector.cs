using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Branch
{
    public class CookieBranchDetector : IBranchDetector
    {
        private String cookieName;
        private JsonSerializer jsonSerializer;
        private IHttpContextAccessor context;

        public CookieBranchDetector(String cookieName, JsonSerializer jsonSerializer, IHttpContextAccessor context)
        {
            this.cookieName = cookieName;
            this.jsonSerializer = jsonSerializer;
        }

        public string RequestedBranch
        {
            get
            {
                return "master";
            }
        }
    }
}
