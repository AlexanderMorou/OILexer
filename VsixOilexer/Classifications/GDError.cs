using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using System.Windows;
using System.Windows.Media;


namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration.Classifications
{
    [ClassificationType(ClassificationTypeNames = "OILexer: Error")]
    [Export(typeof(EditorFormatDefinition))]
    [Name("OILexer: Error")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    public class GDError :
        ClassificationFormatDefinition
    {
        private static TextDecoration mainDecoration = new TextDecoration(TextDecorationLocation.Underline, new Pen(Brushes.Red, 1), -2, TextDecorationUnit.Pixel, TextDecorationUnit.Pixel);

        public GDError()
        {
            this.TextDecorations = new TextDecorationCollection(1);
            this.TextDecorations.Add(mainDecoration);
            this.ForegroundCustomizable = true;
            this.DisplayName = "OILexer: Error";
        }
    }
}
