using System;
using System.Collections.Generic;
using System.Text;
using LibGit2Sharp;

namespace EdityMcEditface.Mvc
{
    public class DefaultGitCredentialsProvider : IGitCredentialsProvider
    {
        public Credentials GetCredentials(string url, string usernameFromUrl, SupportedCredentialTypes types)
        {
            return null;
        }
    }
}
