using System.Collections.Generic;
using System.IO;

namespace EdityMcEditface.HtmlRenderer
{
    public interface IFileFinder
    {
        EdityProject Project { get; }
        IEnumerable<Template> Templates { get; }
        void copyDependencyFiles(string baseOutDir, PageStack pageStack);
        void copyProjectContent(string outDir);
        void deleteFile(string file);
        void deleteFolder(string folder);
        bool doesLayoutExist(string layoutName);
        IEnumerable<string> enumerateDirectories(string path);
        IEnumerable<string> enumerateFiles(string path);
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