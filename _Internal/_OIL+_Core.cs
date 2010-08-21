using Microsoft.CSharp;
using System;
using System.Linq;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Oilexer;
using Oilexer.Compiler;
using Oilexer.Types;
using Oilexer.Utilities.Collections;
using Oilexer.Types.Members;
using Oilexer.Translation;
using System.Text.RegularExpressions;
namespace Oilexer._Internal
{
    partial class _OIL
    {
        internal static partial class _Core
        {
            
            private static MemoryStream BufferArea = new MemoryStream();
            private static StreamReader BufferReader = new StreamReader(BufferArea);
            private static StreamWriter BufferWriter = new StreamWriter(BufferArea);
            internal static CSharpIntermediateCompilerModule DefaultCSharpCompilerModule = new CSharpIntermediateCompilerModule();
            private static string runtimeDirectory;

            static _Core()
            {
                DefaultCSharpCodeProvider = new CSharpCodeProvider();
                __defaultCSharpCodeTranslator = new CSharpCodeTranslator();
            }

            internal static readonly ReadOnlyCollection<Type> AutoFormTypes = new ReadOnlyCollection<Type>
            (
                new List<Type>
                (
                    new Type[]
                    {
                        typeof(byte),
                        typeof(sbyte),
                        typeof(ushort),
                        typeof(short),
                        typeof(uint),
                        typeof(int),
                        typeof(ulong),
                        typeof(long),
                        typeof(void),
                        typeof(bool),
                        typeof(char),
                        typeof(decimal),
                        typeof(float),
                        typeof(double),
                        typeof(object),
                        typeof(string)
                    }
                )
            );

            internal static string GetRuntimeDirectory()
            {
                if (_OIL._Core.runtimeDirectory == null)
                    _OIL._Core.runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
                return _OIL._Core.runtimeDirectory;
            }

            internal static CSharpCodeProvider DefaultCSharpCodeProvider;
            private static CSharpCodeTranslator __defaultCSharpCodeTranslator;
            //private static VBCodeTranslator __defaultVBCodeTranslator;

            internal static CSharpCodeTranslator DefaultCSharpCodeTranslator
            {
                get
                {
                    if (__defaultCSharpCodeTranslator == null)
                        __defaultCSharpCodeTranslator = new CSharpCodeTranslator();
                    if (__defaultCSharpCodeTranslator.Options != CodeGeneratorHelper.DefaultTranslatorOptions)
                        __defaultCSharpCodeTranslator.Options = CodeGeneratorHelper.DefaultTranslatorOptions;
                    if (__defaultCSharpCodeTranslator.Target != null)
                        __defaultCSharpCodeTranslator.Target = null;
                    return __defaultCSharpCodeTranslator;
                }
            }

            //internal static VBCodeTranslator DefaultVBCodeTranslator
            //{
            //    get
            //    {
            //        if (__defaultVBCodeTranslator == null)
            //            __defaultVBCodeTranslator = new VBCodeTranslator();
            //        if (__defaultVBCodeTranslator.Options != CodeGeneratorHelper.DefaultTranslatorOptions)
            //            __defaultVBCodeTranslator.Options = CodeGeneratorHelper.DefaultTranslatorOptions;
            //        if (__defaultVBCodeTranslator.Target != null)
            //            __defaultVBCodeTranslator.Target = null;
            //        return __defaultVBCodeTranslator;
            //    }
            //}

            internal static CodeGeneratorOptions DefaultDOMOptions = new CodeGeneratorOptions();

            internal static bool ImplementsNameCollideCheck(IDeclarationTarget source, string interfaceName, ICodeTranslationOptions options)
            {
                IDeclarationTarget idt = source;
                string shortInterfaceName = interfaceName;
                if (shortInterfaceName.Contains("."))
                    shortInterfaceName = shortInterfaceName.Substring(shortInterfaceName.LastIndexOf('.') + 1);
                int hitCount = 0;
                bool autoResolve = options.AutoResolveReferences;
                if (autoResolve)
                    options.AutoResolveReferences = false;
                List<string> names = new List<string>();
                while (idt != null)
                {
                    if (idt is IInterfaceImplementableType)
                    {
                        IInterfaceImplementableType parent = (IInterfaceImplementableType)idt;
                        foreach (ITypeReference itr in parent.ImplementsList)
                        {
                            string shortCurrentName;
                            string currentName = itr.TypeInstance.GetTypeName(options);
                            if (currentName.Contains("."))
                                shortCurrentName = currentName.Substring(currentName.LastIndexOf('.') + 1);
                            else
                                shortCurrentName = currentName;
                            //Increment the hit count if the short name of the current
                            //interface equals that of the one requested.  And if the exact
                            //name hasn't been encountered yet.  If it hasn't then it's a valid match
                            //otherwise it's just the same exact one.
                            if (shortCurrentName == shortInterfaceName && !names.Contains(currentName))
                                hitCount++;
                            names.Add(currentName);
                        }
                    }
                    idt = idt.ParentTarget;
                }
                if (autoResolve)
                    options.AutoResolveReferences = autoResolve;
                return hitCount > 1;
            }

            internal static string GenerateMemberSnippet(ICodeDOMTranslationOptions options, CodeTypeMember codeMember)
            {
                BufferArea.SetLength(0);
                BufferArea.Seek(0, SeekOrigin.Begin);
                if (options.LanguageProvider != null)
                    options.LanguageProvider.GenerateCodeFromMember(codeMember, BufferWriter, _OIL._Core.DefaultDOMOptions);
                else
                    DefaultCSharpCodeProvider.GenerateCodeFromMember(codeMember, BufferWriter, _OIL._Core.DefaultDOMOptions);
                BufferWriter.Flush();
                BufferArea.Seek(0, SeekOrigin.Begin);
                return BufferReader.ReadToEnd();
            }

            internal static string GenerateExpressionSnippet(ICodeDOMTranslationOptions options, CodeExpression codeExpression)
            {
                BufferArea.SetLength(0);
                BufferArea.Seek(0, SeekOrigin.Begin);
                if (options.LanguageProvider != null)
                    options.LanguageProvider.GenerateCodeFromExpression(codeExpression, BufferWriter, DefaultDOMOptions);
                else
                    DefaultCSharpCodeProvider.GenerateCodeFromExpression(codeExpression, BufferWriter, DefaultDOMOptions);
                BufferWriter.Flush();
                BufferArea.Seek(0, SeekOrigin.Begin);
                return BufferReader.ReadToEnd();
            }

            internal static void InsertPartialDeclarationTypes<TItem, TCodeDOM, TMembers>(string itemName, TItem item, TMembers membersCollection, ICodeDOMTranslationOptions options)
                where TItem :
                    class,
                    ISegmentableDeclarationTarget<TItem>,
                    ITypeParent
                where TCodeDOM :
                    CodeObject
                where TMembers :
                    CollectionBase,
                    IList
            {
                if (options.AllowPartials)
                {
                    IDictionary<AutoRegionAreas, int> startRanges = new Dictionary<AutoRegionAreas, int>();
                    IDictionary<AutoRegionAreas, int> rangeLengths = new Dictionary<AutoRegionAreas, int>();

                    startRanges[AutoRegionAreas.Classes] = membersCollection.Count;
                    foreach (IClassType subTypeClass in item.Classes.Values)
                    {
                        if (subTypeClass.ParentTarget == item)
                            membersCollection.Add(subTypeClass.GenerateCodeDom(options));
                        foreach (IClassType partial in subTypeClass.Partials)
                            if (partial.ParentTarget == item)
                                membersCollection.Add(partial.GenerateCodeDom(options));
                    }
                    rangeLengths[AutoRegionAreas.Classes] = membersCollection.Count - startRanges[AutoRegionAreas.Classes];

                    startRanges[AutoRegionAreas.Delegates] = membersCollection.Count;
                    foreach (IDelegateType subTypeDelegate in item.Delegates.Values)
                        if (subTypeDelegate.ParentTarget == item)
                            membersCollection.Add(subTypeDelegate.GenerateCodeDom(options));
                    rangeLengths[AutoRegionAreas.Delegates] = membersCollection.Count - startRanges[AutoRegionAreas.Delegates];

                    startRanges[AutoRegionAreas.Enumerators] = membersCollection.Count;
                    foreach (IEnumeratorType subTypeDelegate in item.Enumerators.Values)
                        if (subTypeDelegate.ParentTarget == item)
                            membersCollection.Add(subTypeDelegate.GenerateCodeDom(options));
                    rangeLengths[AutoRegionAreas.Enumerators] = membersCollection.Count - startRanges[AutoRegionAreas.Enumerators];

                    startRanges[AutoRegionAreas.Interfaces] = membersCollection.Count;
                    foreach (IInterfaceType subTypeInterface in item.Interfaces.Values)
                    {
                        if (subTypeInterface.ParentTarget == item)
                            membersCollection.Add(subTypeInterface.GenerateCodeDom(options));
                        foreach (IInterfaceType partial in subTypeInterface.Partials)
                            if (partial.ParentTarget == item)
                                membersCollection.Add(partial.GenerateCodeDom(options));
                    }
                    rangeLengths[AutoRegionAreas.Interfaces] = membersCollection.Count - startRanges[AutoRegionAreas.Interfaces];

                    startRanges[AutoRegionAreas.Structures] = membersCollection.Count;
                    foreach (IStructType subTypeStruct in item.Structures.Values)
                    {
                        if (subTypeStruct.ParentTarget == item)
                            membersCollection.Add(subTypeStruct.GenerateCodeDom(options));
                        foreach (IStructType partial in subTypeStruct.Partials)
                            if (partial.ParentTarget == item)
                                membersCollection.Add(partial.GenerateCodeDom(options));
                    }
                    rangeLengths[AutoRegionAreas.Structures] = membersCollection.Count - startRanges[AutoRegionAreas.Structures];

                    if (options.AllowRegions)
                    {
                        if (options.AutoRegionsFor(AutoRegionAreas.NestedTypes) && membersCollection.Count > 0)
                        {
                            ((CodeTypeDeclaration)membersCollection[0]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_NestedTypes)));
                            ((CodeTypeDeclaration)membersCollection[membersCollection.Count - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Classes) && (rangeLengths[AutoRegionAreas.Classes] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Classes]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Classes)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Classes] + rangeLengths[AutoRegionAreas.Classes] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Delegates) && (rangeLengths[AutoRegionAreas.Delegates] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Delegates]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Delegates)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Delegates] + rangeLengths[AutoRegionAreas.Delegates] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Enumerators) && (rangeLengths[AutoRegionAreas.Enumerators] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Enumerators]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Enumerators)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Enumerators] + rangeLengths[AutoRegionAreas.Enumerators] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Interfaces) && (rangeLengths[AutoRegionAreas.Interfaces] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Interfaces]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Interfaces)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Interfaces] + rangeLengths[AutoRegionAreas.Interfaces] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Structures) && (rangeLengths[AutoRegionAreas.Structures] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Structures]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Structures)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Structures] + rangeLengths[AutoRegionAreas.Structures] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                    }
                }
                else if (item.IsRoot)
                {
                    //ToDo: Insert code to properly manage root members who have 
                    //partial instances inserted in the type groups.
                    IDictionary<AutoRegionAreas, int> startRanges = new Dictionary<AutoRegionAreas, int>();
                    IDictionary<AutoRegionAreas, int> rangeLengths = new Dictionary<AutoRegionAreas, int>();

                    startRanges[AutoRegionAreas.Classes] = membersCollection.Count;
                    foreach (CodeTypeDeclaration declClass in item.Classes.GenerateCodeDom(options))
                        membersCollection.Add(declClass);
                    rangeLengths[AutoRegionAreas.Classes] = membersCollection.Count - startRanges[AutoRegionAreas.Classes];

                    startRanges[AutoRegionAreas.Delegates] = membersCollection.Count;
                    foreach (CodeTypeDelegate declDelegate in item.Delegates.GenerateCodeDom(options))
                        membersCollection.Add(declDelegate);
                    rangeLengths[AutoRegionAreas.Delegates] = membersCollection.Count - startRanges[AutoRegionAreas.Delegates];

                    startRanges[AutoRegionAreas.Enumerators] = membersCollection.Count;
                    foreach (CodeTypeDeclaration declEnum in item.Enumerators.GenerateCodeDom(options))
                        membersCollection.Add(declEnum);
                    rangeLengths[AutoRegionAreas.Enumerators] = membersCollection.Count - startRanges[AutoRegionAreas.Enumerators];

                    startRanges[AutoRegionAreas.Interfaces] = membersCollection.Count;
                    foreach (CodeTypeDeclaration declInterface in item.Interfaces.GenerateCodeDom(options))
                        membersCollection.Add(declInterface);
                    rangeLengths[AutoRegionAreas.Interfaces] = membersCollection.Count - startRanges[AutoRegionAreas.Interfaces];

                    startRanges[AutoRegionAreas.Structures] = membersCollection.Count;
                    foreach (CodeTypeDeclaration declStruct in item.Structures.GenerateCodeDom(options))
                        membersCollection.Add(declStruct);
                    rangeLengths[AutoRegionAreas.Structures] = membersCollection.Count - startRanges[AutoRegionAreas.Structures];

                    //Check the nested type insert first.
                    if (options.AllowRegions)
                    {
                        if (options.AutoRegionsFor(AutoRegionAreas.NestedTypes) && membersCollection.Count > 0)
                        {
                            ((CodeTypeDeclaration)membersCollection[0]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_NestedTypes)));
                            ((CodeTypeDeclaration)membersCollection[membersCollection.Count - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Classes) && (rangeLengths[AutoRegionAreas.Classes] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Classes]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Classes)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Classes] + rangeLengths[AutoRegionAreas.Classes] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Delegates) && (rangeLengths[AutoRegionAreas.Delegates] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Delegates]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Delegates)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Delegates] + rangeLengths[AutoRegionAreas.Delegates] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Enumerators) && (rangeLengths[AutoRegionAreas.Enumerators] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Enumerators]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Enumerators)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Enumerators] + rangeLengths[AutoRegionAreas.Enumerators] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Interfaces) && (rangeLengths[AutoRegionAreas.Interfaces] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Interfaces]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Interfaces)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Interfaces] + rangeLengths[AutoRegionAreas.Interfaces] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                        if (options.AutoRegionsFor(AutoRegionAreas.Structures) && (rangeLengths[AutoRegionAreas.Structures] > 0))
                        {
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Structures]]).StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format(MaintenanceResources.AutoRegions_BasePattern, itemName, MaintenanceResources.AutoRegions_Structures)));
                            ((CodeTypeDeclaration)membersCollection[startRanges[AutoRegionAreas.Structures] + rangeLengths[AutoRegionAreas.Structures] - 1]).EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                        }
                    }
                }
            }

            internal static void GetDeclaredHierarchy(IDeclaredTypeReference typeReference, ITypeReferenceCollection typeParameters, out List<IDeclaredType> hierarchy, out Stack<ITypeReference> typeParamsStack)
            {
                IDeclaredType type = typeReference.TypeInstance;
                hierarchy = new List<IDeclaredType>();
                List<ITypeReference> typeParams = new List<ITypeReference>(typeParameters);
                typeParams.Reverse();
                typeParamsStack = new Stack<ITypeReference>(typeParams);

                for (IDeclarationTarget iDecTar = type; iDecTar != null && iDecTar is IDeclaredType; iDecTar = iDecTar.ParentTarget)
                    hierarchy.Add((IDeclaredType)iDecTar);
                hierarchy.Reverse();
            }

            internal static string GetBoxedCommentText(string commentBase)
            {
                string[] commentLines = (from line in commentBase.Split(new string[] { "\r\n" }, StringSplitOptions.None)
                                         select string.Format(" |  {0}", line)).ToArray();

                int maximumLength = commentLines.Max(p => p.Length);
                //for (int i = 0; i < commentLines.Length; i++)
                //{
                //    commentLines[i] = string.Format(" |  {0}", commentLines[i]);
                //    if (commentLines[i].Length > maximumLength)
                //        maximumLength = commentLines[i].Length;

                //}
                StringBuilder sb = new StringBuilder();
                maximumLength += 4;
                sb.Append(" /* ");
                sb.Append('-', maximumLength - 6);
                sb.AppendLine("\\");
                foreach (string s in commentLines)
                {
                    if (s == " |  -")
                    {
                        sb.Append(" |");
                        sb.Append('-', maximumLength - 4);
                    }
                    else
                    {
                        sb.Append(s);
                        sb.Append(' ', maximumLength - (s.Length + 2));
                    }
                    sb.AppendLine("|");
                }
                sb.Append(" \\");
                sb.Append('-', maximumLength - 6);
                sb.Append(" */");
                return sb.ToString();
            }


            internal static string GetCSharpCommentText(string commentBase, bool docComment)
            {
                string result = "";
                if (docComment)
                {
                    if (commentBase.Contains("\r\n"))
                    {
                        StringBuilder sb = new StringBuilder();
                        string[] commentLines = commentBase.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        bool firstItem = true;
                        for (int i = 0; i < commentLines.Length; i++)
                        {
                            string s = commentLines[i];
                            if (firstItem)
                                firstItem = false;
                            else
                                sb.AppendLine();
                            sb.Append("/// ");
                            sb.Append(s);
                        }
                        result = sb.ToString();
                    }
                    else
                    {
                        result = string.Format("/// {0}", commentBase);
                    }
                }
                else if (commentBase.Contains("\r\n"))
                    result = _OIL._Core.GetBoxedCommentText(commentBase);
                else
                    result = string.Format("// {0}", commentBase);
                return result;
            }

            internal static string GetVBCommentText(string commentBase, bool docComment)
            {
                string result = "";
                if (docComment)
                {
                    if (commentBase.Contains("\r\n"))
                    {
                        StringBuilder sb = new StringBuilder();
                        string[] commentLines = commentBase.Split(new char[] { '\n' });
                        bool firstItem = true;
                        for (int i = 0; i < commentLines.Length; i++)
                        {
                            string s = commentLines[i];
                            if (firstItem)
                                firstItem = false;
                            else
                                sb.AppendLine();
                            if (s.Length > 0 && s[s.Length - 1] == '\r')
                                s = s.Substring(0, s.Length - 1);
                            sb.Append("''' ");
                            sb.Append(s);
                        }
                        result = sb.ToString();
                    }
                    else
                    {
                        result = string.Format("''' {0}", commentBase);
                    }
                }
                else if (commentBase.Contains("\r\n"))
                    result = "'" + string.Join("\r\n'", GetBoxedCommentText(commentBase).Split(new string[] { "\r\n" }, StringSplitOptions.None));
                else
                    result = string.Format("' {0}", commentBase);

                return result;
            }

            internal static StringBuilder GetTypesUsedComment(ref ProjectDependencyReport pdr, IIntermediateCodeTranslatorOptions options)
            {
                StringBuilder typesUsed;
                typesUsed = new StringBuilder();
                typesUsed.AppendLine(String.Format("There were {0} types used by this file", pdr.SourceData.Count));
                var typesUsedListing = (from reference in pdr.SourceData
                                        let externType = reference.TypeInstance as IExternType
                                        let declType = reference.TypeInstance as IDeclaredType
                                        let nameSpace = externType == null ?
                                                            declType == null ?
                                                                string.Empty :
                                                                declType.GetNamespace().FullName :
                                                            externType.Type.Namespace
                                        let typeName = reference.ToString(options)
                                        let realTypeName = typeName.Contains(nameSpace) ?
                                                           typeName.Substring(nameSpace.Length+1) :
                                                           typeName
                                        orderby nameSpace ascending,
                                                realTypeName ascending
                                        select realTypeName).ToArray().FixedJoinSeries(", ");
                typesUsed.Append(typesUsedListing);
                if (pdr.CompiledAssemblyReferences.Count > 0)
                {
                    typesUsed.AppendLine();
                    typesUsed.AppendLine("-");
                    typesUsed.AppendLine(string.Format("There were {0} assemblies referenced:", pdr.CompiledAssemblyReferences.Count));
                    var compiledAssembliesUsedListing = (from assembly in pdr.CompiledAssemblyReferences
                                                         let name = assembly.GetName().Name
                                                         orderby name
                                                         select name).ToArray().FixedJoinSeries(", ");
                    typesUsed.Append(compiledAssembliesUsedListing);
                }
                return typesUsed;
            }

            /// <summary>
            /// Determines whether the <paramref name="declaration"/> is a candidate for a module in
            /// the Visual Basic.NET language.
            /// </summary>
            /// <param name="declaration">The <see cref="IDeclarationTarget"/> to check to see if it
            /// is a vb module candidate.</param>
            /// <returns>true if the <paramref name="declaration"/> is a valid candidate.</returns>
            /// <remarks>If the <paramref name="declaration"/> is a valid candidate, the translator
            /// will flatten the partial hierarchy using the options' <see cref="ICodeTranslationOptions.AllowPartials"/> property.</remarks>
            internal static bool DeclarationIsVBModuleCandidate(IDeclarationTarget declaration)
            {
                if (!(declaration is IClassType))
                    return false;
                IClassType @class = (IClassType)declaration;
                if ((@class.TypeParameters == null || @class.TypeParameters.Count == 0) && @class.IsStatic)
                {
                    return @class.ParentTarget is INameSpaceDeclaration && @class.Partials.Count == 0;
                }
                return false;
            }

            internal static bool DeclarationIsVBModule(IDeclarationTarget declaration, IIntermediateCodeTranslatorOptions options)
            {
                if (!(declaration is IClassType))
                    return false;
                IClassType @class = (IClassType)declaration;
                if (@class.TypeParameters == null || @class.TypeParameters.Count == 0)
                {
                    if (@class.ParentTarget is INameSpaceDeclaration && @class.IsStatic)
                    {
                        if (options.AllowPartials)
                        {
                            //Basically if all other partials have no members,
                            //Then it means that it is still a module.
                            foreach (IClassType ict in @class.GetRootDeclaration().Partials)
                                if (ict == @class)
                                    continue;
                                else if (ict.GetMemberCount(false) > 0 || ict.GetTypeCount(false) > 0)
                                    return false;
                            if (@class != @class.GetRootDeclaration() && @class.GetRootDeclaration().GetMemberCount(false) > 0)
                                return false;
                        }
                        return true;
                    }
                }
                return false;
            }

            public static string BuildMemberReferenceComment(ICodeTranslationOptions options, IMember member)
            {
                if (options == null)
                    throw new ArgumentNullException("options");
                string parentCref = ((IType)member.ParentTarget).GetTypeName(options, true);
                //If there is a build trail to go by, check to see what kind of scope
                //is available.  If the parent of the field is available, then referencing
                //field should be easier.
                if (options.BuildTrail != null)
                {
                    IDeclarationTarget idt = member.ParentTarget;
                    while (idt != null && !(idt is INameSpaceDeclaration))
                        idt = idt.ParentTarget;
                    INameSpaceDeclaration currentNameSpace = null;
                    bool importsContainsNameSpace = false;
                    if (idt != null && options.ImportList != null)
                        importsContainsNameSpace = options.ImportList.Contains(((INameSpaceDeclaration)(idt)).FullName);
                    if (!importsContainsNameSpace)
                        for (int i = 0; i < options.BuildTrail.Count; i++)
                            if (Special.GetThisAt(options.BuildTrail, i) is INameSpaceDeclaration)
                                currentNameSpace = (INameSpaceDeclaration)Special.GetThisAt(options.BuildTrail, i);
                            else
                                break;

                    if (currentNameSpace != null && idt != null && (currentNameSpace.FullName == ((INameSpaceDeclaration)idt).FullName || currentNameSpace.FullName.Contains(((INameSpaceDeclaration)idt).FullName + ".")))
                    {
                        /* *
                         * The build trail's current namespace contains the full name of the 
                         * parent namespace.
                         * */
                        parentCref = parentCref.Substring(currentNameSpace.FullName.Length+1);
                    }
                    else if (importsContainsNameSpace)
                    {
                        /* *
                         * The import list contains the namespace
                         * */
                        parentCref = parentCref.Substring(((INameSpaceDeclaration)idt).FullName.Length + 1);
                    }
                    else
                        goto _verbose;
                }
                return string.Format(_CommentConstants.SeeCrefTag, string.Format("{0}.{1}", parentCref, member.Name));
            _verbose:
                return string.Format(_CommentConstants.SeeCrefTag, string.Format("{0}.{1}", parentCref, member.Name));
            }
        }
    }
}