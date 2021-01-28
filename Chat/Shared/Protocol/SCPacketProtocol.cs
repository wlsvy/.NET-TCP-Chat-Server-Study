﻿namespace Shared.Protocol
{
    public enum SCPacketProtocol : byte
    {
        Invalid,
        SC_Pong,
        SC_Login,
        SC_CreateAccount,
        SC_ChatMessage,
    }
}
