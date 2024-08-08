using Core;
using RunAndTagCore;

namespace RunAndTag;

public class World(Map map, Player me, Player friend)
{
    public Map Map = map;
    public Player Me = me;
    public Player Friend = friend;
    public byte Role = GameRole.Unknown;

    public void Update(Map map, Player me, Player friend, byte role)
    {
        Map = map;
        Me = me;
        Friend = friend;
        Role = role;
    }
}