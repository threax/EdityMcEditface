using EdityMcEditface.Mvc.Repositories;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.Tests;
using Xunit;

namespace EdityMcEditface.Mvc.Tests
{
    public class BranchRepositoryTests
    {
        private Mockup mockup = new Mockup();
        private String basePath;

        public BranchRepositoryTests()
        {
            basePath = this.GetType().Name;
        }

        [Fact]
        public void ListEmpty()
        {
            using (var dir = new SelfDeletingDirectory(Path.GetFullPath(Path.Combine(basePath, nameof(this.ListEmpty)))))
            {
                Repository.Init(dir.Path, false);
                using (var repo = new Repository(dir.Path))
                {
                    var branchRepo = new BranchRepository(repo);
                    var branches = branchRepo.List();
                    Assert.Empty(branches.Items);
                }
            }
        }

        [Fact]
        public void ListAutoMaster()
        {
            using (var dir = new SelfDeletingDirectory(Path.GetFullPath(Path.Combine(basePath, nameof(this.ListAutoMaster)))))
            {
                Repository.Init(dir.Path, false);
                using (var repo = new Repository(dir.Path))
                {
                    var testFilePath = Path.Combine(dir.Path, "test.txt");
                    File.WriteAllText(testFilePath, "Some test data.");
                    Commands.Stage(repo, testFilePath);
                    var signature = new Signature("Test Bot", "testbot@editymceditface.com", DateTime.Now);
                    repo.Commit("Added test data", signature, signature);

                    var branchRepo = new BranchRepository(repo);
                    var branches = branchRepo.List();
                    Assert.NotEmpty(branches.Items);
                    Assert.Equal("master", branches.Items.First().FriendlyName);
                    Assert.Equal("refs/heads/master", branches.Items.First().CanonicalName);
                }
            }
        }
    }
}
