using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
namespace Oilexer.Translation
{
    partial class CSharpCodeTranslator
    {
        internal static readonly ReadOnlyDictionary<Type, Keywords> CSharpAutoFormTypeLookup;

        private static ReadOnlyDictionary<Type, Keywords> InitializeCSharpAutoFormTypeLookup()
        {
            //Create a lookup for integral type-name translation from (as an example)
            //'System.String' to 'string'
            Dictionary<Type, Keywords> lookupOriginal = new Dictionary<Type, Keywords>();
            lookupOriginal.Add(typeof(byte), Keywords.Byte);
            lookupOriginal.Add(typeof(sbyte), Keywords.Sbyte);
            lookupOriginal.Add(typeof(ushort), Keywords.Ushort);
            lookupOriginal.Add(typeof(short), Keywords.Short);
            lookupOriginal.Add(typeof(uint), Keywords.Uint);
            lookupOriginal.Add(typeof(int), Keywords.Int);
            lookupOriginal.Add(typeof(ulong), Keywords.Ulong);
            lookupOriginal.Add(typeof(long), Keywords.Long);
            lookupOriginal.Add(typeof(void), Keywords.Void);
            lookupOriginal.Add(typeof(bool), Keywords.Bool);
            lookupOriginal.Add(typeof(char), Keywords.Char);
            lookupOriginal.Add(typeof(decimal), Keywords.Decimal);
            lookupOriginal.Add(typeof(float), Keywords.Float);
            lookupOriginal.Add(typeof(double), Keywords.Double);
            lookupOriginal.Add(typeof(object), Keywords.Object);
            lookupOriginal.Add(typeof(string), Keywords.String);
            return new ReadOnlyDictionary<Type, Keywords>(lookupOriginal);
        }

        public enum Keywords : int
        {
            ///<summary>
            ///CSharp Keyword for "as" which is 2 characters long.
            ///</summary>
            As = -839403585,

            ///<summary>
            ///CSharp Keyword for "do" which is 2 characters long.
            ///</summary>
            Do = -840190020,

            ///<summary>
            ///CSharp Keyword for "if" which is 2 characters long.
            ///</summary>
            If = -840648761,

            ///<summary>
            ///CSharp Keyword for "in" which is 2 characters long.
            ///</summary>
            In = -840124473,

            ///<summary>
            ///CSharp Keyword for "is" which is 2 characters long.
            ///</summary>
            Is = -839403577,

            ///<summary>
            ///CSharp Keyword for "for" which is 3 characters long.
            ///</summary>
            For = 1390811594,

            ///<summary>
            ///CSharp Keyword for "get" which is 3 characters long.
            ///</summary>
            Get = -1741749505,

            ///<summary>
            ///CSharp Keyword for "int" which is 3 characters long.
            ///</summary>
            Int = -1741290739,

            ///<summary>
            ///CSharp Keyword for "new" which is 3 characters long.
            ///</summary>
            New = -2145034023,

            ///<summary>
            ///CSharp Keyword for "out" which is 3 characters long.
            ///</summary>
            Out = -1740700921,

            ///<summary>
            ///CSharp Keyword for "ref" which is 3 characters long.
            ///</summary>
            Ref = 133510650,

            ///<summary>
            ///CSharp Keyword for "set" which is 3 characters long.
            ///</summary>
            Set = -1741749485,

            ///<summary>
            ///CSharp Keyword for "try" which is 3 characters long.
            ///</summary>
            Try = -1693450135,

            ///<summary>
            ///CSharp Keyword for "base" which is 4 characters long.
            ///</summary>
            Base = -1456304535,

            ///<summary>
            ///CSharp Keyword for "bool" which is 4 characters long.
            ///</summary>
            Bool = 798688685,

            ///<summary>
            ///CSharp Keyword for "byte" which is 4 characters long.
            ///</summary>
            Byte = 1274151684,

            ///<summary>
            ///CSharp Keyword for "case" which is 4 characters long.
            ///</summary>
            Case = -1456304536,

            ///<summary>
            ///CSharp Keyword for "char" which is 4 characters long.
            ///</summary>
            Char = -421335326,

            ///<summary>
            ///CSharp Keyword for "else" which is 4 characters long.
            ///</summary>
            Else = -1455976858,

            ///<summary>
            ///CSharp Keyword for "enum" which is 4 characters long.
            ///</summary>
            Enum = 1236039580,

            ///<summary>
            ///CSharp Keyword for "goto" which is 4 characters long.
            ///</summary>
            Goto = -1875246337,

            ///<summary>
            ///CSharp Keyword for "lock" which is 4 characters long.
            ///</summary>
            Lock = 1167972383,

            ///<summary>
            ///CSharp Keyword for "long" which is 4 characters long.
            ///</summary>
            Long = -1885564648,

            ///<summary>
            ///CSharp Keyword for "null" which is 4 characters long.
            ///</summary>
            Null = 1202628576,

            ///<summary>
            ///CSharp Keyword for "this" which is 4 characters long.
            ///</summary>
            This = 1178749465,

            ///<summary>
            ///CSharp Keyword for "true" which is 4 characters long.
            ///</summary>
            True = -292522067,

            ///<summary>
            ///CSharp Keyword for "uint" which is 4 characters long.
            ///</summary>
            Uint = -400453345,

            ///<summary>
            ///CSharp Keyword for "void" which is 4 characters long.
            ///</summary>
            Void = 75909655,

            ///<summary>
            ///CSharp Keyword for "break" which is 5 characters long.
            ///</summary>
            Break = 22182330,

            ///<summary>
            ///CSharp Keyword for "catch" which is 5 characters long.
            ///</summary>
            Catch = 461028241,

            ///<summary>
            ///CSharp Keyword for "class" which is 5 characters long.
            ///</summary>
            Class = 351315815,

            ///<summary>
            ///CSharp Keyword for "const" which is 5 characters long.
            ///</summary>
            Const = 748112283,

            ///<summary>
            ///CSharp Keyword for "event" which is 5 characters long.
            ///</summary>
            Event = -1560446698,

            ///<summary>
            ///CSharp Keyword for "false" which is 5 characters long.
            ///</summary>
            False = -419012663,

            ///<summary>
            ///CSharp Keyword for "fixed" which is 5 characters long.
            ///</summary>
            Fixed = 1709130670,

            ///<summary>
            ///CSharp Keyword for "float" which is 5 characters long.
            ///</summary>
            Float = 807124363,

            ///<summary>
            ///CSharp Keyword for "sbyte" which is 5 characters long.
            ///</summary>
            Sbyte = 1225964229,

            ///<summary>
            ///CSharp Keyword for "short" which is 5 characters long.
            ///</summary>
            Short = 1535819814,

            ///<summary>
            ///CSharp Keyword for "throw" which is 5 characters long.
            ///</summary>
            Throw = 82367591,

            ///<summary>
            ///CSharp Keyword for "ulong" which is 5 characters long.
            ///</summary>
            Ulong = -766853033,

            ///<summary>
            ///CSharp Keyword for "using" which is 5 characters long.
            ///</summary>
            Using = 59180213,

            ///<summary>
            ///CSharp Keyword for "where" which is 5 characters long.
            ///</summary>
            Where = 729250605,

            ///<summary>
            ///CSharp Keyword for "while" which is 5 characters long.
            ///</summary>
            While = 430703593,

            ///<summary>
            ///CSharp Keyword for "yield" which is 5 characters long.
            ///</summary>
            Yield = -1184596614,

            ///<summary>
            ///CSharp Keyword for "double" which is 6 characters long.
            ///</summary>
            Double = 1235497039,

            ///<summary>
            ///CSharp Keyword for "extern" which is 6 characters long.
            ///</summary>
            Extern = 130792137,

            ///<summary>
            ///CSharp Keyword for "object" which is 6 characters long.
            ///</summary>
            Object = -737642562,

            ///<summary>
            ///CSharp Keyword for "params" which is 6 characters long.
            ///</summary>
            Params = -314125111,

            ///<summary>
            ///CSharp Keyword for "public" which is 6 characters long.
            ///</summary>
            Public = -770498947,

            ///<summary>
            ///CSharp Keyword for "return" which is 6 characters long.
            ///</summary>
            Return = -1160069800,

            ///<summary>
            ///CSharp Keyword for "sealed" which is 6 characters long.
            ///</summary>
            Sealed = 765372749,

            ///<summary>
            ///CSharp Keyword for "sizeof" which is 6 characters long.
            ///</summary>
            Sizeof = -1421070506,

            ///<summary>
            ///CSharp Keyword for "static" which is 6 characters long.
            ///</summary>
            Static = -1972197055,

            ///<summary>
            ///CSharp Keyword for "string" which is 6 characters long.
            ///</summary>
            String = 1236128813,

            ///<summary>
            ///CSharp Keyword for "struct" which is 6 characters long.
            ///</summary>
            Struct = 2001916978,

            ///<summary>
            ///CSharp Keyword for "switch" which is 6 characters long.
            ///</summary>
            Switch = 1956447343,

            ///<summary>
            ///CSharp Keyword for "typeof" which is 6 characters long.
            ///</summary>
            Typeof = 2101930777,

            ///<summary>
            ///CSharp Keyword for "unsafe" which is 6 characters long.
            ///</summary>
            Unsafe = -1872410350,

            ///<summary>
            ///CSharp Keyword for "ushort" which is 6 characters long.
            ///</summary>
            Ushort = -718368731,

            ///<summary>
            ///CSharp Keyword for "checked" which is 7 characters long.
            ///</summary>
            Checked = -136748835,

            ///<summary>
            ///CSharp Keyword for "decimal" which is 7 characters long.
            ///</summary>
            Decimal = 2033559650,

            ///<summary>
            ///CSharp Keyword for "default" which is 7 characters long.
            ///</summary>
            Default = 1948332219,

            ///<summary>
            ///CSharp Keyword for "finally" which is 7 characters long.
            ///</summary>
            Finally = -435538119,

            ///<summary>
            ///CSharp Keyword for "foreach" which is 7 characters long.
            ///</summary>
            Foreach = 1184841701,

            ///<summary>
            ///CSharp Keyword for "partial" which is 7 characters long.
            ///</summary>
            Partial = 882274439,

            ///<summary>
            ///CSharp Keyword for "private" which is 7 characters long.
            ///</summary>
            Private = 1801699217,

            ///<summary>
            ///CSharp Keyword for "virtual" which is 7 characters long.
            ///</summary>
            Virtual = 895381333,

            ///<summary>
            ///CSharp Keyword for "abstract" which is 8 characters long.
            ///</summary>
            Abstract = 280883515,

            ///<summary>
            ///CSharp Keyword for "continue" which is 8 characters long.
            ///</summary>
            Continue = 1055810917,

            ///<summary>
            ///CSharp Keyword for "delegate" which is 8 characters long.
            ///</summary>
            Delegate = -1136950777,

            ///<summary>
            ///CSharp Keyword for "explicit" which is 8 characters long.
            ///</summary>
            Explicit = -657828123,

            ///<summary>
            ///CSharp Keyword for "implicit" which is 8 characters long.
            ///</summary>
            Implicit = -698918591,

            ///<summary>
            ///CSharp Keyword for "internal" which is 8 characters long.
            ///</summary>
            Internal = -2005216162,

            ///<summary>
            ///CSharp Keyword for "operator" which is 8 characters long.
            ///</summary>
            Operator = -1316706190,

            ///<summary>
            ///CSharp Keyword for "override" which is 8 characters long.
            ///</summary>
            Override = 345858392,

            ///<summary>
            ///CSharp Keyword for "readonly" which is 8 characters long.
            ///</summary>
            Readonly = 827624532,

            ///<summary>
            ///CSharp Keyword for "volatile" which is 8 characters long.
            ///</summary>
            Volatile = 1453402100,

            ///<summary>
            ///CSharp Keyword for "__arglist" which is 9 characters long.
            ///</summary>
            __Arglist = -543827188,

            ///<summary>
            ///CSharp Keyword for "__makeref" which is 9 characters long.
            ///</summary>
            __Makeref = -2005933233,

            ///<summary>
            ///CSharp Keyword for "__reftype" which is 9 characters long.
            ///</summary>
            __Reftype = 76350221,

            ///<summary>
            ///CSharp Keyword for "interface" which is 9 characters long.
            ///</summary>
            Interface = 775541518,

            ///<summary>
            ///CSharp Keyword for "namespace" which is 9 characters long.
            ///</summary>
            Namespace = -474292063,

            ///<summary>
            ///CSharp Keyword for "protected" which is 9 characters long.
            ///</summary>
            Protected = 170650980,

            ///<summary>
            ///CSharp Keyword for "unchecked" which is 9 characters long.
            ///</summary>
            Unchecked = 2097807219,

            ///<summary>
            ///CSharp Keyword for "__refvalue" which is 10 characters long.
            ///</summary>
            __Refvalue = -1570265131,

            ///<summary>
            ///CSharp Keyword for "stackalloc" which is 10 characters long.
            ///</summary>
            Stackalloc = -1340370832,
        }


    }
}
