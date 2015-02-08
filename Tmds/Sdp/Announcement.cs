using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tmds.Sdp
{
    class Announcement
    {
        public enum MessageType
        {
            Announcement,
            Deletion
        }
        public byte Version { get; set; }
        public MessageType Type { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsCompressed { get; set; }
        public ArraySegment<byte> AuthenticationData { get; set; }
        public ushort Hash { get; set; }
        public IPAddress Source { get; set; }
        public string PayloadType { get; set; }
        public ArraySegment<byte> Payload { get; set; }

        public void Decompress()
        {
            if (!IsCompressed)
            {
                return;
            }

            MemoryStream stream = new MemoryStream(Payload.Array, Payload.Offset + 2, Payload.Count - 2);
            DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
            stream = new MemoryStream();
            deflateStream.CopyTo(stream);
            IsCompressed = false;
            Payload = new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
        }
    }
}
