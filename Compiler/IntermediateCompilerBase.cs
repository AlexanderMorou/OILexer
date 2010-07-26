using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.FileModel;
using Oilexer.Types;
using System.IO;
using System.CodeDom.Compiler;
using Oilexer.Utilities.Arrays;
using System.Reflection;
using Oilexer.Types.Members;
using Oilexer.Expression;
using Oilexer.Translation;
using System.Runtime.CompilerServices;

namespace Oilexer.Compiler
{
    public abstract class IntermediateCompilerBase :
        IIntermediateCompiler
    {
        private Predicate<IIntermediateProject> fileRegulationDelegate;
        /// <summary>
        /// Data member for <see cref="Translator"/>.
        /// </summary>
        private IIntermediateCodeTranslator translator;
        /// <summary>
        /// Data member for <see cref="Project"/>.
        /// </summary>
        private IIntermediateProject project;
        /// <summary>
        /// Data member for <see cref="Module"/>.
        /// </summary>
        private IIntermediateCompilerModule module;
        private IIntermediateCompilerOptions options;
        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerBase"/> with the <paramref name="project"/> and
        /// <paramref name="translator"/> provided.
        /// </summary>
        /// <param name="project">The <see cref="IIntermediateProject"/> that needs compiled.</param>
        /// <param name="translator">The <see cref="IIntermediateCodeTranslator"/> root to
        /// translate the intermediate code into a proper langauge for compilation.</param>
        /// <param name="options">The <see cref="IIntermediateCompilerOptions"/> which guides the <see cref="Compile()"/> process.</param>
        internal IntermediateCompilerBase(IIntermediateProject project, IIntermediateCodeTranslator translator, IIntermediateCompilerOptions options, IIntermediateCompilerModule module)
        {
            this.translator = translator;
            this.project = project;
            this.options = options;
            this.module = module;
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerBase"/> instance with the <paramref name="project"/>, 
        /// <paramref name="translator"/> and <paramref name="fileRegulationDelegate"/> provided.
        /// </summary>
        /// <param name="project">The <see cref="IIntermediateProject"/> that needs compiled.</param>
        /// <param name="translator">The <see cref="IIntermediateProject"/> root to translate
        /// the intermediate code into a proper langauge for compilation.</param>
        /// <param name="options">The <see cref="IIntermediateCompilerOptions"/> which guides the <see cref="Compile()"/> process.</param>
        /// <param name="module">The <see cref="IIntermediateCompilerModule"/> that determines how
        /// to process the files.</param>
        /// <param name="fileRegulationDelegate">The <see cref="Predicate{T}"/> which determines
        /// whether to translate any given <see cref="IIntermediateProject"/> partial
        /// into code.</param>
        internal IntermediateCompilerBase(IIntermediateProject project, IIntermediateCodeTranslator translator, IIntermediateCompilerOptions options, IIntermediateCompilerModule module, Predicate<IIntermediateProject> fileRegulationDelegate)
            : this(project, translator, options, module)
        {
            this.fileRegulationDelegate = fileRegulationDelegate;
        }

        /// <summary>
        /// Returns whether the <see cref="IntermediateCompilerBase"/> should compile in the 
        /// associated file.
        /// </summary>
        /// <param name="projectFile">The <see cref="IIntermediateProject"/> instance as a root or partial
        /// that is to be checked.</param>
        /// <returns>true if the <paramref name="projectFile"/> should be compiled with the rest;
        /// false, otherwise.</returns>
        internal bool HandleFileCreation(IIntermediateProject projectFile)
        {
            if (this.fileRegulationDelegate != null)
                return this.fileRegulationDelegate(projectFile);
            return true;
        }


        #region IIntermediateCompiler Members

        /// <summary>
        /// Returns/sets the <see cref="IIntermediateCompilerOptions"/> that are used to guide the
        /// <see cref="Compile()"/> process.
        /// </summary>
        public IIntermediateCompilerOptions Options
        {
            get
            {
                return this.options;
            }
            set
            {
                this.options = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="TranslationProcessingSource"/> which notes the means to which 
        /// the intermediate construct is translated into code.
        /// </summary>
        public TranslationProcessingSource GeneratorSource
        {
            get
            {
                if (this.translator is ICodeDomTranslator)
                    return TranslationProcessingSource.CodeDom;
                else
                    return TranslationProcessingSource.OIL;
            }
        }

        public string CompileToResponseFile()
        {
            TemporaryDirectory td;
            TempFileCollection tfc;
            List<string> files;
            Stack<IIntermediateProject> partialCompletions;
            Dictionary<IIntermediateModule, List<string>> moduleFiles;
            TranslateSource(out td, out tfc, out files, out partialCompletions, out moduleFiles);

            IIntermediateCompilerCommandLineModule module;
            List<string> commandSequences;
            string response;
            CompileToResponse(td, tfc, files, partialCompletions, moduleFiles, out module, out commandSequences, out response);

            return response;
        }

        public IIntermediateCompilerResults Compile()
        {
            TemporaryDirectory td;
            TempFileCollection tfc;
            List<string> files;
            Stack<IIntermediateProject> partialCompletions;
            Dictionary<IIntermediateModule, List<string>> moduleFiles;
            TranslateSource(out td, out tfc, out files, out partialCompletions, out moduleFiles);

            switch (this.Module.Type)
            {
                case CompilerModuleType.Callback:
                    {
                        IIntermediateCompilerCallbackModule module = (IIntermediateCompilerCallbackModule)this.Module;
                    }
                    break;
                case CompilerModuleType.CommandLine:
                    {

                        IIntermediateCompilerCommandLineModule module;
                        List<string> commandSequences;
                        string response;
                        CompileToResponse(td, tfc, files, partialCompletions, moduleFiles, out module, out commandSequences, out response);
                        return module.Compile(commandSequences.ToArray(), response, tfc, this.Options);
                    }
            }
            throw new Exception("The method or operation is not implemented.");
        }

        private void CompileToResponse(TemporaryDirectory td, TempFileCollection tfc, List<string> files, Stack<IIntermediateProject> partialCompletions, Dictionary<IIntermediateModule, List<string>> moduleFiles, out IIntermediateCompilerCommandLineModule module, out List<string> commandSequences, out string response)
        {
            module = (IIntermediateCompilerCommandLineModule)this.Module;
            commandSequences = new List<string>();
            if (this.Project.OutputType == ProjectOutputType.ConsoleApplication || this.Project.OutputType == ProjectOutputType.WindowsApplication)
            {
                IMethodMember entryPoint = Project.EntryPoint;
                if (entryPoint != null && ((IDeclaredType)entryPoint.ParentTarget).Module != Project.RootModule)
                {
                    IClassType entryPointCover = Project.Partials.AddNew().DefaultNameSpace.Classes.AddNew(Project.Classes.GetUniqueName("_cover"));

                    entryPointCover.Attributes.AddNew(typeof(CompilerGeneratedAttribute));
                    entryPointCover.IsStatic = true;
                    entryPointCover.AccessLevel = DeclarationAccessLevel.Internal;
                    entryPointCover.Module = Project.RootModule;
                    IMethodMember coverPoint = entryPointCover.Methods.AddNew(new TypedName(entryPoint.Name, entryPoint.ReturnType));
                    coverPoint.Attributes.AddNew(typeof(CompilerGeneratedAttribute));
                    foreach (IMethodParameterMember impm in entryPoint.Parameters.Values)
                        coverPoint.Parameters.AddNew(new TypedName(impm.Name, impm.ParameterType));

                    coverPoint.Summary = "Entrypoint cover method, invokes the true entrypoint in a different module.";
                    IMethodInvokeExpression imie = entryPoint.GetReference().Invoke();
                    foreach (IMethodParameterMember impm in entryPoint.Parameters.Values)
                        imie.ArgumentExpressions.Add(impm.GetReference());
                    if (!(coverPoint.ReturnType.Equals(typeof(void).GetTypeReference())))
                    {
                        coverPoint.Return(imie);
                    }
                    else
                        coverPoint.CallMethod(imie);
                    string entryFile = WriteFile(td, tfc, partialCompletions, entryPointCover);
                    moduleFiles[Project.RootModule].Add(entryFile);
                    files.Add(entryFile);
                    commandSequences.Add(module.GetEntryPointCommand(coverPoint, this.Options));
                    this.Project.Partials.Remove((IIntermediateProject)entryPointCover.ParentTarget.ParentTarget);
                }
                else
                {
                    commandSequences.Add(module.GetEntryPointCommand(entryPoint, this.Options));
                }
            }
            List<string> moduleCommandLines = new List<string>();
            foreach (IIntermediateModule iim in this.Project.Modules.Values)
            {
                string[] moduleFileNames = new string[moduleFiles[iim].Count + (iim == this.Project.RootModule ? this.Options.ExtraFiles.Count() : 0)];
                for (int i = 0; i < moduleFiles[iim].Count; i++)
                    moduleFileNames[i] = moduleFiles[iim][i];
                if (iim == this.Project.RootModule)
                {
                    int exC = this.Options.ExtraFiles.Count();
                    int exO = moduleFiles[iim].Count;
                    int index = exO;
                    foreach (var extraFile in this.Options.ExtraFiles)
                    {
                        var tempFile = td.Directories.GetTemporaryDirectory(iim.Name).Files.GetTemporaryFile(Path.GetFileName(extraFile));
                        tempFile.CloseStream();
                        File.Copy(extraFile, tempFile.FileName, true);
                        moduleFileNames[index++] = tempFile.FileName;
                    }
                }
                moduleCommandLines.AddRange(module.GetModuleCommand(iim, moduleFileNames, null, this.Options));
            }
            bool allowPartials = this.Translator.Options.AllowPartials;
            this.Translator.Options.AllowPartials = false;
            ProjectDependencyReport pdr = new ProjectDependencyReport(this.Project, this.Translator.Options);
            pdr.Begin();
            this.Translator.Options.AllowPartials = allowPartials;
            if (module.Supports(CompilerModuleSupportFlags.Optimization))
                commandSequences.Add(module.GetOptimizationCommand(this.Options.Optimize, this.Options));
            if (module.Supports(CompilerModuleSupportFlags.DebuggerSupport))
                commandSequences.Add(module.GetDebugCommand(this.Options.DebugSupport, this.Options));
            if (module.Supports(CompilerModuleSupportFlags.XMLDocumentation))
                commandSequences.Add(module.GetXMLDocumentationCommand(this.Options.GenerateXMLDocs, this.Options));
            commandSequences.AddRange(module.GetReferencesCommand(Tweaks.TranslateArray(pdr.CompiledAssemblyReferences.ToArray(), a =>
            {
                return a.Location;
            }), this.Options));
            if (module.Supports(CompilerModuleSupportFlags.MultiFileAssemblies))
                commandSequences.AddRange(moduleCommandLines.ToArray());
            else
            {
                commandSequences.Add(module.GetOutputCommand(this.Options.Target, this.Options));
                commandSequences.AddRange(module.GetSourcesCommand(files.ToArray(), this.Options));
            }
            response = null;
            if (module.Supports(CompilerModuleSupportFlags.ResponseFile))
            {
                response = tfc.AddExtension("response");
                StreamWriter responseWriter = new StreamWriter(new FileStream(response, FileMode.Create));
                //string[] lines = fullCommand.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in commandSequences)
                    responseWriter.WriteLine(s);
                responseWriter.Flush();
                responseWriter.Close();
            }
        }

        private void TranslateSource(out TemporaryDirectory td, out TempFileCollection tfc, out List<string> files, out Stack<IIntermediateProject> partialCompletions, out Dictionary<IIntermediateModule, List<string>> moduleFiles)
        {
            td = TemporaryFileHelper.GetTemporaryDirectory("Oilexer", false).Directories.GetTemporaryDirectory(this.Project.Name);
            tfc = new TempFileCollection(td.Path, td.Keep);
            files = new List<string>();
            partialCompletions = new Stack<IIntermediateProject>();
            moduleFiles = new Dictionary<IIntermediateModule, List<string>>();
            bool attributesOverride = !this.Translator.Options.AllowPartials;
            if (!attributesOverride)
                foreach (IIntermediateModule iim in this.Project.Modules.Values)
                {
                    moduleFiles.Add(iim, new List<string>());
                    foreach (IDeclaredType idt in iim.DeclaredTypes)
                    {
                        string currentFile = string.Empty;
                        IDeclaredType target = (idt is ISegmentableDeclaredType) ? ((ISegmentableDeclaredType)idt).GetRootDeclaration() : idt;
                        currentFile = WriteFile(td, tfc, partialCompletions, target);
                        if (currentFile != null)
                        {
                            files.Add(currentFile);
                            moduleFiles[iim].Add(currentFile);
                        }
                        //Don't forget to include the partials...
                        if (target is ISegmentableDeclaredType)
                            foreach (IDeclaredType partial in ((ISegmentableDeclaredType)idt).Partials)
                            {
                                currentFile = WriteFile(td, tfc, partialCompletions, partial);
                                if (currentFile != null)
                                {
                                    files.Add(currentFile);
                                    moduleFiles[iim].Add(currentFile);
                                }
                            }
                    }
                }
            if (this.Project.Attributes.Count > 0 || attributesOverride)
            {
                if (attributesOverride)
                    moduleFiles.Add(project.RootModule, new List<string>());
                string attrFile = null;
                /* *
                 * Only add it if there are no types in the 
                 * root declaration of the project.
                 * */
                var p = this.Project;
                if (!p.IsRoot)
                    p = p.GetRootDeclaration();
                if (!partialCompletions.Contains(this.Project))
                {
                    attrFile = WriteAttributeFile(td, tfc, partialCompletions);
                    if (attrFile != null)
                        moduleFiles[project.RootModule].Add(attrFile);
                }
            }

        }

        private string WriteAttributeFile(TemporaryDirectory td, TempFileCollection tfc, Stack<IIntermediateProject> partialCompletions)
        {
            if (td == null)
                throw new ArgumentNullException("td");
            var root = this.Project;
            if (!root.IsRoot)
                root = root.GetRootDeclaration();
            if (!partialCompletions.Contains(root))
            {
                bool autoResolveRefs = this.Translator.Options.AutoResolveReferences;
                this.Translator.Options.AutoResolveReferences = false;
                TemporaryFile tf = td.Directories.GetTemporaryDirectory(root.RootModule.Name).Files.GetTemporaryFile(string.Format("AssemblyInfo.cs"));
                tfc.AddFile(tf.FileName, false);
                this.Translator.Options.AutoResolveReferences = autoResolveRefs;
                tf.OpenStream(FileMode.Create);
                this.Translator.Target = new IndentedTextWriter(new StreamWriter(tf.FileStream));
                this.Translator.TranslateProject(root);
                this.Translator.Target.Flush();
                partialCompletions.Push(root);
                tf.CloseStream();
                return tf.FileName;
            }
            return null;
        }

        private string WriteFile(TemporaryDirectory td, TempFileCollection tfc, Stack<IIntermediateProject> partialCompletions, IDeclaredType target)
        {
            if (td == null)
                throw new ArgumentNullException("td");
            if (!partialCompletions.Contains(target.Project))
            {
                bool autoResolveRefs = this.Translator.Options.AutoResolveReferences;
                this.Translator.Options.AutoResolveReferences = false;
                var folderName = GetFolderName(target);
                var tempDir = td;
                string[] folders = folderName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in folders)
                    tempDir = tempDir.Directories.GetTemporaryDirectory(item);
                TemporaryFile tf = tempDir.Files.GetTemporaryFile(string.Format("{0}.cs", GetTargetFullName(target)));
                tfc.AddFile(tf.FileName, false);
                this.Translator.Options.AutoResolveReferences = autoResolveRefs;
                //If it's segmentable, there's going to be multiple files...
                tf.OpenStream(FileMode.Create);
                this.Translator.Target = new IndentedTextWriter(new StreamWriter(tf.FileStream));
                this.Translator.TranslateProject(target.Project);
                this.Translator.Target.Flush();
                tf.CloseStream();
                partialCompletions.Push(target.Project);
                return tf.FileName;
            }
            return null;
        }

        private string GetFolderName(IDeclaredType target)
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

        private string GetTargetFullName(IDeclaredType target)
        {
            if (target.ParentTarget is IDeclaredType)
                return string.Format("{0}+{1}", GetTargetFullName((IDeclaredType)target.ParentTarget), target.Name);
            return target.Name;
        }

        public IIntermediateProject Project
        {
            get { return this.project; }
        }

        /// <summary>
        /// Returns the <see cref="IIntermediateCodeTranslator"/> which is responsible for translating
        /// the <see cref="Project"/> into valid code for the <see cref="IntermediateCompilerBase"/>'s
        /// backend.
        /// </summary>
        public IIntermediateCodeTranslator Translator
        {
            get
            {
                return this.translator;
            }
        }

        /// <summary>
        /// Returns the compiler module used to interact with the
        /// compiler regardless of the kind of intermediateCompiler.
        /// </summary>
        public IIntermediateCompilerModule Module
        {
            get { return this.module; }
        }

        #endregion

    }
}
