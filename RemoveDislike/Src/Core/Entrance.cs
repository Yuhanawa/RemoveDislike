using RemoveDislike.Core.Clean;

namespace RemoveDislike.Core
{
    public static class Entrance
    {
        public static void Init()
        {
            ConfigHelper.Load();
            Cleaner.Load();
        }

        public static string IGet(string key) => I18NUtils.Get(key);
        
    }
}