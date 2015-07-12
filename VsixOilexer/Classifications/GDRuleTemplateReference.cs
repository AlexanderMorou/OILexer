using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration.Classifications
{
    [ClassificationType(ClassificationTypeNames = "OILexer: Rule Template Reference")]
    [Export(typeof(EditorFormatDefinition))]
    [Name("OILexer: Rule Template Reference")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    public class GDRuleTemplateReference :
        ClassificationFormatDefinition
    {
        public GDRuleTemplateReference()
        {
            this.ForegroundColor = Colors.Navy;
            this.IsBold = true;
            this.ForegroundCustomizable = true;
            this.DisplayName = "OILexer: Rule Template Reference";
        }
    }
}
