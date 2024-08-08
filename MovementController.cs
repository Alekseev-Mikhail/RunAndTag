using Client;
using Core;
using RunAndTagCore;
using SFML.Window;

namespace RunAndTag;

public class MovementController(World world) : KeyboardController
{
    private readonly RayMath _rayMath = new();

    public bool WasMovement;
    public byte LastInputIndex;
    public byte UpMoveCount;
    public byte DownMoveCount;
    public byte LeftMoveCount;
    public byte RightMoveCount;

    public override void OnSetup()
    {
        AddRepeatKey(Keyboard.Key.Left, () => RotatePlayer(-world.Me.RotationVelocity));
        AddRepeatKey(Keyboard.Key.Right, () => RotatePlayer(world.Me.RotationVelocity));

        AddRepeatKey(Keyboard.Key.W, () =>
        {
            UpMoveCount++;
            MovePlayer(270f);
        });
        AddRepeatKey(Keyboard.Key.S, () =>
        {
            DownMoveCount++;
            MovePlayer(90f);
        });
        AddRepeatKey(Keyboard.Key.A, () =>
        {
            LeftMoveCount++;
            MovePlayer(180f);
        });
        AddRepeatKey(Keyboard.Key.D, () =>
        {
            RightMoveCount++;
            MovePlayer(0f);
        });
    }

    public void Reset()
    {
        UpMoveCount = 0;
        DownMoveCount = 0;
        LeftMoveCount = 0;
        RightMoveCount = 0;
    }
    
    public override void OnKeyPressed(KeyEventArgs args) => TurnOnRepeatKey(args.Code);

    public override void OnKeyReleased(KeyEventArgs args) => TurnOffRepeatKey(args.Code);

    private void RotatePlayer(float delta) => world.Me.Direction += delta;

    private void MovePlayer(float direction)
    {
        WasMovement = true;
        LastInputIndex++;
        _rayMath.Step(world.Me, world.Map, float.DegreesToRadians(direction), world.Me.MovementVelocity);
        
        if (_rayMath.ResultRay.IsWallExist) return;
        world.Me.X = _rayMath.ResultRay.Target.X;
        world.Me.Y = _rayMath.ResultRay.Target.Y;
    }
}