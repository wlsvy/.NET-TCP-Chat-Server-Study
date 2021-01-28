﻿namespace Shared.Protocol
{
    public enum CSPacketProtocol : byte
    {
        Invalid,
        CS_Ping,
        CS_Login,
        CS_CreateAccount,
        CS_ChatMessage,
    }
}
