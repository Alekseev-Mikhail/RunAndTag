using Client;
using Core;
using RunAndTag;
using SFML.Window;

const string tileSet = "###########" +
                       "#         #" +
                       "#  #  ##  #" +
                       "#     ##  #" +
                       "#  #      #" +
                       "# #####   #" +
                       "# #   # # #" +
                       "#     # # #" +
                       "# #   # # #" +
                       "# #       #" +
                       "###########";
const int mapWidth = 11;

var player = new Player(5f, 2.8f, 0f, 0.01f);
var map = new Map(tileSet, mapWidth, '#');

var preSettings = new ContextSettings { AntialiasingLevel = 8 };
var settings = new Settings(0.01f, 20f, 75f, 3f);
var movement = new MovementController(player, map, settings);

var viewport = new Viewport(player, map, settings, 200);
var window = new GameWindow(400, 400, "Run 'n' Tag", Styles.Close, preSettings, settings, viewport);

var client = new GameClient();

client.Connect("localhost", 8080, 60);
window.BindKeyboardController(movement);
window.StartBlocking();