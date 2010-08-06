using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.FileModel;
using System.CodeDom.Compiler;
using Oilexer.Types;
using System.IO;
using Oilexer._Internal;
namespace Oilexer.Translation
{
    internal static class ProjectTranslator
    {
        internal static string GetFileNameFor(IDeclaredType type, string relativeRoot, IIntermediateProject project, string extension, IIntermediateCodeTranslatorOptions options, bool invertSeparator)
        {
            var bt = options.CurrentType;
            var currentFile = ObtainTypeFileName(bt, relativeRoot,project, extension);
            var targetFile = ObtainTypeFileName(type, relativeRoot, project, extension);
            if (currentFile == targetFile)
                return string.Empty;
            var actualRelativeRoot = GetRelativeRoot(new string[] { currentFile, targetFile });
            var uptrail = @"..\".Repeat(Path.GetDirectoryName(targetFile).Substring(actualRelativeRoot.Length).Split(new Char[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries).Length);
            if (invertSeparator)
                return GetExtensionFromRelativeRoot(targetFile, actualRelativeRoot, uptrail).Replace(@"\", "/");
            else
                return GetExtensionFromRelativeRoot(targetFile, actualRelativeRoot, uptrail);
        }

        private static string ObtainTypeFileName(IDeclaredType type, string relativeRoot, IIntermediateProject project, string extension)
        {
            return string.Format("{0}\\{1}\\{2}\\{3}{4}", relativeRoot, project.Name, GetFolderName(type), GetTargetFullName(type), extension);
        }

        public static void WriteProject(IIntermediateProject project, IIntermediateCodeTranslator translator, string temporaryDirectory, out TemporaryDirectory td, out TempFileCollection tfc, out List<string> files, out Stack<IIntermediateProject> partialCompletions, out Dictionary<IIntermediateModule, List<string>> moduleFiles, bool keepTempFiles, bool allowPartials, string extension = ".cs", bool purgeTempDir=false, string tabString = "    ")
        {
            td = TemporaryFileHelper.GetNonStandardTempDirectory(temporaryDirectory, project.Name, keepTempFiles);
            if (purgeTempDir)
            {
                try
                {
                    Directory.Delete(td.Path, true);
                }
                catch (IOException) { }
                Directory.CreateDirectory(td.Path);
            }
            tfc = new TempFileCollection(td.Path, td.Keep);
            files = new List<string>();
            partialCompletions = new Stack<IIntermediateProject>();
            moduleFiles = new Dictionary<IIntermediateModule, List<string>>();
            bool attributesOverride = !allowPartials;
            if (!attributesOverride)
                foreach (IIntermediateModule iim in project.Modules.Values)
                {
                    moduleFiles.Add(iim, new List<string>());
                    foreach (IDeclaredType idt in iim.DeclaredTypes)
                    {
                        string currentFile = string.Empty;
                        IDeclaredType target = (idt is ISegmentableDeclaredType) ? ((ISegmentableDeclaredType)idt).GetRootDeclaration() : idt;
                        currentFile = WriteFile(project, translator, td, tfc, partialCompletions, target, extension, tabString);
                        if (currentFile != null)
                        {
                            files.Add(currentFile);
                            moduleFiles[iim].Add(currentFile);
                        }
                        //Don't forget to include the partials...
                        if (target is ISegmentableDeclaredType)
                            foreach (IDeclaredType partial in ((ISegmentableDeclaredType)idt).Partials)
                            {
                                currentFile = WriteFile(project, translator, td, tfc, partialCompletions, partial, extension, tabString);
                                if (currentFile != null)
                                {
                                    files.Add(currentFile);
                                    moduleFiles[iim].Add(currentFile);
                                }
                            }
                    }
                }
            if (project.Attributes.Count > 0 || attributesOverride)
            {
                if (attributesOverride)
                    moduleFiles.Add(project.RootModule, new List<string>());
                string attrFile = null;
                /* *
                 * Only add it if there are no types in the 
                 * root declaration of the project.
                 * */
                var p = project;
                if (!p.IsRoot)
                    p = p.GetRootDeclaration();
                if (!partialCompletions.Contains(project))
                {
                    attrFile = WriteAttributeFile(project, translator, td, tfc, partialCompletions, extension, tabString);
                    if (attrFile != null)
                        moduleFiles[project.RootModule].Add(attrFile);
                }
            }
        }

        private static string WriteAttributeFile(IIntermediateProject project, IIntermediateCodeTranslator translator, TemporaryDirectory td, TempFileCollection tfc, Stack<IIntermediateProject> partialCompletions, string extension, string tabString)
        {
            if (td == null)
                throw new ArgumentNullException("td");
            var root = project;
            if (!root.IsRoot)
                root = root.GetRootDeclaration();
            if (!partialCompletions.Contains(root))
            {
                bool autoResolveRefs = translator.Options.AutoResolveReferences;
                translator.Options.AutoResolveReferences = false;
                TemporaryFile tf = td.Directories.GetTemporaryDirectory(root.RootModule.Name).Files.GetTemporaryFile(string.Format("AssemblyInfo{0}", extension));
                tfc.AddFile(tf.FileName, false);
                translator.Options.AutoResolveReferences = autoResolveRefs;
                tf.OpenStream(FileMode.Create);
                translator.Target = new IndentedTextWriter(new StreamWriter(tf.FileStream), tabString);
                translator.TranslateProject(root);
                translator.Target.Flush();
                partialCompletions.Push(root);
                tf.CloseStream();
                return tf.FileName;
            }
            return null;
        }

        internal static string WriteFile(IIntermediateProject project, IIntermediateCodeTranslator translator, TemporaryDirectory td, TempFileCollection tfc, Stack<IIntermediateProject> partialCompletions, IDeclaredType target, string extension, string tabString = "    ")
        {
            if (td == null)
                throw new ArgumentNullException("td");
            if (!partialCompletions.Contains(target.Project))
            {
                bool autoResolveRefs = translator.Options.AutoResolveReferences;
                translator.Options.AutoResolveReferences = false;
                var folderName = GetFolderName(target);
                var tempDir = td;
                string[] folders = folderName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in folders)
                    tempDir = tempDir.Directories.GetTemporaryDirectory(item);
                TemporaryFile tf = tempDir.Files.GetTemporaryFile(string.Format("{0}{1}", GetTargetFullName(target), extension));
                tfc.AddFile(tf.FileName, tfc.KeepFiles);
                translator.Options.AutoResolveReferences = autoResolveRefs;
                //If it's segmentable, there's going to be multiple files...
                tf.OpenStream(FileMode.Create);
                translator.Target = new IndentedTextWriter(new StreamWriter(tf.FileStream), tabString);
                translator.TranslateProject(target.Project);
                translator.Target.Flush();
                tf.CloseStream();
                partialCompletions.Push(target.Project);
                return tf.FileName;
            }
            return null;
        }

        private static string GetTargetFullName(IDeclaredType target)
        {
            if (target.ParentTarget is IDeclaredType)
                return string.Format("{0}+{1}", GetTargetFullName((IDeclaredType)target.ParentTarget), target.Name);
            return target.Name;
        }

        private static string GetFolderName(IDeclaredType target)
        {
            if (target.ParentTarget is IDeclaredType)
            {
                return GetFolderName((IDeclaredType)target.ParentTarget);
            }
            else if (target.ParentTarget is INameSpaceDeclaration)
            {
                Stack<INameSpaceDeclaration> sources = new Stack<INameSpaceDeclaration>();
                var project = target.Project;
                var dns = project.DefaultNameSpace.GetRootDeclaration();
                INameSpaceDeclaration current = (INameSpaceDeclaration)target.ParentTarget;
                while (current != null)
                {
                    if (current.ParentTarget is INameSpaceDeclaration)
                    {
                        if (current == project.DefaultNameSpace.GetRootDeclaration())
                        {
                            goto combineName;
                        }
                        else
                            sources.Push(current);
                        current = (current.ParentTarget as INameSpaceDeclaration).GetRootDeclaration();
                    }
                    else if (current.ParentTarget is IIntermediateProject)
                        goto combineName;
                }
            combineName:
                string name = target.Module.Name;
                foreach (var item in sources)
                {
                    name += @"\" + item.Name;
                }
                return name;
            }
            return target.Module.Name;
        }

        public static string GetExtensionFromRelativeRoot(string path, string relativeRoot, string uptrail = @".\")
        {
            string rPath = null;
            if (relativeRoot != null)
            {
                if (path == relativeRoot)
                    rPath = uptrail;
                else
                    rPath = uptrail + path.Substring(relativeRoot.Length + 1);
            }
            else
                rPath = path;
            return rPath;
        }

        public static string GetRelativeRoot(this IEnumerable<string> files)
        {
            var parts = (from f in files
                         let ePath = Path.GetDirectoryName(f)
                         orderby ePath.Length descending
                         select ePath).First().Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            string relativeRoot = null;
            for (int i = 0; i < parts.Length; i++)
            {
                string currentRoot = string.Join(@"\", parts, 0, parts.Length - i);
                if (files.All(p => p.Contains(currentRoot)))
                {
                    relativeRoot = currentRoot;
                    break;
                }
            }
            return relativeRoot;
        }
    }
}
