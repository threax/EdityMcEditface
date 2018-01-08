using EdityMcEditface.Mvc.Repositories;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                    Assert.Single(branches.Items);
                    Assert.Equal("master", branches.Items.First().FriendlyName);
                    Assert.Equal("refs/heads/master", branches.Items.First().CanonicalName);
                }
            }
        }

        [Fact]
        public void ListUpstreamOnlyMaster()
        {
            using (var dir = new SelfDeletingDirectory(Path.GetFullPath(Path.Combine(basePath, nameof(this.ListUpstreamOnlyMaster)))))
            {
                var upstreamPath = Path.Combine(dir.Path, "Upstream");
                var authorPath = Path.Combine(dir.Path, "Author");
                var downstreamPath = Path.Combine(dir.Path, "Downstream");

                Repository.Init(upstreamPath, true);
                Repository.Clone(upstreamPath, authorPath);
                using (var repo = new Repository(authorPath))
                {
                    var testFilePath = Path.Combine(dir.Path, "Author/test.txt");
                    File.WriteAllText(testFilePath, "Some test data.");
                    Commands.Stage(repo, testFilePath);
                    var signature = new Signature("Test Bot", "testbot@editymceditface.com", DateTime.Now);
                    repo.Commit("Added test data", signature, signature);

                    var authorBranchRepo = new BranchRepository(repo);
                    authorBranchRepo.Add("sidebranch");

                    repo.Network.Push(repo.Branches, new PushOptions() {  });
                }

                Repository.Clone(upstreamPath, downstreamPath);
                using (var repo = new Repository(downstreamPath))
                {
                    var branchRepo = new BranchRepository(repo);
                    var branches = branchRepo.List();
                    Assert.Single(branches.Items);
                    Assert.Equal("master", branches.Items.First().FriendlyName);
                    Assert.Equal("refs/heads/master", branches.Items.First().CanonicalName);
                }
            }
        }

        [Fact]
        public async Task UpstreamWithCheckout()
        {
            using (var dir = new SelfDeletingDirectory(Path.GetFullPath(Path.Combine(basePath, nameof(this.UpstreamWithCheckout)))))
            {
                var upstreamPath = Path.Combine(dir.Path, "Upstream");
                var authorPath = Path.Combine(dir.Path, "Author");
                var downstreamPath = Path.Combine(dir.Path, "Downstream");
                var contents = "Main Branch";
                var sideContents = "Side branch";
                var sideContentsRemoteChanges = "Side branch remote changes";
                var identity = new Identity("Test Bot", "testbot@editymceditface.com");

                Repository.Init(upstreamPath, true);
                Repository.Clone(upstreamPath, authorPath);
                using (var repo = new Repository(authorPath))
                {
                    var testFilePath = Path.Combine(dir.Path, "Author/test.txt");

                    //Create some test data on master
                    File.WriteAllText(testFilePath, contents);
                    Commands.Stage(repo, testFilePath);
                    var sig = new Signature(identity, DateTime.Now);
                    repo.Commit("Added test data", sig, sig);

                    //Switch to side branch, and make update
                    var authorBranchRepo = new BranchRepository(repo);
                    authorBranchRepo.Add("sidebranch");
                    authorBranchRepo.Checkout("sidebranch", new Signature(identity, DateTime.Now));
                    File.WriteAllText(testFilePath, sideContents);
                    Commands.Stage(repo, testFilePath);
                    sig = new Signature(identity, DateTime.Now);
                    repo.Commit("Updated branch data", sig, sig);

                    //Back to master
                    authorBranchRepo.Checkout("master", new Signature(identity, DateTime.Now));
                    String masterText = File.ReadAllText(testFilePath);
                    Assert.Equal(contents, masterText);

                    var syncRepo = new SyncRepository(repo, mockup.Get<ICommitRepository>());
                    await syncRepo.Push();
                }

                Repository.Clone(upstreamPath, downstreamPath);
                using (var repo = new Repository(downstreamPath))
                {
                    var testFilePath = Path.Combine(dir.Path, "Downstream/test.txt");
                    var branchRepo = new BranchRepository(repo);

                    //First check master
                    String masterText = File.ReadAllText(testFilePath);
                    Assert.Equal(contents, masterText);

                    //Swith to side branch and check
                    branchRepo.Checkout("sidebranch", new Signature(identity, DateTime.Now));
                    String sideText = File.ReadAllText(testFilePath);
                    Assert.Equal(sideContents, sideText);

                    //Now make some changes and send them back
                    File.WriteAllText(testFilePath, sideContentsRemoteChanges);
                    Commands.Stage(repo, testFilePath);
                    var sig = new Signature(identity, DateTime.Now);
                    repo.Commit("Updated branch remotely", sig, sig);

                    var syncRepo = new SyncRepository(repo, mockup.Get<ICommitRepository>());
                    await syncRepo.Push();
                }

                using (var repo = new Repository(authorPath))
                {
                    var syncRepo = new SyncRepository(repo, mockup.Get<ICommitRepository>());
                    await syncRepo.Pull(new Signature(identity, DateTime.Now));

                    var testFilePath = Path.Combine(dir.Path, "Author/test.txt");
                    var branchRepo = new BranchRepository(repo);
                    branchRepo.Checkout("sidebranch", new Signature(identity, DateTime.Now));

                    String text = File.ReadAllText(testFilePath);
                    Assert.Equal(sideContentsRemoteChanges, text);
                }
            }
        }
    }
}
