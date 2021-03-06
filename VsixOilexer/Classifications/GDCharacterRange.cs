﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration.Classifications
{
    [ClassificationType(ClassificationTypeNames = "OILexer: Character Range")]
    [Export(typeof(EditorFormatDefinition))]
    [Name("OILexer: Character Range")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    public class GDCharacterRange :
        ClassificationFormatDefinition
    {
        public GDCharacterRange()
        {
            this.ForegroundColor = Colors.DarkGray;
            this.ForegroundCustomizable = true;
            this.DisplayName = "OILexer: Character Range";
        }
    }
}
