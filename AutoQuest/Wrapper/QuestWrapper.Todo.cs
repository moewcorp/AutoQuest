using AutoQuest.QuestStep;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets2;
using QuestResolve.QuestStep;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.RegularExpressions;

namespace AutoQuest
{
    internal partial class QuestWrapper
    {
        //TODO
        public bool CheckCanAccepteOrAccepted()
        {
            if (!IsComplete)
            {
                if (IsQuestAccepted)
                    return true;
                else
                {
                    foreach (var item in Quest.PreviousQuest.Select(q => QuestWrapper.GetQuestById(q.Row)))
                    {
                        if(!item.IsComplete)
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }
        public unsafe bool TryGetTask([NotNullWhen(true)]out IStep? todo)
        {
            if(TryTodo(out var todod))
            {
                if(todod.level.Territory.Value.RowId != Svc.ClientState.TerritoryType)
                {
                    todo = Step.CreateTeleport(todod.level.Territory.Value.RowId, new System.Numerics.Vector3(todod.level.X, todod.level.Y, todod.level.Z));
                    return true;
                }
                if (QuestEventHandler->IsBattleNpcOwner())
                {
                    foreach (var oss in Quest.QuestParams.Where(s => s.ScriptInstruction.ToString().Contains("ENEMY")))
                    {
                        if (FindObjectWithLayoutIDInMemory(oss.ScriptArg, out var PP))
                        {
                            todo = Step.CreateEnemy(PP,this, oss.ScriptArg);
                            return true;
                        }
                    }
                }
                if (FindObjectWithDataIdInMemory(todod.obj, out var obj))
                {
                    if ((obj.Position - Svc.ClientState.LocalPlayer.Position).Length() > 3)
                    {
                        todo = Step.CreateMoveTarget(obj, (new Vector3(todod.level.X, todod.level.Y, todod.level.Z) - Svc.ClientState.LocalPlayer.Position).Length() > 25);
                        return true;
                    }
                    else
                    {
                        if (CurrentSeq != 0xff)
                        {
                            //if (obj is Character c && c.StatusFlags.HasFlag(Dalamud.Game.ClientState.Objects.Enums.StatusFlags.Hostile))
                            //if(QuestEventHandler->IsBattleNpcOwner())
                            //{
                            //    foreach(var oss in Quest.QuestParams.Where(s=>s.ScriptInstruction.ToString().Contains("ENEMY")))
                            //    {
                            //        LogHelper.Print(oss.ScriptArg.ToString());
                            //        if(FindObjectWithDataIdInMemory(oss.ScriptArg, out var PP))
                            //        {
                            //            todo = Step.CreateEnemy(PP);
                            //            return true;
                            //        }
                            //    }
                            //}
                            //else
                            {
                                if(Quest.QuestListenerParams.Where(q=>q.Listener == todod.obj).TryGetFirst(q=>q.QuestUInt8A == 8,out var lis))
                                {
                                    var eventItem = QuestEventHandler->GetEventItems();
                                    if (eventItem.Count != 0)
                                    {
                                        foreach (var item in eventItem)
                                        {
                                            if (QuestEventHandler->IsEventItemUsable(obj, item.ItemId))
                                            {
                                                todo = Step.CreateUseEventItem(item.ItemId, obj);
                                                return true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (TryGetSayEvent(todod.obj,CurrentSeq,obj,out var str))
                                    {
                                        todo = Step.CreateSay($"/s {str}");
                                        return true;
                                    }
                                }
                            }
                            todo = Step.CreateEventStart(this, obj);
                            return true;
                        }
                    }
                }
                else
                {
                    todo = Step.CreateMovePostion(todod.level,(new Vector3(todod.level.X, todod.level.Y, todod.level.Z)-Svc.ClientState.LocalPlayer.Position).Length() > 25,res => FindObjectWithDataIdInMemory(todod.obj, out var obj));
                    return true;
                }
            }
            todo = null;
            return false;
        }
        public bool TryGetTodoLevelsAndListenersByCurrentSeq(out List<Level> levels,out List<uint> listeners)
        {
            levels = new List<Level>();
            listeners = new List<uint>();
            var seq = CurrentSeq;
            if (seq == 0)
            {
                levels.Add(Quest.IssuerLocation.Value);
                listeners.Add(Quest.IssuerLocation.Value.Object.Row);
            }
            else
            {
                foreach (var todo in Quest.TodoParams)
                {
                    if(todo.ToDoCompleteSeq == seq)
                    {
                        foreach (var l in todo.ToDoLocation)
                        {
                            if (l.Value != null)
                            {
                                levels.Add(l.Value);
                            }    
                        }
                    }
                }
            }
            foreach(var lis in Quest.QuestListenerParams)
            {
                if(lis.ActorSpawnSeq == seq && lis.Listener != 0)
                {
                    listeners.Add(lis.Listener);
                }
            }
            return levels.Count > 0 && listeners.Count > 0;
        }
        public unsafe bool TryGetSayEvent(uint obj,byte seq,GameObject gameObject, out string str)
        {
            if (Quest.QuestListenerParams.Where(q => q.Listener == obj).Any(q => q.QuestUInt8A == 23) && DecompileCode.Value.TryGetValue("IsAcceptSayEvent", out var f))
            {
                string pattern = @"(?<=CompareString\()[\w\d.]+,\s+_ARG_0_\.(.*?),(.*?)(?=\))|(?<=GET )\w+\.\w+(?="")";
                string pseq = @"_ARG_1_:GetQuestSequence\(\(_ARG_0_:GetQuestId\(\)\)\) == _ARG_0_\.SEQ_(.*?) ";
                string pobj = @"_ARG_2_:GetBaseId\(\) == _ARG_0_\.(.*) ";
                foreach (var s in f.Code)
                {
                    var reg = Regex.Match(s.Value ?? "null", pattern);
                    if (reg.Success)
                    {
                        if (f.Code.TryGetValue(s.Key - 1, out var seqstr) && seqstr != null)
                        {
                            var regseq = Regex.Match(seqstr, pseq);
                            var objreg = Regex.Match(seqstr, pobj);
                            if (f.Code.TryGetValue(s.Key - 2, out seqstr) && seqstr != null)
                            {
                                if(!regseq.Success) regseq = Regex.Match(seqstr, pseq);
                                if(!objreg.Success) objreg = Regex.Match(seqstr, pobj);
                            }
                            if (regseq.Success && uint.Parse(regseq.Groups[1].Value) != seq)
                            {
                                continue;
                            }
                            if (objreg.Success && Quest.QuestParams.TryGetFirst(o => o.ScriptInstruction.ToString() == objreg.Groups[1].Value, out var oo))
                            {
                                if (oo.ScriptArg == obj && Quest.QuestTextMessages.TryGetFirst(s => s.Value?.Variable.ToString() == reg.Groups[1].Value, out var name) && name != null)
                                {
                                    str = name.Value?.Value.ToString() ?? "null";
                                    var ret = QuestEventHandler->IsAcceptSayEvent(gameObject, str);
                                    return ret.Result[1] is bool a && a;
                                }
                            }
                        }
                    }
                }
            }
            str = "null";
            return false;
        }
        public unsafe bool TryTodo(out (Level? level, uint obj) todo)
        {
            foreach (var q in QuestManager.Instance()->NormalQuestsSpan)
            {
                if ((q.QuestId | 0x10000u) == QuestId)
                {
                    for (var i = 0 ; i < 8; i ++ )
                    {
                        if ((q.Variables[5] & (1 << (7 - i))) == 0)
                        {
                            todo.level = Quest.TodoParams.First(t=>t.ToDoCompleteSeq == CurrentSeq).ToDoLocation[i].Value!;
                            todo.obj = todo.level.Object.Row != 0 ? todo.level.Object.Row : Quest.QuestListenerParams.Where(x=>x.ActorSpawnSeq == CurrentSeq && x.ActorDespawnSeq != 0xff).ToArray()[i].Listener;
                            return true;
                        }
                    }
                }
            }
            if(CurrentSeq == 0)
            {
                todo = (Quest.IssuerLocation.Value, Quest.IssuerLocation.Value.Object.Row);
                return true;
            }    
            todo = (null, 0);
            return false;
        }
        public unsafe byte CurrentSeq
        {
            get
            {
                foreach(var item in QuestManager.Instance()->NormalQuestsSpan)
                {
                    if ((item.QuestId | 0x10000u) == QuestId)
                        return item.Sequence;
                }
                return 0;
            }
        }
        public unsafe bool IsDone(int index)
        {
            if (index > 0 && index < 8)
            {
                foreach (var item in QuestManager.Instance()->NormalQuestsSpan)
                {
                    if ((item.QuestId | 0x10000u) == QuestId)
                        return ((item.Variables[5] >> (8 - index)) & 1) == 1;
                }
            }
            return false;
        }
    }
}
