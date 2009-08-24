using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Oilexer.Parser
{
    public sealed class StringStream :
        Stream
    {
        private enum ErrorID
        {
            SeekBeforeBeginning,
            ActionOnDisposed
        }
        private string streamData;
        private int position;
        public StringStream(string streamData)
            :
            base()
        {
            this.streamData = streamData;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            return;
        }

        public override long Length
        {
            get { return this.streamData.Length; }
        }

        public override long Position
        {
            get
            {
                return (long)this.position;
            }
            set
            {
                if (value > (long)int.MaxValue)
                    throw new ArgumentOutOfRangeException("value");
                else if (value < 0)
                    ThrowError(ErrorID.SeekBeforeBeginning);
                this.position = (int)value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");
            if (buffer.Length - offset < count)
                throw new ArgumentOutOfRangeException("count");
            int length = (int)this.Length - (int)this.Position;
            if (length > count)
                length = count;
            if (length <= 0)
                return 0;
            for (int i = 0; i < length; i++)
                buffer[offset + i] = (byte)this.streamData[this.position + i];
            this.position += length;
            return length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset > (long)int.MaxValue)
                throw new ArgumentOutOfRangeException("offset");
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0)
                        this.ThrowError(ErrorID.SeekBeforeBeginning);
                    this.position = (int)offset;
                    break;
                case SeekOrigin.Current:
                    if (this.position + offset < 0)
                        this.ThrowError(ErrorID.SeekBeforeBeginning);
                    this.position += (int)offset;
                    break;
                case SeekOrigin.End:
                    if (this.Length + offset < 0)
                        this.ThrowError(ErrorID.SeekBeforeBeginning);
                    this.position = (int)this.Length + (int)offset;
                    break;
                default:
                    break;
            }
            return (long)this.position;
        }

        private void ThrowError(ErrorID error)
        {
            switch (error)
            {
                case ErrorID.SeekBeforeBeginning:
                    throw new IOException("A seek was attempted that was before the beginning of the stream.");
                case ErrorID.ActionOnDisposed:
                    throw new ObjectDisposedException(this.GetType().Name);
                default:
                    break;
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("SetLength not supported.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Write not supported.");
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                this.position = 0;
                this.streamData = null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
