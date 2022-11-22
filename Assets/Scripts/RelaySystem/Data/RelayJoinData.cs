using System;

namespace RelaySystem.Data {
    public struct RelayJoinData
    {
        public string joinCode;
        public string pv4Address;
        public ushort port;
        public Guid allocationID;
        public byte[] allocationIDBytes;
        public byte[] connectionData;
        public byte[] hostConnectionData;
        public byte[] key;
    }
}