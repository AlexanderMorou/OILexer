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
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Translation;
using Microsoft.CSharp;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
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
                        result = string.Format("''' {0}", commentBase);
                }
                else if (commentBase.Contains("\r\n"))
                    result = "'" + string.Join("\r\n'", GetBoxedCommentText(commentBase).Split(new string[] { "\r\n" }, StringSplitOptions.None));
                else
                    result = string.Format("' {0}", commentBase);

                return result;
            }

            /// <summary>
            /// Determines whether the <paramref name="declaration"/> is a candidate for a module in
            /// the Visual Basic language.
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
                return false;
            }
        }
    }
}