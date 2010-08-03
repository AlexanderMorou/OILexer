using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Oilexer.FileModel
{
    public static class TemporaryFileHelper
    {
        public static Dictionary<string, TemporaryDirectory> TemporaryPaths;
        public static Dictionary<string, TemporaryFile> TemporaryFiles;

        static TemporaryFileHelper()
        {
            TemporaryPaths = new Dictionary<string, TemporaryDirectory>();
            TemporaryFiles = new Dictionary<string, TemporaryFile>();
            string tempPath = Path.GetTempPath();
            TemporaryPaths.Add(tempPath, new TemporaryDirectory(tempPath, true));
        }

        internal static TemporaryFile GetFile(string file, bool keep)
        {
            if (TemporaryFiles.ContainsKey(file))
                return TemporaryFiles[file];
            else
            {
                TemporaryFile tempFile = new TemporaryFile(file, false, keep);
                TemporaryFiles.Add(file, tempFile);
                return tempFile;
            }
        }

        internal static TemporaryFile GetFile(string file)
        {
            return TemporaryFileHelper.GetFile(file, true);
        }
        
        internal static TemporaryDirectory GetDirectory(string path)
        {
            path = Path.GetFullPath(path);
            if (path.Substring(path.Length - 1) != @"\")
                path += @"\";
            if (TemporaryPaths.ContainsKey(path))
                return TemporaryPaths[path];
            else
            {
                TemporaryDirectory tempDir = new TemporaryDirectory(path, true);
                TemporaryPaths.Add(path, tempDir);
                return tempDir;
            }
        }

        public static string GetTemporaryName()
        {
            return GetTemporaryName(Path.GetTempPath());
        }

        public static string GetTemporaryName(string path)
        {
            return GetTemporaryName(path, Path.GetRandomFileName());
        }

        public static string GetTemporaryName(string path, string pattern)
        {
            string fileName = null;
            string fileNameBase = pattern;
            string fileNameExt = "";
            if (fileNameBase.LastIndexOf('.') > -1)
            {
                fileNameExt = fileNameBase.Substring(fileNameBase.LastIndexOf('.'));
                fileNameBase = fileNameBase.Substring(0, fileNameBase.LastIndexOf('.'));
            }

            string temporaryPath = path;
            int index = 0;
            bool valid = false;

            while (!valid)
            {
                if (index++ != 0)
                    fileName = string.Format("{0}{1}{2}{3}", path, fileNameBase, index, fileNameExt);
                else
                    fileName = string.Format("{0}{1}{2}", path, fileNameBase, fileNameExt);
                valid = (!(File.Exists(fileName)));
            }
            return fileName;
        }

        public static string GetTemporaryPath()
        {
            return Path.GetTempPath();
        }

        public static string GetTemporaryPath(string pattern)
        {
            string path = GetTemporaryName(Path.GetTempPath(), pattern);
            return string.Format(@"{0}\", path);
        }

        public static TemporaryDirectory GetTemporaryDirectory(string dirName, bool keep)
        {
            if (dirName == "." || dirName.Contains(".."))
                throw new ArgumentException("Cannot refer to temp dir or parent.", "dirName");
            if (dirName.Contains(@"\") || dirName.Contains("/"))
                throw new ArgumentException("Cannot be more than one level deep.", "dirName");
            string fullPath = Path.GetTempPath();
            if (fullPath.Substring(fullPath.Length - 1) != @"\")
                fullPath += @"\";
            fullPath += dirName;
            if (fullPath.Substring(fullPath.Length - 1) != @"\")
                fullPath += @"\";
            if (!TemporaryFileHelper.TemporaryPaths.ContainsKey(fullPath))
                TemporaryPaths.Add(fullPath, new TemporaryDirectory(fullPath, keep));
            return TemporaryPaths[fullPath];            
        }
        public static TemporaryDirectory GetNonStandardTempDirectory(string path, string dirName, bool keep = true)
        {
            if (dirName == "." || dirName.Contains(".."))
                throw new ArgumentException("Cannot refer to temp dir or parent.", "dirName");
            if (dirName.Contains(@"\") || dirName.Contains("/"))
                throw new ArgumentException("Cannot be more than one level deep.", "dirName");
            string fullPath = path;
            if (fullPath.Substring(fullPath.Length - 1) != @"\")
                fullPath += @"\";
            fullPath += dirName;
            if (fullPath.Substring(fullPath.Length - 1) != @"\")
                fullPath += @"\";
            if (!TemporaryFileHelper.TemporaryPaths.ContainsKey(fullPath))
                TemporaryPaths.Add(fullPath, new TemporaryDirectory(fullPath, keep));
            return TemporaryPaths[fullPath];
        }
    }
}
