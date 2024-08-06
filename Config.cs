namespace RunAndTag;

public class Config(
    uint width,
    uint height,
    string title,
    uint antialiasingLevel,
    float graphicQuality,
    float renderDistance,
    float fov,
    float sensitivity,
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
    public float GraphicQuality = graphicQuality;
    public float RenderDistance = renderDistance;
    public float Fov = fov;
    public float Sensitivity = sensitivity;
    public uint PlayerViewRayCount = playerViewRayCount;
    public string Address = address;
    public int Port = port;
    public int Tps = tps;
}