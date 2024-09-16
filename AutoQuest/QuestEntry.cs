using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Runtime.Loader;

namespace AutoQuest
{
    public class QuestEntry : IDalamudPlugin
    {
        public string Name => GetType().Name;
        public static DalamudPluginInterface PluginInterface;

        public QuestEntry(DalamudPluginInterface pluginInterface)
        {
            PluginInterface = pluginInterface;
            ECommonsMain.Init(pluginInterface, this);
            SkipManager.Instance.Init();
            AutoQuestManager.Instance.Init();
            Svc.PluginInterface.UiBuilder.Draw += DrawEntrySetting;
            Svc.PluginInterface.UiBuilder.OpenMainUi += UiBuilder_OpenMainUi;
        }

        private void UiBuilder_OpenMainUi()
        {
            Show = !Show;
        }

        public void Dispose()
        {
            AutoQuestManager.Instance.Dispose();
            SkipManager.Instance.Dispose();
            TaskManager.Instance.Dispose();
            Svc.PluginInterface.UiBuilder.Draw -= DrawEntrySetting;
        }
        private bool Show = true;
        public void DrawEntrySetting()
        {
            if (Show)
            {
                ImGui.Begin("Setting", ref Show);
                ImGuiEx.EzTabBar("Questddw", ("Setting", SettingWindows.DrawSetting, null, true), ("Debug", SettingWindows.DrawDebug, null, true));
            }
        }
    }
}
