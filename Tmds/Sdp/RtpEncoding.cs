using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tmds.Sdp
{
    public class RtpEncoding
    {
        // video
        public const string Raw = "raw";
        public const string H264 = "H264";
        public const string H264Svc = "H264-SVC";
        public const string Jpeg2000 = "jpeg2000";
        public const string Mpeg4Visual = "MP4V-ES";

        // audio
        public const string L8 = "L8";
        public const string L16 = "L16";
        public const string L24 = "L24";
        public const string Pcmu = "PCMU";
        public const string AC3 = "ac3";
        public const string EnhancedAC3 = "eac3";
        public const string Midi = "rtp-midi";
        public const string Vorbis = "vorbis";
        public const string Speex = "speex";
        public const string Mpeg4Audio = "MP4A-LATM";

        // multi
        public const string Mpeg4ES = "mpeg4-generic";
    }
}
