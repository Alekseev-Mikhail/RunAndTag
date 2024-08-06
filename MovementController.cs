using Client;
using Core;
using RunAndTagCore;
using SFML.Window;

namespace RunAndTag;

public class MovementController(LocalWorld world, Config config) : KeyboardController
{
    private readonly RayMath _rayMath = new();

    public override void OnSetup()
    {
        AddRepeatKey(Keyboard.Key.Left, () => RotatePlayer(-config.Sensitivity));
        AddRepeatKey(Keyboard.Key.Right, () => RotatePlayer(config.Sensitivity));

        AddRepeatKey(Keyboard.Key.W, () => MovePlayer(270f));
        AddRepeatKey(Keyboard.Key.S, () => MovePlayer(90f));
        AddRepeatKey(Keyboard.Key.A, () => MovePlayer(180f));
        AddRepeatKey(Keyboard.Key.D, () => MovePlayer(0f));
    }

    public override void OnKeyPressed(KeyEventArgs args) => TurnOnRepeatKey(args.Code);

    public override void OnKeyReleased(KeyEventArgs args) => TurnOffRepeatKey(args.Code);

    private void RotatePlayer(float delta) => world.Me.Direction += delta;

    private void MovePlayer(float direction)
    {
        _rayMath.Step(world.Me, world.Map, float.DegreesToRadians(direction), world.Me.Velocity);

        if (_rayMath.ResultRay.IsWallExist) return;
        world.Me.X = _rayMath.ResultRay.Target.X;
        world.Me.Y = _rayMath.ResultRay.Target.Y;
    }
}