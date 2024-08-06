using Client;
using ClientNetwork;
using LiteNetLib.Utils;
using RunAndTagCore;
using SFML.Window;

namespace RunAndTag;

public class GameClient : RemoteClient
{
    private readonly Config _config;
    private readonly LocalWorld _world;
    private readonly GameWindow _window;
    private readonly MovementController _movement;

    public GameClient(LocalWorld world, Config config)
    {
        var viewport = new Viewport(world, config);

        _config = config;
        _world = world;
        _window = new GameWindow(
            config.Width,
            config.Height,
            config.Title,
            Styles.Close,
            config.AntialiasingLevel,
            viewport
        );
        _movement = new MovementController(world, config);

        Manager.AutoRecycle = true;
    }

    public void Connect() => Connect(_config.Address, _config.Port, _config.Tps);

    public void ShowWindow() => _window.StartBlocking();

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