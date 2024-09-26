using AutoQuest.QuestStep;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets2;
using Lumina.Text;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static Lumina.Excel.GeneratedSheets2.Quest;

namespace AutoQuest
{
    internal unsafe partial class QuestWrapper
    {
        public QuestWrapper(Excel.GeneratedSheets.Quest quest):this() { Quest = quest; }
        private QuestWrapper(ushort questId) : this(questId | 0x10000u) { }
        public QuestWrapper(uint questId) : this(Svc.Data.GetExcelSheet<Excel.GeneratedSheets.Quest>().GetRow(questId)) { }
        public Excel.GeneratedSheets.Quest Quest { get; private set; }
        public uint QuestId => Quest.RowId;
        public Struct.QuestEventHandler* QuestEventHandler => (Struct.QuestEventHandler*)EventFramework.Instance()->GetEventHandlerById(QuestId);
        public bool IsQuestAccepted => QuestManager.Instance()->IsQuestAccepted(QuestId);
        public Level GetTodoLocationLevelIdBySeq(byte seq) => Quest.TodoParams.First(x => x.ToDoCompleteSeq == seq).ToDoLocation.First().Value;
        public IEnumerable<(uint, uint)> GetLinenerBySeq(byte seq) => Quest.QuestListenerParams.Where(q => q.ActorSpawnSeq == seq).Select(q => (q.Listener, q.ConditionValue));
        public bool CheckInLocation()
        {
            if (IsQuestAccepted)
            {
                var level = GetTodoLocationLevelIdBySeq(QuestManager.Instance()->GetQuestById((ushort)(QuestId & 0xFFFFu))->Sequence);
                if (level != null)
                {
                    return (new Vector3(level.X, level.Y, level.Z) - Svc.ClientState.LocalPlayer.Position).Length() < (level.Radius > 0 ? level.Radius : 3f);
                }
            }
            return false;
        }
        public bool FindRelatedObjectInMemory([NotNullWhen(true)]out List<IGameObject> objects)
        {
            objects = new List<IGameObject>();
            if (QuestEventHandler != null)
            {
                foreach (var obj in QuestEventHandler->LuaEventHandler.EventHandler.EventObjects)
                {
                    var o = Svc.Objects.CreateObjectReference((nint)obj.Value);
                    if(o != null)
                        objects.Add(o);
                }
            }
            return objects.Count > 0;
        }
        public bool FindObjectWithDataIdInMemory(uint dataId,[NotNullWhen(true)] out IGameObject? obj)
        {
            obj = null;
            if (FindRelatedObjectInMemory(out var list))
            {
                foreach(var c in list.OrderBy(o => (o.Position - Svc.ClientState.LocalPlayer.Position).Length()))
                {
                    if (c.DataId == dataId)
                    {
                        obj = c;
                        break;
                    }
                }
            }
            return obj != null;
        }
        public bool FindObjectWithLayoutIDInMemory(uint LayoutID, [NotNullWhen(true)] out IGameObject? obj)
        {
            obj = null;
            if (FindRelatedObjectInMemory(out var list))
            {
                foreach (var c in list.OrderBy(o => (o.Position - Svc.ClientState.LocalPlayer.Position).Length()))
                {
                    if (c.Struct()->LayoutId == LayoutID)
                    {
                        obj = c;
                        break;
                    }
                }
            }
            return obj != null;
        }
        public SeString? GetQuestMessageByRow(uint row) => Quest.AllQuestMessage[row].Value?.Value;
        public SeString? GetQuestSeqMessage(uint seq)
        {
            if (seq == 0xff) seq = GetMaxSeq();
            ArgumentOutOfRangeException.ThrowIfGreaterThan(seq, 23u, nameof(seq));
            return GetQuestMessageByRow(seq);
        }
        public SeString? GetQuestSeqMessage(byte seq) => GetQuestSeqMessage((uint)seq);
        public SeString? GetQuestTodoMessage(uint seq)
        {
            if (seq == 0xff) seq = GetMaxSeq();
            ArgumentOutOfRangeException.ThrowIfZero(seq,nameof(seq));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(seq, 23u, nameof(seq));
            return Quest.QuestTodoMessages[seq].Value?.Value;
        }
        public SeString? GetQuestTodoMessage(byte seq) => GetQuestTodoMessage((uint)seq);
        public SeString? GetQuestTextMessage(uint index) => GetQuestMessageByRow(index + 48u);
        public SeString? GetQuestTextMessage(byte index) => GetQuestTextMessage((uint)index);
        public string QuestMessagePath => Quest.MessagesPath;
        public string LuaFilePath => Quest.LuaFile.Path;
        public bool IsComplete => QuestManager.IsQuestComplete(QuestId);
        public IEnumerable<(byte Seq, List<(QuestListenerParamsStruct Listener, LazyRow<Level> Level)> Info)> MainInfo => Quest.QuestListenerParams.Where(l => l.ActorDespawnSeq != 0xff && !(IsPopRangeTarget(l.Listener) && GetFirstSeq(l.Listener) != l.ActorSpawnSeq)).Zip(Quest.TodoParams.SelectMany(t => t.ToDoLocation.Where(t => t.Value != null && t.Value.RowId != 0).ToList())).GroupBy(e => e.First.ActorSpawnSeq).Select(g => (g.Key, g.ToList()));
        public Dictionary<byte, QuestSequence> Sequence => Quest.QuestListenerParams.Where(x => x.Listener != 0).Select(x => x.ActorSpawnSeq).ToHashSet().Select(x => new QuestSequence(this, x)).ToDictionary(x => x.Seq);
        public bool IsPopRangeTarget(uint BaseId)
        {
            for(var i = 0; i < 50; i++)
            {
                if (Quest.QuestParams[i].ScriptInstruction.ToString() =="")
                    break;
                if (Quest.QuestParams[i].ScriptArg == BaseId)
                {
                    var j = i;
                    while(++j < 50)
                    {
                        var str = Quest.QuestParams[j].ScriptInstruction.ToString();
                        if (str.Contains("POPRANGE"))
                            return true;
                        if (str.Contains("ACTOR") || str.Contains("EOBJECT"))
                        {
                            break;
                        }
                    }
                }
            }
            return false;
        }
        public byte GetFirstSeq(uint listener) => Quest.QuestListenerParams.First(l => l.ActorDespawnSeq != 0xff && l.Listener == listener).ActorSpawnSeq;
        public byte GetMaxSeq()
        {
            var todo = Quest.TodoParams;
            for (byte i = 0; i < 24 ; i++)
            {
                if (todo[i].ToDoCompleteSeq == 0xff)
                    return (byte)(i + 1);
            }
            throw new Exception();
        }
        public byte? _MaxSeq;
        public byte MaxSeq
        {
            get
            {
                if(_MaxSeq == null)
                {
                    _MaxSeq = GetMaxSeq();
                }
                return (byte)_MaxSeq;
            }
        }
    }
}
