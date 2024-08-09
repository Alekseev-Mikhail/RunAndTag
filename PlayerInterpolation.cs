using RunAndTagCore;
using SFML.System;
using Timer = System.Timers.Timer;

namespace RunAndTag;

public class PlayerInterpolation
{
    private readonly World _world;
    private readonly Timer _timer;
    private readonly float _duration;

    private readonly List<Vector2f> _points = [];
    private int _step = 1;
    private float _lastX;
    private float _lastY;

    public PlayerInterpolation(World world, float duration)
    {
        _world = world;
        _timer = new Timer(1);
        _duration = duration;
        
        _timer.Elapsed += (_, _) => Interpolate();
    }

    public void AddPointAndStart(float x, float y)
    {
        _points.Add(new Vector2f(x, y));

        _timer.Start();
    }
    
    private void Interpolate()
    {
        if (_step > _duration)
        {
            _points.RemoveAt(0);
            _lastX = _world.Friend.X;
            _lastY = _world.Friend.Y;
            _step = 1;
            
            if (_points.Count == 0) _timer.Stop();
            return;
        }

        _step++;
        
        var factor = (_duration - _step) / _duration;
        _world.Friend.X = _points[0].X * (1 - factor) + _lastX * factor;
        _world.Friend.Y = _points[0].Y * (1 - factor) + _lastY * factor;
    }
}