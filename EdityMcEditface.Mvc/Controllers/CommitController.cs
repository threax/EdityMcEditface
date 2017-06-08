using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models.Git;
using EdityMcEditface.Mvc.Repositories;
using LibGit2Sharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    [Authorize(Roles = Roles.EditPages)]
    public class CommitController : Controller
    {
        public static class Rels
        {
            public const String GetUncommittedChanges = "GetUncommittedChanges";
            public const String Commit = "Commit";
            public const String GetUncommittedDiff = "GetUncommittedDiff";
        }

        private ICommitRepository commitRepository;
        private ITargetFileInfoProvider fileInfoProvider;

        public CommitController(ICommitRepository commitRepository, ITargetFileInfoProvider fileInfoProvider)
        {
            this.commitRepository = commitRepository;
            this.fileInfoProvider = fileInfoProvider;
        }

        [HttpPut]
        [HalRel(Rels.Commit)]
        public void Put([FromServices]Signature signature, [FromBody]NewCommit newCommit)
        {
            this.commitRepository.Commit(signature, newCommit);
        }

        [HttpGet]
        [HalRel(Rels.GetUncommittedChanges)]
        public UncommittedChangeCollection Get()
        {
            return this.commitRepository.UncommittedChanges();
        }

        [HttpGet("{FilePath}")]
        [HalRel(Rels.GetUncommittedDiff)]
        public DiffInfo UncommittedDiff(String filePath)
        {
            var targetFileInfo = fileInfoProvider.GetFileInfo(filePath, HttpContext.Request.PathBase);
            var diff = commitRepository.UncommittedDiff(targetFileInfo);
            diff.FilePath = filePath;
            return diff;
        }
    }
}
