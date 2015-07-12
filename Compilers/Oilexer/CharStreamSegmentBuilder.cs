using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Cli;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Abstract.Members;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class CharStreamSegmentBuilder
    {
        private ParserCompiler _compiler;
        private IIntermediateAssembly _assembly;
        private IIntermediateClassType _charStreamSegmentClass;
        private IIntermediateInterfaceType _charStreamSegmentInterface;
        private IIntermediateInterfacePropertyMember _length;
        private IIntermediateClassPropertyMember _lengthImpl;
        private IIntermediateCliManager _identityManager;
        public void Build(ParserCompiler compiler, IIntermediateAssembly assembly)
        {
            this._compiler = compiler;
            this._assembly = assembly;
            this._identityManager = ((IIntermediateCliManager)(assembly.IdentityManager));
            CreateICharStreamSegment(assembly);
            CreateCharStreamSegment(assembly);
        }

        private void CreateICharStreamSegment(IIntermediateAssembly assembly)
        {
            this._charStreamSegmentInterface = assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}CharStreamSegment", this._compiler.Source.Options.AssemblyName);
            this._charStreamSegmentInterface.AccessLevel = AccessLevelModifiers.Public;
            this.CreatePosition();
            this.CreateLength();
        }

        private void CreateLength()
        {
            this._length = this.ICharStreamSegment.Properties.Add(new TypedName("Length", RuntimeCoreType.Int32, this._identityManager), true, false);
        }

        private void CreatePosition()
        {
            this.StartPosition = this.ICharStreamSegment.Properties.Add(new TypedName("StartPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
            this.EndPosition = this.ICharStreamSegment.Properties.Add(new TypedName("EndPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
        }

        private void CreateCharStreamSegment(IIntermediateAssembly assembly)
        {
            this._charStreamSegmentClass = assembly.DefaultNamespace.Parts.Add().Classes.Add("{0}CharStreamSegment", this._compiler.Source.Options.AssemblyName);
            this._charStreamSegmentClass.ImplementedInterfaces.ImplementInterfaceQuick(this._charStreamSegmentInterface);
            this.CreateStartPositionImpl();
            this.CreateEndPositionImpl();
            this.CreateLengthImpl();
            this.CreateOwnerImpl();
            this.LengthImpl.AccessLevel = this.StartPositionImpl.AccessLevel = this.EndPositionImpl.AccessLevel = AccessLevelModifiers.Public;
            this._StartPositionImpl.AccessLevel = this._EndPositionImpl.AccessLevel = AccessLevelModifiers.Private;
            this.CharStreamSegment.AccessLevel = AccessLevelModifiers.Public;
            
        }

        public void Build2(CharStreamBuilder builder)
        {
            this.CharStreamBuilder = builder;
            this.CreateOwnerImpl();
            this.CreateConstructor();
            this.CreateValue();
            this.CreateValueImpl();
        }

        private void CreateValue()
        {
            this.ValuePropImpl = this.CharStreamSegment.Properties.Add(new TypedName("Value", RuntimeCoreType.String, this._identityManager), true, false);
            this.ValuePropImpl.GetMethod.Return(_OwnerImpl.GetReference().GetIndexer(this._StartPositionImpl.GetReference(), this._EndPositionImpl.GetReference()));
            this.ValuePropImpl.AccessLevel = AccessLevelModifiers.Public;
        }

        private void CreateValueImpl()
        {
            this.ValueProp = this.ICharStreamSegment.Properties.Add(new TypedName("Value", RuntimeCoreType.String, this._identityManager), true, false);
        }

        private void CreateConstructor()
        {
            var ctor = this.CharStreamSegment.Constructors.Add(new TypedNameSeries(new TypedName("owner", this.CharStreamBuilder.ICharStream), new TypedName("start", RuntimeCoreType.Int32, this._identityManager), new TypedName("end", RuntimeCoreType.Int32, this._identityManager)));
            var ownerParam = ctor.Parameters["owner"];
            var startParam = ctor.Parameters["start"];
            var endParam = ctor.Parameters["end"];
            ctor.If(ownerParam.EqualTo(IntermediateGateway.NullValue))
                .Throw(this._identityManager.ObtainTypeReference(typeof(ArgumentNullException)).GetNewExpression(ownerParam.Name.ToPrimitive()));
            ctor.Assign(this._OwnerImpl, ownerParam);
            ctor.Assign(this._StartPositionImpl, startParam);
            ctor.Assign(this._EndPositionImpl, endParam);
            ctor.AccessLevel = AccessLevelModifiers.Public;
        }

        private void CreateOwnerImpl()
        {
            if (this.CharStreamBuilder == null)
                return;
            this.OwnerImpl = this.CharStreamSegment.Properties.Add(new TypedName("Owner", this.CharStreamBuilder.ICharStream), true, false);
            this._OwnerImpl = this.CharStreamSegment.Fields.Add(new TypedName("_owner", this.CharStreamBuilder.ICharStream));
            this.OwnerImpl.GetMethod.Return(this._OwnerImpl);
            this.OwnerImpl.AccessLevel = AccessLevelModifiers.Public;
            this._OwnerImpl.AccessLevel = AccessLevelModifiers.Private;
        }

        private void CreateEndPositionImpl()
        {
            this._EndPositionImpl = this.CharStreamSegment.Fields.Add(new TypedName("_endPosition", RuntimeCoreType.Int32, this._identityManager));

            this.EndPositionImpl = this.CharStreamSegment.Properties.Add(new TypedName("EndPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
            this.EndPositionImpl.GetMethod.Return(this._EndPositionImpl);
        }

        private void CreateLengthImpl()
        {

            this._lengthImpl = this.CharStreamSegment.Properties.Add(new TypedName("Length", RuntimeCoreType.Int32, this._identityManager), true, false);
            this.LengthImpl.GetMethod.Return(this._EndPositionImpl.Subtract(this._StartPositionImpl));

        }

        private void CreateStartPositionImpl()
        {
            this._StartPositionImpl = this.CharStreamSegment.Fields.Add(new TypedName("_startPosition", RuntimeCoreType.Int32, this._identityManager));
            this.StartPositionImpl = this.CharStreamSegment.Properties.Add(new TypedName("StartPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
            this.StartPositionImpl.GetMethod.Return(this._StartPositionImpl);
        }


        public IIntermediateClassType CharStreamSegment { get { return this._charStreamSegmentClass; } }
        public IIntermediateInterfaceType ICharStreamSegment { get { return this._charStreamSegmentInterface; } }

        public IIntermediateInterfacePropertyMember Length { get { return this._length; } }
        public IIntermediateInterfacePropertyMember StartPosition { get; set; }
        public IIntermediateInterfacePropertyMember EndPosition { get; set; }

        public IIntermediateClassPropertyMember LengthImpl { get { return this._lengthImpl; } }
        public IIntermediateClassPropertyMember StartPositionImpl { get; set; }

        public IIntermediateClassFieldMember _StartPositionImpl { get; set; }

        public IIntermediateClassFieldMember _EndPositionImpl { get; set; }

        public IIntermediateClassPropertyMember EndPositionImpl { get; set; }

        public CharStreamBuilder CharStreamBuilder { get; set; }

        public IIntermediateClassPropertyMember OwnerImpl { get; set; }

        public IIntermediateClassFieldMember _OwnerImpl { get; set; }

        public IIntermediateClassPropertyMember ValuePropImpl { get; set; }

        public IIntermediateInterfacePropertyMember ValueProp { get; set; }
    }
}
