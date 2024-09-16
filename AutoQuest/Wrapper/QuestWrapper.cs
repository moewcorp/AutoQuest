using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel.GeneratedSheets2;
using Lumina.Text;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

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
        public bool FindRelatedObjectInMemory([NotNullWhen(true)]out List<GameObject> objects)
        {
            objects = new List<GameObject>();
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
        public bool FindObjectWithDataIdInMemory(uint dataId,[NotNullWhen(true)] out GameObject? obj)
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
        public bool FindObjectWithLayoutIDInMemory(uint LayoutID, [NotNullWhen(true)] out GameObject? obj)
        {
            obj = null;
            if (FindRelatedObjectInMemory(out var list))
            {
                foreach (var c in list.OrderBy(o => (o.Position - Svc.ClientState.LocalPlayer.Position).Length()))
                {
                    if (c.Struct()->LayoutID == LayoutID)
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
        public object? DataIdToObject(uint dataid)
        {
            if(dataid < 10000000)//MONSTER
            {
            }
            else if(dataid < 2000000)//ENPC
            {

            }
            else if(dataid < 30000000)
            {

            }else if(dataid < 40000000)
            {

            }else if(dataid < 50000000)
            {

            }else
            {

            }
            return null;
        }
        public bool ParseQuest()
        {
            return false;
        }
        public object? DataIdToObject(uint dataId, uint additionalDataId)
        {
            if (dataId < 10000000)//MONSTER
            {
                return Svc.Data.GetExcelSheet<BNpcName>()?.GetRow(additionalDataId);
            }
            else if (dataId < 2000000)//ENPC
            {
                return Svc.Data.GetExcelSheet<ENpcBase>()?.GetRow(dataId);
            }
            else if (dataId < 30000000)
            {
                return Svc.Data.GetExcelSheet<EventItem>()?.GetRow(dataId);
            }
            else if (dataId < 40000000)
            {

            }
            else if (dataId < 50000000)
            {

            }
            else
            {

            }
            return null;
        }
    }
}
