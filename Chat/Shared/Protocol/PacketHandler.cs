namespace Shared.Protocol
{
    public abstract class PacketHandler
    {
        public virtual void HANDLE_SC_Ping_NTF(long sequenceNumber) { }
        public virtual void HANDLE_SC_Login_RSP(long accountId) { }
        public virtual void HANDLE_SC_CreateAccount_RSP(long accountId) { }
        public virtual void HANDLE_CS_Login_REQ(string id, string password) { }
        public virtual void HANDLE_CS_Pong_NTF(long sequenceNumber) { }
        public virtual void HANDLE_CS_CreateAccount_REQ(string id, string password) { }
    }

    public interface ICSPacketHandler
    {
        void HANDLE_CS_Login_REQ(string id, string password) { }
        void HANDLE_CS_Pong_NTF(long sequenceNumber) { }
        void HANDLE_CS_CreateAccount_REQ(string id, string password) { }
    }

    public interface ISCPacketHandler
    {
        void HANDLE_SC_Ping_NTF(long sequenceNumber) { }
        void HANDLE_SC_Login_RSP(long accountId) { }
        void HANDLE_SC_CreateAccount_RSP(long accountId) { }
    }
}
