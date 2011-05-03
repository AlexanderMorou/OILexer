using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Expression;
using Oilexer.Statements;
using System.CodeDom.Compiler;
using System.CodeDom;
using Oilexer._Internal;
using Oilexer.Utilities.Arrays;
using System.Collections;
using Oilexer.Properties;
//using System.Windows.Forms;
using System.Diagnostics;
using Oilexer.Comments;
using System.Text.RegularExpressions;

namespace Oilexer.Translation
{
    public partial class CSharpCodeTranslator :
        IntermediateCodeTranslator
    {
        private bool suppressLineTerminator=false;
        private static Dictionary<int, Dictionary<int, string>> keywordLookup = new Dictionary<int, Dictionary<int, string>>();
        private Stack<INameSpaceDeclaration> namespaceForward = new Stack<INameSpaceDeclaration>();
        private string subToolVersion;
        static CSharpCodeTranslator()
        {
            for (int i = 2; i <= 10; i++)
                InitializeLookup(i);
            CSharpAutoFormTypeLookup = InitializeCSharpAutoFormTypeLookup();
        }
        //*/
        /// <summary>
        /// Translates a class-based declared type.
        /// </summary>
        /// <param name="classType">The <see cref="IClassType"/> to translate.</param>
        public override void TranslateType(IClassType classType)
        {
            //If it's a partial and the options disallow them, exit.
            //The full root member will export all members as necessary.
            if (!base.Options.AllowPartials && classType.IsPartial)
                return;
            //Only bother with this stuff if it's the root declaration.  
            //Why do needless work?
            if (classType.IsRoot)
            {
                //The attributes defined, but only on the root version, because
                //if certain attributes are redeclared, you get runtime errors 
                //(see: AttributeUsageAttribute.AllowMultiple).
                this.TranslateAttributes(classType, classType.Attributes);
                //public/private/internal/protected/protected+internal which one?
                this.TranslateConceptAccessModifiers(classType);
                //abstract?
                this.TranslateConceptNotInstantiableClass(classType);
                //sealed?
                this.TranslateConceptNotInheritableClass(classType);
                //static?
                this.TranslateConceptNotInheritableOrInstantiableClass(classType);
            }
            //partial?
            this.TranslateConceptPartial(classType);

            //Denote entity classification.
            TranslateConceptKeyword(Keywords.Class);
            base.Write(" ", TranslatorFormatterTokenType.Other);

            //Name
            this.TranslateConceptIdentifier(classType, true);

            //Type-parameter list.
            if (classType.TypeParameters != null && classType.TypeParameters.Count > 0)
                this.TranslateTypeParameters((ITypeParameterMembers)classType.TypeParameters);

            if (classType.IsRoot)
            {
                //Fix for when the base-type contains a type-parameter of a nested type.
                Options.BuildTrail.Pop();
                //--Implements and base type.
                bool firstMember = true;
                base.IncreaseIndent();
                //Emit the base-type, only if it derives from something -other- than object.
                if (classType.BaseType != null && (!(classType.BaseType.Equals(typeof(object).GetTypeReference()))))
                {
                    //Not the first item any more (see implements list)
                    firstMember = false;
                    //Because it derives from...
                    base.WriteLine(" :", TranslatorFormatterTokenType.Operator);
                    //classType.BaseType
                    this.TranslateConceptTypeName(classType.BaseType);
                }
                if (classType.ImplementsList != null && classType.ImplementsList.Count > 0)
                {

                    //Denote implementation, if the base-type is null or 'System.Object'.
                    if (firstMember)
                        base.WriteLine(" :", TranslatorFormatterTokenType.Operator);

                    foreach (ITypeReference itr in classType.ImplementsList)
                    {
                        //If it's the first, don't include the ',\r\n'
                        if (firstMember)
                            firstMember = false;
                        else
                            base.WriteLine(",", TranslatorFormatterTokenType.Operator);

                        this.TranslateConceptTypeName(itr);
                    }
                }
                //Resume normal indent
                base.DecreaseIndent();
                //Worry about the constraints if it's the root version, only.
                TranslateGroupConstraints(classType.TypeParameters);
                Options.BuildTrail.Push(classType);
            }

            //Line-Break.
            base.WriteLine();

            //Start class body.
            base.WriteLine("{", TranslatorFormatterTokenType.Operator);
            base.IncreaseIndent();

            //Counts, used to separate types/members with a spacer.
            int memberCount = 0,
                typeCount = 0;

            /* *
             * Count the members based upon whether or not there are to be partials, 
             * if so, only include the members from the current entity, otherwise, all members.
             * 
             * The parameter for the count methods uses inverted logic of the AllowPartials property
             * on the options.
             * */
            memberCount = classType.GetMemberCount(!base.Options.AllowPartials);
            typeCount = classType.GetTypeCount(!base.Options.AllowPartials);

            //Translate the 'MemberParentType' members
            this.TranslateMemberParentTypeMembers(classType);

            //Space the members from the types.
            if (memberCount > 0 && typeCount > 0)
                base.WriteLine();

            //Nested types, if it's a resources class, ignore the sub-types, they are 
            //all null.
            if (!(classType is IDeclarationResources))
                this.TranslateTypeParentTypes(classType);

            //End class body.
            base.DecreaseIndent();
            base.WriteLine("}", TranslatorFormatterTokenType.Operator);

        }

        /// <summary>
        /// Translates an enumerator-based declared type.
        /// </summary>
        /// <param name="enumeratorType">The <see cref="IEnumeratorType"/> to translate.</param>
        public override void TranslateType(IEnumeratorType enumeratorType)
        {
            
            this.TranslateAttributes(enumeratorType, enumeratorType.Attributes);
            this.TranslateConceptAccessModifiers(enumeratorType);
            this.TranslateConceptKeyword(Keywords.Enum);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptIdentifier(enumeratorType, true);
            switch (enumeratorType.BaseType)
            {
                case EnumeratorBaseType.UByte:
                    base.Write(" : ", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Byte);
                    break;
                case EnumeratorBaseType.SByte:
                    base.Write(" : ", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Sbyte);
                    break;
                case EnumeratorBaseType.UShort:
                    base.Write(" : ", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Ushort);
                    break;
                case EnumeratorBaseType.Short:
                    base.Write(" : ", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Short);
                    break;
                case EnumeratorBaseType.UInt:
                    base.Write(" : ", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Uint);
                    break;
                case EnumeratorBaseType.ULong:
                    base.Write(" : ", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Ulong);
                    break;
                case EnumeratorBaseType.SLong:
                    base.Write(" : ", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Long);
                    break;
                case EnumeratorBaseType.SInt:
                default:
                    break;
            }
            base.WriteLine();
            base.WriteLine("{", TranslatorFormatterTokenType.Operator);
            base.IncreaseIndent();
            this.TranslateMembers(enumeratorType, enumeratorType.Fields);
            base.DecreaseIndent();
            base.WriteLine("}", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates a delegate-based declared type.
        /// </summary>
        /// <param name="delegateType">The <see cref="IDelegateType"/> to translate.</param>
        public override void TranslateType(IDelegateType delegateType)
        {
            TranslateConceptAccessModifiers(delegateType);
            TranslateConceptKeyword(Keywords.Delegate);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            if (delegateType.ReturnType == null)
                TranslateConceptKeyword(Keywords.Void);
            else
                TranslateConceptTypeName(delegateType.ReturnType);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            TranslateConceptIdentifier(delegateType, true);
            if (delegateType.TypeParameters.Count > 0)
                TranslateTypeParameters((ITypeParameterMembers)delegateType.TypeParameters);
            TranslateParameters(delegateType.Parameters);
            if (delegateType.TypeParameters.Count > 0)
            {
                TranslateGroupConstraints(delegateType.TypeParameters);
            }
            base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates an interface-based declared type.
        /// </summary>
        /// <param name="interfaceType">The <see cref="IInterfaceType"/> to translate.</param>
        public override void TranslateType(IInterfaceType interfaceType)
        {
            //Only bother with this stuff if it's the root declaration, why do
            //needless work?
            if (interfaceType.IsRoot)
            {
                //The attributes defined.
                this.TranslateAttributes(interfaceType, interfaceType.Attributes);
                //public/private/internal/protected/protected+internal which one?
                this.TranslateConceptAccessModifiers(interfaceType);
            }
            //partial?
            this.TranslateConceptPartial(interfaceType);
            this.TranslateConceptKeyword(Keywords.Interface);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptIdentifier(interfaceType, true);
            if (interfaceType.TypeParameters.Count > 0)
                this.TranslateTypeParameters((ITypeParameterMembers)interfaceType.TypeParameters);
            //--Implements list.
            if (interfaceType.ImplementsList != null && interfaceType.ImplementsList.Count > 0)
            {
                base.IncreaseIndent();

                bool flag = true;
                //Denote implementation, if the base-type is null or 'System.Object'.
                if (flag)
                    base.WriteLine(" :", TranslatorFormatterTokenType.Operator);
                foreach (ITypeReference itr in interfaceType.ImplementsList)
                {
                    //If it's the first, don't include the ',\r\n'
                    if (flag)
                        flag = false;
                    else
                        base.WriteLine(",", TranslatorFormatterTokenType.Operator);

                    this.TranslateConceptTypeName(itr);
                }
                //Resume normal indent.
                base.DecreaseIndent();
            }

            //Worry about the constraints if it's the root version, only.
            if (interfaceType.IsRoot)
                TranslateGroupConstraints(interfaceType.TypeParameters);
            base.WriteLine();
            base.WriteLine("{", TranslatorFormatterTokenType.Operator);
            base.IncreaseIndent();
            //Members
            this.TranslateSignatureMemberParentTypeMembers(interfaceType);
            base.DecreaseIndent();
            base.WriteLine("}", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates a structure-based declared type.
        /// </summary>
        /// <param name="structureType">The <see cref="IStructType"/> to translate.</param>
        public override void TranslateType(IStructType structureType)
        {
            if (!base.Options.AllowPartials && structureType.IsPartial)
                return;
            //Only bother with this stuff if it's the root declaration.  
            //Why do needless work?
            if (structureType.IsRoot)
            {
                //The attributes defined, but only on the root version, because
                //if certain attributes are redeclared, you get runtime errors 
                //(see: AttributeUsageAttribute.AllowMultiple).
                this.TranslateAttributes(structureType, structureType.Attributes);
                //public/private/internal/protected/protected+internal which one?
                this.TranslateConceptAccessModifiers(structureType);
            }
            //partial?
            this.TranslateConceptPartial(structureType);

            //Keyword
            TranslateConceptKeyword(Keywords.Struct);
            base.Write(" ", TranslatorFormatterTokenType.Other);

            //Name
            this.TranslateConceptIdentifier(structureType, true);

            //Type-parameter list.
            if (structureType.TypeParameters.Count > 0)
                this.TranslateTypeParameters((ITypeParameterMembers)structureType.TypeParameters);

            //--Implements and base type.
            bool firstMember = true;
            base.IncreaseIndent();

            if (structureType.ImplementsList != null && structureType.ImplementsList.Count > 0)
            {
                //Fix for when a type-parameter of the struct is a nested type of the struct.
                Options.BuildTrail.Pop();
                //Denote implementation.
                base.WriteLine(" :", TranslatorFormatterTokenType.Operator);
                foreach (ITypeReference itr in structureType.ImplementsList)
                {
                    //If it's the first, don't include the ',\r\n'
                    if (firstMember)
                        firstMember = false;
                    else
                        base.WriteLine(",", TranslatorFormatterTokenType.Operator);

                    this.TranslateConceptTypeName(itr);
                }
                Options.BuildTrail.Push(structureType);
            }
            //Resume normal indent.
            base.DecreaseIndent();

            base.WriteLine();
            firstMember = true;
            //Worry about the constraints if it's the root version, only.
            if (structureType.IsRoot)
                TranslateGroupConstraints(structureType.TypeParameters);
            base.WriteLine("{", TranslatorFormatterTokenType.Operator);
            base.IncreaseIndent();
            //Members
            int memberCount = 0, typeCount = 0;
            if (base.Options.AllowPartials)
            {
                memberCount = structureType.GetMemberCount(false);
                typeCount = structureType.GetTypeCount(false);
            }
            else
            {
                memberCount = structureType.GetMemberCount(true);
                typeCount = structureType.GetTypeCount(true);
            }
            this.TranslateMemberParentTypeMembers(structureType);
            //Space the members from the types.
            if (memberCount > 0 && typeCount > 0)
                base.WriteLine();
            //Nested types.
            this.TranslateTypeParentTypes(structureType);
            base.DecreaseIndent();
            base.WriteLine("}", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateType(IDeclaredType declaredType)
        {
            /* *
             * Cache the current type, in the event that the declaredType
             * is nested, then denote, to the formatter, which type is the
             * actively translated type.
             * */
            var currentType = this.Options.CurrentType;
            this.Options.CurrentType = declaredType;
            base.TranslateType(declaredType);
            /* *
             * For the sake of nesting, restore the previous 'currentType'
             * to whatever it was before, either null or the parent type
             * of declaredType.
             * */
            this.Options.CurrentType = currentType;
        }

        /// <summary>
        /// Translates an <see cref="IIntermediateProject"/> as a whole or 
        /// partial by partial.
        /// </summary>
        /// <param name="project">The <see cref="IIntermediateProject"/> to translate.</param>
        /// <remarks>Depending on the <see cref="Options"/>, this will translate all declared namespaces 
        /// or just those on the current partial.</remarks>
        protected override void TranslateProjectInner(IIntermediateProject project)
        {
            if (((!(base.Options.AllowPartials)) && (project.IsPartial)))
                return;
            Stopwatch sw = new Stopwatch();
            StringBuilder typesUsed;
            sw.Start();
            this.TranslateConceptComment(this.GeneratedMessageText, false);

            if (base.Options.AutoResolveReferences)
            {
                ProjectDependencyReport pdr = new ProjectDependencyReport(project, base.Options);
                pdr.Reduce();
                foreach (string s in pdr.NameSpaceDependencies)
                    Options.ImportList.Add(s);
                typesUsed = _OIL._Core.GetTypesUsedComment(ref pdr, base.Options);
            }
            else
                typesUsed = null;
            if (Options.ImportList.Count > 0)
            {

                foreach (string s in Options.ImportList)
                {
                    this.TranslateConceptKeyword(Keywords.Using);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    base.Write(s, TranslatorFormatterTokenType.NameSpace);
                    base.WriteLine(";", TranslatorFormatterTokenType.Operator);
                }
                base.WriteLine();
            }

            if (project.IsRoot)
            {
                TranslateAttributes(AttributeTargets.Assembly, project, project.Attributes);
                if (project.Attributes.Count > 0)
                    base.WriteLine();
            }

            Options.ImportList.Clear();
            this.TranslateNameSpaces(project, project.NameSpaces);
            if ((project.GetTypeCount(!base.Options.AllowPartials) > 0) && (project.GetNameSpaceCount(!base.Options.AllowPartials) > 0))
                base.Target.WriteLine();
            TranslateTypeParentTypes(project);
            sw.Stop();
            string typesUsedExp;
            if (typesUsed == null)
                typesUsedExp = "";
            else
                typesUsedExp = string.Format("\r\n{0}", typesUsed.ToString());
            this.TranslateConceptComment(string.Format("This file took {0} to generate.\r\nDate generated: {1}{2}", sw.Elapsed, DateTime.Now, typesUsedExp), false);
            /* *
             * Remove the intermediate project file/partial from the build trail.
             * */
        }

        /// <summary>
        /// Translates a namespace declaration.
        /// </summary>
        /// <param name="nameSpace">The <see cref="INameSpaceDeclaration"/> to translate.</param>
        /// <remarks>Depending on the <see cref="IntermediateCodeTranslator.Options"/>, this will translate all of the declared 
        /// child namespaces and the current <see cref="INameSpaceDeclaration"/>'s types or just 
        /// those on the current partial.</remarks>
        public override void TranslateNameSpace(INameSpaceDeclaration nameSpace)
        {
            if (!(nameSpace.GetNameSpaceCount(!(Options.AllowPartials)) > 0 || nameSpace.GetTypeCount(!base.Options.AllowPartials) > 0))
                return;
            base.Options.BuildTrail.Push(nameSpace);
            bool forwardOnly;
            if (forwardOnly = (nameSpace.GetTypeCount(!base.Options.AllowPartials) == 0))
                namespaceForward.Push(nameSpace);
            if (!forwardOnly)
            {
                this.TranslateConceptKeyword(Keywords.Namespace);
                base.Write(" ", TranslatorFormatterTokenType.Other);

                if (namespaceForward.Count > 0)
                {
                    var newSet = new Stack<INameSpaceDeclaration>(namespaceForward);
                    foreach (var forward in newSet)
                    {
                        this.TranslateConceptIdentifier(forward, true);
                        base.Write(".", TranslatorFormatterTokenType.Operator);
                    }
                }
                this.TranslateConceptIdentifier(nameSpace, true);
                base.WriteLine();
                base.WriteLine("{", TranslatorFormatterTokenType.Operator);
                base.IncreaseIndent();
                this.TranslateTypeParentTypes(nameSpace);
            }
            this.TranslateNameSpaces(nameSpace, nameSpace.ChildSpaces);
            if (!forwardOnly)
            {
                base.DecreaseIndent();
                base.WriteLine("}", TranslatorFormatterTokenType.Operator);
            }
            if (forwardOnly)
                namespaceForward.Pop();
            base.Options.BuildTrail.Pop();
        }

        public override void TranslateMembers(IEnumeratorType parent, IEnumTypeFieldMembers fieldMembers)
        {
            bool flag = true;
            foreach (IFieldMember ifm in fieldMembers.Values)
            {
                if (flag)
                    flag = false;
                else
                {
                    base.WriteLine(", ", TranslatorFormatterTokenType.Operator);
                    base.WriteLine();
                }
                if (base.Options.AutoComments)
                {
                    if (ifm.Summary != null && ifm.Summary != string.Empty)
                        TranslateConceptComment(GetSummaryDocumentComment(ifm.Summary), true);
                    if (ifm.Remarks != null && ifm.Remarks != string.Empty)
                        TranslateConceptComment(GetRemarksDocumentComment(ifm.Remarks), true);
                }
                this.TranslateAttributes(ifm, ifm.Attributes);
                this.TranslateConceptIdentifier(ifm, true);
                if (ifm.InitializationExpression != null)
                {
                    base.Write(" = ", TranslatorFormatterTokenType.Operator);
                    this.TranslateExpression(ifm.InitializationExpression);
                }
            }
            base.WriteLine();
        }
        private string GetAmbigMethodComments<TParameter, TTypeParameter, TSignatureDom, TParent>(IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethodSigMember)
            where TParameter :
                IParameteredParameterMember<TParameter, TSignatureDom, TParent>
            where TTypeParameter :
                IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
            where TSignatureDom :
                CodeMemberMethod,
                new()
            where TParent :
                IDeclarationTarget
        {
            if (Options.AutoComments)
            {
                StringBuilder result = new StringBuilder();
                foreach (var param in ambigMethodSigMember.Parameters.Values)
                    if (!string.IsNullOrEmpty(param.DocumentationComment))
                        result.Append(ResolveDocumentationCommentLookups(GetTerminableDocumentComment(param.DocumentationComment.HTMLEncode(false), "param", "name", param.Name)));
                return result.ToString();
            }
            else
                return string.Empty;
        }

        public override void TranslateMember<TParameter, TTypeParameter, TSignatureDom, TParent>(IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethodSigMember)
        {
            TranslateAutoComments(ambigMethodSigMember, ()=>GetAmbigMethodComments<TParameter, TTypeParameter, TSignatureDom, TParent>(ambigMethodSigMember));
            //Only emit the modifiers if it's part of an instanciable type.
            this.TranslateAttributes(ambigMethodSigMember, ambigMethodSigMember.Attributes);
            if (ambigMethodSigMember is IMethodMember)
            {
                if (((IMethodMember)ambigMethodSigMember).PrivateImplementationTarget == null)
                {
                    IMethodMember imm = (IMethodMember)ambigMethodSigMember;
                    this.TranslateConceptAccessModifiers(imm);
                    if (imm.IsVirtual)
                    {
                        this.TranslateConceptKeyword(Keywords.Virtual);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                    }
                    else if (imm.IsAbstract)
                    {
                        this.TranslateConceptKeyword(Keywords.Abstract);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                    }
                    else if (imm.HidesPrevious)
                    {
                        if (imm.IsStatic)
                        {
                            this.TranslateConceptKeyword(Keywords.Static);
                            base.Write(" ", TranslatorFormatterTokenType.Other);
                        }
                        this.TranslateConceptKeyword(Keywords.New);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                    }
                    else if (imm.Overrides)
                    {
                        this.TranslateConceptKeyword(Keywords.Override);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                        if (imm.IsFinal)
                        {
                            this.TranslateConceptKeyword(Keywords.Sealed);
                            base.Write(" ", TranslatorFormatterTokenType.Other);
                        }
                    }
                    else if (imm.IsStatic)
                        if (imm.IsStatic)
                        {
                            this.TranslateConceptKeyword(Keywords.Static);
                            base.Write(" ", TranslatorFormatterTokenType.Other);
                        }
                }
            }
            else if (ambigMethodSigMember.HidesPrevious)
            {
                this.TranslateConceptKeyword(Keywords.New);
                base.Write(" ", TranslatorFormatterTokenType.Other);
            }
            this.TranslateConceptTypeName(ambigMethodSigMember.ReturnType);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            //If the method implements an interface member privately.
            if (ambigMethodSigMember is IMethodMember && ((IMethodMember)(ambigMethodSigMember)).PrivateImplementationTarget != null)
            {
                //Emit its name, and a '.'
                this.TranslateConceptTypeName(((IMethodMember)(ambigMethodSigMember)).PrivateImplementationTarget);
                base.Write(".", TranslatorFormatterTokenType.Operator);
            }
            //Emit the typename.
            this.TranslateConceptIdentifier(ambigMethodSigMember, true);
            //Emit the type-parameter names, if necessary.
            if (ambigMethodSigMember.TypeParameters.Count > 0)
                this.TranslateTypeParameters((ITypeParameterMembers)ambigMethodSigMember.TypeParameters);

            //Emit the parameters.
            this.TranslateParameters(ambigMethodSigMember.Parameters);

            //Emit the type-parameter constraints.
            if (ambigMethodSigMember.TypeParameters.Count > 0)
            {
                if (!(ambigMethodSigMember is IMethodMember) || ((IMethodMember)(ambigMethodSigMember)).PrivateImplementationTarget == null)
                    TranslateGroupConstraints(ambigMethodSigMember.TypeParameters);
            }
            else if (ambigMethodSigMember is IMethodMember && (!((IMethodMember)(ambigMethodSigMember)).IsAbstract))
                base.WriteLine();
            if (ambigMethodSigMember.ParentTarget is ISignatureMemberParentType)
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
            if (ambigMethodSigMember is IMethodMember)
                if (!((IMethodMember)(ambigMethodSigMember)).IsAbstract)
                    this.TranslateStatementBlock(((IMethodMember)ambigMethodSigMember).Statements);
                else
                    base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        protected void TranslateGroupConstraints<TItem, TDom, TParent>(ITypeParameterMembers<TItem, TDom, TParent> typeParameters)
            where TItem :
                ITypeParameterMember<TDom, TParent>
            where TDom :
                CodeTypeParameter,
                new()
            where TParent :
                IDeclaration
        {
            if (!(typeParameters != null && typeParameters.Count > 0))
                return;
            foreach (TItem itpm in typeParameters.Values)
            {
                if (itpm.Constraints.Count > 0 || itpm.RequiresConstructor || itpm.SpecialCondition != TypeParameterSpecialCondition.None)
                    base.WriteLine();
                this.TranslateConstraints(itpm);
            }
        }

        public override void TranslateMember(IIndexerSignatureMember indexerSigMember)
        {
            TranslateAutoComments(indexerSigMember);
            this.TranslateAttributes(indexerSigMember, indexerSigMember.Attributes);
            //Property type.
            this.TranslateConceptTypeName(indexerSigMember.PropertyType);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptKeyword(Keywords.This);
            TranslateParameters(indexerSigMember.Parameters);
            base.WriteLine();
            //Write the property method bodies
            base.Write("{ ", TranslatorFormatterTokenType.Operator);
            base.IncreaseIndent();
            if (indexerSigMember.HasGet)
            {
                this.TranslateConceptKeyword(Keywords.Get);
                base.Write("; ", TranslatorFormatterTokenType.Operator);
            }
            if (indexerSigMember.HasSet)
            {
                this.TranslateConceptKeyword(Keywords.Set);
                base.Write("; ", TranslatorFormatterTokenType.Operator);
            }
            base.DecreaseIndent();
            base.WriteLine("}", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateMember(IIndexerMember propertyMember)
        {
            TranslateAutoComments(propertyMember);
            this.TranslateAttributes(propertyMember, propertyMember.Attributes);
            if (propertyMember.PrivateImplementationTarget == null)
            {
                TranslateConceptAccessModifiers(propertyMember);
                if (propertyMember.IsVirtual)
                {
                    this.TranslateConceptKeyword(Keywords.Virtual);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                }
                else if (propertyMember.HidesPrevious)
                {
                    if (propertyMember.IsStatic)
                    {
                        this.TranslateConceptKeyword(Keywords.Static);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                    }
                    this.TranslateConceptKeyword(Keywords.New);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                }
                else if (propertyMember.Overrides)
                {
                    this.TranslateConceptKeyword(Keywords.Override);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    if (propertyMember.IsFinal)
                    {
                        this.TranslateConceptKeyword(Keywords.Sealed);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                    }
                }
                else if (propertyMember.IsStatic)
                    if (propertyMember.IsStatic)
                    {
                        this.TranslateConceptKeyword(Keywords.Static);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                    }
            }
            //Property type.
            this.TranslateConceptTypeName(propertyMember.PropertyType);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            if (propertyMember.PrivateImplementationTarget != null)
            {
                this.TranslateConceptTypeName(propertyMember.PrivateImplementationTarget);
                base.Write(".", TranslatorFormatterTokenType.Operator);
            }
            this.TranslateConceptKeyword(Keywords.This);
            TranslateParameters(propertyMember.Parameters);
            base.WriteLine();
            //Write the property method bodies
            base.WriteLine("{", TranslatorFormatterTokenType.Operator);
            base.IncreaseIndent();
            if (propertyMember.HasGet)
            {
                this.TranslateConceptKeyword(Keywords.Get);
                base.WriteLine();
                this.TranslateStatementBlock(propertyMember.GetPart.Statements);
            }
            if (propertyMember.HasSet)
            {
                this.TranslateConceptKeyword(Keywords.Set);
                base.WriteLine();
                this.TranslateStatementBlock(propertyMember.SetPart.Statements);
            }
            base.DecreaseIndent();
            base.WriteLine("}", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateMember<TParent>(IPropertySignatureMember<TParent> ambigPropertySigMember)
        {
            TranslateAutoComments(ambigPropertySigMember);
            this.TranslateAttributes(ambigPropertySigMember, ambigPropertySigMember.Attributes);
            //public/private/internal/protected/protected+internal?
            //But only if the private implementation is null.
            IPropertyMember ipm = null;
            if (ambigPropertySigMember is IPropertyMember)
            {
                ipm = ((IPropertyMember)ambigPropertySigMember);
                if (ipm.PrivateImplementationTarget == null)
                    this.TranslateConceptAccessModifiers(ambigPropertySigMember);
                if (ipm.IsAbstract)
                {
                    this.TranslateConceptKeyword(Keywords.Abstract);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                }
                else if (ipm.IsVirtual)
                {
                    this.TranslateConceptKeyword(Keywords.Virtual);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                }
                else if (ipm.HidesPrevious)
                {
                    if (ipm.IsStatic)
                    {
                        this.TranslateConceptKeyword(Keywords.Static);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                    }
                    this.TranslateConceptKeyword(Keywords.New);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                }
                else if (ipm.Overrides)
                {
                    this.TranslateConceptKeyword(Keywords.Override);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    if (ipm.IsFinal)
                    {
                        this.TranslateConceptKeyword(Keywords.Sealed);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                    }
                }
                else if (ipm.IsStatic)
                {
                    this.TranslateConceptKeyword(Keywords.Static);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                }
            }
            //Property type.
            this.TranslateConceptTypeName(ambigPropertySigMember.PropertyType);

            base.Write(" ", TranslatorFormatterTokenType.Other);
            if (ipm != null && ipm.PrivateImplementationTarget != null)
            {
                //Note it's private implementation base.target.
                this.TranslateConceptTypeName(ipm.PrivateImplementationTarget);
                base.Write(".", TranslatorFormatterTokenType.Operator);
            }

            //Write the name of the property, adjust accordingly if the type is a reserved name.
            this.TranslateConceptIdentifier(ambigPropertySigMember, true);

            //If it's not a signature.
            if (ipm != null && !ipm.IsAbstract)
            {
                base.WriteLine();
                //Write the property method bodies
                base.WriteLine("{", TranslatorFormatterTokenType.Operator);
                base.IncreaseIndent();
                //if (ipm.Attributes != null && ipm.Attributes.Count>0)

                if (ipm.HasGet)
                {
                    if (ipm.GetPart.Attributes.Count > 0)
                        TranslateAttributes(ipm.GetPart, ipm.GetPart.Attributes);
                    this.TranslateConceptKeyword(Keywords.Get);
                    base.WriteLine();
                    this.TranslateStatementBlock(ipm.GetPart.Statements);
                }
                if (ipm.HasSet)
                {
                    if (ipm.SetPart.Attributes.Count > 0)
                        TranslateAttributes(ipm.SetPart, ipm.SetPart.Attributes);
                    this.TranslateConceptKeyword(Keywords.Set);
                    base.WriteLine();
                    this.TranslateStatementBlock(ipm.SetPart.Statements);
                }
                base.DecreaseIndent();
                base.WriteLine("}", TranslatorFormatterTokenType.Operator);
            }
            else
            {
                base.Write("{ ", TranslatorFormatterTokenType.Operator);
                if (ambigPropertySigMember.HasGet)
                {
                    this.TranslateConceptKeyword(Keywords.Get);
                    base.Write("; ", TranslatorFormatterTokenType.Operator);
                }
                if (ambigPropertySigMember.HasSet)
                {
                    this.TranslateConceptKeyword(Keywords.Set);
                    base.Write("; ", TranslatorFormatterTokenType.Operator);
                }
                base.WriteLine("}", TranslatorFormatterTokenType.Operator);
            }
        }

        public override void TranslateMember(IFieldMember fieldMember)
        {
            TranslateAutoComments(fieldMember);
            this.TranslateAttributes(fieldMember, fieldMember.Attributes);
            this.TranslateConceptAccessModifiers(fieldMember);
            if (fieldMember.IsStatic)
            {
                TranslateConceptKeyword(Keywords.Static);
                base.Write(" ", TranslatorFormatterTokenType.Other);
            }
            else if (fieldMember.IsConstant && !(fieldMember.ParentTarget is IEnumeratorType))
            {
                TranslateConceptKeyword(Keywords.Const);
                base.Write(" ", TranslatorFormatterTokenType.Other);
            }
            this.TranslateConceptTypeName(fieldMember.FieldType);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptIdentifier(fieldMember, true);
            if (fieldMember.InitializationExpression != null)
            {
                base.Write(" = ", TranslatorFormatterTokenType.Operator);
                this.TranslateExpression(fieldMember.InitializationExpression);
            }
            base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        private void TranslateAutoComments(IAutoCommentMember autoCommentMember, Func<string> innerComment=null)
        {
            if (base.Options.AutoComments)
            {
                if (autoCommentMember.Summary != null && autoCommentMember.Summary != string.Empty)
                    TranslateConceptComment(ResolveDocumentationCommentLookups(GetSummaryDocumentComment(autoCommentMember.Summary.HTMLEncode(false))), true);
                if (innerComment != null)
                {
                    var innerCommentValue = innerComment();
                    if (!string.IsNullOrEmpty(innerCommentValue))
                        TranslateConceptComment(innerCommentValue, true);
                }
                if (autoCommentMember.Remarks != null && autoCommentMember.Remarks != string.Empty)
                    TranslateConceptComment(ResolveDocumentationCommentLookups(GetRemarksDocumentComment(autoCommentMember.Remarks.HTMLEncode(false))), true);
            }
        }

        public override void TranslateMember(IConstructorMember constructorMember)
        {
            this.TranslateAttributes(constructorMember, constructorMember.Attributes);
            if (constructorMember.IsStatic)
            {
                this.TranslateConceptKeyword(Keywords.Static);
                base.Write(" ", TranslatorFormatterTokenType.Other);
            }
            else
                this.TranslateConceptAccessModifiers(constructorMember);
            this.TranslateConceptIdentifier(constructorMember.ParentTarget, true);
            this.TranslateParameters(constructorMember.Parameters);
            this.IncreaseIndent();
            base.WriteLine();
            switch (constructorMember.CascadeExpressionsTarget)
            {
                case ConstructorCascadeTarget.Undefined:
                    break;
                case ConstructorCascadeTarget.Base:
                    base.Write(" : ", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Base);
                    base.Write("(", TranslatorFormatterTokenType.Operator);
                    this.IncreaseIndent();
                    this.TranslateExpressionGroup(constructorMember.CascadeMembers);
                    this.DecreaseIndent();
                    base.WriteLine(")", TranslatorFormatterTokenType.Operator);
                    break;
                case ConstructorCascadeTarget.This:
                    base.Write(" : ", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.This);
                    base.Write("(", TranslatorFormatterTokenType.Operator);
                    this.IncreaseIndent();
                    this.TranslateExpressionGroup(constructorMember.CascadeMembers);
                    this.DecreaseIndent();
                    base.WriteLine(")", TranslatorFormatterTokenType.Operator);
                    break;
                default:
                    break;
            }
            this.DecreaseIndent();
            this.TranslateStatementBlock(constructorMember.Statements);
        }

        public override void TranslateMember<TParameter, TParameteredDom, TParent>(IParameteredParameterMember<TParameter, TParameteredDom, TParent> ambigParamMember)
        {
            this.TranslateAttributes(ambigParamMember, ambigParamMember.Attributes);
            switch (ambigParamMember.Direction)
            {
                case FieldDirection.Out:
                    TranslateConceptKeyword(Keywords.Out);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    break;
                case FieldDirection.Ref:
                    TranslateConceptKeyword(Keywords.Ref);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    break;
            }
            this.TranslateConceptTypeName(ambigParamMember.ParameterType);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptIdentifier(ambigParamMember, true);

        }

        public override void TranslateMember<TDom, TParent>(ITypeParameterMember<TDom, TParent> typeParamMember)
        {
            this.TranslateConceptIdentifier(typeParamMember, true);
        }

        /// <summary>
        /// Translates an assignment statement.
        /// </summary>
        /// <param name="assignStatement">The <see cref="IAssignStatement"/> which contains the 
        /// <see cref="IAssignStatement.Reference"/> to set to the <see cref="IAssignStatement.Value"/>.</param>
        public override void TranslateStatement(IAssignStatement assignStatement)
        {
            this.TranslateExpression(assignStatement.Reference);
            base.Write(" = ", TranslatorFormatterTokenType.Operator);
            this.TranslateExpression(assignStatement.Value);
            if (!suppressLineTerminator)
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates a break statement that exists loops, switches, and so on.
        /// </summary>
        /// <param name="breakStatement">The <see cref="IBreakStatement"/> that exits the current 
        /// code-block that has a <see cref="IBreakTargetExitPoint"/>.</param>
        public override void TranslateStatement(IBreakStatement breakStatement)
        {
            TranslateConceptKeyword(Keywords.Break);
            if (!suppressLineTerminator)
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates a comment statement.
        /// </summary>
        /// <param name="commentStatement">The <see cref="ICommentStatement"/> that represents
        /// a mention to what the code does.</param>
        public override void TranslateStatement(ICommentStatement commentStatement)
        {
            TranslateConceptComment(commentStatement.Comment, false);
        }

        /// <summary>
        /// Translates a standard case logical if/then statement.
        /// </summary>
        /// <param name="ifThenStatement">The <see cref="IConditionStatement"/> which 
        /// manages the flow of the execution.</param>
        public override void TranslateStatement(IConditionStatement ifThenStatement)
        {
            this.TranslateConceptKeyword(Keywords.If);
            bool paramsNeeded = (!(ifThenStatement.Condition is IBinaryOperationExpression));
            if (paramsNeeded)
                base.Write(" (", TranslatorFormatterTokenType.Operator);
            else
                base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateExpression(ifThenStatement.Condition);
            if (paramsNeeded)
                base.WriteLine(")", TranslatorFormatterTokenType.Operator);
            else
                base.WriteLine();
            this.TranslateStatementBlock(ifThenStatement.Statements);
            if (ifThenStatement.FalseBlock.Count > 0)
            {
                if (ifThenStatement.FalseBlock.Count == 1 &&
                    ifThenStatement.FalseBlock[0] is IConditionStatement)
                {
                    this.TranslateConceptKeyword(Keywords.Else);
                    DecreaseIndent();
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    this.TranslateStatementBlock(ifThenStatement.FalseBlock);
                    IncreaseIndent();
                }
                else
                {
                    this.TranslateConceptKeyword(Keywords.Else);
                    base.WriteLine();
                    this.TranslateStatementBlock(ifThenStatement.FalseBlock);
                }
            }
        }

        /// <summary>
        /// Translates a standard switch/[select case] statement.
        /// </summary>
        /// <param name="switchStatement">The <see cref="ISwitchStatement"/> which
        /// handles large constant based cases.</param>
        public override void TranslateStatement(ISwitchStatement switchStatement)
        {
            TranslateConceptKeyword(Keywords.Switch);
            base.Write(" (", TranslatorFormatterTokenType.Operator);
            TranslateExpression(switchStatement.CaseSwitch);
            base.WriteLine(")", TranslatorFormatterTokenType.Operator);
            base.WriteLine("{", TranslatorFormatterTokenType.Operator);
            base.IncreaseIndent();
            foreach (ISwitchStatementCase @case in switchStatement.Cases)
            {
                foreach (IExpression expr in @case.Cases)
                {
                    TranslateConceptKeyword(Keywords.Case);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    TranslateExpression(expr);
                    base.WriteLine(":", TranslatorFormatterTokenType.Operator);
                }
                if (@case.LastIsDefaultCase)
                {
                    TranslateConceptKeyword(Keywords.Default);
                    base.WriteLine(":", TranslatorFormatterTokenType.Operator);
                }
                base.IncreaseIndent();
                base.TranslateStatementBlock(@case.Statements);
                TranslateConceptKeyword(Keywords.Break);
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
                base.DecreaseIndent();
            }
            base.DecreaseIndent();
            base.WriteLine("}", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates an enumerator statement that iterates an <see cref="IEnumerable"/>
        /// entity.
        /// </summary>
        /// <param name="enumStatement">The <see cref="IEnumeratorStatement"/> that
        /// is to be translated.</param>
        public override void TranslateStatement(IEnumeratorStatement enumStatement)
        {
            this.TranslateConceptKeyword(Keywords.Foreach);
            base.Write(" (", TranslatorFormatterTokenType.Operator);
            this.TranslateConceptTypeName(enumStatement.CurrentMember.LocalType);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptIdentifier(enumStatement.CurrentMember, true);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptKeyword(Keywords.In);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            TranslateExpression(enumStatement.EnumeratorSource);
            base.WriteLine(")", TranslatorFormatterTokenType.Operator);
            bool autoDecl = enumStatement.CurrentMember.AutoDeclare;
            enumStatement.CurrentMember.AutoDeclare = false;
            this.TranslateStatementBlock(enumStatement.Statements);
            enumStatement.CurrentMember.AutoDeclare = autoDecl;
        }

        /// <summary>
        /// Translates a for range statement which is an iteration statement that has 
        /// a clearly defined start and end and optional step as well.
        /// </summary>
        /// <param name="forRangeStatement">The <see cref="IForRangeStatement"/> to be translated.</param>
        public override void TranslateStatement(IForRangeStatement forRangeStatement)
        {
            this.TranslateConceptKeyword(Keywords.For);
            base.Write(" (", TranslatorFormatterTokenType.Operator);
            this.TranslateStatement(forRangeStatement.IterationIndex.GetDeclarationStatement());
            base.Write(";", TranslatorFormatterTokenType.Operator);
            IExpression iterIndRef = forRangeStatement.IterationIndex.GetReference();
            this.TranslateExpression(iterIndRef);
            base.Write(" <= ", TranslatorFormatterTokenType.Operator);
            this.TranslateExpression(forRangeStatement.Max);
            base.Write(";", TranslatorFormatterTokenType.Operator);
            this.TranslateExpression(iterIndRef);
            if (forRangeStatement.Step == null || forRangeStatement.Step is PrimitiveExpression && ((PrimitiveExpression)forRangeStatement.Step).TypeCode == TypeCode.Int32 && (int)((PrimitiveExpression)forRangeStatement.Step).Value == 1)
                base.Write("++",TranslatorFormatterTokenType.Operator);
            else
            {
                base.Write("+=", TranslatorFormatterTokenType.Operator);
                this.TranslateExpression(forRangeStatement.Step);
            }
            base.WriteLine(")", TranslatorFormatterTokenType.Operator);
            this.TranslateStatementBlock(forRangeStatement.Statements);
        }

        /// <summary>
        /// Translates an iteration statement which allows for a more complex variant of <see cref="TranslateStatement(IForRangeStatement)"/>.
        /// </summary>
        /// <param name="iterationStatement"></param>
        public override void TranslateStatement(IIterationStatement iterationStatement)
        {
            suppressLineTerminator = true;
            TranslateConceptKeyword(Keywords.For);
            base.Write("(", TranslatorFormatterTokenType.Operator);
            if (iterationStatement.InitializationStatement != null)
                TranslateStatement(iterationStatement.InitializationStatement);
            base.Write("; ", TranslatorFormatterTokenType.Operator);
            if (iterationStatement.TestExpression != null)
                TranslateExpression(iterationStatement.TestExpression);
            base.Write("; ", TranslatorFormatterTokenType.Operator);
            if (iterationStatement.IncrementStatement != null)
                TranslateStatement(iterationStatement.IncrementStatement);
            base.WriteLine(")", TranslatorFormatterTokenType.Operator);
            suppressLineTerminator = false;
            TranslateStatementBlock(iterationStatement.Statements);
        }

        /// <summary>
        /// Translates a local declaration statement.
        /// </summary>
        /// <param name="localDeclare">The <see cref="ILocalDeclarationStatement"/> which is to be defined 
        /// at the current point.</param>
        public override void TranslateStatement(ILocalDeclarationStatement localDeclare)
        {
            IStatementBlockLocalMember isblm = localDeclare.ReferencedMember;
            this.TranslateConceptTypeName(isblm.LocalType);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptIdentifier(isblm, true);
            if (isblm.InitializationExpression != null)
            {
                base.Write(" = ", TranslatorFormatterTokenType.Operator);
                this.TranslateExpression(isblm.InitializationExpression);
            }
            if (!suppressLineTerminator)
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates a method return statement which can exit the procedure on non-returning methods
        /// or yield a return value (<see cref="IReturnStatement.Result"/>).
        /// </summary>
        /// <param name="returnStatement">The <see cref="IReturnStatement"/> to translate.</param>
        public override void TranslateStatement(IReturnStatement returnStatement)
        {
            TranslateConceptKeyword(Keywords.Return);
            if (returnStatement.Result != null)
            {
                base.Write(" ", TranslatorFormatterTokenType.Other);
                this.TranslateExpression(returnStatement.Result);
            }
            if (!suppressLineTerminator)
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates a simple method call statement.
        /// </summary>
        /// <param name="callMethodStatement">The <see cref="ISimpleStatement"/> to translate.</param>
        public override void TranslateStatement(ISimpleStatement callMethodStatement)
        {
            this.TranslateExpression(callMethodStatement.Expression);
            if (!suppressLineTerminator)
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates a series of <see cref="IStatement"/> instances.
        /// </summary>
        /// <param name="statementBlock">The <see cref="IStatementBlock"/> to translate.</param>
        public override void TranslateStatementBlock(IStatementBlock statementBlock)
        {
            if (statementBlock.Parent is IBlockStatement || 
               (statementBlock.Parent is IConditionStatement) && 
               (statementBlock.Count != 1) || (!(statementBlock.Parent is IConditionStatement)))
            {
                base.WriteLine("{", TranslatorFormatterTokenType.Operator);
            }
            if (!(statementBlock.Parent is IBlockStatement))
                base.IncreaseIndent();
            base.TranslateStatementBlock(statementBlock);
            /* *
             * If someone has a label statement last, include a customary ';' to prevent 
             * compile errors.
             * */
            if ((statementBlock.Count > 0))
            {
                var ls = (statementBlock[statementBlock.Count - 1] as ILabelStatement);
                if (ls != null && !ls.Skip)
                    base.Target.WriteLine(";");
            }
            if (!(statementBlock.Parent is IBlockStatement))
                base.DecreaseIndent();
            if (statementBlock.Parent is IBlockStatement || (statementBlock.Parent is IConditionStatement) && (statementBlock.Count != 1) || (!(statementBlock.Parent is IConditionStatement)))
            {
                base.WriteLine("}", TranslatorFormatterTokenType.Operator);
            }
        }

        /// <summary>
        /// Translates the 'Partial' concept, if supported.
        /// </summary>
        /// <param name="seggedDecl">The segented declaration which needs the 'partial' keyword
        /// for that language translated to the <see cref="Target"/>.</param>
        public override void TranslateConceptPartial(ISegmentableDeclarationTarget seggedDecl)
        {
            if (!base.Options.AllowPartials)
                return;
            if ((seggedDecl.IsPartial) || (seggedDecl.IsRoot && seggedDecl.Partials.Count > 0))
            {
                TranslateConceptKeyword(Keywords.Partial);
                base.Write(" ", TranslatorFormatterTokenType.Other);
            }
        }

        public override void TranslateConceptAccessModifiers(IDeclaration decl)
        {
            //Pretty basic, if the base.target is a namespace translate family accessors 
            //to assembly, and non-public accessors to assembly.
            switch (decl.AccessLevel)
            {
                case DeclarationAccessLevel.Public:
                    TranslateConceptKeyword(Keywords.Public);
                    break;
                case DeclarationAccessLevel.Private:
                    if (decl.ParentTarget is INameSpaceDeclaration || decl.ParentTarget is IIntermediateProject)
                        TranslateConceptKeyword(Keywords.Internal);
                    else
                        TranslateConceptKeyword(Keywords.Private);
                    break;
                case DeclarationAccessLevel.Internal:
                    TranslateConceptKeyword(Keywords.Internal);
                    break;
                case DeclarationAccessLevel.Protected:
                    if (decl.ParentTarget is INameSpaceDeclaration || decl.ParentTarget is IIntermediateProject)
                    {
                        TranslateConceptKeyword(Keywords.Internal);
                    }
                    else
                    {
                        TranslateConceptKeyword(Keywords.Protected);
                    }
                    break;
                case DeclarationAccessLevel.ProtectedInternal:
                    if (decl.ParentTarget is INameSpaceDeclaration || decl.ParentTarget is IIntermediateProject)
                        TranslateConceptKeyword(Keywords.Internal);
                    else
                    {
                        TranslateConceptKeyword(Keywords.Protected);
                        base.Write(" ", TranslatorFormatterTokenType.Other);
                        TranslateConceptKeyword(Keywords.Internal);
                    }
                    break;
                default:
                    if (decl.ParentTarget is INameSpaceDeclaration || decl.ParentTarget is IIntermediateProject)
                        TranslateConceptKeyword(Keywords.Internal);
                    else
                        TranslateConceptKeyword(Keywords.Private);
                    break;
            }
            base.Write(" ", TranslatorFormatterTokenType.Other);
        }

        public override void TranslateStatement(IBreakTargetExitPoint breakTarget)
        {
            //Breaks don't need targets in C#.
            if (!breakTarget.Skip)
                TranslateStatement((ILabelStatement)breakTarget);
            return;
        }

        /// <summary>
        /// Translates the 'NotInstantiable'/'abstract' concept.
        /// </summary>
        /// <param name="classType"></param>
        public override void TranslateConceptNotInstantiableClass(IClassType classType)
        {
            if (classType.IsAbstract)
            {
                TranslateConceptKeyword(Keywords.Abstract);
                base.Write(" ", TranslatorFormatterTokenType.Other);
            }
        }

        /// <summary>
        /// Translates the 'NotInheritable'/'sealed' concept.
        /// </summary>
        /// <param name="classType">The <see cref="IClassType"/> which is not inheritable, i.e. sealed.</param>
        public override void TranslateConceptNotInheritableClass(IClassType classType)
        {
            if (classType.IsSealed)
            {
                TranslateConceptKeyword(Keywords.Sealed);
                base.Write(" ", TranslatorFormatterTokenType.Other);
            }
        }

        /// <summary>
        /// Translates the static concept
        /// </summary>
        /// <param name="classType"><para>In C# this is a 'static' class.</para></param>
        public override void TranslateConceptNotInheritableOrInstantiableClass(IClassType classType)
        {
            if (classType.IsStatic)
            {
                TranslateConceptKeyword(Keywords.Static);
                base.Write(" ", TranslatorFormatterTokenType.Other);
            }
        }

        public override sealed bool IsKeyword(string identifier)
        {
            int length = identifier.Length;
            if (!keywordLookup.ContainsKey(identifier.Length))
                return false;
            return keywordLookup[length].ContainsKey(identifier.GetHashCode());
        }

        private static void InitializeLookup(int length)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            switch (length)
            {
                case 2:
                    //Two letter...
                    result.Add((int)Keywords.As, Resources.CSharpKeyWord_02_As);
                    result.Add((int)Keywords.Do, Resources.CSharpKeyWord_02_Do);
                    result.Add((int)Keywords.If, Resources.CSharpKeyWord_02_If);
                    result.Add((int)Keywords.In, Resources.CSharpKeyWord_02_In);
                    result.Add((int)Keywords.Is, Resources.CSharpKeyWord_02_Is);
                    break;
                case 3:
                    //Three letter...
                    result.Add((int)Keywords.For, Resources.CSharpKeyWord_03_For);
                    result.Add((int)Keywords.Get, Resources.CSharpKeyWord_03_Get);
                    result.Add((int)Keywords.Int, Resources.CSharpKeyWord_03_Int);
                    result.Add((int)Keywords.New, Resources.CSharpKeyWord_03_New);
                    result.Add((int)Keywords.Out, Resources.CSharpKeyWord_03_Out);
                    result.Add((int)Keywords.Ref, Resources.CSharpKeyWord_03_Ref);
                    result.Add((int)Keywords.Set, Resources.CSharpKeyWord_03_Set);
                    result.Add((int)Keywords.Try, Resources.CSharpKeyWord_03_Try);
                    break;
                case 4:
                    //Four letter...
                    result.Add((int)Keywords.Base, Resources.CSharpKeyWord_04_Base);
                    result.Add((int)Keywords.Bool, Resources.CSharpKeyWord_04_Bool);
                    result.Add((int)Keywords.Byte, Resources.CSharpKeyWord_04_Byte);
                    result.Add((int)Keywords.Case, Resources.CSharpKeyWord_04_Case);
                    result.Add((int)Keywords.Char, Resources.CSharpKeyWord_04_Char);
                    result.Add((int)Keywords.Else, Resources.CSharpKeyWord_04_Else);
                    result.Add((int)Keywords.Enum, Resources.CSharpKeyWord_04_Enum);
                    result.Add((int)Keywords.Goto, Resources.CSharpKeyWord_04_Goto);
                    result.Add((int)Keywords.Lock, Resources.CSharpKeyWord_04_Lock);
                    result.Add((int)Keywords.Long, Resources.CSharpKeyWord_04_Long);
                    result.Add((int)Keywords.Null, Resources.CSharpKeyWord_04_Null);
                    result.Add((int)Keywords.This, Resources.CSharpKeyWord_04_This);
                    result.Add((int)Keywords.True, Resources.CSharpKeyWord_04_True);
                    result.Add((int)Keywords.Uint, Resources.CSharpKeyWord_04_Uint);
                    result.Add((int)Keywords.Void, Resources.CSharpKeyWord_04_Void);
                    break;
                case 5:
                    //Five letter...
                    result.Add((int)Keywords.Break, Resources.CSharpKeyWord_05_Break);
                    result.Add((int)Keywords.Catch, Resources.CSharpKeyWord_05_Catch);
                    result.Add((int)Keywords.Class, Resources.CSharpKeyWord_05_Class);
                    result.Add((int)Keywords.Const, Resources.CSharpKeyWord_05_Const);
                    result.Add((int)Keywords.Event, Resources.CSharpKeyWord_05_Event);
                    result.Add((int)Keywords.False, Resources.CSharpKeyWord_05_False);
                    result.Add((int)Keywords.Fixed, Resources.CSharpKeyWord_05_Fixed);
                    result.Add((int)Keywords.Float, Resources.CSharpKeyWord_05_Float);
                    result.Add((int)Keywords.Sbyte, Resources.CSharpKeyWord_05_Sbyte);
                    result.Add((int)Keywords.Short, Resources.CSharpKeyWord_05_Short);
                    result.Add((int)Keywords.Throw, Resources.CSharpKeyWord_05_Throw);
                    result.Add((int)Keywords.Ulong, Resources.CSharpKeyWord_05_Ulong);
                    result.Add((int)Keywords.Using, Resources.CSharpKeyWord_05_Using);
                    result.Add((int)Keywords.Where, Resources.CSharpKeyWord_05_Where);
                    result.Add((int)Keywords.While, Resources.CSharpKeyWord_05_While);
                    result.Add((int)Keywords.Yield, Resources.CSharpKeyWord_05_Yield);
                    break;
                case 6:
                    //Six letter...
                    result.Add((int)Keywords.Double, Resources.CSharpKeyWord_06_Double);
                    result.Add((int)Keywords.Extern, Resources.CSharpKeyWord_06_Extern);
                    result.Add((int)Keywords.Object, Resources.CSharpKeyWord_06_Object);
                    result.Add((int)Keywords.Params, Resources.CSharpKeyWord_06_Params);
                    result.Add((int)Keywords.Public, Resources.CSharpKeyWord_06_Public);
                    result.Add((int)Keywords.Return, Resources.CSharpKeyWord_06_Return);
                    result.Add((int)Keywords.Sealed, Resources.CSharpKeyWord_06_Sealed);
                    result.Add((int)Keywords.Sizeof, Resources.CSharpKeyWord_06_Sizeof);
                    result.Add((int)Keywords.Static, Resources.CSharpKeyWord_06_Static);
                    result.Add((int)Keywords.String, Resources.CSharpKeyWord_06_String);
                    result.Add((int)Keywords.Struct, Resources.CSharpKeyWord_06_Struct);
                    result.Add((int)Keywords.Switch, Resources.CSharpKeyWord_06_Switch);
                    result.Add((int)Keywords.Typeof, Resources.CSharpKeyWord_06_Typeof);
                    result.Add((int)Keywords.Unsafe, Resources.CSharpKeyWord_06_Unsafe);
                    result.Add((int)Keywords.Ushort, Resources.CSharpKeyWord_06_Ushort);
                    break;
                case 7:
                    //Seven letter...
                    result.Add((int)Keywords.Checked, Resources.CSharpKeyWord_07_Checked);
                    result.Add((int)Keywords.Decimal, Resources.CSharpKeyWord_07_Decimal);
                    result.Add((int)Keywords.Default, Resources.CSharpKeyWord_07_Default);
                    result.Add((int)Keywords.Finally, Resources.CSharpKeyWord_07_Finally);
                    result.Add((int)Keywords.Foreach, Resources.CSharpKeyWord_07_Foreach);
                    result.Add((int)Keywords.Partial, Resources.CSharpKeyWord_07_Partial);
                    result.Add((int)Keywords.Private, Resources.CSharpKeyWord_07_Private);
                    result.Add((int)Keywords.Virtual, Resources.CSharpKeyWord_07_Virtual);
                    break;
                case 8:
                    //Eight Letter...
                    result.Add((int)Keywords.Abstract, Resources.CSharpKeyWord_08_Abstract);
                    result.Add((int)Keywords.Continue, Resources.CSharpKeyWord_08_Continue);
                    result.Add((int)Keywords.Delegate, Resources.CSharpKeyWord_08_Delegate);
                    result.Add((int)Keywords.Explicit, Resources.CSharpKeyWord_08_Explicit);
                    result.Add((int)Keywords.Implicit, Resources.CSharpKeyWord_08_Implicit);
                    result.Add((int)Keywords.Internal, Resources.CSharpKeyWord_08_Internal);
                    result.Add((int)Keywords.Operator, Resources.CSharpKeyWord_08_Operator);
                    result.Add((int)Keywords.Override, Resources.CSharpKeyWord_08_Override);
                    result.Add((int)Keywords.Readonly, Resources.CSharpKeyWord_08_Readonly);
                    result.Add((int)Keywords.Volatile, Resources.CSharpKeyWord_08_Volatile);
                    break;
                case 9:
                    //Nine Letter...
                    result.Add((int)Keywords.__Arglist, Resources.CSharpKeyWord_09___arglist);
                    result.Add((int)Keywords.__Makeref, Resources.CSharpKeyWord_09___makeref);
                    result.Add((int)Keywords.__Reftype, Resources.CSharpKeyWord_09___reftype);
                    result.Add((int)Keywords.Interface, Resources.CSharpKeyWord_09_Interface);
                    result.Add((int)Keywords.Namespace, Resources.CSharpKeyWord_09_Namespace);
                    result.Add((int)Keywords.Protected, Resources.CSharpKeyWord_09_Protected);
                    result.Add((int)Keywords.Unchecked, Resources.CSharpKeyWord_09_Unchecked);
                    break;
                case 10:
                    //Ten Letter...
                    result.Add((int)Keywords.__Refvalue, Resources.CSharpKeyWord_10___refvalue);
                    result.Add((int)Keywords.Stackalloc, Resources.CSharpKeyWord_10_Stackalloc);
                    break;
                default:
                    result = null;
                    //The lookup  isn't valid.
                    return;
            }
            keywordLookup.Add(length, result);
        }


        public override string EscapeIdentifier(string identifier)
        {
            return string.Format("@{0}", identifier);
        }

        public override void TranslateParameters<TParameter, TParameteredDom, TParent>(IParameteredParameterMembers<TParameter, TParameteredDom, TParent> parameterMembers)
        {
            if (parameterMembers.TargetDeclaration is IIndexerSignatureMember || parameterMembers.TargetDeclaration is IIndexerMember)
                base.Write("[", TranslatorFormatterTokenType.Operator);
            else
                base.Write("(", TranslatorFormatterTokenType.Operator);
            bool flag = true;
            base.IncreaseIndent();
            foreach (IParameteredParameterMember<TParameter, TParameteredDom, TParent> tp in parameterMembers.Values)
            {
                if (tp.Attributes != null && tp.Attributes.Count > 0)
                {
                    if (flag)
                    {
                        base.WriteLine();
                        flag = false;
                    }
                    else
                        base.WriteLine(", ", TranslatorFormatterTokenType.Operator);
                }
                else if (flag)
                    flag = false;
                else
                    base.Write(", ", TranslatorFormatterTokenType.Operator);

                this.TranslateMember(tp);
            }
            base.DecreaseIndent();
            if (parameterMembers.TargetDeclaration is IIndexerSignatureMember || parameterMembers.TargetDeclaration is IIndexerMember)
                base.Write("]", TranslatorFormatterTokenType.Operator);
            else
                base.Write(")", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateTypeParameters(ITypeParameterMembers typeParameterMembers)
        {
            bool flag = true;
            base.Write("<", TranslatorFormatterTokenType.Operator);
            foreach (ITypeParameterMember itpm in typeParameterMembers.Values)
            {
                if (flag)
                    flag = false;
                else
                    base.Write(", ", TranslatorFormatterTokenType.Operator);
                this.TranslateMember(itpm);
            }
            base.Write(">", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateExpression(IArrayIndexerExpression arrayIndexerExpression)
        {
            this.TranslateExpression(arrayIndexerExpression.Reference);
            base.Write("[", TranslatorFormatterTokenType.Operator);
            base.IncreaseIndent();
            base.WriteLine();
            base.DecreaseIndent();
            this.TranslateExpressionGroup(arrayIndexerExpression.Indices);
            base.Write("]", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateExpression(IUnaryOperationExpression unOpExpression)
        {
            bool parenthesis = true;
            switch (unOpExpression.Operation)
            {
                case UnaryOperations.Negate:
                    base.Write("-", TranslatorFormatterTokenType.Operator);
                    break;
                case UnaryOperations.Plus:
                    base.Write("+", TranslatorFormatterTokenType.Operator);
                    break;
                case UnaryOperations.Indirection:
                    base.Write("*", TranslatorFormatterTokenType.Operator);
                    break;
                case UnaryOperations.AddressOf:
                    base.Write("&", TranslatorFormatterTokenType.Operator);
                    break;
                case UnaryOperations.LogicalNot:
                    base.Write("!", TranslatorFormatterTokenType.Operator);
                    break;
                case UnaryOperations.Compliment:
                    base.Write("~", TranslatorFormatterTokenType.Operator);
                    break;
                case UnaryOperations.PrefixIncrement:
                    base.Write("++", TranslatorFormatterTokenType.Operator);
                    parenthesis = false;
                    break;
                case UnaryOperations.PrefixDecrement:
                    base.Write("--", TranslatorFormatterTokenType.Operator);
                    parenthesis = false;
                    break;
                case UnaryOperations.SizeOf:
                    base.Write("sizeof", TranslatorFormatterTokenType.Operator);
                    break;
                default:
                    break;
            }
            if (parenthesis)
                base.Write("(", TranslatorFormatterTokenType.Operator);
            this.TranslateExpression(unOpExpression.TargetExpression);
            if (parenthesis)
                base.Write(")", TranslatorFormatterTokenType.Operator);

        }

        public override void TranslateExpression(IBinaryOperationExpression binOpExpression)
        {
            base.Write("(", TranslatorFormatterTokenType.Operator);
            this.TranslateExpression(binOpExpression.LeftSide);
            switch (binOpExpression.Operation)
            {
                case CodeBinaryOperatorType.Add:
                    base.Write(" + ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.Assign:
                    base.Write(" = ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.BitwiseAnd:
                    base.Write(" & ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.BitwiseOr:
                    base.Write(" | ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.BooleanAnd:
                    base.Write(" && ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.BooleanOr:
                    base.Write(" || ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.Divide:
                    base.Write(" / ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.GreaterThan:
                    base.Write(" > ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.GreaterThanOrEqual:
                    base.Write(" >= ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.IdentityEquality:
                case CodeBinaryOperatorType.ValueEquality:
                    base.Write(" == ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.IdentityInequality:
                    base.Write(" != ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.LessThan:
                    base.Write(" < ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.LessThanOrEqual:
                    base.Write(" <= ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.Modulus:
                    base.Write(" % ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.Multiply:
                    base.Write(" * ", TranslatorFormatterTokenType.Operator);
                    break;
                case CodeBinaryOperatorType.Subtract:
                    base.Write(" - ", TranslatorFormatterTokenType.Operator);
                    break;
                default:
                    break;
            }
            this.TranslateExpression(binOpExpression.RightSide);
            base.Write(")", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateExpression(ICastExpression castExpression)
        {
            base.Write("((", TranslatorFormatterTokenType.Operator);
            this.TranslateConceptTypeName(castExpression.Type);
            base.Write(")(", TranslatorFormatterTokenType.Operator);
            this.TranslateExpression(castExpression.Target);
            base.Write("))", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateExpression(ICreateArrayExpression expression)
        {
            TranslateConceptKeyword(Keywords.New);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptTypeName(expression.ArrayType);
            base.Write("[", TranslatorFormatterTokenType.Operator);
            if (expression.SizeExpression != null)
                this.TranslateExpression(expression.SizeExpression);
            else if ((expression.Initializers == null) || (expression.Initializers.Count == 0))
                base.Write("0", TranslatorFormatterTokenType.Number);
            base.Write("]", TranslatorFormatterTokenType.Operator);
            if ((expression.Initializers != null) && (expression.Initializers.Count > 0))
            {
                base.IncreaseIndent();
                base.WriteLine();
                base.WriteLine("{", TranslatorFormatterTokenType.Operator);
                this.TranslateExpressionGroup(expression.Initializers);
                base.WriteLine();
                base.Write("}", TranslatorFormatterTokenType.Operator);
                base.DecreaseIndent();
            }
        }

        public override void TranslateExpression(ICreateNewObjectExpression expression)
        {
            TranslateConceptKeyword(Keywords.New);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateConceptTypeName(expression.NewType);
            base.Write("(", TranslatorFormatterTokenType.Operator);
            this.TranslateExpressionGroup(expression.Arguments);
            base.Write(")", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateExpression(IDirectionExpression directionExpression)
        {
            switch (directionExpression.Direction)
            {
                case FieldDirection.Out:
                    this.TranslateConceptKeyword(Keywords.Out);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    break;
                case FieldDirection.Ref:
                    this.TranslateConceptKeyword(Keywords.Ref);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    break;
                case FieldDirection.In:
                default:
                    break;
            }
            TranslateExpression(directionExpression.DirectedExpression);
        }

        public override void TranslateExpression(IEventReferenceExpression eventReferenceExpression)
        {
            this.TranslateExpression(eventReferenceExpression.Reference);
            this.TranslateConceptIdentifier(eventReferenceExpression.Name, TranslatorFormatterMemberType.Event);
        }

        public override void TranslateExpression(IFieldReferenceExpression fieldRefExpression)
        {
            if (fieldRefExpression.Reference is ITypeReferenceExpression && ((ITypeReferenceExpression)(fieldRefExpression.Reference)).TypeReference is IDeclaredTypeReference)
            {
                IDeclaredType idt = ((IDeclaredTypeReference)(((ITypeReferenceExpression)(fieldRefExpression.Reference)).TypeReference)).TypeInstance;
                if (idt is ISegmentableDeclaredType)
                    idt = ((ISegmentableDeclaredType)(idt)).GetRootDeclaration();
                if (!base.Options.BuildTrail.Contains(idt))
                {
                    this.TranslateExpression(fieldRefExpression.Reference);
                    base.Write(".", TranslatorFormatterTokenType.Operator);
                }
            }
            else
            {
                this.TranslateExpression(fieldRefExpression.Reference);
                base.Write(".", TranslatorFormatterTokenType.Operator);
            }
            if (fieldRefExpression is FieldMember.ReferenceExpression)
                this.TranslateConceptIdentifier(((FieldMember.ReferenceExpression)fieldRefExpression).referencePoint, false);
            else
                this.TranslateConceptIdentifier(fieldRefExpression.Name, TranslatorFormatterMemberType.Field);
        }

        public override void TranslateExpression(IGetResourceExpression getResourceExpression)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void TranslateExpression(IIndexerReferenceExpression indexerRefExpression)
        {
            this.TranslateExpression(indexerRefExpression.Reference);
            base.Write("[", TranslatorFormatterTokenType.Operator);
            this.TranslateExpressionGroup(indexerRefExpression.Indices);
            base.Write("]", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateExpression(ILocalReferenceExpression localRefExpression)
        {
            if (localRefExpression is StatementBlockLocalMember.ReferenceExpression)
                this.TranslateConceptIdentifier(((StatementBlockLocalMember.ReferenceExpression)localRefExpression).referencePoint, false);
            else
                this.TranslateConceptIdentifier(localRefExpression.Name, TranslatorFormatterMemberType.Local);
        }

        public override void TranslateExpression(IMethodInvokeExpression methodInvExpression)
        {
            this.TranslateExpression(methodInvExpression.Reference);
            base.Write("(", TranslatorFormatterTokenType.Operator);
            this.TranslateExpressionGroup(methodInvExpression.ArgumentExpressions);
            base.Write(")", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateExpression(IMethodReferenceExpression methodRefExpression)
        {
            bool includeReference = true;
            if (methodRefExpression.Reference is ITypeReferenceExpression && ((ITypeReferenceExpression)(methodRefExpression.Reference)).TypeReference is IDeclaredTypeReference)
            {
                IDeclaredType idt = ((IDeclaredTypeReference)(((ITypeReferenceExpression)(methodRefExpression.Reference)).TypeReference)).TypeInstance;
                if (idt is ISegmentableDeclaredType)
                    idt = ((ISegmentableDeclaredType)(idt)).GetRootDeclaration();
                if (base.Options.BuildTrail.Contains(idt))
                    includeReference = false;
            }
            if (methodRefExpression is MethodMember.ReferenceExpression)
            {
                if (includeReference)
                {
                    MethodMember.ReferenceExpression mmre = methodRefExpression as MethodMember.ReferenceExpression;
                    if ((mmre.referencePoint.PrivateImplementationTarget == null) || (mmre.Reference is ICastExpression))
                    {
                        this.TranslateExpression(methodRefExpression.Reference);
                        base.Write(".", TranslatorFormatterTokenType.Operator);
                    }
                    else
                    {
                        this.TranslateExpression(new CastExpression(methodRefExpression.Reference, mmre.referencePoint.PrivateImplementationTarget));
                        base.Write(".", TranslatorFormatterTokenType.Operator);
                    }
                }
                this.TranslateConceptIdentifier(((MethodMember.ReferenceExpression)methodRefExpression).referencePoint, false);
            }
            else
            {
                if (includeReference)
                {
                    this.TranslateExpression(methodRefExpression.Reference);
                    base.Write(".", TranslatorFormatterTokenType.Operator);
                }
                this.TranslateConceptIdentifier(methodRefExpression.Name, TranslatorFormatterMemberType.Method);
            }
            if (methodRefExpression.TypeArguments != null && methodRefExpression.TypeArguments.Count > 0)
            {
                base.Write("<", TranslatorFormatterTokenType.Operator);
                this.TranslateTypeReferenceCollection(methodRefExpression.TypeArguments);
                base.Write(">", TranslatorFormatterTokenType.Operator);
            }
        }

        public override void TranslateExpression(IParameterReferenceExpression paramRefExpression)
        {
            if (paramRefExpression is MethodParameterMember.ReferenceExpression)
                this.TranslateConceptIdentifier(((MethodParameterMember.ReferenceExpression)paramRefExpression).referencePoint, false);
            else if (paramRefExpression is IndexerParameterMember.ReferenceExpression)
                this.TranslateConceptIdentifier(((IndexerParameterMember.ReferenceExpression)paramRefExpression).referencePoint, false);
            else
                this.TranslateConceptIdentifier(paramRefExpression.Name, TranslatorFormatterMemberType.Parameter);
        }

        public override void TranslateExpression(IPrimitiveExpression primitiveExpression)
        {
            switch (primitiveExpression.TypeCode)
            {
                case TypeCode.Boolean:
                    bool b = (bool)primitiveExpression.Value;
                    if (b)
                        this.TranslateConceptKeyword(Keywords.True);
                    else
                        this.TranslateConceptKeyword(Keywords.False);
                    break;
                case TypeCode.Byte:
                    base.Write("((", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Byte);
                    base.Write(")(", TranslatorFormatterTokenType.Operator);
                    base.Write(primitiveExpression.Value.ToString(), TranslatorFormatterTokenType.Number);
                    base.Write(")", TranslatorFormatterTokenType.Operator);
                    break;
                case TypeCode.Char:
                    base.Write("'", TranslatorFormatterTokenType.String);
                    switch ((char)primitiveExpression.Value)
                    {
                        case '\\':
                            base.Write("\\\\", TranslatorFormatterTokenType.String);
                            break;
                        case '\0':
                            base.Write("\\0", TranslatorFormatterTokenType.String);
                            break;
                        case '\n':
                            base.Write("\\n", TranslatorFormatterTokenType.String);
                            break;
                        case '\r':
                            base.Write("\\r", TranslatorFormatterTokenType.String);
                            break;
                        case '\'':
                            base.Write("\\'", TranslatorFormatterTokenType.String);
                            break;
                        case '\f':
                            base.Write("\\f", TranslatorFormatterTokenType.String);
                            break;
                        case '\v':
                            base.Write("\\v", TranslatorFormatterTokenType.String);
                            break;
                        case '\t':
                            base.Write("\\t", TranslatorFormatterTokenType.String);
                            break;
                        case '\x85':
                            base.Write("\\x85", TranslatorFormatterTokenType.String);
                            break;
                        default:
                            if (((char)(primitiveExpression.Value)) > 255)
                            {
                                var baseHexVal = string.Format("{0:x}", (int)((char)primitiveExpression.Value));
                                while (baseHexVal.Length < 4)
                                    baseHexVal = "0" + baseHexVal;
                                base.Write(string.Format("\\u{0}", baseHexVal), TranslatorFormatterTokenType.String);
                            }
                            else
                                base.Write(primitiveExpression.Value.ToString(), TranslatorFormatterTokenType.String);
                            break;
                    }
                    base.Write("'", TranslatorFormatterTokenType.String);
                    break;
                case TypeCode.Double:
                    base.Write(string.Format("{0:E}", primitiveExpression.Value.ToString()), TranslatorFormatterTokenType.Number);
                    break;
                case TypeCode.Empty:
                    TranslateConceptKeyword(Keywords.Null);
                    break;
                case TypeCode.Int16:
                    base.Write("((", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Short);
                    base.Write(")(", TranslatorFormatterTokenType.Operator);
                    base.Write(primitiveExpression.Value.ToString(), TranslatorFormatterTokenType.Number);
                    base.Write(")", TranslatorFormatterTokenType.Operator);
                    break;
                case TypeCode.Int32:
                    base.Write(primitiveExpression.Value.ToString(), TranslatorFormatterTokenType.Number);
                    break;
                case TypeCode.Int64:
                    base.Write(string.Format("{0}L", primitiveExpression.Value.ToString()), TranslatorFormatterTokenType.Number);
                    break;
                case TypeCode.SByte:
                    base.Write("((", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Sbyte);
                    base.Write(")(", TranslatorFormatterTokenType.Operator);
                    base.Write(primitiveExpression.Value.ToString(), TranslatorFormatterTokenType.Number);
                    base.Write(")", TranslatorFormatterTokenType.Operator);
                    break;
                case TypeCode.Single:
                    base.Write(string.Format("{0:G}f", primitiveExpression.Value.ToString()),TranslatorFormatterTokenType.Number);
                    break;
                case TypeCode.String:
                    base.Write(this.EscapeString((string)primitiveExpression.Value, base.Target.Indent),TranslatorFormatterTokenType.String);
                    break;
                case TypeCode.UInt16:
                    base.Write("((", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.Ushort);
                    base.Write(")(", TranslatorFormatterTokenType.Operator);
                    base.Write(primitiveExpression.Value.ToString(), TranslatorFormatterTokenType.Number);
                    base.Write(")", TranslatorFormatterTokenType.Operator);
                    break;
                case TypeCode.UInt32:
                    base.Write(primitiveExpression.Value.ToString(), TranslatorFormatterTokenType.Number);
                    break;
                case TypeCode.UInt64:
                    base.Write(string.Format("{0}L", primitiveExpression.Value.ToString()), TranslatorFormatterTokenType.Number);
                    break;
                case TypeCode.Object:
                default:
                    break;
            }
        }

        public override void TranslateExpression(IPropertyReferenceExpression propRefExpression)
        {
            bool includeReference = true;
            if (propRefExpression.Reference is ITypeReferenceExpression && ((ITypeReferenceExpression)(propRefExpression.Reference)).TypeReference is IDeclaredTypeReference)
            {
                IDeclaredType idt = ((IDeclaredTypeReference)(((ITypeReferenceExpression)(propRefExpression.Reference)).TypeReference)).TypeInstance;
                if (idt is ISegmentableDeclaredType)
                    idt = ((ISegmentableDeclaredType)(idt)).GetRootDeclaration();
                if (base.Options.BuildTrail.Contains(idt))
                    includeReference = false;
            }
            if (propRefExpression is PropertyMember.ReferenceExpression)
            {
                PropertyMember.ReferenceExpression mmre = propRefExpression as PropertyMember.ReferenceExpression;
                if (includeReference)
                {
                    if ((mmre.referencePoint.PrivateImplementationTarget == null) || (mmre.Reference is ICastExpression))
                    {
                        this.TranslateExpression(propRefExpression.Reference);
                        base.Write(".", TranslatorFormatterTokenType.Operator);
                    }
                    else
                    {
                        this.TranslateExpression(new CastExpression(propRefExpression.Reference, mmre.referencePoint.PrivateImplementationTarget));
                        base.Write(".", TranslatorFormatterTokenType.Operator);
                    }
                }
                this.TranslateConceptIdentifier(((PropertyMember.ReferenceExpression)propRefExpression).referencePoint, false);
            }
            else
            {
                if (includeReference)
                {
                    this.TranslateExpression(propRefExpression.Reference);
                    base.Write(".", TranslatorFormatterTokenType.Operator);
                }
                this.TranslateConceptIdentifier(propRefExpression.Name, TranslatorFormatterMemberType.Property);
            }
        }

        public override sealed void TranslateExpression(IPropertySetValueReferenceExpression propSetValRefExpression)
        {
            base.Write("value", TranslatorFormatterTokenType.Keyword);
        }

        public override void TranslateExpression(IThisReferenceExpression thisRefExpression)
        {
            this.TranslateConceptKeyword(Keywords.This);
        }

        public override void TranslateExpression(ITypeOfExpression typeOfExpression)
        {
            this.TranslateConceptKeyword(Keywords.Typeof);
            base.Write("(", TranslatorFormatterTokenType.Operator);
            this.TranslateConceptTypeName(typeOfExpression.TypeReference);
            base.Write(")", TranslatorFormatterTokenType.Operator);
            if (typeOfExpression.TypeReference.ByRef)
            {
                base.Write(".", TranslatorFormatterTokenType.Operator);
                base.Write("MakeByRefType", TranslatorFormatterMemberType.Method, typeof(Type).GetTypeReference().TypeInstance);
                base.Write("()", TranslatorFormatterTokenType.Operator);
            }
        }

        public override void TranslateExpression(ITypeReferenceExpression typeRefExpression)
        {
            this.TranslateConceptTypeName(typeRefExpression.TypeReference);
        }

        public override void TranslateExpressionGroup(IExpressionCollection expressions)
        {
            bool flag = true;
            if (expressions.Count == 0)
                return;
            base.IncreaseIndent();
            int index = 0;
            foreach (IExpression iexp in expressions)
            {
                if (flag)
                    flag = false;
                else if (index % 3 == 0)
                    base.WriteLine(", ", TranslatorFormatterTokenType.Operator);
                else
                    base.Write(", ", TranslatorFormatterTokenType.Operator);
                this.TranslateExpression(iexp);
                index++;
            }
            base.DecreaseIndent();
        }

        public override void TranslateTypeReferenceCollection(ITypeReferenceCollection typeRefCol)
        {
            bool flag = true;
            foreach (ITypeReference itr in typeRefCol)
            {
                if (flag)
                    flag = false;
                else
                    base.Write(", ", TranslatorFormatterTokenType.Operator);
                this.TranslateConceptTypeName(itr);
            }
        }

        public override string EscapeString(string value, int indentLevel)
        {
            StringBuilder sb = new StringBuilder((int)((float)(value.Length + 8) * 1.1));
            sb.Append(@"""");
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                //bool b = (i == (value.Length - 1));
                switch (c)
                {
                    case '"':
                        sb.Append(@"\""");
                        break;
                    case '\\':
                        sb.Append(@"\\");
                        break;
                    case '\r':
                        sb.Append(@"\r");
                        break;
                    case '\n':
                        sb.Append(@"\n");
                        break;
                    case '\0':
                        sb.Append(@"\0");
                        break;
                    case '\u2028':
                        sb.Append(@"\u2028");
                        break;
                    case '\u2029':
                        sb.Append(@"\u2029");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            sb.Append(@"""");
            return sb.ToString();
        }

        private bool TranslateConceptTypeName(Type type)
        {
            if (_OIL._Core.AutoFormTypes.Contains(type))
            {
                TranslateConceptKeyword(CSharpAutoFormTypeLookup[type]);
                return true;
            }
            return false;
        }

        public override void TranslateConceptTypeName(ITypeReference typeRef)
        {
            if (!(typeRef.ArrayElementType == null || typeRef.ArrayRank == 0))
            {
                this.TranslateConceptTypeName(typeRef.ArrayElementType);
                base.Write("[", TranslatorFormatterTokenType.Operator);
                bool flag = true;
                for (int i = 0; i < typeRef.ArrayRank; i++)
                    if (flag)
                        flag = false;
                    else
                        base.Write(",", TranslatorFormatterTokenType.Operator);
                base.Write("]", TranslatorFormatterTokenType.Operator);
            }
            else
            {
                base.TranslateConceptTypeName(typeRef);
                if (typeRef.Nullable)
                    base.Write("?", TranslatorFormatterTokenType.Operator);
                if (typeRef.PointerRank > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append('*', typeRef.PointerRank);
                    base.Write(sb.ToString(), TranslatorFormatterTokenType.Operator);
                }
            }
        }

        public override sealed void TranslateConceptTypeName(IExternTypeReference typeRef, ITypeReferenceCollection typeParameters)
        {
            IExternType type = typeRef.TypeInstance;
            if (TranslateConceptTypeName(type.Type))
                return;
            List<ITypeReference> typeParams = new List<ITypeReference>(typeParameters);
            typeParams.Reverse();
            Stack<ITypeReference> typeParamsStack = new Stack<ITypeReference>(typeParams);

            List<Type> hierarchy = new List<Type>();

            for (Type t = type.Type; t != null; t = t.DeclaringType)
                hierarchy.Add(t);
            hierarchy.Reverse();
            if (((typeRef.ResolutionOptions & TypeReferenceResolveOptions.GlobalType) == TypeReferenceResolveOptions.GlobalType))
                base.Write("global::", TranslatorFormatterTokenType.Keyword);
            if (((typeRef.ResolutionOptions & TypeReferenceResolveOptions.FullType) == TypeReferenceResolveOptions.FullType) || !base.Options.AutoResolveReferences || ((typeRef.ResolutionOptions & TypeReferenceResolveOptions.GlobalType) == TypeReferenceResolveOptions.GlobalType))
                base.Write(string.Format("{0}.", type.Type.Namespace), TranslatorFormatterTokenType.NameSpace);
            Dictionary<Type, List<Type>> genParams = new Dictionary<Type, List<Type>>();
            bool flag = true;
            foreach (Type t in hierarchy)
            {
                genParams[t] = new List<Type>();
                if (t.IsGenericType)
                    genParams[t].AddRange(t.GetGenericArguments());
                if (t.DeclaringType != null)
                    //Children include their parents' type-arguments.
                    for (Type t3 = t.DeclaringType; t3 != null; t3 = t3.DeclaringType)
                        foreach (Type t2 in genParams[t3])
                            genParams[t].RemoveAt(0);
                string tName = t.Name;
                if (tName.Contains("`"))
                    tName = tName.Substring(0, tName.IndexOf('`'));
                if (flag)
                    flag = false;
                else
                    base.Write(".", TranslatorFormatterTokenType.Operator);

                this.TranslateConceptIdentifier(tName, (ExternType)t);
                if (genParams[t].Count > 0)
                {
                    base.Write("<", TranslatorFormatterTokenType.Operator);
                    bool flag2 = true;
                    foreach (Type t2 in genParams[t])
                    {
                        if (flag2)
                            flag2 = false;
                        else
                            base.Write(", ", TranslatorFormatterTokenType.Operator);
                        if (typeParamsStack.Count > 0)
                            this.TranslateConceptTypeName(typeParamsStack.Pop());
                    }
                    base.Write(">", TranslatorFormatterTokenType.Operator);
                }
            }
        }

        public override sealed void TranslateConceptTypeName(IDeclaredTypeReference typeReference, ITypeReferenceCollection typeParameters)
        {
            List<IDeclaredType> hierarchy;
            Stack<ITypeReference> typeParamsStack;
            _OIL._Core.GetDeclaredHierarchy(typeReference, typeParameters, out hierarchy, out typeParamsStack);
            bool flag = true;
            if (((typeReference.ResolutionOptions & TypeReferenceResolveOptions.GlobalType) == TypeReferenceResolveOptions.GlobalType))
                base.Write("global::", TranslatorFormatterTokenType.Keyword);
            if (hierarchy[0].ParentTarget is INameSpaceDeclaration)
                if (((typeReference.ResolutionOptions & TypeReferenceResolveOptions.FullType) == TypeReferenceResolveOptions.FullType) || !base.Options.AutoResolveReferences || ((typeReference.ResolutionOptions & TypeReferenceResolveOptions.GlobalType) == TypeReferenceResolveOptions.GlobalType))
                    base.Write(string.Format("{0}.", ((INameSpaceDeclaration)hierarchy[0].ParentTarget).FullName), TranslatorFormatterTokenType.NameSpace);
            for (int j = 0; j < hierarchy.Count; j++)
            {
                IDeclaredType idt;
                if (hierarchy[j] is ISegmentableDeclaredType)
                    idt = ((ISegmentableDeclaredType)hierarchy[j]).GetRootDeclaration();
                else
                    idt = hierarchy[j];
                bool isLast = (idt == hierarchy[hierarchy.Count - 1]);
                bool excluded = false;
                excluded = (!isLast && base.Options.BuildTrail.Contains(idt) && (typeReference.TypeParameters.Count == 0 || AllAreGenericParameters(typeReference, typeParameters))  && typeReference.ResolutionOptions == TypeReferenceResolveOptions.UseGeneratorOptions);
                if (!excluded)
                {
                    if (flag)
                        flag = false;
                    else
                        base.Write(".", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptIdentifier(hierarchy[j], false);
                    if (idt.IsGeneric && idt is IParameteredDeclaredType)
                    {
                        IParameteredDeclaredType ipdt = ((IParameteredDeclaredType)(idt));
                        if (ipdt.TypeParameters != null && ipdt.TypeParameters.Count > 0)
                        {
                            base.Write("<", TranslatorFormatterTokenType.Operator);
                            bool flag2 = true;
                            for (int i = 0; i < ipdt.TypeParameters.Count; i++)
                            {
                                if (flag2)
                                    flag2 = false;
                                else
                                    base.Write(", ", TranslatorFormatterTokenType.Operator);
                                this.TranslateConceptTypeName(typeParamsStack.Pop());
                            }
                            base.Write(">", TranslatorFormatterTokenType.Operator);
                        }
                    }
                }
                else if (idt.IsGeneric && idt is IParameteredDeclaredType && ((IParameteredDeclaredType)idt).TypeParameters != null && ((IParameteredDeclaredType)idt).TypeParameters.Count > 0)
                    for (int i = 0; i < ((IParameteredDeclaredType)idt).TypeParameters.Count; i++)
                        typeParamsStack.Pop();
            }
        }

        private bool AllAreGenericParameters(IDeclaredTypeReference typeReference, ITypeReferenceCollection typeParameters)
        {
            ITypeReference itr = typeReference.TypeInstance.GetTypeReference();
            if (itr.TypeParameters.Count != typeParameters.Count)
                return false;
            for (int i = 0; i < itr.TypeParameters.Count; i++)
                if (!itr.TypeParameters[i].Equals(typeReference.TypeParameters[i]))
                    return false;
            return true;
        }

        public override sealed void TranslateConceptTypeName(ITypeParameterMember type, ITypeReferenceCollection typeParameters)
        {
            this.TranslateConceptIdentifier(type, false);
        }

        public override void TranslateConstraints<TDom, TParent>(ITypeParameterMember<TDom, TParent> ambigTypeParamMember)
        {
            base.IncreaseIndent();
            List<ITypeReference> typeRefClasses = new List<ITypeReference>();
            List<ITypeReference> typeRefInterfaces = new List<ITypeReference>();
            foreach (ITypeReference itr in ambigTypeParamMember.Constraints)
                if (itr.TypeInstance.IsClass || itr.TypeInstance.IsDelegate)
                    typeRefClasses.Add(itr);
                else if (itr.TypeInstance.IsInterface || itr.TypeInstance is ITypeParameterMember)
                    typeRefInterfaces.Add(itr);
                else
                    throw new NotSupportedException("Cannot use a constraint of given type.");
            if (ambigTypeParamMember.Constraints.Count > 0 || (ambigTypeParamMember.SpecialCondition != TypeParameterSpecialCondition.None) | ambigTypeParamMember.RequiresConstructor)
            {
                this.TranslateConceptKeyword(Keywords.Where);
                base.Write(" ", TranslatorFormatterTokenType.Other);
                this.TranslateMember<TDom, TParent>(ambigTypeParamMember);
                base.WriteLine(" :", TranslatorFormatterTokenType.Operator);
                base.IncreaseIndent();
                bool firstMember = true;
                List<ITypeReference> typeRefArranged = new List<ITypeReference>();
                switch (ambigTypeParamMember.SpecialCondition)
                {
                    case TypeParameterSpecialCondition.Class:
                        this.TranslateConceptKeyword(Keywords.Class);
                        firstMember = false;
                        break;
                    case TypeParameterSpecialCondition.ValueType:
                        this.TranslateConceptKeyword(Keywords.Struct);
                        firstMember = false;
                        break;
                    case TypeParameterSpecialCondition.None:
                    default:
                        typeRefArranged.AddRange(typeRefClasses);
                        break;
                }
                typeRefArranged.AddRange(typeRefInterfaces);
                foreach (ITypeReference itr in typeRefArranged)
                {
                    if (firstMember)
                        firstMember = false;
                    else
                        base.WriteLine(",", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptTypeName(itr);
                }
                if (ambigTypeParamMember.RequiresConstructor)
                {
                    if (!firstMember)
                        base.WriteLine(",", TranslatorFormatterTokenType.Operator);
                    this.TranslateConceptKeyword(Keywords.New);
                    base.Write("()", TranslatorFormatterTokenType.Operator);
                }
                base.DecreaseIndent();
            }
            base.DecreaseIndent();
        }

        public override void TranslateConceptComment(string commentBase, bool docComment)
        {
            string result = _OIL._Core.GetCSharpCommentText(commentBase.Replace("\t", "    "), docComment);
            string[] resultLines = result.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string s in resultLines)
                base.WriteLine(s, TranslatorFormatterTokenType.Comment);
        }

        public override string SubToolVersion
        {
            get
            {
                if (subToolVersion == null)
                    subToolVersion = typeof(CSharpCodeTranslator).Assembly.GetName().Version.ToString();
                return this.subToolVersion;
            }
        }

        public override string SubToolName
        {
            get
            {
                return "Oilexer.CSharpCodeTranslator";
            }
        }

        public override string Language
        {
            get
            {
                return string.Format("C# (Runtime version: {0})", typeof(string).Assembly.ImageRuntimeVersion);
            }
        }

        public override void TranslateAttribute(AttributeTargets target, IAttributeDeclarationTarget attributeSource, IAttributeDeclaration attribute)
        {
            if ((attributeSource is ISegmentableDeclarationTarget) && (!((ISegmentableDeclarationTarget)attributeSource).IsRoot))
                return;
            base.Write("[", TranslatorFormatterTokenType.Operator);
            bool addSpecifier = true;
            switch (target)
            {
                case AttributeTargets.Assembly:
                    base.Write("assembly", TranslatorFormatterTokenType.Keyword);
                    break;
                case AttributeTargets.Class:
                case AttributeTargets.Delegate:
                case AttributeTargets.Enum:
                case AttributeTargets.Interface:
                case AttributeTargets.Struct:
                    base.Write("type", TranslatorFormatterTokenType.Keyword);
                    break;
                case AttributeTargets.Constructor:
                case AttributeTargets.Method:
                    base.Write("method", TranslatorFormatterTokenType.Keyword);
                    break;
                case AttributeTargets.Event:
                    TranslateConceptKeyword(Keywords.Event);
                    break;
                case AttributeTargets.Field:
                    base.Write("field", TranslatorFormatterTokenType.Keyword);
                    break;
                case AttributeTargets.GenericParameter:
                    break;
                case AttributeTargets.Module:
                    base.Write("module", TranslatorFormatterTokenType.Keyword);
                    break;
                case AttributeTargets.Parameter:
                    base.Write("param", TranslatorFormatterTokenType.Keyword);
                    break;
                case AttributeTargets.Property:
                    base.Write("property", TranslatorFormatterTokenType.Keyword);
                    break;
                case AttributeTargets.ReturnValue:
                    base.Write("return", TranslatorFormatterTokenType.Keyword);
                    break;
                default:
                    addSpecifier = false;
                    break;
            }
            if (addSpecifier)
            {
                base.Write(":", TranslatorFormatterTokenType.Operator);
                base.Write(" ", TranslatorFormatterTokenType.Other);
            }
            TranslateAttributeInternal(attribute);
            base.WriteLine("]", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateAttribute(IAttributeDeclarationTarget attributeSource, IAttributeDeclaration attribute)
        {

            if ((attributeSource is ISegmentableDeclarationTarget) && (!((ISegmentableDeclarationTarget)attributeSource).IsRoot))
                return;
            base.Write("[", TranslatorFormatterTokenType.Operator);
            TranslateAttributeInternal(attribute);
            base.WriteLine("]", TranslatorFormatterTokenType.Operator);
        }

        private void TranslateAttributeInternal(IAttributeDeclaration attribute)
        {
            IAttributeConstructorParameter[] normal = (from a in attribute.Parameters
                                                       where (!(a is IAttributePropertyParameter))
                                                       select a).ToArray();
            IAttributeConstructorParameter[] named =  (from a in attribute.Parameters
                                                       where (a is IAttributePropertyParameter)
                                                       select a).ToArray();
            this.TranslateConceptTypeName(attribute.AttributeType);
            base.Write("(", TranslatorFormatterTokenType.Operator);
            bool firstMember = true;
            foreach (IAttributeConstructorParameter normalParam in normal)
            {
                if (firstMember)
                    firstMember = false;
                else
                    base.Write(", ", TranslatorFormatterTokenType.Operator);
                this.TranslateExpression(normalParam.Value);
            }

            foreach (IAttributePropertyParameter propParam in named)
            {
                if (firstMember)
                    firstMember = false;
                else
                    base.Write(", ", TranslatorFormatterTokenType.Operator);
                base.TranslateConceptIdentifier(propParam.Name, TranslatorFormatterMemberType.Parameter);
                base.Write(" = ", TranslatorFormatterTokenType.Operator);
                this.TranslateExpression(propParam.Value);
            }
            base.Write(")", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateConceptRegionStart(string regionText)
        {
            base.Write("#region ", TranslatorFormatterTokenType.Keyword);
            base.WriteLine(regionText, TranslatorFormatterTokenType.String);
        }

        public override void TranslateConceptRegionEnd(string regionText)
        {
            base.Write("#endregion", TranslatorFormatterTokenType.Keyword);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            TranslateConceptComment(regionText, false);
        }

        public override void TranslateConceptKeyword(int keyWord)
        {
            this.TranslateConceptKeyword((Keywords)keyWord);
        }

        private void TranslateConceptKeyword(Keywords keyword)
        {
            string _keyword = "";
            switch (keyword)
            {
                case Keywords.As:
                case Keywords.Do:
                case Keywords.If:
                case Keywords.In:
                case Keywords.Is:
                    _keyword = keywordLookup[2][(int)keyword];
                    break;
                case Keywords.For:
                case Keywords.Get:
                case Keywords.Int:
                case Keywords.New:
                case Keywords.Out:
                case Keywords.Ref:
                case Keywords.Set:
                case Keywords.Try:
                    _keyword = keywordLookup[3][(int)keyword];
                    break;
                case Keywords.Base:
                case Keywords.Bool:
                case Keywords.Byte:
                case Keywords.Case:
                case Keywords.Char:
                case Keywords.Else:
                case Keywords.Enum:
                case Keywords.Goto:
                case Keywords.Lock:
                case Keywords.Long:
                case Keywords.Null:
                case Keywords.This:
                case Keywords.True:
                case Keywords.Uint:
                case Keywords.Void:
                    _keyword = keywordLookup[4][(int)keyword];
                    break;
                case Keywords.Break:
                case Keywords.Catch:
                case Keywords.Class:
                case Keywords.Const:
                case Keywords.Event:
                case Keywords.False:
                case Keywords.Fixed:
                case Keywords.Float:
                case Keywords.Sbyte:
                case Keywords.Short:
                case Keywords.Throw:
                case Keywords.Ulong:
                case Keywords.Using:
                case Keywords.Where:
                case Keywords.While:
                case Keywords.Yield:
                    _keyword = keywordLookup[5][(int)keyword];
                    break;
                case Keywords.Double:
                case Keywords.Extern:
                case Keywords.Object:
                case Keywords.Params:
                case Keywords.Public:
                case Keywords.Return:
                case Keywords.Sealed:
                case Keywords.Sizeof:
                case Keywords.Static:
                case Keywords.String:
                case Keywords.Struct:
                case Keywords.Switch:
                case Keywords.Typeof:
                case Keywords.Unsafe:
                case Keywords.Ushort:
                    _keyword = keywordLookup[6][(int)keyword];
                    break;
                case Keywords.Checked:
                case Keywords.Decimal:
                case Keywords.Default:
                case Keywords.Finally:
                case Keywords.Foreach:
                case Keywords.Partial:
                case Keywords.Private:
                case Keywords.Virtual:
                    _keyword = keywordLookup[7][(int)keyword];
                    break;
                case Keywords.Abstract:
                case Keywords.Continue:
                case Keywords.Delegate:
                case Keywords.Explicit:
                case Keywords.Implicit:
                case Keywords.Internal:
                case Keywords.Operator:
                case Keywords.Override:
                case Keywords.Readonly:
                case Keywords.Volatile:
                    _keyword = keywordLookup[8][(int)keyword];
                    break;
                case Keywords.__Arglist:
                case Keywords.__Makeref:
                case Keywords.__Reftype:
                case Keywords.Interface:
                case Keywords.Namespace:
                case Keywords.Protected:
                case Keywords.Unchecked:
                    _keyword = keywordLookup[9][(int)keyword];
                    break;
                case Keywords.__Refvalue:
                case Keywords.Stackalloc:
                    _keyword = keywordLookup[10][(int)keyword];
                    break;
                default:
                    break;
            }
            base.Write(_keyword, TranslatorFormatterTokenType.Keyword);
        }

        /// <summary>
        /// Translates a go-to statement which transfers the execution point to the label defined
        /// in the <see cref="IGoToLabelStatement"/>.
        /// </summary>
        /// <param name="gotoLabelStatement">The <see cref="IGoToLabelStatement"/></param>
        public override void TranslateStatement(IGoToLabelStatement gotoLabelStatement)
        {
            this.TranslateConceptKeyword(Keywords.Goto);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            //this.TranslateConceptIdentifier(gotoLabelStatement.LabelStatement.Name);
            this.TranslateConceptIdentifier(gotoLabelStatement.LabelStatement.Name, gotoLabelStatement.LabelStatement, false);
            base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        /// <summary>
        /// Translates a label statement which marks a point at which execution can be forwarded to.
        /// </summary>
        /// <param name="labelStatement">The <see cref="ILabelStatement"/> to translate.</param>
        public override void TranslateStatement(ILabelStatement labelStatement)
        {
            base.DecreaseIndent();
            TranslateConceptIdentifier(labelStatement.Name, labelStatement, true);
            base.WriteLine(":", TranslatorFormatterTokenType.Operator);
            base.IncreaseIndent();
        }

        public override void TranslateComment(IDocumentationComment docComment)
        {
            TranslateConceptComment(docComment.BuildCommentBody(base.Options), true);
        }

        public override void TranslateExpression(IBaseReferenceExpression baseRefExpression)
        {
            TranslateConceptKeyword(Keywords.Base);
        }

        public override void TranslateMember<TOperator>(IOperatorOverloadMember<TOperator> operatorOverloadMember)
        {
            if (typeof(TOperator) == typeof(OverloadableBinaryOperators) && operatorOverloadMember is IBinaryOperatorOverloadMember)
                this.TranslateMember((IBinaryOperatorOverloadMember)operatorOverloadMember);
            else if (typeof(TOperator) == typeof(OverloadableUnaryOperators) && operatorOverloadMember is IUnaryOperatorOverloadMember)
                this.TranslateMember((IUnaryOperatorOverloadMember)operatorOverloadMember);
            else
                throw new NotSupportedException(string.Format("the coercion type with operator {0} is not supported", typeof(TOperator).Name));
        }

        public override void TranslateMember(IUnaryOperatorOverloadMember unaryMember)
        {
            
        }

        public override void TranslateMember(IBinaryOperatorOverloadMember binaryMember)
        {
            TranslateConceptAccessModifiers(binaryMember);
            TranslateConceptKeyword(Keywords.Static);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            switch (binaryMember.Operator)
            {
                case OverloadableBinaryOperators.Add:
                case OverloadableBinaryOperators.Subtract:
                case OverloadableBinaryOperators.Multiply:
                case OverloadableBinaryOperators.Divide:
                case OverloadableBinaryOperators.ExclusiveOr:
                case OverloadableBinaryOperators.Modulus:
                case OverloadableBinaryOperators.LogicalAnd:
                case OverloadableBinaryOperators.LogicalOr:
                case OverloadableBinaryOperators.LeftShift:
                case OverloadableBinaryOperators.RightShift:
                    TranslateConceptTypeName(binaryMember.ParentTarget.GetTypeReference());
                    break;
                case OverloadableBinaryOperators.IsEqualTo:
                case OverloadableBinaryOperators.IsNotEqualTo:
                case OverloadableBinaryOperators.LessThan:
                case OverloadableBinaryOperators.GreaterThan:
                case OverloadableBinaryOperators.LessThanOrEqualTo:
                case OverloadableBinaryOperators.GreaterThanOrEqualTo:
                    TranslateConceptTypeName(typeof(bool).GetTypeReference());
                    break;
                default:
                    break;
            }
            base.Write(" ", TranslatorFormatterTokenType.Other);
            TranslateConceptKeyword(Keywords.Operator);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            switch (binaryMember.Operator)
            {
                case OverloadableBinaryOperators.Add:
                    base.Write("+", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.Subtract:
                    base.Write("-", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.Multiply:
                    base.Write("*", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.Divide:
                    base.Write("/", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.Modulus:
                    base.Write("%", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.LogicalAnd:
                    base.Write("&", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.LogicalOr:
                    base.Write("|", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.ExclusiveOr:
                    base.Write("^", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.LeftShift:
                    base.Write("<<", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.RightShift:
                    base.Write(">>", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.IsEqualTo:
                    base.Write("==", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.IsNotEqualTo:
                    base.Write("!=", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.LessThan:
                    base.Write("<", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.GreaterThan:
                    base.Write(">", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.LessThanOrEqualTo:
                    base.Write("<=", TranslatorFormatterTokenType.Operator);
                    break;
                case OverloadableBinaryOperators.GreaterThanOrEqualTo:
                    base.Write(">=", TranslatorFormatterTokenType.Operator);
                    break;
                default:
                    break;
            }
            base.Write("(", TranslatorFormatterTokenType.Operator);
            switch (binaryMember.ContainingSide)
            {
                case BinaryOperatorOverloadContainingSide.LeftSide:
                    base.TranslateConceptTypeName(binaryMember.ParentTarget.GetTypeReference());
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    base.Write(binaryMember.LeftParameter.Name, TranslatorFormatterMemberType.Parameter);

                    base.Write(",", TranslatorFormatterTokenType.Operator);

                    base.TranslateConceptTypeName(binaryMember.OtherSide);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    base.Write(binaryMember.RightParameter.Name, TranslatorFormatterMemberType.Parameter);
                    break;
                case BinaryOperatorOverloadContainingSide.RightSide:
                    base.TranslateConceptTypeName(binaryMember.OtherSide);
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    base.Write(binaryMember.LeftParameter.Name, TranslatorFormatterMemberType.Parameter);

                    base.Write(",", TranslatorFormatterTokenType.Operator);

                    base.TranslateConceptTypeName(binaryMember.ParentTarget.GetTypeReference());
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    base.Write(binaryMember.RightParameter.Name, TranslatorFormatterMemberType.Parameter);
                    break;
                case BinaryOperatorOverloadContainingSide.Both:
                default:
                    base.TranslateConceptTypeName(binaryMember.ParentTarget.GetTypeReference());
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    base.Write(binaryMember.LeftParameter.Name, TranslatorFormatterMemberType.Parameter);

                    base.Write(",", TranslatorFormatterTokenType.Operator);

                    base.TranslateConceptTypeName(binaryMember.ParentTarget.GetTypeReference());
                    base.Write(" ", TranslatorFormatterTokenType.Other);
                    base.Write(binaryMember.RightParameter.Name, TranslatorFormatterMemberType.Parameter);
                    break;
            }
            base.Write(")", TranslatorFormatterTokenType.Operator);
            base.WriteLine();
            this.TranslateStatementBlock(binaryMember.Statements);
        }
        public override void TranslateMember(ITypeConversionOverloadMember typeConversionMember)
        {
            TranslateAttributes(typeConversionMember, typeConversionMember.Attributes);
            TranslateConceptAccessModifiers(typeConversionMember);
            TranslateConceptKeyword(Keywords.Static);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            switch (typeConversionMember.Requirement)
            {
                case TypeConversionRequirement.Explicit:
                    TranslateConceptKeyword(Keywords.Explicit);
                    break;
                case TypeConversionRequirement.Implicit:
                    TranslateConceptKeyword(Keywords.Implicit);
                    break;
            }
            base.Write(" ", TranslatorFormatterTokenType.Other);
            TranslateConceptKeyword(Keywords.Operator);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            switch (typeConversionMember.Direction)
            {
                case TypeConversionDirection.ToContainingType:
                    TranslateConceptTypeName(typeConversionMember.ParentTarget.GetTypeReference());
                    base.Write("(", TranslatorFormatterTokenType.Operator);
                    TranslateConceptTypeName(typeConversionMember.CoercionType);
                    break;
                case TypeConversionDirection.FromContainingType:
                    TranslateConceptTypeName(typeConversionMember.CoercionType);
                    base.Write("(", TranslatorFormatterTokenType.Operator);
                    TranslateConceptTypeName(typeConversionMember.ParentTarget.GetTypeReference());
                    break;
            }
            base.Write(" ", TranslatorFormatterTokenType.Other);
            base.Write(typeConversionMember.Source.Name, TranslatorFormatterMemberType.Parameter);
            base.Write(")", TranslatorFormatterTokenType.Operator);
            base.WriteLine();
            this.TranslateStatementBlock(typeConversionMember.Statements);
        }

        public override void TranslateStatement(ICrementStatement crementStatement)
        {
            switch (crementStatement.CrementType)
            {
                case CrementType.Prefix:
                    switch (crementStatement.Operation)
                    {
                        case CrementOperation.Increment:
                            base.Write("++", TranslatorFormatterTokenType.Operator);
                            break;
                        case CrementOperation.Decrement:
                            base.Write("--", TranslatorFormatterTokenType.Operator);
                            break;
                    }
                    TranslateExpression(crementStatement.Target);
                    break;
                case CrementType.Postfix:
                    TranslateExpression(crementStatement.Target);
                    switch (crementStatement.Operation)
                    {
                        case CrementOperation.Increment:
                            base.Write("++", TranslatorFormatterTokenType.Operator);
                            break;
                        case CrementOperation.Decrement:
                            base.Write("--", TranslatorFormatterTokenType.Operator);
                            break;
                    }
                    break;
            }
            if (!suppressLineTerminator)
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateStatement(IBlockStatement blockStatement)
        {
            TranslateStatementBlock(blockStatement.Statements);
        }

        public override void TranslateStatement(IYieldStatement yieldStatement)
        {
            TranslateConceptKeyword(Keywords.Yield);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            TranslateConceptKeyword(Keywords.Return);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            this.TranslateExpression(yieldStatement.Result);
            if (!suppressLineTerminator)
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }

        public override void TranslateStatement(IYieldBreakStatement breakStatement)
        {
            TranslateConceptKeyword(Keywords.Yield);
            base.Write(" ", TranslatorFormatterTokenType.Other);
            TranslateConceptKeyword(Keywords.Break);
            if (!suppressLineTerminator)
                base.WriteLine(";", TranslatorFormatterTokenType.Operator);
        }
    }
}
