using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal class SyntaxActivationEnumItem :
        SyntaxActivationItem
    {
        public SyntaxActivationEnumItem(TokenEnumFinalData sourceRoot, ProjectConstructor.EnumStateMachineData source)
        {
            this.SourceRoot = sourceRoot;
            this.Source = source;
        }

        public TokenEnumFinalData SourceRoot { get; private set; }
        public ProjectConstructor.EnumStateMachineData Source { get; private set; }

        #region ISyntaxActivationItem Members

        public override SyntaxActivationItemType Type
        {
            get { return SyntaxActivationItemType.EnumItemReference; }
        }

        #endregion
        public override string ToString()
        {
            return string.Format("Enum item {0}", this.Source.Source);
        }
    }
}
