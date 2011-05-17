using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Windows.Media;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration.Classifications
{
    [ClassificationType(ClassificationTypeNames = "OILexer: Rule Reference")]
    [Export(typeof(EditorFormatDefinition))]
    [Name("OILexer: Rule Reference")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    public class GDRuleReference :
        ClassificationFormatDefinition
    {
        public GDRuleReference()
        {
            this.ForegroundColor = Colors.Purple;
            this.IsBold = true;
            this.ForegroundCustomizable = true;
            this.DisplayName = "OILexer: Rule Reference";
        }
    }
}
