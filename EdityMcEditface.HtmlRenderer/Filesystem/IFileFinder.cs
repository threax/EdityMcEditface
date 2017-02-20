using EdityMcEditface.HtmlRenderer.FileInfo;
using System;
using System.Collections.Generic;
using System.IO;

namespace EdityMcEditface.HtmlRenderer
{
    public interface IFileFinder
    {
        EdityProject Project { get; }
        IEnumerable<Template> Templates { get; }
        void CopyDependencyFiles(string baseOutDir, PageStack pageStack);
        void CopyProjectContent(string outDir);
        void DeleteFile(string file);
        void DeleteFolder(string folder);
        bool DoesLayoutExist(string layoutName);
        IEnumerable<String> EnumerateContentDirectories(String path, String searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);
        IEnumerable<String> EnumerateContentFiles(String path, String searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);
        void ErasePage(string file);
        PageDefinition GetProjectPageDefinition(ITargetFileInfo fileInfo);
        string GetProjectRelativePath(string path);
        bool IsValidWritablePath(string path);
        PageStackItem LoadPageStackContent(string path);
        PageStackItem LoadPageStackLayout(string path);
        Stream ReadFile(string file);
        void SavePageDefinition(PageDefinition definition, ITargetFileInfo fileInfo);
        Stream WriteFile(string file);
    }
}