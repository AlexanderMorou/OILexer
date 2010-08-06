using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using Oilexer.Types;

namespace Oilexer.Translation
{
    partial class CodeDomTranslator<T>
    {
        private class MixedOptions :
            IIntermediateCodeTranslatorOptions,
            ICodeDOMTranslationOptions,
            IDisposable
        {
            /// <summary>
            /// Data member for <see cref="Options"/> when <see cref="originalDom"/> is false.
            /// </summary>
            private CodeGeneratorOptions _Options;
            /// <summary>
            /// Data member for <see cref="Provider"/> when <see cref="originalDom"/> is false.
            /// </summary>
            private CodeDomProvider provider;
            /// <summary>
            /// Data member determining the type of original source <see cref="options"/>.
            /// </summary>
            private bool originalDom;
            /// <summary>
            /// The <see cref="ICodeTranslationOptions"/> that is sourced for most cases.
            /// </summary>
            internal ICodeTranslationOptions options;

            public MixedOptions(ICodeDOMTranslationOptions o)
            {
                originalDom = true;
                this.options = o;
            }

            public MixedOptions(IIntermediateCodeTranslatorOptions o)
            {
                originalDom = false;
                this.options = o;
            }

            #region IIntermediateCodeTranslatorOptions Members

            public Func<IType, string> GetFileNameOf { get; set; }

            public Func<IIntermediateProject, int> GetLineNumber { get; set; }

            public IIntermediateCodeTranslatorFormatter Formatter
            {
                get
                {
                    throw new NotSupportedException("The Code-Dom translator does not support a formatter.");
                }
                set
                {
                    throw new NotSupportedException("The Code-Dom translator does not support a formatter.");
                }
            }

            #endregion

            #region ICodeDOMTranslationOptions Members

            public CodeDomProvider LanguageProvider
            {
                get
                {
                    if (originalDom)
                        return ((ICodeDOMTranslationOptions)this.options).LanguageProvider;
                    else
                        return this.provider;
                }
                set
                {
                    if (originalDom)
                        ((ICodeDOMTranslationOptions)this.options).LanguageProvider = value;
                    else
                        this.provider = value;
                }
            }

            public CodeGeneratorOptions Options
            {
                get
                {
                    if (originalDom)
                        return ((ICodeDOMTranslationOptions)this.options).Options;
                    else
                        return _Options;
                }
                set
                {
                    if (originalDom)
                        ((ICodeDOMTranslationOptions)this.options).Options = value;
                    else
                        _Options = value;
                }
            }

            #endregion

            #region ICodeTranslationOptions Members

            public bool AutoResolveReferences
            {
                get
                {
                    return this.options.AutoResolveReferences;
                }
                set
                {
                    this.options.AutoResolveReferences = value;
                }
            }

            public ICodeGeneratorNameHandler NameHandler
            {
                get { return this.options.NameHandler; }
            }

            public ICollection<string> ImportList
            {
                get { return this.options.ImportList; }
            }

            public INameSpaceDeclaration CurrentNameSpace
            {
                get
                {
                    return this.options.CurrentNameSpace;
                }
                set
                {
                    this.options.CurrentNameSpace = value;
                }
            }

            public Stack<IDeclarationTarget> BuildTrail
            {
                get { return this.options.BuildTrail; }
            }

            public IDeclaredType CurrentType { get; set; }

            public AutoRegionAreas AutoRegions
            {
                get
                {
                    return this.options.AutoRegions;
                }
                set
                {
                    this.options.AutoRegions = value;
                }
            }

            public bool AllowRegions
            {
                get
                {
                    return this.options.AllowRegions;
                }
                set
                {
                    this.options.AllowRegions = value;
                }
            }

            public bool AllowPartials
            {
                get
                {
                    return this.options.AllowPartials;
                }
                set
                {
                    this.options.AllowPartials = value;
                }
            }

            public bool AutoRegionsFor(AutoRegionAreas area)
            {
                return this.options.AutoRegionsFor(area);
            }

            public bool AutoComments
            {
                get
                {
                    return this.options.AutoComments;
                }
                set
                {
                    this.options.AutoComments = value;
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                this.options = null;
                this.provider = null;
                this._Options = null;
            }

            #endregion
        }
    }
}
