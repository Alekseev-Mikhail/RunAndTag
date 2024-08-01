using Client;
using Core;
using SFML.Window;

namespace RunAndTag;

public class MovementController(Player player, Map map, Settings settings) : KeyboardController
{
    private readonly RayMath _rayMath = new(player, map);

    public override void OnSetup()
    {
        AddRepeatKey(Keyboard.Key.Left, () => RotatePlayer(-settings.Sensitivity));
        AddRepeatKey(Keyboard.Key.Right, () => RotatePlayer(settings.Sensitivity));

        AddRepeatKey(Keyboard.Key.W, () => MovePlayer(270f));
        AddRepeatKey(Keyboard.Key.S, () => MovePlayer(90f));
        AddRepeatKey(Keyboard.Key.A, () => MovePlayer(180f));
        AddRepeatKey(Keyboard.Key.D, () => MovePlayer(0f));
    }

    public override void OnKeyPressed(KeyEventArgs args) => TurnOnRepeatKey(args.Code);

    public override void OnKeyReleased(KeyEventArgs args) => TurnOffRepeatKey(args.Code);

    private void RotatePlayer(float delta) => player.Direction += delta;

    private void MovePlayer(float direction)
    {
        _rayMath.Step(float.DegreesToRadians(direction), player.Velocity);

        if (_rayMath.ResultRay.IsWallExist) return;
        player.X = _rayMath.ResultRay.Target.X;
        player.Y = _rayMath.ResultRay.Target.Y;
    }
}