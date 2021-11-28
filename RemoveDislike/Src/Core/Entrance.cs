using RemoveDislike.Core.Clean;

namespace RemoveDislike.Core
{
    public static class Entrance
    {
        public static Cleaner Cleaner { get; private set; }

        public static void Init()
        {
            ConfigHelper.Load();
            Cleaner = new Cleaner();
        }
    }
}