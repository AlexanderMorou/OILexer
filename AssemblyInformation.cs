using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Resources;
using Oilexer.Utilities.Arrays;
using System.Linq;
namespace Oilexer
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class AssemblyInformation :
        IAssemblyInformation
    {
        /// <summary>
        /// Data member for <see cref="AssemblyName"/>
        /// </summary>
        string assemblyName;
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
        private bool loadedFromGac;
        private bool strongName;
        private string path;
        public AssemblyInformation(Assembly assembly)
        {
            AttributeCollection attrC = new AttributeCollection(assembly.GetCustomAttributes(false).Cast<Attribute>().ToArray());
            if (attrC[typeof(AssemblyTitleAttribute)] != null)
                this.title = ((AssemblyTitleAttribute)attrC[typeof(AssemblyTitleAttribute)]).Title;
            if (attrC[typeof(AssemblyCompanyAttribute)] != null)
                this.company = ((AssemblyCompanyAttribute)attrC[typeof(AssemblyCompanyAttribute)]).Company;
            if (attrC[typeof(AssemblyDescriptionAttribute)] != null)
                this.description = ((AssemblyDescriptionAttribute)attrC[typeof(AssemblyDescriptionAttribute)]).Description;
            if (attrC[typeof(AssemblyCopyrightAttribute)] != null)
                this.copyright = ((AssemblyCopyrightAttribute)attrC[typeof(AssemblyCopyrightAttribute)]).Copyright;
            if (attrC[typeof(AssemblyProductAttribute)] != null)
                this.product = ((AssemblyProductAttribute)attrC[typeof(AssemblyProductAttribute)]).Product;
            if (attrC[typeof(AssemblyTrademarkAttribute)] != null)
                this.trademark = ((AssemblyTrademarkAttribute)attrC[typeof(AssemblyTrademarkAttribute)]).Trademark;
            if (attrC[typeof(AssemblyFileVersionAttribute)] != null)
                this.fileVersion = new Version(((AssemblyFileVersionAttribute)attrC[typeof(AssemblyFileVersionAttribute)]).Version);
            this.assemblyVersion = assembly.GetName().Version;
            this.culture = CultureIdentifiers.GetIdentifierById(assembly.GetName().CultureInfo.LCID);
            /*
            if (attrC[typeof(NeutralResourcesLanguageAttribute)] != null)
                this.culture = CultureIdentifier.defaultCultureIDByCultureName[((NeutralResourcesLanguageAttribute)attrC[typeof(NeutralResourcesLanguageAttribute)]).CultureName];
            //*/
            this.path = assembly.Location;
            this.strongName = attrC[typeof(AssemblyKeyFileAttribute)] != null;
            this.loadedFromGac = assembly.GlobalAssemblyCache;
            this.assemblyName = assembly.GetName().Name;
        }

        public bool LoadedFromGAC
        {
            get
            {
                return this.loadedFromGac;
            }
        }
        public bool StrongName
        {
            get
            {
                return this.strongName;
            }
        }
        public string Path
        {
            get
            {
                return this.path;
            }
        }

        #region IAssemblyInformation Members
        
        public string AssemblyName
        {
            get
            {
                return assemblyName;
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public string Company
        {
            get
            {
                return this.company;
            }
        }

        public string Product
        {
            get
            {
                return this.product;
            }
        }

        public string Copyright
        {
            get
            {
                return this.copyright;
            }
        }

        public string Trademark
        {
            get
            {
                return this.trademark;
            }
        }

        public ICultureIdentifier Culture
        {
            get
            {
                return this.culture;
            }
        }

        public Version FileVersion
        {
            get { return this.fileVersion; }
        }

        public Version AssemblyVersion
        {
            get { return this.assemblyVersion; }
        }

        #endregion
    }
}
