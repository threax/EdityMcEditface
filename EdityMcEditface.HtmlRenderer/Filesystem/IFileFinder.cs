using EdityMcEditface.HtmlRenderer.FileInfo;
using System;
using System.Collections.Generic;
using System.IO;

namespace EdityMcEditface.HtmlRenderer
{
    public interface IFileFinder
    {
        /// <summary>
        /// The EdityProject loaded from this file finder.
        /// </summary>
        EdityProject Project { get; }

        /// <summary>
        /// The templates defined in the files.
        /// </summary>
        IEnumerable<Template> Templates { get; }

        /// <summary>
        /// Copy the linked content files for the given page stack.
        /// </summary>
        /// <param name="baseOutDir">The base out directory to copy files to.</param>
        /// <param name="pageStack">The page stack to read content files from.</param>
        void CopyLinkedFiles(string baseOutDir, PageStack pageStack);

        /// <summary>
        /// Copy any other content defined in the project.
        /// </summary>
        /// <param name="outDir">The directory to copy to.</param>
        void CopyProjectContent(string outDir);

        /// <summary>
        /// Delete a file from the project.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        void DeleteFile(string file);

        /// <summary>
        /// Delete a folder from the project.
        /// </summary>
        /// <param name="folder">The folder to delete.</param>
        void DeleteFolder(string folder);

        /// <summary>
        /// Check to see if a named layout exists.
        /// </summary>
        /// <param name="layoutName">The name of the layout to lookup.</param>
        /// <returns>True if the layout exists.</returns>
        bool DoesLayoutExist(string layoutName);

        /// <summary>
        /// Enumerate through the directories marked as having content.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The pattern to search for.</param>
        /// <param name="searchOption">The search options</param>
        /// <returns></returns>
        IEnumerable<String> EnumerateContentDirectories(String path, String searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Enumerate through the files marked as having content.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The pattern to search for.</param>
        /// <param name="searchOption">The search options</param>
        /// <returns></returns>
        IEnumerable<String> EnumerateContentFiles(String path, String searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Erase a page and all its associated files.
        /// </summary>
        /// <param name="file">The name of the html file to erase.</param>
        void ErasePage(string file);

        /// <summary>
        /// Call this to prepublish a page by moving it to the prepublished files.
        /// </summary>
        /// <param name="file">The page's html file to move.</param>
        void PrepublishPage(String file);

        /// <summary>
        /// Get the definition for the specified page.
        /// </summary>
        /// <param name="fileInfo">The file information for the page.</param>
        /// <returns></returns>
        PageDefinition GetProjectPageDefinition(ITargetFileInfo fileInfo);

        /// <summary>
        /// Get the path relative to the project root directory.
        /// </summary>
        /// <param name="path">The path to make relative.</param>
        /// <returns></returns>
        string GetProjectRelativePath(string path);

        /// <summary>
        /// Determine if a path can be written to legally.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns></returns>
        bool IsValidWritablePath(string path);

        /// <summary>
        /// Load the content for a page stack.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        PageStackItem LoadPageStackContent(string path);

        /// <summary>
        /// Load the layout for a page stack.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        PageStackItem LoadPageStackLayout(string path);

        /// <summary>
        /// Read the file and return a stream.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Stream ReadFile(string file);

        /// <summary>
        /// Save the page definition.
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="fileInfo"></param>
        void SavePageDefinition(PageDefinition definition, ITargetFileInfo fileInfo);

        /// <summary>
        /// Write a file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Stream WriteFile(string file);

        /// <summary>
        /// Load a section file and return its contents.
        /// </summary>
        /// <param name="path">The path to load.</param>
        /// <returns>The contents of path.</returns>
        String LoadSection(string path);
    }
}