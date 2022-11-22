using Unity.Collections;
using Unity.Netcode;

namespace Multiplayer {
  public struct MultiplayerMenuPlayer : INetworkSerializable, System.IEquatable<MultiplayerMenuPlayer>
  {
    public FixedString128Bytes name;
    public int clientId;
    public int shipIndex;
    public bool ready;

    public MultiplayerMenuPlayer(int id)
    {
      clientId = id;
      name = "New Player";
      shipIndex = 0;
      ready = false;
    }

    public MultiplayerMenuPlayer(FixedString128Bytes n, int id, int si, bool r)
    {
      clientId = id;
      name = n;
      shipIndex = si;
      ready = r;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
      if (serializer.IsReader)
      {
        var reader = serializer.GetFastBufferReader();
        reader.ReadValueSafe(out name);
        reader.ReadValueSafe(out clientId);
        reader.ReadValueSafe(out shipIndex);
        reader.ReadValueSafe(out ready);
      }
      else
      {
        var writer = serializer.GetFastBufferWriter();
        writer.WriteValueSafe(name);
        writer.WriteValueSafe(clientId);
        writer.WriteValueSafe(shipIndex);
        writer.WriteValueSafe(ready);
      }
    }

    public bool Equals(MultiplayerMenuPlayer other)
    {
      return clientId == other.clientId && name == other.name && shipIndex == other.shipIndex && ready == other.ready;
    }
  }
}
