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
        bool doesLayoutExist(string layoutName);
        IEnumerable<string> EnumerateDirectories(string path, FileType fileTypes);
        IEnumerable<string> EnumerateFiles(string path, FileType fileTypes);
        void erasePage(string file);
        void eraseProjectFile(string file);
        PageDefinition getProjectPageDefinition(TargetFileInfo fileInfo);
        string getProjectRelativePath(string path);
        bool isValidWritablePath(string path);
        PageStackItem loadPageStackContent(string path);
        PageStackItem loadPageStackLayout(string path);
        Stream readFile(string file);
        void savePageDefinition(PageDefinition definition, TargetFileInfo fileInfo);
        Stream writeFile(string file);
    }
}