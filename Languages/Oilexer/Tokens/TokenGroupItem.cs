using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class TokenGroupItem :
        ControlledCollection<ITokenExpression>,
        ITokenGroupItem
    {
        /// <summary>
        /// Data member for <see cref="DefaultSoftRefOrValue"/>.
        /// </summary>
        private string defaultSoftRefOrValue;
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
        private bool siblingAmbiguity;

        public TokenGroupItem(string fileName, ITokenExpression[] items, int column, int line, long position)
        {
            this.FileName = fileName;
            foreach (ITokenExpression ipr in items)
                baseList.Add(ipr);
            this.column = column;
            this.line = line;
            this.position = position;
        }

        /// <summary>
        /// Creates a copy of the current <see cref="ITokenGroupItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITokenGroupItem"/> with the data
        /// members of the current <see cref="ITokenGroupItem"/>.</returns>
        public ITokenGroupItem Clone()
        {
            ITokenExpression[] ipr = new ITokenExpression[this.Count];
            base.CopyTo(ipr, 0);
            TokenGroupItem prgi = new TokenGroupItem(this.FileName, ipr, this.Column, this.Line, this.Position);
            prgi.repeatOptions = this.repeatOptions;
            prgi.name = this.name;
            return prgi;
        }
        public bool SiblingAmbiguity
        {
            get { return this.siblingAmbiguity; }
            internal set { this.siblingAmbiguity = value; }
        }

        //#region ITokenItem Members


        public string DefaultSoftRefOrValue
        {
            get { return this.defaultSoftRefOrValue; }
            internal set { this.defaultSoftRefOrValue = value; }
        }

        ITokenItem ITokenItem.Clone()
        {
            return this.Clone();
        }
        //#endregion

        //#region IScannableEntryItem Members

        /// <summary>
        /// Returns the name of the <see cref="TokenGroupItem"/>, if it was defined.
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

        IScannableEntryItem IScannableEntryItem.Clone()
        {
            return this.Clone();
        }

        //#endregion

        public override string ToString()
        {
            return BuildString();
        }

        private string BuildString(bool appendExtra = true)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            bool lastCarriage = false;
            if (this.Count == 0)
            {
                if (appendExtra)
                    sb.Append("()");
                goto ExitPoint;
            }
            foreach (ITokenExpression ite in this.baseList)
            {
                string s = ite.ToString();
                if (first)
                {
                    if (appendExtra)
                    {
                        if (baseList.Count > 1 || s.Contains("\r\n"))
                        {
                            sb.Append("\r\n");
                            lastCarriage = true;
                        }
                        sb.Append("(");
                        if (baseList.Count > 1)
                        {
                            sb.AppendLine();
                            sb.Append("\t");
                        }
                    }
                    first = false;
                }
                else
                {
                    sb.AppendLine(" | ");
                    sb.Append("\t");
                }
                sb.Append(s.Replace("\r\n", "\r\n\t"));
            }
            if (appendExtra)
                if (baseList.Count > 1 || lastCarriage)
                {
                    sb.AppendLine();
                    sb.Append(")");
                }
                else
                    sb.Append(")");
        ExitPoint:
            if (appendExtra)
            {
                if (this.name != null && this.name != string.Empty)
                    sb.Append(string.Format(":{0};{1}", this.Name, this.ToStringFurtherOptions()));
                sb.Append(repeatOptions.ToString());
                string result = sb.ToString();
                sb.Remove(0, sb.Length);
                string[] lines = result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                result = string.Empty;
                for (int i = 0; i < lines.Length; i++)
                    if (string.IsNullOrEmpty(lines[i].Trim().Replace("\t", string.Empty)))
                        continue;
                    else
                    {
                        result += lines[i];
                        if (i < lines.Length - 1)
                            result += "\r\n";
                    }
                if (lastCarriage)
                    return "\r\n" + result;
                else
                    return result;
            }
            else
                return sb.ToString();
        }

        internal string ToStringFurtherOptions()
        {
            if (this.DefaultSoftRefOrValue != null)
            {
                return string.Format("Default={0};", this.DefaultSoftRefOrValue);
            }
            return null;
        }


        //#region ITokenExpressionSeries Members


        public string FileName { get; private set; }

        public string GetBodyString()
        {
            return this.BuildString(false);
        }

        //#endregion

    }
}
