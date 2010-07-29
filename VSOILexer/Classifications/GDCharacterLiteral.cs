﻿using System;
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
    [ClassificationType(ClassificationTypeNames = "OILexer: Literal: Character")]
    [Export(typeof(EditorFormatDefinition))]
    [Name("OILexer: Literal: Character")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    public class GDCharacterLiteral :
        ClassificationFormatDefinition
    {
        public GDCharacterLiteral()
        {
            this.ForegroundColor = Colors.Maroon;
            this.ForegroundCustomizable = true;
            this.DisplayName = "OILexer: Literal: Character";
        }
    }
}