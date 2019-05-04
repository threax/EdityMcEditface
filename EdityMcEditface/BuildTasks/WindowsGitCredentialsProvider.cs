using EdityMcEditface.Mvc;
using LibGit2Sharp;
using Microsoft.Alm.Authentication;
using System;

namespace EdityMcEditface.BuildTasks
{
    public class WindowsGitCredentialsProvider : IGitCredentialsProvider
    {
        public Credentials GetCredentials(string url, string usernameFromUrl, SupportedCredentialTypes types)
        {
            var uri = new Uri(url);
            var host = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
            var secrets = new SecretStore("git");
            var auth = new BasicAuthentication(secrets);
            var creds = auth.GetCredentials(new TargetUri(host));

            return new UsernamePasswordCredentials()
            {
                Username = creds.Username,
                Password = creds.Password
            };
        }
    }
}
