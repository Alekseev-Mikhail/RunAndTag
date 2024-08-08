using Core;
using LiteNetLib.Utils;
using RunAndTagCore;

namespace RunAndTag;

public class ClientSerializer(NetDataWriter writer, World world)
{
    public void SerializeMovementInput(byte inputIndex, byte up, byte down, byte left, byte right)
    {
        writer.Put(MessageType.MovementInputType);
        writer.Put(inputIndex);
        writer.Put(up);
        writer.Put(down);
        writer.Put(left);
        writer.Put(right);
    }

    public void DeserializeFullSnapshot(NetDataReader reader)
    {
        var map = DeserializeMap(reader);
        var seeker = DeserializePlayer(reader);
        var hider = DeserializePlayer(reader);
        var role = reader.GetByte();

        if (role == GameRole.Seeker) world.Update(map, seeker, hider, role);
        else world.Update(map, hider, seeker, role);
    }

    public void DeserializeDeltaSnapshot(NetDataReader reader, byte lastInputIndex)
    {
        var inputIndex = reader.GetByte();
        
        if (inputIndex == lastInputIndex)
        {
            DeserializePosition(world.Me, reader);
            DeserializePosition(world.Friend, reader);
            return;
        }

        DeserializePosition(world.Friend, reader);
        DeserializePosition(world.Friend, reader);
    }

    private static void DeserializePosition(Player player, NetDataReader reader)
    {
        player.X = reader.GetFloat();
        player.Y = reader.GetFloat();
    }

    private static Player DeserializePlayer(NetDataReader reader) => new(
        reader.GetFloat(),
        reader.GetFloat(),
        reader.GetFloat(),
        reader.GetFloat(),
        reader.GetFloat(),
        reader.GetFloat(),
        reader.GetFloat(),
        reader.GetFloat()
    );

    private static Map DeserializeMap(NetDataReader reader)
    {
        var tileSet = reader.GetString();
        var width = reader.GetInt();
        var wallTile = reader.GetChar();
        return new Map(tileSet, width, wallTile);
    }
}