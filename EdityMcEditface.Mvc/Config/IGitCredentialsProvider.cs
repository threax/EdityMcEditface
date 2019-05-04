using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc
{
    public interface IGitCredentialsProvider
    {
        /// <summary>
        /// Get credentials for connecting to a remote git repo.
        /// </summary>
        /// <returns></returns>
        Credentials GetCredentials(string url, string usernameFromUrl, SupportedCredentialTypes types);
    }
}
