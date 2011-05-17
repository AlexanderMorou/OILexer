using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using AllenCopeland.Abstraction.Slf._Internal;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Cli;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Languages;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Oil;
using AllenCopeland.Abstraction.Slf.Oil.Members;
using AllenCopeland.Abstraction.Slf.Translation;
using Microsoft.CSharp;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    partial class _OIL
    {
        internal static partial class _Core
        {
            
            private static MemoryStream BufferArea = new MemoryStream();
            private static StreamReader BufferReader = new StreamReader(BufferArea);
            private static StreamWriter BufferWriter = new StreamWriter(BufferArea);
            private static string runtimeDirectory;

            internal static string GetRuntimeDirectory()
            {
                if (_OIL._Core.runtimeDirectory == null)
                    _OIL._Core.runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
                return _OIL._Core.runtimeDirectory;
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
            /*
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
                                        select realTypeName).ToArray().FixedJoin(", ");
                typesUsed.Append(typesUsedListing);
                if (pdr.CompiledAssemblyReferences.Count > 0)
                {
                    typesUsed.AppendLine();
                    typesUsed.AppendLine("-");
                    typesUsed.AppendLine(string.Format("There were {0} assemblies referenced:", pdr.CompiledAssemblyReferences.Count));
                    var compiledAssembliesUsedListing = (from assembly in pdr.CompiledAssemblyReferences
                                                         let name = assembly.GetName().Name
                                                         orderby name
                                                         select name).ToArray().FixedJoin(", ");
                    typesUsed.Append(compiledAssembliesUsedListing);
                }
                return typesUsed;
            }
            //*/


            /// <summary>
            /// Determines whether the <paramref name="declaration"/> is a candidate for a module in
            /// the Visual Basic.NET language.
            /// </summary>
            /// <param name="declaration">The <see cref="IIntermediateDeclaration"/> to check to see if it
            /// is a vb module candidate.</param>
            /// <returns>true if the <paramref name="declaration"/> is a valid candidate.</returns>
            /// <remarks>If the <paramref name="declaration"/> is a valid candidate, the translator
            /// will flatten the partial hierarchy using the options' <see cref="IIntermediateCodeTranslatorOptions.AllowPartials"/> property.</remarks>
            internal static bool DeclarationIsVBModuleCandidate(IIntermediateDeclaration declaration)
            {
                if (!(declaration is IIntermediateClassType))
                    return false;
                IIntermediateClassType @class = (IIntermediateClassType)declaration;
                if ((@class.TypeParameters == null || @class.TypeParameters.Count == 0) && @class.SpecialModifier != SpecialClassModifier.None)
                    return @class.Parent is IIntermediateNamespaceDeclaration && @class.Parts.Count == 0;
                return false;
            }

            internal static bool DeclarationIsVBModule(IIntermediateDeclaration declaration, IIntermediateCodeTranslatorOptions options)
            {
                if (!(declaration is IIntermediateClassType))
                    return false;
                IIntermediateClassType @class = (IIntermediateClassType)declaration;
                if (@class.TypeParameters == null || @class.TypeParameters.Count == 0)
                {
                    if (@class.Parent is IIntermediateNamespaceDeclaration && @class.SpecialModifier != AllenCopeland.Abstraction.Slf.Abstract.SpecialClassModifier.None)
                    {
                        if (options.AllowPartials)
                        {
                            //Basically if all other partials have no members,
                            //Then it means that it is still a module.
                            foreach (IIntermediateClassType ict in @class.GetRoot().Parts)
                                if (ict == @class)
                                    continue;
                                else if (ict.Members.Count > 0 || ict.Types.Count > 0)
                                    return false;
                            if (@class != @class.GetRoot() && @class.GetRoot().Members.Count > 0)
                                return false;
                        }
                        return true;
                    }
                }
                return false;
            }
            /*//
            public static string BuildMemberReferenceComment(IIntermediateCodeTranslatorOptions options, IIntermediateMember member)
            {
                if (options == null)
                    throw new ArgumentNullException("options");
                string parentCref = ((IType)member.Parent).BuildTypeName(options, true);
                //If there is a build trail to go by, check to see what kind of scope
                //is available.  If the parent of the field is available, then referencing
                //field should be easier.
                if (options.BuildTrail != null)
                {
                    var idt = member.Parent;
                    while (idt != null && !(idt is IIntermediateNamespaceDeclaration))
                        if (idt is IIntermediateMember)
                            idt = ((IIntermediateMember)(idt)).Parent;
                        else if (idt is IIntermediateType)
                            idt = ((IIntermediateType)(idt)).Parent;
                    IIntermediateNamespaceDeclaration currentNameSpace = null;
                    bool importsContainsNameSpace = false;
                    if (idt != null && options.ImportList != null)
                        importsContainsNameSpace = options.ImportList.Contains(((IIntermediateNamespaceDeclaration)(idt)).FullName);
                    if (!importsContainsNameSpace)
                        for (int i = 0; i < options.BuildTrail.Count; i++)
                            if (Special.GetThisAt(options.BuildTrail, i) is IIntermediateNamespaceDeclaration)
                                currentNameSpace = (IIntermediateNamespaceDeclaration)Special.GetThisAt(options.BuildTrail, i);
                            else
                                break;

                    if (currentNameSpace != null && idt != null && (currentNameSpace.FullName == ((IIntermediateNamespaceDeclaration)idt).FullName || currentNameSpace.FullName.Contains(((IIntermediateNamespaceDeclaration)idt).FullName + ".")))
                    {
                        /* *
                         * The build trail's current namespace contains the full name of the 
                         * parent namespace.
                         * * /
                        parentCref = parentCref.Substring(currentNameSpace.FullName.Length+1);
                    }
                    else if (importsContainsNameSpace)
                    {
                        /* *
                         * The import list contains the namespace
                         * * /
                        parentCref = parentCref.Substring(((IIntermediateNamespaceDeclaration)idt).FullName.Length + 1);
                    }
                    else
                        goto _verbose;
                }
                return string.Format(_CommentConstants.SeeCrefTag, string.Format("{0}.{1}", parentCref, member.Name));
            _verbose:
                return string.Format(_CommentConstants.SeeCrefTag, string.Format("{0}.{1}", parentCref, member.Name));
            }
            //*/
        }
    }
}