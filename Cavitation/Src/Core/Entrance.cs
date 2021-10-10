using Cavitation.Core.Clean;

namespace Cavitation.Core
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