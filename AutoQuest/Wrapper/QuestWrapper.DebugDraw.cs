using AutoQuest.Excel;
using AutoQuest.Extension;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets2;
using QuestResolve;
using System.Numerics;
using static AutoQuest.QuestEntry;

namespace AutoQuest
{
    internal partial class QuestWrapper
    {
        private bool curs;
        private bool seq;
        public unsafe void Draw()
        {
            if (ImGui.TreeNode($"{Quest.Name} {QuestId} {CurrentSeq} {(long)QuestEventHandler:X}### q{Quest.Name}"))
            {
                try
                {
                    ImGui.SameLine();
                    if (ImGui.Button($"C###c{Quest.Name}"))
                        ImGui.SetClipboardText(((long)QuestEventHandler).ToString("X"));
                    TreeNode.Draw($"Main###main{Quest.Name}", DrawMain);
                    if (ImGui.TreeNode($"QuestMessge###msg{Quest.Name}"))
                    {
                        var seq = Quest.QuestSeqMessages.Where(item => item.Value != null && item.Value.Value.ToString() != "").ToList();
                        if (ImGui.TreeNode($"Seq {seq.Count}###msgseq{Quest.Name}"))
                        {
                            foreach (var item in seq)
                            {
                                ImGui.Text(item.Value.ToString());
                            }
                            ImGui.TreePop();
                        }
                        var todo = Quest.QuestTodoMessages.Where(item => item.Value != null && item.Value.Value.ToString() != "").ToList();
                        if (ImGui.TreeNode($"Todo {todo.Count}###msgtodo{Quest.Name}"))
                        {
                            foreach (var item in todo)
                            {
                                ImGui.Text(item.Value.ToString());
                            }
                            ImGui.TreePop();
                        }
                        var text = Quest.QuestTextMessages.Where(item => item.Value != null && item.Value.Value.ToString() != "").ToList();
                        if (ImGui.TreeNode($"Text {text.Count}###msgText{Quest.Name}"))
                        {
                            foreach (var item in text)
                            {
                                ImGui.Text(item.Value.ToString());
                            }
                            ImGui.TreePop();
                        }
                        ImGui.TreePop();
                    }
                    if (ImGui.TreeNode($"Obj###osd{Quest.Name}"))
                    {
                        try
                        {
                            if (QuestEventHandler != null)
                            {
                                foreach (var a in QuestEventHandler->Listeners)
                                {
                                    foreach (var b in a.Item2)
                                    {
                                        ImGui.Text($"seq:{a.Item1} v:{b}");
                                    }
                                }
                                foreach (var obj in QuestEventHandler->LuaEventHandler.EventHandler.EventObjects)
                                {
                                    if (curs && Quest.QuestListenerParams.Where(x => x.Listener != 0 && x.ActorSpawnSeq != CurrentSeq).Any(x => x.Listener == obj.Value->DataID))
                                        continue;
                                    var o = Svc.Objects.CreateObjectReference((nint)obj.Value);
                                    if (o == null)
                                        continue;

                                    ImGui.Text($"{obj.Value->DataID} {obj.Value->LayoutID} {QuestEventHandler->IsAcceptEvent(o)} {obj.Value->GetObjectKind()} {QuestEventHandler->IsBattleNpcOwner()} {QuestEventHandler->IsBattleNpcTriggerOwner(o)} {QuestEventHandler->IsTargetingPossible(o)} {obj.Value->GetObjectKind()}");
                                    if (Svc.GameGui.WorldToScreen(obj.Value->Position, out var screenPos))
                                    {
                                        ImGui.GetBackgroundDrawList().AddCircleFilled(screenPos, 5, 0xff0000ff);
                                        var fd = obj.Value->DrawObject;
                                        ImGui.GetBackgroundDrawList().AddText(screenPos + new Vector2(0, 5), 0xff0000ff, obj.Value->DataID.ToString() + " " + (fd != null && fd->IsVisible).ToString() + " " + QuestEventHandler->IsAcceptEvent(Svc.Objects.CreateObjectReference((nint)obj.Value)) + " " + ((ulong)(obj.Value->GetObjectID())).ToString("X"));
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error(e.ToString());
                        }
                        finally
                        {
                            ImGui.TreePop();
                        }
                    }
                    if (ImGui.TreeNode($"TodoLocation###t{Quest.Name}"))
                    {
                        foreach (var todo in Quest.TodoParams)
                        {
                            if (todo.ToDoCompleteSeq == 0)
                                continue;
                            foreach (var todolel in todo.ToDoLocation)
                            {
                                if (todolel.Value?.Territory.Value is null)
                                    continue;
                                ImGui.Text($"{todo.ToDoCompleteSeq} {todo.ToDoQty} {todolel.Value?.Territory.Value?.PlaceName.Value?.Name} {todolel.Value?.X} {todolel.Value?.Y} {todolel.Value?.Z} {todolel.Value.Radius} {todolel.Value?.Type} {todolel.Value?.Object.Row}");
                                if (QuestEventHandler != null)
                                {
                                    var s = QuestEventHandler->GetTodoArgs((byte)(todo.ToDoCompleteSeq == 0xff ? (byte)(GetMaxSeq() - 1) : (todo.ToDoCompleteSeq - 1)));
                                    ImGui.SameLine();
                                    ImGui.Text(s.Current.ToString() + " " + s.Max.ToString());
                                }
                            }
                        }
                        if (ImGui.Button($"IsAcceptEvent###isa{Quest.Name}"))
                            LogHelper.Info(QuestEventHandler->IsAcceptEvent(Svc.Targets.Target).ToString());
                        ImGui.TreePop();
                    }
                    if (ImGui.TreeNode($"QuestListener###l{Quest.Name}"))
                    {
                        ImGui.Checkbox($"ShowCurSeq###qlch{Quest.Name}", ref curs);
                        ImGui.SameLine();
                        ImGui.Checkbox($"ShowDeSpawnSeq###qldd{Quest.Name}", ref seq);
                        if (TryTodo(out var todo))
                        {
                            ImGui.SameLine();
                            ImGui.Text($"{todo.level.Territory.Value.PlaceName.Value.Name} {new Vector3(todo.level.X, todo.level.Y, todo.level.Z)} {todo.level.Object.Row} {todo.obj}");
                            ImGui.SameLine();
                            if (ImGui.Button($"MoveTo###mov2{Quest.Name}"))
                            {
                                if (TryGetTask(out var ta))
                                {
                                    ta.Start();
                                }
                            }

                        }
                        var j = 1;
                        for (var i = 0u; i < 64; i++)
                        {
                            var Listener = Quest.QuestListenerParams[i];
                            if (curs && Listener.ActorSpawnSeq != CurrentSeq)
                                continue;
                            if (seq && Listener.ActorDespawnSeq == 0xff)
                                continue;
                            if (Listener.Listener == 0)
                                continue;
                            //if (this.a())
                            {
                                var str = i.ToString().PadLeft(2, '0');
                                foreach (var s in Listener.GetPropertyNameAndValueString().Select(s => s + " "))
                                    str += s;
                                ImGui.Text(str);
                                ImGui.Text(QuestEventHandler->IsAcceptEvent(Listener).ToString() + " " + QuestEventHandler->IsAnnounce(Listener).ToString() + QuestEventHandler->IsQualified(Listener));
                                if (Listener.Listener == 5000000)
                                {
                                    //var obj = GameFunction.GetRangeData(Listener.ConditionValue);
                                    var range = Svc.Data.GetExcelSheet<Level>().GetRow(Listener.ConditionValue);
                                    if (range != null)
                                    //if (obj != nint.Zero)
                                    {
                                        ImGui.Text(QuestEventHandler->IsInEventRange(Listener.ConditionValue).ToString() + " " + range.Territory.Value.PlaceName.Value.Name +(new Vector3(range.X,range.Y,range.Z)).ToString());
                                    }
                                }
                            }
                            //else
                            //    ImGui.Text(i.ToString().PadLeft(2, '0') + ":" + $"Listener:{Listener.Listener:D7} SpawnSeq:{Listener.ActorSpawnSeq} DespawnSeq:{Listener.ActorDespawnSeq} ConditionValue:{Listener.ConditionValue} ConditionType:{Listener.ConditionType} QualifiedBool:{Listener.QualifiedBool} QuestUInt8A:{Listener.QuestUInt8A}");
                        }
                        ImGui.TreePop();
                    }
                    if (ImGui.TreeNode($"QuestParams###qp{Quest.Name}"))
                    {
                        foreach (var s in Quest.QuestParams)
                        {
                            if (s.ScriptArg == 0)
                                continue;
                            ImGui.Text(s.ScriptInstruction + ":" + s.ScriptArg.ToString());
                        }
                        ImGui.TreePop();
                    }
                    if (ImGui.TreeNode($"Lua###lua{Quest.Name}"))
                    {
                        foreach (var (funcName, code) in DecompileCode.Value)
                        {
                            if (ImGui.TreeNode($"{funcName}###code{Quest.Name}{funcName}"))
                            {
                                code.Draw();
                                ImGui.TreePop();
                            }
                        }
                        ImGui.TreePop();
                    }
                    if (ImGui.Button($"Start###btn{Quest.Name}"))
                    {
                        AutoQuestManager.Instance.Quests.Push(this);
                    }
                    ImGui.SameLine();
                    if (ImGui.Button($"Save###aad{Quest.Name}"))
                    {
                        DecompileAndSave();
                    }
                    var d = Quest.QuestListenerParams.Where(s => s.ActorSpawnSeq == CurrentSeq && s.Listener.IsRange()).FirstOrDefault();
                    if (d.ConditionValue != 0)
                    {
                        ImGui.SameLine();
                        if (ImGui.Button("EnterRange"))
                            VoidEvent.SendPackt(new EventEnterRange(d.ConditionValue, QuestId));
                    }
                    if (QuestEventHandler != null)
                    {
                        if (QuestEventHandler->TryGetEventItems(out var results))
                        {
                            for (int i = 0; i < results.Count; i++)
                            {
                                ImGui.Text(results[i].ItemId + " " + results[i].Count.ToString() + " " + results[i].Unk.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }
                finally
                {
                    ImGui.TreePop();
                }
            }
        }
        public unsafe void DrawMain()
        {
            try
            {
                ImGui.Text($"Start {Svc.Data.GetExcelSheet<ENpcResident>().GetRow(Quest.IssuerStart.Row).Singular}#{Quest.IssuerStart.Row} {Quest.IssuerLocation.Value.Info()}");
                foreach (var SeqMessages in Quest.QuestTodoMessages.Where(s => s.Value.Value.ToString() != ""))
                {
                    var seq = SeqMessages.Value.Variable.ToString().Split('_').Last().ToNumber();
                    ImGui.Text($"{seq} - 0xFF  {SeqMessages.Value.Value}");
                    {
                        var loc = Quest.TodoParams.Where(t => t.ToDoCompleteSeq == (seq + 1) || (MaxSeq == (seq+1) && t.ToDoCompleteSeq == 0xff)).First();
                        var i = 0;
                        foreach (var Msg in Quest.QuestListenerParams.Where(l => (l.ActorSpawnSeq == (seq + 1) || (l.ActorSpawnSeq == 0XFF && MaxSeq == (seq + 1))) && l.ActorDespawnSeq != 0xff && l.Listener != 5020000))
                        {
                            ImGui.Text($"    {new QuestListenerString(Msg, Quest.QuestParams.Where(s => s.ScriptInstruction.ToString().Contains("INSTANCEDUNGEON")).FirstOrDefault().ScriptArg)} {loc.ToDoLocation[i++].Value.Info()}");
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
    public class TreeNode : IDisposable
    {
        bool ret = false;
        public TreeNode(string name)
        {
            ret = ImGui.TreeNode(name);
        }
        public void Dispose()
        {
            if (ret)
            {
                ImGui.TreePop();
            }
        }
        public static void Draw(string name, System.Action draw)
        {
            using (var d = new TreeNode(name))
            {
                if(d.ret)
                {
                    draw.Invoke();
                }
            }
        }
    }
}
