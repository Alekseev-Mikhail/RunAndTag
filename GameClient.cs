using ClientNetwork;
using LiteNetLib.Utils;
using RunAndTagCore;

namespace RunAndTag;

public class GameClient : RemoteClient
{
    protected override void OnNetworkReceiveEvent(NetDataReader reader)
    {
        var identifier = reader.Get<PlayerIdentifier>();
        Console.WriteLine(identifier.Id);
        Console.WriteLine(identifier.Role);
    }
}