using Client;
using Core;
using SFML.Graphics;

namespace RunAndTag;

public class Viewport(Player player, Map map, Settings settings, uint playerViewRayCount) : IViewport
{
    private readonly RayMath _rayMath = new(player, map);
    private readonly Ray _lastPlayerViewRay = new();

    private float _tileSize;
    private readonly float _playerSize = 5f;

    private int _tilePreset;
    private int _playerPreset;
    private int _rayPreset;

    public void OnSetup(Render render, uint width, uint height)
    {
        _tileSize = GetTileSize(width, height);

        _tilePreset = render.AddPreset(_tileSize, _tileSize);
        _playerPreset = render.AddPreset(_playerSize);
        _rayPreset = render.AddPreset(new Color(148, 41, 61), new Color(237, 45, 81));
    }

    public void OnRender(Render render, uint width, uint height)
    {
        RenderPlayerView(render);
        RenderTiles(render);
        RenderPlayer(render);
    }

    private void RenderTiles(Render render)
    {
        for (int currentY = 0; currentY < map.Height; currentY++)
        {
            for (int currentX = 0; currentX < map.Width; currentX++)
            {
                if (map.GetTile(currentX, currentY) != map.WallTile) continue;
                render.Perform(_tilePreset, currentX * _tileSize, currentY * _tileSize);
            }
        }
    }

    private void RenderPlayer(Render render) => render.Perform(
        _playerPreset,
        player.X * _tileSize - _playerSize,
        player.Y * _tileSize - _playerSize
    );

    private void RenderPlayerView(Render render)
    {
        var startDirection = player.Direction - settings.FOV / 2f;
        var degreePerRay = settings.FOV / playerViewRayCount;

        for (var rayIndex = 0; rayIndex < playerViewRayCount; rayIndex++)
        {
            var currentDirection = startDirection + rayIndex * degreePerRay;
            var currentDirectionInRadians = float.DegreesToRadians(currentDirection);
            _rayMath.Release(currentDirectionInRadians, settings.RenderDistance, settings.GraphicQuality);

            if (rayIndex == 0 || rayIndex == playerViewRayCount - 1) RenderFullRay(render);
            else if (!IsNeighbourTile()) RenderPartialRay(render);

            _lastPlayerViewRay.Copy(_rayMath.ResultRay);
        }
    }

    private void RenderFullRay(Render render) =>
        RenderRay(render, player.X * _tileSize, player.Y * _tileSize);

    private void RenderPartialRay(Render render) =>
        RenderRay(render, _lastPlayerViewRay.Target.X * _tileSize, _lastPlayerViewRay.Target.Y * _tileSize);

    private void RenderRay(Render render, float x, float y) => render.Perform(
        _rayPreset,
        x,
        y,
        _rayMath.ResultRay.Target.X * _tileSize,
        _rayMath.ResultRay.Target.Y * _tileSize
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
        return width < height ? (float)width / map.Width : (float)height / map.Height;
    }
}