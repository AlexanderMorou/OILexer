using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public class PreprocessorIfDirective :
        PreprocessorDirectiveBase,
        IPreprocessorIfDirective,
        IOilexerGrammarEntry
    {
        /// <summary>
        /// Data member for <see cref="Condition"/>.
        /// </summary>
        private IPreprocessorCLogicalOrConditionExp condition;
        /// <summary>
        /// Data member fro <see cref="Next"/>.
        /// </summary>
        private IPreprocessorIfDirective next;
        /// <summary>
        /// Data member for <see cref="Previous"/>.
        /// </summary>
        private IPreprocessorIfDirective previous;

        private DirectiveBody body;

        private string filename;

        private EntryPreprocessorType ppType;
        /// <summary>
        /// Creates a new <see cref="PreprocessorIfDirective"/> with the <paramref name="condition"/>
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="condition">The condition which determines whether the <see cref="IPreprocessorIfDirective"/>
        /// is entered.</param>
        /// <param name="fileName">The file in which the <see cref="PreprocessorIfDirective"/> was declared
        /// in.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorIfDirective"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorIfDirective"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorIfDirective"/> 
        /// was declared at.</param>
        public PreprocessorIfDirective(EntryPreprocessorType ppType, IPreprocessorCLogicalOrConditionExp condition, string filename, int column, int line, long position)
            : base(column, line, position)
        {
            this.filename = filename;
            switch (ppType)
            {
                case EntryPreprocessorType.If:
                case EntryPreprocessorType.IfIn:
                case EntryPreprocessorType.IfNotDefined:
                case EntryPreprocessorType.IfDefined:
                case EntryPreprocessorType.ElseIf:
                case EntryPreprocessorType.ElseIfIn:
                case EntryPreprocessorType.ElseIfDefined:
                case EntryPreprocessorType.Else:
                    break;
                default:
                    throw new ArgumentException("ppType");
            }
            this.ppType = ppType;
            this.condition = condition;
            this.next = null;
            this.previous = null;
        }
        public PreprocessorIfDirective(int column, int line, long position)
            : base(column, line, position)
        {
            this.ppType = EntryPreprocessorType.Else;
        }

        public override EntryPreprocessorType Type
        {
            get
            {
                return this.ppType;
            }
        }

        public override string ToString()
        {
            switch (this.ppType)
            {
                case EntryPreprocessorType.If:
                    return string.Format("#if {0}", this.Condition);
                case EntryPreprocessorType.IfIn:
                    return string.Format("#ifin {0}", this.Condition);
                case EntryPreprocessorType.IfNotDefined:
                    return string.Format("#ifndef {0}", this.Condition);
                case EntryPreprocessorType.IfDefined:
                    return string.Format("#ifdef {0}", this.Condition);
                case EntryPreprocessorType.ElseIf:
                    return string.Format("#elif {0}", this.Condition);
                case EntryPreprocessorType.ElseIfIn:
                    return string.Format("#elifin {0}", this.Condition);
                case EntryPreprocessorType.ElseIfDefined:
                    return string.Format("#elifdef {0}", this.Condition);
                case EntryPreprocessorType.Else:
                    return "#else";
            }
            throw new InvalidOperationException();
        }

        //#region IPreprocessorIfDirective Members

        /// <summary>
        /// Returns the transitionFirst if directive in the series.
        /// </summary>
        public IPreprocessorIfDirective First
        {
            get
            {
                IPreprocessorIfDirective f = this;
                while (f.Previous != null)
                    f = f.Previous;
                return f;
            }
        }

        /// <summary>
        /// Returns the last if directive in the series.
        /// </summary>
        /// <remarks>If the <see cref="Last"/>'s <see cref="Condition"/> is null, 
        /// it was defined as an else directive.</remarks>
        public IPreprocessorIfDirective Last
        {
            get
            {
                IPreprocessorIfDirective f = this;
                while (f.Next != null)
                    f = f.Next;
                return f;
            }
        }
        /// <summary>
        /// Returns the <see cref="IPreprocessorCLogicalOrConditionExp"/> which relates
        /// to the condition of the <see cref="IPreprocessorIfDirective"/>.
        /// </summary>
        public IPreprocessorCLogicalOrConditionExp Condition
        {
            get
            {
                return this.condition;
            }
        }

        /// <summary>
        /// Returns the next <see cref="IPreprocessorIfDirective"/> in the list.
        /// </summary>
        public IPreprocessorIfDirective Next
        {
            get { return this.next; }
            internal set { this.next = value; }
        }

        /// <summary>
        /// Returns hte previous <see cref="IPreprocessorIfDirective"/> in the list.
        /// </summary>
        public IPreprocessorIfDirective Previous
        {
            get { return this.previous; }
            internal set { this.previous = value; }
        }

        //#endregion

        //#region IPreprocessorIfDirective Members


        public IPreprocessorDirectives Body
        {
            get
            {
                if (this.body == null)
                    this.body = new DirectiveBody();
                return this.body;
            }
        }

        protected void Body_Add(IPreprocessorDirective directive)
        {
            ((DirectiveBody)this.Body).Add(directive);
        }
        //#endregion

        protected internal class DirectiveBody :
            ControlledCollection<IPreprocessorDirective>,
            IPreprocessorDirectives
        {
            internal DirectiveBody()
            {

            }

            internal void Add(IPreprocessorDirective directive)
            {
                baseList.Add(directive);
            }
        }

        //#region IOilexerGrammarEntry Members

        public string FileName
        {
            get { return this.filename; }
        }

        //#endregion
    }
}
