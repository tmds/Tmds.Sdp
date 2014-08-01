//Copyright (C) 2014  Tom Deseyn

//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.

//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tmds.Sdp
{
    class Attribute
    {
        public const string Category = "cat"; // string
        public const string Keywords = "keywds"; // string
        public const string PacketTime = "ptime"; // time in milliseconds
        public const string MaxPacketTime = "maxptime"; // time in milliseconds
        public const string RtpEncoding = "rtpmap"; // <pt> <encoding>/<clockrate>[/encoding params]
        public const string ReceiveOnly = "recvonly"; // (bool)
        public const string SendReceive = "sendrecv"; // (bool)
        public const string SendOnly = "sendonly"; // (bool)
        public const string Inactive = "inactive"; // (bool)
        public const string Orientation = "orient"; // portrait/landscape/seascape
        public const string ConferenceType = "type"; // broadcast/meeting/moderated/test/H332
        public const string CharacterSet = "charset"; // IANA registered character set identifier
        public const string SdpLanguage = "sdplang"; // rfc3066 tag
        public const string Language = "lang"; // rfc3066 tag
        public const string Framerate = "framerate"; // <integer>.<fraction>
        public const string Quality = "quality"; // 0-10
        public const string FormatParameters = "fmtp"; // format + format specific parameters
    }
    class AttributeValue
    {
        public const string OrientationPortrait = "portrait";
        public const string OrientationLandscape = "landscape";
        public const string OrientationSeascape = "seascape";

        public const string ConferenceBroadcast = "broadcast";
        public const string ConferenceMeeting = "meeting";
        public const string ConferenceModerated = "moderated";
        public const string ConferenceTest = "test";
        public const string ConferenceH332 = "H332";

        public const int QualityBest = 10;
        public const int QualityDefault = 5;
        public const int QualityWorst = 0;
    }
}
