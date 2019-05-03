using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Tests
{
    class RetainContentsStream : Stream
    {
        private MemoryStream internalStream = new MemoryStream();

        protected override void Dispose(bool disposing)
        {
            if (Contents == null)
            {
                Contents = GetStreamContents();
                internalStream.Dispose();
            }
            base.Dispose(disposing);
        }

        private String GetStreamContents()
        {
            internalStream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(internalStream))
            {
                return reader.ReadToEnd();
            }
        }

        public String Contents { get; private set; }

        public override bool CanRead => internalStream.CanRead;

        public override bool CanSeek => internalStream.CanSeek;

        public override bool CanWrite => internalStream.CanWrite;

        public override long Length => internalStream.Length;

        public override long Position { get => internalStream.Position; set => internalStream.Position = value; }

        public override void Flush()
        {
            internalStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return internalStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return internalStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            internalStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            internalStream.Write(buffer, offset, count);
        }
    }
}
