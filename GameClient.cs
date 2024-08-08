using Client;
using ClientNetwork;
using LiteNetLib;
using LiteNetLib.Utils;
using RunAndTagCore;
using SFML.Window;

namespace RunAndTag;

public class GameClient : RemoteClient
{
    private readonly Config _config;
    private readonly GameWindow _window;
    private readonly MovementController _movement;
    private readonly ClientSerializer _serializer;

    public GameClient(World world, Config config)
    {
        var viewport = new Viewport(world, config);

        _config = config;
        _window = new GameWindow(
            config.Width,
            config.Height,
            config.Title,
            Styles.Close,
            config.AntialiasingLevel,
            viewport
        );
        _movement = new MovementController(world);
        _serializer = new ClientSerializer(Writer, world);
    }

    public void Connect() => Connect(_config.Address, _config.Port, _config.Tps);

    public void ShowWindow() => _window.StartBlocking();

    protected override void OnTick()
    {
        if (!_movement.WasMovement) return;
        _movement.WasMovement = false;
        
        _serializer.SerializeMovementInput(
            _movement.LastInputIndex,
            _movement.UpMoveCount,
            _movement.DownMoveCount,
            _movement.LeftMoveCount,
            _movement.RightMoveCount
        );
        Server.Send(Writer, DeliveryMethod.Unreliable);
        _movement.Reset();
        Writer.Reset();
    }

    protected override void OnMessage(NetDataReader reader)
    {
        switch (MessageType.GetType(reader))
        {
            case MessageType.FullSnapshotType:
                _serializer.DeserializeFullSnapshot(reader);
                _window.BindKeyboardController(_movement);
                break;
            case MessageType.DeltaSnapshotType:
                _serializer.DeserializeDeltaSnapshot(reader, _movement.LastInputIndex);
                break;
        }
    }
}