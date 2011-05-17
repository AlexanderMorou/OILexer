// Guids.cs
// MUST match guids.h
using System;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    static class GuidList
    {
        public const string guidVSOILexerPkgString = "7641d00b-5a73-4fb6-87ac-492c77b44f6c";
        public const string guidVSOILexerCmdSetString = "b74b61f4-b194-4feb-94e6-da7a29f86c4d";

        public static readonly Guid guidVSOILexerCmdSet = new Guid(guidVSOILexerCmdSetString);
    };
}