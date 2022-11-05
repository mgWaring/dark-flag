using System;

namespace RelaySystem.Data {
    public struct RelayHostData {
        public string name;
        public string joinCode;
        public string pv4Address;
        public ushort port;
        public Guid allocationID;
        public byte[] allocationIDBytes;
        public byte[] connectionData;
        public byte[] key;
    }
}