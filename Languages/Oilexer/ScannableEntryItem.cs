using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public abstract class ScannableEntryItem :
        IScannableEntryItem
    {
        /// <summary>
        /// Data member for <see cref="Column"/>.
        /// </summary>
        private int column;
        /// <summary>
        /// Data member for <see cref="Line"/>.
        /// </summary>
        private int line;
        /// <summary>
        /// Data member for <see cref="Position"/>.
        /// </summary>
        private long position;

        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="RepeatOptions"/>.
        /// </summary>
        private ScannableEntryItemRepeatInfo repeatOptions;

        /// <summary>
        /// Creates a new <see cref="ScannableEntryItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="ScannableEntryItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="ScannableEntryItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="ScannableEntryItem"/> was declared.</param>
        protected ScannableEntryItem(int column, int line, long position)
        {
            this.column = column;
            this.line = line;
            this.position = position;
        }

        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="ScannableEntryItem"/> was declared at.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Returns the line index the <see cref="ScannableEntryItem"/> was declared at.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Returns the position in the file the <see cref="ScannableEntryItem"/> was declared at.
        /// </summary>
        public long Position
        {
            get { return this.position; }
        }

        //#region IScannableEntryItem Members

        /// <summary>
        /// Returns the name of the <see cref="ScannableEntryItem"/>, if it was defined.
        /// </summary>
        /// <remarks>Can be null.</remarks>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Returns the repeat options of the <see cref="ScannableEntryItem"/>
        /// </summary>
        public ScannableEntryItemRepeatInfo RepeatOptions
        {
            get { return this.repeatOptions; }
            set
            {
                this.repeatOptions = value;
            }
        }

        /// <summary>
        /// Creates a copy of the current <see cref="ScannableEntryItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ScannableEntryItem"/> with the data
        /// members of the current <see cref="ScannableEntryItem"/>.</returns>
        public IScannableEntryItem Clone()
        {
            return ((IScannableEntryItem)(this.OnClone()));
        }

        //#endregion

        /// <summary>
        /// Type-inspecific 'onclone' to allow for hiding of <see cref="Clone()"/>.
        /// </summary>
        /// <returns>Varies on implementation.</returns>
        protected abstract object OnClone();

        /// <summary>
        /// Copies the data-members of the current <see cref="ScannableEntryItem"/> to the <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The <see cref="IScannableEntryItem"/> which needs
        /// the data copied.</param>
        /// <remarks>Supplement to <see cref="OnClone()"/>.</remarks>
        protected internal virtual void CloneData(IScannableEntryItem target)
        {
            if (target.RepeatOptions == ScannableEntryItemRepeatInfo.None 
             && target.RepeatOptions != this.RepeatOptions)
                target.RepeatOptions = this.repeatOptions;
            if (this is TokenItem)
                ((TokenItem)(target)).SiblingAmbiguity = ((TokenItem)(this)).SiblingAmbiguity;
            if (!string.IsNullOrEmpty(this.name))
                if (target.Name != this.Name)
                    target.Name = name;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this.Name))
                sb.Append(string.Format(":{0};{1}", this.Name, this.ToStringFurtherOptions()));
            sb.Append(repeatOptions.ToString());
            return sb.ToString();
        }

        protected abstract string ToStringFurtherOptions();
    }
}
