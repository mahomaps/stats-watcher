namespace MMStatsWatcher;

[Flags]
public enum UsageFlags : int
{
    AboutScreen = 1,
    OthersScreen = 2,
    SettingsScreen = 4,
    Geolocation = 8,
    Search = 16,
    RouteBuildByFoot = 32,
    RouteBuildByAuto = 64,
    RouteBuildByPt = 128,
    RouteFollow = 256,
    RouteDetails = 512,
}