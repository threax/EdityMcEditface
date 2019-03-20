using EdityMcEditface.Mvc;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.BuiltInTools;

namespace EdityMcEditface.ToolControllers
{
    public class RepoTools
    {
        const string origin = "origin";

        public static Task Clone(ToolArgs a, ProjectConfiguration projectConfig)
        {
            if (a.Args.Count < 1)
            {
                throw new InvalidOperationException("You must include the source repository that you want to clone.");
            }
            var log = a.Scope.ServiceProvider.GetRequiredService<ILogger<Repository>>();
            var sourceRepo = a.Args[0];
            var masterDir = Path.GetFullPath(Path.Combine(projectConfig.ProjectPath, "Master"));
            var cloneDir = projectConfig.ProjectPath;

            if (projectConfig.ProjectMode == ProjectMode.OneRepoPerUser)
            {
                cloneDir = Path.Combine(projectConfig.ProjectPath, "Sync");

                if (Directory.Exists(masterDir))
                {
                    throw new InvalidOperationException($"Directory {masterDir} already exists. No clone will take place.");
                }
            }

            cloneDir = Path.GetFullPath(cloneDir);

            //If clone repo exists, do nothing
            if (Directory.Exists(cloneDir) && Directory.EnumerateFileSystemEntries(cloneDir).Any())
            {
                throw new InvalidOperationException($"Directory {cloneDir} already exists and is not empty. No clone will take place.");
            }

            //Clone origin repo
            log.LogInformation($"Cloning {sourceRepo}");
            Repository.Clone(new Uri(sourceRepo).AbsoluteUri, cloneDir, new CloneOptions()
            {
                CredentialsProvider = (url, user, cred) => GetCredentials(a)
            });

            //Create local master repo, if one repo per user
            if (projectConfig.ProjectMode == ProjectMode.OneRepoPerUser)
            {
                //Create master repo if it does not exist

                if (!Directory.Exists(masterDir))
                {
                    Directory.CreateDirectory(masterDir);
                    Repository.Init(masterDir, true);
                }

                //Add origin and push from sync to master
                var repo = new Repository(cloneDir);
                ChangeRemote(repo, masterDir);

                var remote = repo.Network.Remotes[origin];
                var options = new PushOptions();

                foreach (var branch in repo.Branches.Where(i => !i.IsRemote))
                {
                    log.LogInformation($"Pushing branch {branch.CanonicalName} to local master.");
                    repo.Network.Push(remote, branch.CanonicalName, options);
                }
            }

            return Task.FromResult(0);
        }

        private static UsernamePasswordCredentials GetCredentials(ToolArgs a)
        {
            var userPassCredentials = new UsernamePasswordCredentials()
            {
                Username = "",
                Password = ""
            };

            for (var i = 1; i < a.Args.Count; ++i)
            {
                if (a.Args[i] == "-u")
                {
                    userPassCredentials.Username = a.Args[++i];
                }

                if (a.Args[i] == "-p")
                {
                    userPassCredentials.Password = a.Args[++i];
                }
            }

            return userPassCredentials;
        }

        public static Task PushMaster(ToolArgs a, ProjectConfiguration projectConfig)
        {
            if (projectConfig.ProjectMode != ProjectMode.OneRepoPerUser)
            {
                throw new InvalidOperationException("The project mode must be OneRepoPerUser to use the pushmaster tool");
            }

            if (a.Args.Count < 1)
            {
                throw new InvalidOperationException("You must include the destination repository that you want to push to.");
            }
            var log = a.Scope.ServiceProvider.GetRequiredService<ILogger<Repository>>();
            var destRepo = a.Args[0];
            var masterDir = Path.GetFullPath(Path.Combine(projectConfig.ProjectPath, "Master"));
            if (!Directory.Exists(masterDir))
            {
                throw new InvalidOperationException($"Master dir {masterDir} does not exist. No push will occur.");
            }

            var syncDir = Path.GetFullPath(Path.Combine(projectConfig.ProjectPath, "Sync"));
            if (!Directory.Exists(syncDir))
            {
                throw new InvalidOperationException($"Sync dir {syncDir} does not exist. No push will occur.");
            }

            var syncRepo = new Repository(syncDir);

            //Change origin to master and pull changes from Master
            log.LogInformation("Pulling from master to sync.");
            ChangeRemote(syncRepo, masterDir);
            var result = Commands.Pull(syncRepo, new Signature("syncbot", "syncbot@syncbot", DateTime.Now), new PullOptions());

            //Change origin to new desitination and push
            ChangeRemote(syncRepo, destRepo);

            //Push to dest
            var remote = syncRepo.Network.Remotes[origin];
            var options = new PushOptions()
            {
                CredentialsProvider = (url, user, cred) => GetCredentials(a)
            };
            foreach (var branch in syncRepo.Branches.Where(i => !i.IsRemote))
            {
                log.LogInformation($"Pushing branch {branch.CanonicalName} to origin master.");
                syncRepo.Network.Push(remote, branch.CanonicalName, options);
            }

            return Task.FromResult(0);
        }

        private static void ChangeRemote(Repository repo, String url)
        {
            if (repo.Network.Remotes.Any(i => i.Name == origin))
            {
                var remote = repo.Network.Remotes[origin];
                repo.Network.Remotes.Update(origin, r => r.Url = url);
            }
            else
            {
                throw new InvalidOperationException($"Cannot find origin remote on {repo.Info.WorkingDirectory}. No push will occur.");
            }
        }
    }
}
