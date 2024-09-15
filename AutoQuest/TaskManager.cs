using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;

namespace AutoQuest
{
    internal class TaskManager : IDisposable
    {
        public static TaskManager Instance { get; private set; } = new TaskManager();
        private delegate void EventFinishDelegate(long a1, uint a2, byte a3, uint a4, uint a5);
        private Hook<EventFinishDelegate> OnEventFinish;
        public TaskManager() 
        {
            //E8 ?? ?? ?? ?? 0F ?? ?? ?? ?? ?? ?? 48 ?? ?? ?? 48 ?? ?? ?? 0F 83
            OnEventFinish = Svc.Hook.HookFromAddress(Svc.SigScanner.ScanText("40 ?? 55 41 ?? 41 ?? 41 ?? 48 ?? ?? ?? 33 ?? 45"), new EventFinishDelegate(OnEventFinishDetour));
            OnEventFinish.Enable();
            Svc.Framework.Update += Framework_Update;
        }

        private void Framework_Update(IFramework framework)
        {
            ;
        }

        private void OnEventFinishDetour(long a1, uint a2, byte a3, uint a4, uint a5)
        {
            OnEventFinish.Original(a1, a2, a3, a4, a5);
        }

        public void Dispose()
        {
            OnEventFinish?.Dispose();
            Svc.Framework.Update -= Framework_Update;
        }
    }
}
