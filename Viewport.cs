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

    private int _tilePreset;
    private int _playerPreset;
    private int _rayPreset;

    public void OnSetup(Render render, uint width, uint height)
    {
        _tilePreset = render.AddRectanglePreset();
        _playerPreset = render.AddCirclePreset(PlayerSize);
        _rayPreset = render.AddLinePreset(new Color(148, 41, 61), new Color(237, 45, 81));
    }

    public void OnRender(Render render, uint width, uint height)
    {
        var tileSize = GetTileSize(width, height);
        RenderPlayerView(render, world.Me, tileSize);
        RenderPlayerView(render, world.Friend, tileSize);
        RenderTiles(render, tileSize);
        RenderPlayer(render, world.Me, tileSize);
        RenderPlayer(render, world.Friend, tileSize);
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
            var currentDirectionInRadians = float.DegreesToRadians(currentDirection);
            _rayMath.Release(player, world.Map, currentDirectionInRadians, player.MaxRayDistance,
                player.RayStep);

            if (rayIndex == 0 || rayIndex == config.PlayerViewRayCount - 1) RenderFullRay(render, player, tileSize);
            else if (!IsNeighbourTile()) RenderPartialRay(render, tileSize);

            _lastPlayerViewRay.Copy(_rayMath.ResultRay);
        }
    }

    private void RenderFullRay(Render render, Player player, float tileSize) =>
        RenderRay(render, player.X * tileSize, player.Y * tileSize, tileSize);

    private void RenderPartialRay(Render render, float tileSize) =>
        RenderRay(render, _lastPlayerViewRay.Target.X * tileSize, _lastPlayerViewRay.Target.Y * tileSize, tileSize);

    private void RenderRay(Render render, float x, float y, float tileSize) => render.PerformLine(
        _rayPreset,
        x,
        y,
        _rayMath.ResultRay.Target.X * tileSize,
        _rayMath.ResultRay.Target.Y * tileSize
    );

    private bool IsNeighbourTile()
    {
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