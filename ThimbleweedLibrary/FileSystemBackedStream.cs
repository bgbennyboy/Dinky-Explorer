//using Syroot.BinaryData;
using System.IO;

namespace ThimbleweedLibrary
{
    class FileSystemBackedStream : Stream
    {
        private FileStream baseStream;

        private void CreateBaseStream()
        {
            string tempname = Path.GetTempFileName();
            baseStream = File.Create(tempname, 2048, FileOptions.DeleteOnClose);
        }

        public FileSystemBackedStream()
        {
            CreateBaseStream();
        }

        public FileSystemBackedStream(byte[] data)
        {
            CreateBaseStream();
            baseStream.Write(data, 0, data.Length);
            Flush();
            baseStream.Position = 0;
        }

        public FileSystemBackedStream(Stream other)
        {
            CreateBaseStream();
            other.Position = 0;
            other.CopyTo(baseStream);
            Flush();
            baseStream.Position = 0;
        }

        public override bool CanRead => baseStream.CanRead;

        public override bool CanSeek => baseStream.CanSeek;

        public override bool CanWrite => baseStream.CanWrite;

        public override long Length => baseStream.Length;

        public override long Position { get => baseStream.Position; set => baseStream.Position = value; }

        public override void Flush()
        {
            baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }

        public byte[] ToArray()
        {
            var pos = baseStream.Position;
            byte[] data = new byte[baseStream.Length];
            baseStream.Position = 0;
            baseStream.Read(data, 0, data.Length);
            baseStream.Position = pos;
            return data;
        }
    }

}