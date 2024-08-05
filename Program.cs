using Client;
using Core;
using RunAndTag;
using RunAndTagCore;
using SFML.Window;

const string tileSet = "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "  #     ##    #   ###  # #   #  ###         " +
                       "  #    #  #  # #  #  # # #  ## #   #        " +
                       "  #    #  #  # #  #  # # # # # #            " +
                       "  #    #  #  ###  #  # # # # # #            " +
                       "  #    #  # #   # #  # # # # # #  ##        " +
                       "  #    #  # #   # #  # # ##  # #   #        " +
                       "  ####  ##  #   # ###  # #   #  ###  # # #  " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            " +
                       "                                            ";
const int mapWidth = 44;
var loadingScreen = new Map(tileSet, mapWidth, '#');
var emptyPlayer = new Player(2.6f, 24.3f, 0f, 0f);
var world = new LocalWorld(loadingScreen, emptyPlayer, emptyPlayer);

var settings = new Settings(0.01f, 1f, 75f, 3f);
var movement = new MovementController(world, settings);

var viewport = new Viewport(world, settings, 200);
var window = new GameWindow(400, 400, "Run 'n' Tag", Styles.Close, 8, settings, viewport);

var client = new GameClient(world, window, movement);

client.Connect("localhost", 8080, 50);
window.StartBlocking();