using Core;
using RunAndTag;
using RunAndTagCore;

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

var config = new Config(
    400,
    400,
    "Run 'n' Tag",
    8,
    0.01f,
    1f,
    75f,
    3f,
    200,
    "localhost",
    8080,
    50
);

var client = new GameClient(world, config);

client.Connect();
client.ShowWindow();