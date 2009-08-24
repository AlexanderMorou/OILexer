using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Expression;
using Oilexer.Statements;
namespace Oilexer.Types
{
    public class DeclarationResourcesStringTableEntry :
        IDeclarationResourcesStringTableEntry
    {
        #region DeclarationResourcesStringTableEntry Data members

        private IDeclarationResources declarationTarget = null;
        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="Value"/>.
        /// </summary>
        private string value;
        /// <summary>
        /// Data member for <see cref="DataMember"/>.
        /// </summary>
        private IFieldMember dataMember;
        /// <summary>
        /// Data member for <see cref="AutoMember"/>.
        /// </summary>
        private IPropertyMember autoMember;
        /// <summary>
        /// Stored version of the primitive that is used to call the GetString method.
        /// When the name changes of the <see cref="DeclarationResourcesStringTableEntry"/>
        /// it will be updated by changing this value.
        /// </summary>
        private IPrimitiveExpression namePrimitive = null;
        #endregion

        internal DeclarationResourcesStringTableEntry(IDeclarationResources declarationTarget, string name, string value)
        {
            if (declarationTarget == null)
                throw new ArgumentNullException("declarationTarget");
            if (name == null || name == string.Empty)
                throw new ArgumentNullException("name");
            if (value == null || value == string.Empty)
                throw new ArgumentNullException("value");
            this.declarationTarget = declarationTarget;
            this.name = name;
            this.value = value;
            this.Rebuild();
        }

        #region IDeclarationResourcesStringTableEntry Members

        /// <summary>
        /// Returns/sets the name of the <see cref="IDeclarationResourcesStringTableEntry"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.namePrimitive.Value = value;
            }
        }

        /// <summary>
        /// Returns/sets the value of the <see cref="IDeclarationResourcesStringTableEntry"/>.
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Returns the data member used to store the value of the <see cref="IDeclarationResourcesStringTableEntry"/>.
        /// </summary>
        public IFieldMember DataMember
        {
            get { return this.dataMember; }
        }

        /// <summary>
        /// Returns the auto-generated member used to retrieve the value of the <see cref="IDeclarationResourcesStringTableEntry"/>.
        /// </summary>
        public IPropertyMember AutoMember
        {
            get { return this.autoMember; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.autoMember != null)
            {
                this.autoMember.Dispose();
                this.autoMember = null;
            }
            if (this.dataMember != null)
            {
                this.dataMember.Dispose();
                this.dataMember = null;
            }
            if (this.declarationTarget != null)
                this.declarationTarget = null;
            this.namePrimitive = null;
            this.name = null;
            this.value = null;
        }

        #endregion

        public void Rebuild()
        {
            if (this.autoMember != null)
            {
                this.declarationTarget.Properties.Remove(this.autoMember);
                this.autoMember.Dispose();
            }
            if (this.dataMember != null)
            {
                this.declarationTarget.Fields.Remove(this.dataMember);
                this.dataMember.Dispose();
            }
            switch (this.declarationTarget.GenerationType)
            {
                case ResourceGenerationType.GeneratedClass:
                    this.autoMember = declarationTarget.Properties.AddNew(new TypedName(name, typeof(string)), true, false);
                    this.autoMember.AccessLevel = DeclarationAccessLevel.Internal;
                    this.namePrimitive = new PrimitiveExpression(name);
                    this.autoMember.GetPart.Return(declarationTarget.ResourceManager.GetReference().GetMethod("GetString").Invoke(namePrimitive));
                    break;
                case ResourceGenerationType.GeneratedClassWithCache:
                    this.dataMember = declarationTarget.Fields.AddNew(new TypedName(string.Format("__{0}_dm_", name), typeof(string)));
                    this.autoMember = declarationTarget.Properties.AddNew(new TypedName(name, typeof(string)), true, false);
                    this.dataMember.IsStatic = true;
                    this.autoMember.IsStatic = true;
                    this.dataMember.AccessLevel = DeclarationAccessLevel.Private;
                    this.autoMember.AccessLevel = DeclarationAccessLevel.Internal;
                    IFieldReferenceExpression dataMemberReference = dataMember.GetReference();
                    IConditionStatement ics = this.autoMember.GetPart.IfThen((Expression.Expression)dataMemberReference == PrimitiveExpression.NullValue);
                    this.namePrimitive = new PrimitiveExpression(name);
                    ics.Assign(dataMemberReference, declarationTarget.ResourceManager.GetReference().GetMethod("GetString").Invoke(namePrimitive));
                    this.autoMember.GetPart.Return(dataMemberReference);
                    break;
                default:
                    break;
            } 
        }
    }
}