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
    [ClassificationType(ClassificationTypeNames = "OILexer: Rule Template Parameter Reference")]
    [Export(typeof(EditorFormatDefinition))]
    [Name("OILexer: Rule Template Parameter Reference")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    public class GDRuleTemplateParameterReference :
        ClassificationFormatDefinition
    {
        public GDRuleTemplateParameterReference()
        {
            this.ForegroundColor = Colors.DarkGray;
            this.ForegroundCustomizable = true;
            this.DisplayName = "OILexer: Rule Template Parameter Reference";
        }
    }
}
