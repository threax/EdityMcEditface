﻿using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.HtmlRenderer.Filesystem;
using EdityMcEditface.Mvc.Models;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    [Authorize(AuthenticationSchemes = AuthCoreSchemes.Bearer, Roles = Roles.CreateDrafts)]
    public class DraftController : Controller
    {
        public static class Rels
        {
            public const String Begin = "BeginDraft";
            public const String List = "ListDrafts";
            public const String Get = "GetDraft";
            public const String SubmitLatestDraft = "SubmitLatestDraft";
            public const String SubmitAllDrafts = "SubmitAllDrafts";
        }

        private IDraftRepository draftRepo;

        public DraftController(IDraftRepository draftRepo)
        {
            this.draftRepo = draftRepo;
        }

        /// <summary>
        /// Start the draft process, this will determine if you can draft and what you should do if you can't.
        /// </summary>
        /// <param name="syncRepo"></param>
        /// <returns>The entry point for drafts.</returns>
        [HttpGet("[action]")]
        [HalRel(Rels.Begin)]
        public async Task<DraftEntryPoint> Begin([FromServices] ISyncRepository syncRepo)
        {
            var syncInfo = await syncRepo.SyncInfo();
            return new DraftEntryPoint()
            {
                HasUncommittedChanges = syncInfo.HasUncomittedChanges,
                HasUnsyncedChanges = syncInfo.NeedsPull || syncInfo.NeedsPush
            };
        }

        /// <summary>
        /// Get the list of pages in draft.
        /// </summary>
        /// <returns>The list of drafted files.</returns>
        [HttpGet]
        [HalRel(Rels.List)]
        public Task<DraftCollection> List([FromQuery]DraftQuery query)
        {
            if (query.File == null)
            {
                //Check url query, if file was in it we are looking for the default file (index usually)
                var hasFile = HttpContext.Request.Query.Any(i => "file".Equals(i.Key, StringComparison.OrdinalIgnoreCase));
                if (hasFile)
                {
                    query.File = "";
                }
            }
            return draftRepo.List(query);
        }

        /// <summary>
        /// Get the draft info for a single file.
        /// </summary>
        /// <param name="file">The file to lookup.</param>
        /// <returns>The draft info for the file.</returns>
        [HttpGet("[action]/{*File}")]
        [HalRel(Rels.Get)]
        public Task<Draft> Get(String file)
        {
            return draftRepo.GetInfo(file);
        }

        /// <summary>
        /// Update all pages and files with never drafted out of date status to their latest draft.
        /// </summary>
        /// <returns></returns>
        [HttpPut("[action]")]
        [HalRel(Rels.SubmitAllDrafts)]
        public Task SubmitAll()
        {
            return draftRepo.SubmitAll();
        }

        /// <summary>
        /// Mark the given file's latest revision as its draft.
        /// </summary>
        /// <param name="file">The name of the file.</param>
        /// <returns></returns>
        [HttpPut("{*File}")]
        [HalRel(Rels.SubmitLatestDraft)]
        public async Task<Draft> Put(String file)
        {
            await draftRepo.Submit(file);
            return await Get(file);
        }
    }
}
