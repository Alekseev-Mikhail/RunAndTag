using Client;
using ClientNetwork;
using LiteNetLib.Utils;
using RunAndTagCore;

namespace RunAndTag;

public class GameClient : RemoteClient
{
    private readonly LocalWorld _world;
    private readonly GameWindow _window;
    private readonly MovementController _movement;
    
    public GameClient(LocalWorld world, GameWindow window, MovementController movement)
    {
        _world = world;
        _window = window;
        _movement = movement;
        
        Manager.AutoRecycle = true;
    }
    
    protected override void OnMessage(NetDataReader reader)
    {
        var type = reader.GetByte();

        switch (type)
        {
            case NetworkSerializer.FullSnapshotType:
                _world.Update(NetworkSerializer.DeserializeFullSnapshot(reader));
                _window.BindKeyboardController(_movement);
                break;
        }
    }
}