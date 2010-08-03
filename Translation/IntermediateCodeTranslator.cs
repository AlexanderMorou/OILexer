using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.CodeDom.Compiler;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Statements;
using Oilexer.Expression;
using Oilexer._Internal;
using Oilexer.Comments;
namespace Oilexer.Translation
{
    /// <summary>
    /// Provides a partial implementation of <see cref="IIntermediateCodeTranslator"/> which
    /// dispatches standard groups and generic types to their respective 'Translate*' methods.
    /// </summary>
    public abstract partial class IntermediateCodeTranslator :
        IntermediateTranslator,
        IIntermediateCodeTranslator
    {
        private IndentedTextWriter target = null;
        private IIntermediateCodeTranslatorOptions options;

        /// <summary>
        /// The UBB formatter which appropriately formats all tokens in a popular online 
        /// forum markup.
        /// </summary>
        public static readonly IIntermediateCodeTranslatorFormatter UBBFormatter = new __UBBFormatter();
        /// <summary>
        /// The HTML formatter which appropriately formats all tokens in HTML tags.
        /// </summary>
        public static readonly IIntermediateCodeTranslatorFormatter HTMLFormatter = new __HTMLFormatter();

        private const string generatedMessageBase =
            "This code was generated by {0}.\r\n" +
            "Version: {1} \r\n" +
            "-\r\n" +
            "To ensure the code works properly,\r\n" +
            "please do not make any changes to the file.\r\n" +
            "-\r\n" +
            "The specific language is {2}\r\n" +
            "Sub-tool Name: {3}\r\n" +
            "Sub-tool Version: {4}";

        #region IIntermediateCodeTranslator Members

        public override void TranslateMember(IMember member)
        {
            options.BuildTrail.Push(member);
            try
            {
                base.TranslateMember(member);
            }
            finally
            {
                options.BuildTrail.Pop();
            }
        }

        public abstract void TranslateConceptPartial(ISegmentableDeclarationTarget seggedDecl);

        public abstract void TranslateConceptAccessModifiers(IDeclaration decl);

        public abstract void TranslateConceptTypeName(IExternTypeReference type, ITypeReferenceCollection typeParameters);

        public abstract void TranslateConceptTypeName(IDeclaredTypeReference type, ITypeReferenceCollection typeParameters);

        public abstract void TranslateConceptTypeName(ITypeParameterMember type, ITypeReferenceCollection typeParameters);

        public virtual void TranslateConceptTypeName(ITypeReference typeRef)
        {
            if (typeRef is IExternTypeReference)
                TranslateConceptTypeName((IExternTypeReference)typeRef, typeRef.TypeParameters);
            else if (typeRef is IDeclaredTypeReference)
                TranslateConceptTypeName((IDeclaredTypeReference)typeRef, typeRef.TypeParameters);
            if (typeRef.TypeInstance is ITypeParameterMember)
                TranslateConceptTypeName((ITypeParameterMember)typeRef.TypeInstance, typeRef.TypeParameters);
        }

        public override void TranslateProject(IIntermediateProject project)
        {
            if (this.Options.Formatter != null)
            {
                string opening = this.options.Formatter.FormatBeginFile();
                if (opening != null)
                    this.Write(opening, TranslatorFormatterTokenType.Preformatted);
                this.TranslateProjectInner(project);
                string closing = this.options.Formatter.FormatEndFile();
                if (closing != null)
                    this.Write(closing, TranslatorFormatterTokenType.Preformatted);
            }
            else
                this.TranslateProjectInner(project);
        }

        protected abstract void TranslateProjectInner(IIntermediateProject project);

        public abstract void TranslateConceptNotInstantiableClass(IClassType classType);

        public abstract void TranslateConceptNotInheritableClass(IClassType classType);

        public abstract void TranslateConceptNotInheritableOrInstantiableClass(IClassType classType);

        public virtual void TranslateConceptIdentifier(IDeclaration decl, bool declarePoint)
        {
            string name = GetConceptIdentifier(decl);
            if (decl is IMethodMember || decl is IMethodSignatureMember ||
                decl is IPropertyMember || decl is IPropertySignatureMember ||
                decl is IFieldMember || decl is IStatementBlockLocalMember ||
                decl is IMethodParameterMember || decl is IMethodSignatureMember ||
                decl is IIndexerParameterMember || decl is IIndexerSignatureParameterMember ||
                decl is IConstructorParameterMember)
                this.Write(name, (IMember)decl, declarePoint);
            else if (decl is INameSpaceDeclaration)
                this.Write(name, TranslatorFormatterTokenType.NameSpace);
            else if (decl is IDeclaredType)
                this.Write(name, (IType)decl, declarePoint);
            else
                this.Write(name, TranslatorFormatterTokenType.Other);
        }

        protected virtual string GetConceptIdentifier(IDeclaration decl)
        {
            string name = null;
            if (options.NameHandler.HandlesName(decl))
                name = options.NameHandler.HandleName(decl);
            else
                name = decl.Name;
            if (this.IsKeyword(name))
                name = EscapeIdentifier(name);
            return name;
        }

        public virtual void TranslateConceptIdentifier(string identifierBase, ILabelStatement label, bool declarePoint)
        {
            string name = null;
            if (options.NameHandler.HandlesName(identifierBase))
                name = options.NameHandler.HandleName(identifierBase);
            else
                name = identifierBase;
            if (this.IsKeyword(name))
                name = EscapeIdentifier(name);
            name = this.GetFormat(name, label, declarePoint);
            this.Write(name, TranslatorFormatterTokenType.Preformatted);
        }

        private string GetFormat(string name, ILabelStatement label, bool declarePoint)
        {
            if (this.options != null &&
                this.options.Formatter != null)
                name = this.options.Formatter.FormatLabelToken(name, label, this.Options, declarePoint);
            return name;
        }

        public virtual void TranslateConceptIdentifier(string identifierBase)
        {
            string name = null;
            if (options.NameHandler.HandlesName(identifierBase))
                name = options.NameHandler.HandleName(identifierBase);
            else
                name = identifierBase;
            if (this.IsKeyword(name))
                name = EscapeIdentifier(name);
            this.Write(name, TranslatorFormatterTokenType.Other);
        }

        public virtual void TranslateConceptIdentifier(string identifierBase, TranslatorFormatterMemberType memberType)
        {
            string name = null;
            if (options.NameHandler.HandlesName(identifierBase))
                name = options.NameHandler.HandleName(identifierBase);
            else
                name = identifierBase;
            if (this.IsKeyword(name))
                name = EscapeIdentifier(name);
            this.Write(name, memberType);
        }

        public virtual void TranslateConceptIdentifier(string identifierBase, IType type)
        {
            string name = null;
            if (options.NameHandler.HandlesName(identifierBase))
                name = options.NameHandler.HandleName(identifierBase);
            else
                name = identifierBase;
            if (this.IsKeyword(name))
                name = EscapeIdentifier(name);
            this.Write(name, type, type is IDeclaredType);
        }

        protected virtual string GetConceptIdentifier(string identifierBase)
        {
            string name = null;
            if (options.NameHandler.HandlesName(identifierBase))
                name = options.NameHandler.HandleName(identifierBase);
            else
                name = identifierBase;
            if (this.IsKeyword(name))
                return EscapeIdentifier(name);
            else
                return name;
        }

        public abstract string EscapeString(string value, int indentLevel);

        public abstract bool IsKeyword(string identifier);

        public abstract string EscapeIdentifier(string identifier);

        public override void TranslateMembers<TParameter, TTypeParameter, TSignatureDom, TParent>(TParent parent, IMethodSignatureMembers<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethodSigMembers)
        {
            string regionText = "";
            bool bRegion = false;
            if (bRegion = (options.AutoRegionsFor(AutoRegionAreas.Methods) && (ambigMethodSigMembers.GetCountForTarget(parent) > 0)))
            {
                regionText = String.Format(_OIL._Core.MaintenanceResources.AutoRegions_BasePattern, parent.Name, " methods");
                this.TranslateConceptRegionStart(regionText);
            }
            this.TranslateMembers<IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent>, TParent, TSignatureDom>(parent, ambigMethodSigMembers);
            if (bRegion)
                this.TranslateConceptRegionEnd(regionText);
        }

        public override void TranslateMembers<TItem, TParent>(TParent parent, IPropertySignatureMembers<TItem, TParent> ambigPropertySigMembers)
        {
            string regionText = "";
            bool bRegion = false;
            if (bRegion = (options.AutoRegionsFor(AutoRegionAreas.Properties) && (ambigPropertySigMembers.GetCountForTarget(parent) > 0)))
            {
                regionText = String.Format(_OIL._Core.MaintenanceResources.AutoRegions_BasePattern, parent.Name, " properties");
                this.TranslateConceptRegionStart(regionText);
            }
            this.TranslateMembers<TItem, TParent, CodeMemberProperty>(parent, ambigPropertySigMembers);
            if (bRegion)
                this.TranslateConceptRegionEnd(regionText);
        }

        public override void TranslateMembers(IMemberParentType parent, IExpressionCoercionMembers coercionMembers)
        {
            foreach (IExpressionCoercionMember iecm in coercionMembers.Values)
            {
                if ((((IDeclarationTarget)(iecm.ParentTarget)) == ((IDeclarationTarget)(parent))) || ((!(options.AllowPartials)) && (parent is ISegmentableDeclarationTarget) && (iecm.ParentTarget is ISegmentableDeclarationTarget) && ((ISegmentableDeclarationTarget)(iecm.ParentTarget)).GetRootDeclaration() == ((ISegmentableDeclarationTarget)(parent)).GetRootDeclaration()))
                {
                    if (iecm is IBinaryOperatorOverloadMember)
                        this.TranslateMember<OverloadableBinaryOperators>((IBinaryOperatorOverloadMember)iecm);
                    else if (iecm is IUnaryOperatorOverloadMember)
                        this.TranslateMember<OverloadableUnaryOperators>((IUnaryOperatorOverloadMember)iecm);
                    else if (iecm is ITypeConversionOverloadMember)
                        this.TranslateMember((ITypeConversionOverloadMember)iecm);
                    else
                        TranslateConceptComment(string.Format("The coercion member ({0}) is not recognized...", iecm.GetUniqueIdentifier()), false);
                }
            }
        }

        public override void TranslateMembers(IFieldParentType parent, IFieldMembers fieldMembers)
        {
            string regionText = "";
            bool bRegion = false;
            if (bRegion = (options.AutoRegionsFor(AutoRegionAreas.Properties) && (fieldMembers.GetCountForTarget(parent) > 0)))
            {
                regionText = String.Format(_OIL._Core.MaintenanceResources.AutoRegions_BasePattern, parent.Name, " data members");
                this.TranslateConceptRegionStart(regionText);
            }
            TranslateMembers((IFieldParentType)parent, (IFieldMembersBase)fieldMembers);
            if (bRegion)
                this.TranslateConceptRegionEnd(regionText);
        }

        public override void TranslateMembers(IMemberParentType parent, IConstructorMembers ctorMembers)
        {
            string regionText = "";
            bool bRegion = false;
            if (bRegion = (options.AutoRegionsFor(AutoRegionAreas.Constructors) && (ctorMembers.GetCountForTarget(parent) > 0)))
            {
                regionText = String.Format(_OIL._Core.MaintenanceResources.AutoRegions_BasePattern, parent.Name, " .ctors");
                this.TranslateConceptRegionStart(regionText);
            }
            base.TranslateMembers(parent, ctorMembers);
            if (bRegion)
                this.TranslateConceptRegionEnd(regionText);
        }

        public override void TranslateType(IDeclaredType declaredType)
        {
            if (declaredType.ParentTarget is INameSpaceDeclaration && declaredType.Module != null)
                TranslateConceptComment(string.Format("Module: {0}", declaredType.Module.Name), false);
            if (declaredType is ISegmentableDeclaredType)
                options.BuildTrail.Push(((ISegmentableDeclaredType)declaredType).GetRootDeclaration());
            else
                options.BuildTrail.Push(declaredType);
            try
            {
                base.TranslateType(declaredType);
            }
            finally
            {
                options.BuildTrail.Pop();
            }
        }

        public override void TranslateMembers<TItem, TParent, TDom>(TParent parent, IMembers<TItem, TParent, TDom> members)
        {
            bool firstMember = true;
            foreach (TItem ti in members.Values)
            {
                if ((((IDeclarationTarget)(ti.ParentTarget)) == ((IDeclarationTarget)(parent))) || ((!(options.AllowPartials)) && (parent is ISegmentableDeclarationTarget) && (ti.ParentTarget is ISegmentableDeclarationTarget) && ((ISegmentableDeclarationTarget)(ti.ParentTarget)).GetRootDeclaration() == ((ISegmentableDeclarationTarget)(parent)).GetRootDeclaration()))
                {
                    if (firstMember)
                        firstMember = false;
                    else
                        this.WriteLine();
                    this.TranslateMember((IMember)ti);
                }
            }
        }

        public override void TranslateTypes<TItem, TDom>(ITypeParent parent, IDeclaredTypes<TItem, TDom> ambigTypes)
        {
            foreach (IDeclaredType<TDom> ambigDeclType in ambigTypes.Values)
            {
                //Emit the root, if it exists as a child of the type,
                //-or- if the type is segmentable, the parent is segmentable, and the instances
                //align.
                if ((ambigDeclType.ParentTarget == parent) || ((!(options.AllowPartials)) && (parent is ISegmentableDeclarationTarget) && (ambigDeclType.ParentTarget is ISegmentableDeclarationTarget) && (((ISegmentableDeclarationTarget)(parent)).GetRootDeclaration() == ((ISegmentableDeclarationTarget)(ambigDeclType.ParentTarget)).GetRootDeclaration())))
                    TranslateType(ambigDeclType);
                //Don't exclude the possibility that a partial of the same type
                //might have been inserted into the -same- parent instance.
                //Not likely, but possible and legal.  But only legal if partials are.
                if (options.AllowPartials && (ambigDeclType is ISegmentableDeclaredType))
                    foreach (IDeclaredType<TDom> ambigDeclTypePartial in ((ISegmentableDeclaredType)(ambigDeclType)).Partials)
                        if (ambigDeclTypePartial.ParentTarget == parent)
                            TranslateType(ambigDeclTypePartial);
            }
        }
        public override void TranslateNameSpaces(INameSpaceParent parent, INameSpaceDeclarations nameSpaces)
        {
            foreach (INameSpaceDeclaration insd in nameSpaces.Values)
            {
                if (insd.ParentTarget == parent || ((!(options.AllowPartials)) && (insd.ParentTarget.GetRootDeclaration() == parent.GetRootDeclaration())))
                    TranslateNameSpace(insd);
                if (options.AllowPartials)
                    foreach (INameSpaceDeclaration insdChild in insd.Partials)
                        if (insdChild.ParentTarget == parent)
                            TranslateNameSpace(insdChild);
            }

        }

        public override void TranslateTypeParentTypes(ITypeParent parent)
        {
            /* *
             * lastBlockFlag ensures that the null line is only inserted if the last block
             * -with members- is present.  This way if the delegates were null but the 
             * classes had members, the enumerators check on the lastblockflag still checks 
             * out with the 'classes' block.
             * */
            ISegmentableDeclarationTarget seggedMember = null;
            if (!options.AllowPartials && parent is ISegmentableDeclarationTarget && ((ISegmentableDeclarationTarget)(parent)).IsPartial)
                return;
            else if (parent is ISegmentableDeclarationTarget)
                seggedMember = (ISegmentableDeclarationTarget)parent;

            bool lastBlockFlag = false;
            string regionText = "";
            bool useRegion = false;
            if (!(parent.Classes == null && parent.Delegates == null && parent.Enumerators == null && parent.Interfaces == null && parent.Structures == null))
                useRegion = (!(parent is INameSpaceDeclaration)) && (!(parent is IDeclarationResources)) && options.AutoRegionsFor(AutoRegionAreas.NestedTypes) && (options.AllowPartials && seggedMember != null && (parent.Classes.GetCountForTarget(parent) > 0 || parent.Delegates.GetCountForTarget(parent) > 0 || parent.Enumerators.GetCountForTarget(parent) > 0 || parent.Interfaces.GetCountForTarget(parent) > 0 || parent.Structures.GetCountForTarget(parent) > 0) || ((seggedMember == null) && (parent.GetTypeCount() > 0)));
            if (useRegion)
            {
                regionText = String.Format(_OIL._Core.MaintenanceResources.AutoRegions_BasePattern, parent.Name, _OIL._Core.MaintenanceResources.AutoRegions_NestedTypes);
                TranslateConceptRegionStart(regionText);
            }

            if ((!(parent is IIntermediateProject)) && !(parent is IDeclarationResources) && parent is IResourceable)
            {
                if (((IResourceable)parent).Resources != null && ((IResourceable)parent).Resources.GetMemberCount(false) > 0)
                {
                    if ((options.AllowPartials) || (parent is ISegmentableDeclarationTarget && ((ISegmentableDeclarationTarget)(parent)).IsRoot))
                    {
                        options.BuildTrail.Push(((IResourceable)parent).Resources);
                        TranslateType(((IResourceable)parent).Resources);
                        options.BuildTrail.Pop();
                    }
                }
            }

            if (parent.Classes != null)
            {
                TranslateTypes(parent, parent.Classes);
                if (!lastBlockFlag)
                    if (seggedMember == null)
                        lastBlockFlag = parent.Classes.Count > 0;
                    else
                        lastBlockFlag = parent.Classes.GetCountForTarget(parent) > 0;
            }

            if (parent.Delegates != null)
            {
                if ((seggedMember != null && parent.Delegates.GetCountForTarget(parent) > 0) || (seggedMember == null) && (parent.Delegates.Count > 0) && (lastBlockFlag))
                {
                    this.WriteLine();
                    lastBlockFlag = false;
                }
                TranslateTypes(parent, parent.Delegates);
                if (!lastBlockFlag)
                    if (seggedMember == null)
                        lastBlockFlag = parent.Delegates.Count > 0;
                    else
                        lastBlockFlag = parent.Delegates.GetCountForTarget(parent) > 0;
            }

            if (parent.Enumerators != null)
            {
                if ((seggedMember != null && parent.Enumerators.GetCountForTarget(parent) > 0) || (seggedMember == null) && (parent.Enumerators.Count > 0) && (lastBlockFlag))
                {
                    this.WriteLine();
                    lastBlockFlag = false;
                }
                TranslateTypes(parent, parent.Enumerators);
                if (!lastBlockFlag)
                    if (seggedMember == null)
                        lastBlockFlag = parent.Enumerators.Count > 0;
                    else
                        lastBlockFlag = parent.Enumerators.GetCountForTarget(parent) > 0;
            }

            if (parent.Interfaces != null)
            {
                if ((seggedMember != null && parent.Interfaces.GetCountForTarget(parent) > 0) || (seggedMember == null) && (parent.Interfaces.Count > 0) && (lastBlockFlag))
                {
                    this.WriteLine();
                    lastBlockFlag = false;
                }
                TranslateTypes(parent, parent.Interfaces);
                if (!lastBlockFlag)
                    if (seggedMember == null)
                        lastBlockFlag = parent.Interfaces.Count > 0;
                    else
                        lastBlockFlag = parent.Interfaces.GetCountForTarget(parent) > 0;
            }

            if (parent.Structures != null)
            {
                if ((seggedMember != null && parent.Structures.GetCountForTarget(parent) > 0) || (seggedMember == null) && (parent.Structures.Count > 0) && (lastBlockFlag))
                {
                    this.WriteLine();
                    lastBlockFlag = false;
                }
                TranslateTypes(parent, parent.Structures);
            }

            if (useRegion)
            {
                TranslateConceptRegionEnd(regionText);
            }
        }

        #endregion

        #region IIntermediateCodeTranslator Members

        public abstract void TranslateConceptRegionStart(string regionText);

        public abstract void TranslateConceptRegionEnd(string regionText);

        public abstract void TranslateConceptKeyword(int keyWord);

        #endregion


        #region IIntermediateCodeTranslator Members

        public IndentedTextWriter Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }

        public IIntermediateCodeTranslatorOptions Options
        {
            get
            {
                return this.options;
            }
            set
            {
                SetOptions(value);
            }
        }

        internal virtual void SetOptions(IIntermediateCodeTranslatorOptions options)
        {
            this.options = options;
        }

        public virtual void TranslateComment(IComment comment)
        {
            if (comment is IDocumentationComment)
                TranslateComment((IDocumentationComment)comment);
        }

        public abstract void TranslateComment(IDocumentationComment docComment);

        public virtual void TranslateComments(IComments comments)
        {
            List<IComment> nonDocCom = new List<IComment>();
            List<IDocumentationComment> docCom = new List<IDocumentationComment>();
            foreach (IComment ic in comments)
                if (ic is IDocumentationComment)
                    docCom.Add((IDocumentationComment)ic);
                else
                    nonDocCom.Add(ic);

            nonDocCom.AddRange(docCom.ToArray());
            docCom.Clear();
            docCom = null;

            foreach (IComment icom in docCom)
                TranslateComment(icom);
        }

        #endregion

        protected string GetTerminableDocumentComment(string comment, string tag)
        {
            return string.Format("<{0}>\r\n{1}\r\n</{0}>", tag, comment);
        }
        protected string GetSummaryDocumentComment(string summary)
        {
            return GetTerminableDocumentComment(summary, "summary");
        }
        protected string GetRemarksDocumentComment(string remarks)
        {
            return GetTerminableDocumentComment(remarks, "remarks");
        }
        protected string GetReturnsDocumentComment(string returns)
        {
            return GetTerminableDocumentComment(returns, "returns");
        }

        protected virtual void WriteLine(string token, TranslatorFormatterMemberType memberType)
        {
            this.Write(token, memberType);
            this.WriteLine();
        }
        protected virtual void Write(string token, TranslatorFormatterMemberType memberType)
        {
            string result = GetFormat(token, memberType);
            target.Write(result);
        }
        protected virtual void Write(string token, IMember member, bool declarePoint)
        {
            string result = GetFormat(token, member, declarePoint);
            target.Write(result);
        }

        protected string GetFormat(string token, IMember member, bool declarePoint)
        {
            IIntermediateCodeTranslatorFormatter formatter = options.Formatter;
            string result = token;
            if (formatter != null)
                result = formatter.FormatMemberNameToken(token, member, Options, declarePoint);
            return result;
        }
        protected string GetFormat(string token, TranslatorFormatterMemberType memberType)
        {
            IIntermediateCodeTranslatorFormatter formatter = options.Formatter;
            string result = token;
            if (formatter != null)
                result = formatter.FormatMemberNameToken(token, memberType);
            return result;
        }
        protected virtual void WriteLine(string token, TranslatorFormatterMemberType memberType, IType parent)
        {
            this.Write(token, memberType, parent);
            this.WriteLine();
        }
        protected virtual void Write(string token, TranslatorFormatterMemberType memberType, IType parent)
        {
            string result = GetFormat(token, memberType, parent);
            target.Write(result);
        }

        protected string GetFormat(string token, TranslatorFormatterMemberType memberType, IType parent)
        {
            IIntermediateCodeTranslatorFormatter formatter = options.Formatter;
            string result = token;
            if (formatter != null)
                result = formatter.FormatMemberNameToken(token, memberType, parent);
            return result;
        }

        protected virtual void WriteLine(string typeToken, IType typeToWrite, bool declarePoint)
        {
            this.Write(typeToken, typeToWrite, declarePoint);
            this.WriteLine();
        }
        protected virtual void Write(string typeToken, IType typeToWrite, bool declarePoint)
        {
            string result = GetFormat(typeToken, typeToWrite, declarePoint);
            target.Write(result);
        }

        protected string GetFormat(string typeToken, IType typeToWrite, bool declarePoint)
        {
            IIntermediateCodeTranslatorFormatter formatter = options.Formatter;
            string result = typeToken;
            if (formatter != null)
                result = formatter.FormatTypeNameToken(typeToken, typeToWrite, Options, declarePoint);
            return result;
        }

        protected virtual void Write(string token, TranslatorFormatterTokenType tokenType)
        {
            string result = GetFormat(token, tokenType);
            target.Write(result);
        }

        protected virtual void Write(string token, TranslatorFormatterTokenType tokenType, ILabelStatement label, bool declarePoint)
        {
            string result = GetFormat(token, tokenType);
            target.Write(result);
        }

        protected string GetFormat(string token, TranslatorFormatterTokenType tokenType)
        {
            IIntermediateCodeTranslatorFormatter formatter = options.Formatter;
            string result = token;
            if (formatter != null)
            {
                switch (tokenType)
                {
                    case TranslatorFormatterTokenType.Keyword:
                        result = formatter.FormatKeywordToken(token);
                        break;
                    case TranslatorFormatterTokenType.Comment:
                        result = formatter.FormatCommentToken(token);
                        break;
                    case TranslatorFormatterTokenType.String:
                        result = formatter.FormatStringToken(token);
                        break;
                    case TranslatorFormatterTokenType.Operator:
                        result = formatter.FormatOperatorToken(token);
                        break;
                    case TranslatorFormatterTokenType.Number:
                        result = formatter.FormatNumberToken(token);
                        break;
                    case TranslatorFormatterTokenType.NameSpace:
                        result = formatter.FormatNameSpace(token);
                        break;
                    case TranslatorFormatterTokenType.Preformatted:
                        break;
                    case TranslatorFormatterTokenType.Other:
                    default:
                        result = formatter.FormatOtherToken(token);
                        break;
                }
            }
            return result;
        }

        protected virtual void WriteLine(string token, TranslatorFormatterTokenType tokenType)
        {
            this.Write(token, tokenType);
            this.WriteLine();
        }

        protected virtual void WriteLine()
        {
            string newLine = "";
            IIntermediateCodeTranslatorFormatter formatter = options.Formatter;
            if (formatter != null)
            {
                newLine = formatter.DenoteNewLine();
            }
            target.WriteLine(newLine);
        }

        /// <summary>
        /// Increases the <see cref="Target"/> indent level.
        /// </summary>
        protected void IncreaseIndent()
        {
            this.target.Indent++;
        }

        /// <summary>
        /// Decreases the <see cref="Target"/> indent level.
        /// </summary>
        protected void DecreaseIndent()
        {
            this.target.Indent--;
        }
        /// <summary>
        /// Returns the string which denotes the sub-tool version.  
        /// Relates specifically to the individual language implementation.
        /// </summary>
        public abstract string SubToolVersion { get; }

        /// <summary>
        /// Returns the string which denotes the sub-tool's name.
        /// Relates specifically to the individual language implementation.
        /// </summary>
        public abstract string SubToolName { get; }

        /// <summary>
        /// Returns the name of the language implemented by the sub-tool.
        /// </summary>
        public abstract string Language { get; }

        /// <summary>
        /// Returns the version of the tool.
        /// </summary>
        public string ToolVersion
        {
            get
            {
                return "1.0.0.0";
            }
        }

        /// <summary>
        /// Returns the name of the intermediate foundation.
        /// </summary>
        public string ToolName
        {
            get
            {
                return "Oilexer";
            }
        }

        /// <summary>
        /// Returns the 'generated by a tool' text for comments.
        /// </summary>
        public string GeneratedMessageText
        {
            get
            {
                return string.Format(generatedMessageBase, ToolName, ToolVersion, Language, SubToolName, SubToolVersion);
            }
        }
    }
}
