using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using Oilexer.Types;
using Oilexer.Expression;
using Oilexer.Utilities.Common;
using System.Globalization;
using System.Resources;
using System.Reflection.Emit;

namespace Oilexer
{
    public class IntermediateProjectInformation :
        IIntermediateProjectInformation
    {
        /// <summary>
        /// Data member for <see cref="AssemblyName"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="Title"/>
        /// </summary>
        //AssemblyTitle
        string title;
        /// <summary>
        /// Data member for <see cref="Description"/>
        /// </summary>
        //AssemblyDescription
        string description;
        /// <summary>
        /// Data member for <see cref="Copyright"/>
        /// </summary>
        //AssemblyCopyright
        string copyright;
        /// <summary>
        /// Data member for <see cref="Company"/>.
        /// </summary>
        //AssemblyCompany
        string company;
        /// <summary>
        /// Data member for <see cref="Product"/>
        /// </summary>
        //AssemblyProduct
        string product;
        /// <summary>
        /// Data member for <see cref="Trademark"/>.
        /// </summary>
        //AssemblyTrademark
        string trademark;
        /// <summary>
        /// Data member for <see cref="Culture"/>.
        /// </summary>
        //NeutralResourcesLanguageAttribute
        ICultureIdentifier culture;
        /// <summary>
        /// Data member for <see cref="FileVersion"/>.
        /// </summary>
        private Version fileVersion;
        /// <summary>
        /// Data member for <see cref="AssemblyVersion"/>.
        /// </summary>
        private Version assemblyVersion;
        /// <summary>
        /// Data member for <see cref="ProjectGuid"/>.
        /// </summary>
        private Guid projectGuid;
        /// <summary>
        /// Data member which relates to the <see cref="IIntermediateProject"/> of the <see cref="IntermediateProjectInformation"/>.
        /// </summary>
        private IIntermediateProject project;

        private Action changeDelegate;

        public IntermediateProjectInformation(IIntermediateProject project, Action changeDelegate)
        {
            this.project = project;
            this.culture = CultureIdentifiers.None;
            this.fileVersion = new Version(1, 0);
            this.assemblyVersion = new Version(1, 0);
            this.title = this.project.Name;
            this.projectGuid = Guid.NewGuid();
            this.changeDelegate = changeDelegate;
        }

        #region IIntermediateProjectInformation Members

        /// <summary>
        /// Returns/sets the name of the project.
        /// </summary>
        public string AssemblyName
        {
            get
            {
                if (this.name == null)
                    this.name = this.project.Name;
                return this.name;
            }
            set
            {
                this.name = value;
                changeDelegate();
            }
        }

        /// <summary>
        /// Returns/sets the title of the project.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                changeDelegate();
            }
        }

        /// <summary>
        /// Returns/sets the description of the project.
        /// </summary>
        public string Description
        {
            get
            {
                if (this.company == null)
                    this.company = ExtractByAttribute<string>(typeof(AssemblyDescriptionAttribute), new TranslateArgument<string, IAttributeDeclaration>(StandardExtractAttribute<string>));
                return this.description;
            }
            set
            {
                this.description = value;
                SetStandardValue(value, typeof(AssemblyDescriptionAttribute));
                changeDelegate();
            }
        }

        /// <summary>
        /// Returns/sets the company name of the project.
        /// </summary>
        public string Company
        {
            get
            {
                if (this.company == null)
                    this.company = ExtractByAttribute<string>(typeof(AssemblyCompanyAttribute), new TranslateArgument<string, IAttributeDeclaration>(StandardExtractAttribute<string>));
                return this.company;
            }
            set
            {
                this.company = value;
                SetStandardValue(value, typeof(AssemblyCompanyAttribute));
                changeDelegate();
            }
        }
        private IAttributeDeclaration ExtractAttribute(Type type)
        {
            ITypeReference itr = type.GetTypeReference();
            if (this.project.Attributes.IsDefined(itr))
                foreach (IAttributeDeclaration iad in this.project.Attributes)
                {
                    if (iad.AttributeType.Equals(itr))
                        return iad;
                }
            else
                return this.project.Attributes.AddNew(itr);
            return null;
        }
        private T ExtractByAttribute<T>(Type type, TranslateArgument<T, IAttributeDeclaration> translator)
        {
            ITypeReference itr = type.GetTypeReference();
            if (this.project.Attributes.IsDefined(itr))
            {
                foreach (IAttributeDeclaration iad in this.project.Attributes)
                    if (iad.AttributeType.Equals(itr))
                        return translator(iad);
            }
            return default(T);
        }

        /// <summary>
        /// Returns/sets the product name of the project.
        /// </summary>
        public string Product
        {
            get
            {
                if (this.product == null)
                    this.product = ExtractByAttribute<string>(typeof(AssemblyProductAttribute), new TranslateArgument<string, IAttributeDeclaration>(StandardExtractAttribute<string>));
                return this.product;
            }
            set
            {
                this.product = value;
                SetStandardValue(value, typeof(AssemblyProductAttribute));
                changeDelegate();
            }
        }

        private void SetStandardValue<T>(T value, Type attrType)
        {
            IAttributeDeclaration iad = ExtractAttribute(attrType);
            iad.Parameters.Clear();
            iad.Parameters.AddNew(new PrimitiveExpression(value));
        }

        /// <summary>
        /// Returns/sets the copyright information of the project.
        /// </summary>
        public string Copyright
        {
            get
            {
                if (this.copyright == null)
                    this.copyright = ExtractByAttribute<string>(typeof(AssemblyCopyrightAttribute), new TranslateArgument<string, IAttributeDeclaration>(StandardExtractAttribute<string>));
                return this.copyright;
            }
            set
            {
                this.copyright = value;
                SetStandardValue(value, typeof(AssemblyCopyrightAttribute));
                changeDelegate();
            }
        }

        /// <summary>
        /// Returns/sets the trademark of the project.
        /// </summary>
        public string Trademark
        {
            get
            {
                if (this.trademark == null)
                    this.trademark = ExtractByAttribute<string>(typeof(AssemblyTrademarkAttribute), new TranslateArgument<string, IAttributeDeclaration>(StandardExtractAttribute<string>));
                return this.trademark;
            }
            set
            {
                this.trademark = value;
                SetStandardValue(value, typeof(AssemblyTrademarkAttribute));
                changeDelegate();
            }
        }

        /// <summary>
        /// Returns/sets the culture, relative to the <see cref="CultureInfo"/>, of the assembly.
        /// </summary>
        [DefaultValue("None")]
        public ICultureIdentifier Culture
        {
            get
            {
                string culture = null;
                if (this.culture == null)
                {
                    culture = ExtractByAttribute<string>(typeof(NeutralResourcesLanguageAttribute), new TranslateArgument<string, IAttributeDeclaration>(StandardExtractAttribute<string>));
                    if (culture != null && culture != string.Empty)
                        this.culture = CultureIdentifier.defaultCultureIDByCultureName[culture];
                }

                return this.culture;
            }
            set
            {
                this.culture = value;
                SetStandardValue(value.Name, typeof(NeutralResourcesLanguageAttribute));
                changeDelegate();
            }
        }

        /// <summary>
        /// Returns/sets the version of the project file.
        /// </summary>
        public Version FileVersion
        {
            get { return this.fileVersion; }
            set
            {
                this.fileVersion = value;
                changeDelegate();
            }
        }

        /// <summary>
        /// Returns/sets the version of the project.
        /// </summary>
        public Version AssemblyVersion
        {
            get { return this.assemblyVersion; }
            set
            {
                this.assemblyVersion = value;
                changeDelegate();
            }
        }

        public Guid ProjectGuid
        {
            get { return this.projectGuid; }
            set { this.projectGuid = value;
            changeDelegate();
            }
        }

        #endregion

        private T StandardExtractAttribute<T>(IAttributeDeclaration attr)
        {
            if (attr.Parameters.Count > 0 && attr.Parameters[0].Value is IPrimitiveExpression)
                return (T)((IPrimitiveExpression)(attr.Parameters[0].Value)).Value;
            return default(T);
        }
    }
}
