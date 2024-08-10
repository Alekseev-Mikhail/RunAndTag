using Client;
using Core;
using RunAndTagCore;
using SFML.Graphics;

namespace RunAndTag;

public class Viewport(World world, Config config) : IViewport
{
    private readonly RayMath _rayMath = new();
    private readonly Ray _lastPlayerViewRay = new();

    private const float PlayerSize = 5f;
    private static readonly Color RayColor = new(237, 62, 94);

    private int _tilePreset;
    private int _playerPreset;

    public void OnSetup(Render render, uint width, uint height)
    {
        _tilePreset = render.AddRectanglePreset();
        _playerPreset = render.AddCirclePreset(PlayerSize);
    }

    public void OnRender(Render render, uint width, uint height)
    {
        var tileSize = GetTileSize(width, height);
        var canSeeFriend = true;

        if (world.Role == GameRole.Seeker) canSeeFriend = IsFriendInViewArea();

        RenderPlayerView(render, world.Me, tileSize);
        if (canSeeFriend) RenderPlayerView(render, world.Friend, tileSize);
        RenderTiles(render, tileSize);
        RenderPlayer(render, world.Me, tileSize);
        if (canSeeFriend) RenderPlayer(render, world.Friend, tileSize);
    }

    private bool IsFriendInViewArea()
    {
        var distance = EngineMath.DistanceBetweenTwoPoints(world.Me.X, world.Friend.X, world.Me.Y, world.Friend.Y);
        var angle = float.RadiansToDegrees(float.Atan2(world.Friend.Y - world.Me.Y, world.Friend.X - world.Me.X));
        var halfOfFov = world.Me.Fov / 2;
        var leftViewEdge = world.Me.Direction - halfOfFov;
        var rightViewEdge = world.Me.Direction + halfOfFov;

        if (angle < 0) angle = 360 + angle;
        if (!(distance < world.Me.MaxRayDistance)) return false;
        if (leftViewEdge < 0 && (angle > leftViewEdge + 360 || angle < rightViewEdge) && !IsBehindWall(angle, distance))
            return true;
        if (rightViewEdge > 360 && (angle > leftViewEdge || angle < rightViewEdge - 360) && !IsBehindWall(angle, distance))
            return true;
        return angle > leftViewEdge && angle < rightViewEdge && !IsBehindWall(angle, distance);
    }

    private bool IsBehindWall(float angle, float distance)
    {
        _rayMath.Release(world.Me, world.Map, angle, distance, world.Me.RayStep);
        return _rayMath.ResultRay.IsWallExist;
    }

    private void RenderTiles(Render render, float tileSize)
    {
        for (var y = 0; y < world.Map.Height; y++)
        {
            for (var x = 0; x < world.Map.Width; x++)
            {
                if (world.Map.GetTile(x, y) != world.Map.WallTile) continue;
                render.PerformRectangle(_tilePreset, x * tileSize, y * tileSize, tileSize, tileSize);
            }
        }
    }

    private void RenderPlayer(Render render, Player player, float tileSize) => render.PerformCircle(
        _playerPreset,
        player.X * tileSize - PlayerSize,
        player.Y * tileSize - PlayerSize
    );

    private void RenderPlayerView(Render render, Player player, float tileSize)
    {
        var startDirection = player.Direction - player.Fov / 2f;
        var degreePerRay = player.Fov / config.PlayerViewRayCount;

        for (var rayIndex = 0; rayIndex < config.PlayerViewRayCount; rayIndex++)
        {
            var currentDirection = startDirection + rayIndex * degreePerRay;
            _rayMath.Release(player, world.Map, currentDirection, player.MaxRayDistance, player.RayStep);

            if (rayIndex == 0 || rayIndex == config.PlayerViewRayCount - 1) RenderFullRay(render, player, tileSize);
            else if (!IsNeighbourTile()) RenderPartialRay(render, tileSize);
            else if (!_rayMath.ResultRay.IsWallExist) RenderEndOfRay(render, tileSize);

            _lastPlayerViewRay.Copy(_rayMath.ResultRay);
        }
    }

    private void RenderFullRay(Render render, Player player, float tileSize) =>
        RenderRay(render, player.X * tileSize, player.Y * tileSize, tileSize);

    private void RenderPartialRay(Render render, float tileSize) =>
        RenderRay(render, _lastPlayerViewRay.Target.X * tileSize, _lastPlayerViewRay.Target.Y * tileSize, tileSize);

    private void RenderEndOfRay(Render render, float tileSize) =>
        render.PerformPoint(_rayMath.ResultRay.Target.X * tileSize, _rayMath.ResultRay.Target.Y * tileSize, RayColor);

    private void RenderRay(Render render, float x, float y, float tileSize) => render.PerformLine(
        x,
        y,
        _rayMath.ResultRay.Target.X * tileSize,
        _rayMath.ResultRay.Target.Y * tileSize,
        RayColor
    );

    private bool IsNeighbourTile()
    {
        if (_rayMath.ResultRay.IsWallExist != _lastPlayerViewRay.IsWallExist) return false;

        var lastTileX = (int)_lastPlayerViewRay.Target.X;
        var lastTileY = (int)_lastPlayerViewRay.Target.Y;
        var tileX = (int)_rayMath.ResultRay.Target.X;
        var tileY = (int)_rayMath.ResultRay.Target.Y;

        return (lastTileX == tileX + 1 || lastTileX == tileX || lastTileX == tileX - 1) &&
               (lastTileY == tileY + 1 || lastTileY == tileY || lastTileY == tileY - 1);
    }

    private float GetTileSize(uint width, uint height)
    {
        return (float)Math.Min(width, height) / Math.Max(world.Map.Width, world.Map.Height);
    }
}