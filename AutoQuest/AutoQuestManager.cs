using ECommons.DalamudServices;
using QuestResolve.QuestStep;

namespace AutoQuest
{
    internal class AutoQuestManager : IDisposable
    {
        public static AutoQuestManager Instance { get; private set; } = new AutoQuestManager();
        public AutoQuestManager()
        {
            Svc.Framework.Update += Framework_Update;
        }
        public void Init()
        {

        }
        internal Stack<QuestWrapper> Quests = new();
        internal IStep? Step = null;
        private void Framework_Update(Dalamud.Plugin.Services.IFramework framework)
        {
            if (Svc.ClientState.LocalPlayer == null)
                return;
            if (Quests.TryPeek(out var quest))
            {
                if (Step != null && !Step.CheckNext)
                {
                    return;
                }
                if (quest.TryGetTask(out var t))
                {
                    Step = t.Start();
                }
            }
        }

        public void Dispose()
        {
            Svc.Framework.Update -= Framework_Update;
        }
    }
}
