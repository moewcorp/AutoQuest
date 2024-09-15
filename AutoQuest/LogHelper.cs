using ECommons.DalamudServices;

namespace AutoQuest
{
    internal static class LogHelper
    {
        public static void Info(string str) => Svc.Log.Info(str);
        public static void Error(string str) => Svc.Log.Error(str);
        public static void Error(object str) => Svc.Log.Error(str.ToString());
    }
}
