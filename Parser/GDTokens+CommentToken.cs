using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    partial class GDTokens
    {
        public class CommentToken :
            GDToken
        {
            private string comment;
            public CommentToken(string comment, int column, int line, long position, bool multiLine=false)
                : base(column, line, position)
            {
                this.MultiLine = multiLine;
                this.comment = comment;
            }
            public bool MultiLine { get; private set; }

            public string Comment
            {
                get
                {
                    return this.comment;
                }
            }

            public override GDTokenType TokenType
            {
                get { return GDTokenType.Comment; }
            }

            public override int Length
            {
                get { return this.comment.Length; }
            }

            public override bool ConsumedFeed
            {
                get { return true; }
            }
        }
    }
}
