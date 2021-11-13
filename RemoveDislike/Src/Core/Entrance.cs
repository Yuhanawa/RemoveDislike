using RemoveDislike.Core.Clean;

namespace RemoveDislike.Core
{
    public static class Entrance
    {
        public static Config Config;
        public static Cleaner Cleaner;

        public static void Init()
        {
            Config = new Config();
            Cleaner= new Cleaner();
        }
    }
}