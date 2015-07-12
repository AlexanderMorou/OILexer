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
    [ClassificationType(ClassificationTypeNames = "OILexer: Whitespace")]
    [Export(typeof(EditorFormatDefinition))]
    [Name("OILexer: Whitespace")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    public class GDWhitespace :
        ClassificationFormatDefinition
    {
        public GDWhitespace()
        {
            this.ForegroundColor = Colors.White;
            this.DisplayName = "OILexer: Whitespace";
            this.ForegroundCustomizable = true;
            this.BackgroundCustomizable = true;
        }
    }
}
