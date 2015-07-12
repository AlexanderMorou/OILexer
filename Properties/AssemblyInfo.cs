using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("OILexer")]
[assembly: AssemblyDescription("Objectified Intermediate Language Lexical Analysis and Parser Builder")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("None")]
[assembly: AssemblyProduct("OILexer")]
[assembly: AssemblyCopyright("Copyright © Allen C. Copeland Jr. 2008-2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("71a499b8-4d6b-44a3-8845-c5b6e88f7ccf")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("0.5.4.0")]
[assembly: AssemblyFileVersion("0.5.4.0")]
#if DEBUG
[assembly: InternalsVisibleTo("OilexerTests")]
#else
[assembly: InternalsVisibleTo("OilexerTests, PublicKey=00240000048000009400000006020000002400005253413100040000010001009982a9fd0d9f7efc18f1da44cf7a85d43b20fd87abb0e46719ec4bdc1b41acb42ca2c032d667030f4b0ba8db26c3b8952d776743b5f1c23d4b956ccbd80d3200b25c611f4300ad09d361c12ef2801ac5f731c63a2248474cc17c5de83572d8bcd5240e925ac8cf391b2b6cdd18c73ab922ff5ea1871cdcd0917a60b88606a996")]
#endif