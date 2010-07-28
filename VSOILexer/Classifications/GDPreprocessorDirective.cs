using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Windows.Media;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Oilexer.VSIntegration.Classifications
{
    [ClassificationType(ClassificationTypeNames = "OILexer: Preprocessor Directive")]
    [Export(typeof(EditorFormatDefinition))]
    [Name("OILexer: Preprocessor Directive")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    public class GDPreprocessorDirective :
        ClassificationFormatDefinition
    {
        public GDPreprocessorDirective()
        {
            this.ForegroundColor = Colors.Blue;
            this.ForegroundCustomizable = true;
            this.DisplayName = "OILexer: Preprocessor Directive";
        }
    }
}
