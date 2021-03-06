﻿using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal interface ICaptureTokenStructure :
        IControlledDictionary<string, ICaptureTokenStructuralItem>,
        ICaptureTokenStructuralItem
    {
        ICaptureTokenStructure Union(ICaptureTokenStructure second);
        ICaptureTokenStructure Concat(ICaptureTokenStructuralItem item);
        string ResultedTypeName { get; set; }

        IIntermediateEnumType AggregateSetEnum { get; set; }

        IIntermediateEnumType[] ResultEnumSet { get; set; }

        IIntermediateClassType ResultClass { get; set; }

        IIntermediateInterfaceType ResultInterface { get; set; }
        HashList<HashList<string>> Structures { get; }
    }
}
