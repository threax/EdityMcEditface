using System.Collections.Generic;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models.Git;
using LibGit2Sharp;

namespace EdityMcEditface.Mvc.Repositories
{
    /// <summary>
    /// This repository handles committing files and viewing outstanding changes.
    /// </summary>
    public interface ICommitRepository
    {
        /// <summary>
        /// Commit the current working directory.
        /// </summary>
        /// <param name="signature">The user signature, from services.</param>
        /// <param name="newCommit">The new commit object.</param>
        void Commit(Signature signature, NewCommit newCommit);

        /// <summary>
        /// Get an enumerable over all uncommitted changes.
        /// </summary>
        /// <returns></returns>
        UncommittedChangeCollection UncommittedChanges();

        /// <summary>
        /// Get the diff of the file between its original and modified version.
        /// </summary>
        /// <param name="fileInfo">The file info for the file to get a diff for.</param>
        /// <returns>The file's diff info.</returns>
        DiffInfo UncommittedDiff(ITargetFileInfo fileInfo);
    }
}