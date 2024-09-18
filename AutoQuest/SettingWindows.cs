using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using ImGuiNET;
using System.Diagnostics;

namespace AutoQuest
{
    internal unsafe static class SettingWindows
    {
        private static QuestWrapper? ca;
        private static string fi = "";
        public static void DrawSetting()
        {
            try
            {
                if (ImGui.BeginCombo("选择###alllld", ca?.Quest.Name ?? "None"))
                {
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        ImGui.InputTextWithHint("###shaixuan", "筛选", ref fi, 0x100);
                        foreach (var i in QuestWrapper.AllQuest)
                        {
                            if (sw.ElapsedMilliseconds > 15)
                                break;
                            var name = i.Quest.Name.ToString();
                            if (fi != "" && !name.Contains(fi))
                                continue;
                            if (ImGui.Selectable($"{i.Quest.Name}###sel{i.Quest.RowId}"))
                            {
                                ca = i;
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        sw.Stop();
                        ImGui.EndCombo();
                    }
                }
                if (ImGui.Button("Start") && ca != null)
                {
                    AutoQuestManager.Instance.Quests.Push(ca);
                }
                ImGui.SameLine();
                if (ImGui.Button("Clear"))
                {
                    AutoQuestManager.Instance.Quests.Clear();
                    AutoQuestManager.Instance.Step?.Cancel();
                    AutoQuestManager.Instance.Step = null;
                }
                //ImGui.SameLine();
                //if (ImGui.Button("EventAction"))
                //{
                //    ActionManager.Instance()->UseAction(ActionType.KeyItem, 2003119, Svc.Targets.Target.Struct()->GetObjectID());
                //}
                //ImGui.SameLine();
                //if (ImGui.Button("StartEvent") && ca != null)
                //{
                //    new VoidEvent((FFXIVClientStructs.FFXIV.Client.Game.Event.EventHandler*)ca.QuestEventHandler).EventStart(Svc.Targets.Target.Struct()->GetObjectID());
                //}
            }
            catch (Exception e)
            {
                LogHelper.Error(e.ToString());
            }
        }
        private static bool a;
        private static bool b;
        private static bool c;
        private static List<uint> TestQuest = new List<uint>()
        {
            69016,
            70213,
            69005
        };
        public static void DrawDebug()
        {
            ImGui.Checkbox("all", ref a);
            ImGui.SameLine();
            ImGui.Checkbox("allopcode", ref b);
            ImGui.SameLine();
            ImGui.Checkbox("stop", ref c);
            if (ImGui.CollapsingHeader("Quest"))
            {
                var list = new List<(ushort QuestId, byte Sequence)>();
                unsafe
                {
                    list = QuestManager.Instance()->NormalQuestsSpan.ToArray().Select(x => (x.QuestId, x.Sequence)).ToList();
                }
                //foreach(var test in TestQuest)
                //{
                //    if (!list.Any(x => x.QuestId == (test & 0xffff)))
                //    {
                //        list.Add(((ushort)(test & 0xffff), 0));
                //    }
                //}
                foreach (var quest in list)
                {
                    if (quest.QuestId == 0)
                        continue;
                    QuestWrapper.GetQuestById(quest.QuestId)?.Draw();
                }
            }
            foreach(var D in EventFramework.Instance()->DirectorModule.DirectorList.Span)
            {
                ImGui.Text(D.Value->String0.ToString());
            }
        }
    }
}
