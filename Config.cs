namespace RunAndTag;

public class Config(
    uint width,
    uint height,
    string title,
    uint antialiasingLevel,
    uint playerViewRayCount,
    string address,
    int port,
    int tps
)
{
    public uint Width = width;
    public uint Height = height;
    public string Title = title;

    public uint AntialiasingLevel = antialiasingLevel;
    public uint PlayerViewRayCount = playerViewRayCount;
    
    public string Address = address;
    public int Port = port;
    public int Tps = tps;
}