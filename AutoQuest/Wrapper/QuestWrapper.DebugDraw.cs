using AutoQuest.Excel;
using AutoQuest.Extension;
using ECommons;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets2;
using QuestResolve;
using System.Numerics;
using System.Runtime.InteropServices;

namespace AutoQuest
{
    internal partial class QuestWrapper
    {
        private bool curs;
        private bool seq;
        private bool ShowIsAnnounce;
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
                    TreeNode.Draw($"QuestMessge###msg{Quest.Name}", () =>
                    {
                        var seq = Quest.QuestSeqMessages.Where(item => item.Value != null && item.Value.Value.ToString() != "").ToList();
                        TreeNode.Draw($"Seq {seq.Count}###msgseq{Quest.Name}", () =>
                        {
                            foreach (var item in seq)
                            {
                                ImGui.Text(item.Value.ToString());
                            }
                        });
                        var todo = Quest.QuestTodoMessages.Where(item => item.Value != null && item.Value.Value.ToString() != "").ToList();
                        TreeNode.Draw($"Todo {seq.Count}###msgtodo{Quest.Name}", () =>
                        {
                            foreach (var item in todo)
                            {
                                ImGui.Text(item.Value.ToString());
                            }
                        });
                        var text = Quest.QuestTextMessages.Where(item => item.Value != null && item.Value.Value.ToString() != "").ToList();
                        TreeNode.Draw($"Text {text.Count}###msgText{Quest.Name}", () =>
                        {
                            foreach (var item in text)
                            {
                                ImGui.Text(item.Value.ToString());
                            }
                        });
                    });
                    TreeNode.Draw($"Obj###osd{Quest.Name}", () =>
                    {
                        if (QuestEventHandler != null)
                        {
                            foreach (var a in QuestEventHandler->Listeners)
                            {
                                foreach (var b in a.Item2)
                                {
                                    ImGui.Text($"seq:{a.Item1.ToString("X").PadLeft(2,'0')} v:{b}");
                                }
                            }
                            foreach (var obj in QuestEventHandler->LuaEventHandler.EventHandler.EventObjects)
                            {
                                if (curs && !Quest.QuestListenerParams.Where(x => x.Listener != 0 && x.ActorSpawnSeq == CurrentSeq).Any(x => x.Listener == obj.Value->DataID))
                                    continue;
                                if (seq && Quest.QuestListenerParams.Where(x => x.Listener != 0 && x.ActorSpawnSeq == CurrentSeq && x.ActorDespawnSeq == 0xff).Any(x => x.Listener == obj.Value->DataID))
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
                    });
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
                        ImGui.SameLine();
                        ImGui.Checkbox($"ShowIsAnnounce###qlia{Quest.Name}", ref ShowIsAnnounce);
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
                        for (var i = 0u; i < 64; i++)
                        {
                            var Listener = Quest.QuestListenerParams[i];
                            if (curs && Listener.ActorSpawnSeq != CurrentSeq)
                                continue;
                            if (seq && Listener.ActorDespawnSeq == 0xff)
                                continue;
                            if (Listener.Listener == 0)
                                continue;
                            if (ShowIsAnnounce && !QuestEventHandler->IsAnnounce(Listener))
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
                using (var indent1 = new ImGuiIndent(20))
                {
                    foreach (var SeqMessages in Quest.QuestTodoMessages.Where(s => s.Value.Value.ToString() != ""))
                    {
                        var seq = SeqMessages.Value.Variable.ToString().Split('_').Last().ToNumber();
                        var loc = Quest.TodoParams.Where(t => t.ToDoCompleteSeq == (seq + 1) || (MaxSeq == (seq + 1) && t.ToDoCompleteSeq == 0xff)).First();
                        var i = 0;
                        var lis = Quest.QuestListenerParams.Where(l => (l.ActorSpawnSeq == (seq + 1) || (l.ActorSpawnSeq == 0XFF && MaxSeq == (seq + 1))) && l.ActorDespawnSeq != 0xff);
                        ImGui.Text($"{loc.ToDoCompleteSeq} - 0xFF  {SeqMessages.Value.Value}");
                        using var indent2 = new ImGuiIndent(20);
                        foreach (var Msg in lis)
                        {
                            //if (Msg.Listener != 5020000 || (lis.Count() == 1 && loc.ToDoLocation[i].Value != null))
                            {
                                var content = 0u;
                                if (Msg.Listener == 5010000)
                                {
                                    var index = Quest.QuestListenerParams.Where(l => l.Listener == 5010000).IndexOf(l => l.ActorSpawnSeq == Msg.ActorSpawnSeq && l.ActorDespawnSeq == Msg.ActorDespawnSeq);
                                    var dungeons = Quest.QuestParams.Where(s => s.ScriptInstruction.ToString().Contains("INSTANCEDUNGEON")).ToList();
                                    if (dungeons.Count > index)
                                        content = dungeons[index].ScriptArg;
                                }
                                var territory = 0u;
                                if (Msg.Listener == 5020000)
                                {
                                    var index = Quest.QuestListenerParams.Where(l => l.Listener == 5020000).IndexOf(l => l.ActorSpawnSeq == Msg.ActorSpawnSeq && l.ActorDespawnSeq == Msg.ActorDespawnSeq);
                                    var territorys = Quest.QuestParams.Where(s => s.ScriptInstruction.ToString().Contains("TERRITORYTYPE")).ToList();
                                    if (territorys.Count > index)
                                        territory = territorys[index].ScriptArg;
                                }
                                ImGui.Text($"{new QuestListenerString(Msg,content,territory)}");
                                if (loc.ToDoLocation[i].Value != null)
                                {
                                    ImGui.SameLine();
                                    ImGuiHelper.ClickText(loc.ToDoLocation[i].Value.Info(), "�����ת", () =>
                                    {
                                        OpenMapWithMapLink(loc.ToDoLocation[i].Value.ToMapLinkString());
                                    });
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return;
            }
        }
        private delegate bool OpenMapWithFlagDelegate(nint ptr, string str);
        private unsafe delegate nint GetUIMapObjectDelegate(UIModule* instance);
        public bool OpenMapWithMapLink(string mapLinkString)
        {
            unsafe
            {
                var uIModule = UIModule.Instance();
                if (uIModule == null)
                {
                    return false;
                }

                IntPtr intPtr = GetVirtualFunction<GetUIMapObjectDelegate>((nint)uIModule, 0, 8)(uIModule);
                if (intPtr == IntPtr.Zero)
                {
                    return false;
                }
                return GetVirtualFunction<OpenMapWithFlagDelegate>(intPtr, 0, 63)(intPtr, mapLinkString);
            }
        }
        public T GetVirtualFunction<T>(IntPtr address, int vtableOffset, int count) where T : class
        {
            IntPtr ptr = Marshal.ReadIntPtr(address, vtableOffset);
            IntPtr ptr2 = Marshal.ReadIntPtr(ptr, IntPtr.Size * count);
            return Marshal.GetDelegateForFunctionPointer<T>(ptr2);
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
    public class ImGuiIndent : IDisposable
    {
        public ImGuiIndent(float indent_w)
        {
            ImGui.Indent(indent_w);
        }

        public void Dispose()
        {
            ImGui.Unindent();
        }
    }
}
