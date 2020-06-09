using System;
using System.Collections.Generic;
using System.Text;

namespace PoopChuteLib
{
    /// <summary> The packet type (the type of the packet) </summary>
    public enum PacketType : ushort
    {
        /// <summary>
        /// Null packet. Contains nothing. Nothing at all. Just like you.
        /// </summary>
        NULL,

        /// <summary>
        /// Specifies that this packet contains authentication information.
        /// </summary>
        AUTH,

        /// <summary>
        /// Specifies that this packet is a request for all the users in the current group.
        /// </summary>
        LIST,

        /// <summary>
        /// Specifies that this packet is a request to join a group.
        /// </summary>
        SETG,

        /// <summary>
        /// Specifies that this packet contains a user@machine string.
        /// </summary>
        NAME,

        /// <summary>
        /// Specifies that this packet contains an error message. fuck
        /// </summary>
        FUCK,

        /// <summary>
        /// Specifies that the connection is about to go down.
        /// </summary>
        DEAD,

        /// <summary>
        /// Specifies the mode this client is operating in. Payload of 0x00 is DAEMON, 0x01 is RECEIVE, 0x02 is SEND
        /// </summary>
        MODE

    }
}
