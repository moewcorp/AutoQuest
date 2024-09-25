using ECommons;
using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets2;
using System.Numerics;
using static Lumina.Excel.GeneratedSheets2.Quest;

namespace AutoQuest.Excel
{
    internal class QuestListenerString
    {
        public uint Listener;
        public uint ConditionValue;
        public uint Content;
        public byte Type;
        public uint TerritoryType;
        public byte ConditionType;
        public QuestListenerString(QuestListenerParamsStruct listener, uint content = 0, uint territory = 0) : this(listener.Listener, listener.ConditionValue, listener.QuestUInt8A, listener.ConditionType, content, territory) { }
        public QuestListenerString(uint listener,uint conditionValue, byte type, byte conditionType, uint content = 0, uint territory = 0)
        {
            Listener = listener;
            ConditionValue = conditionValue;
            Type = type;
            Content = content;
            TerritoryType = territory;
            ConditionType = conditionType;
        }
        public override string ToString()
        {
            return $"{(EventType)Type} Object:{GetTarget()}";
        }
        public static explicit operator string(QuestListenerString s) 
        {
            return s.ToString();
        }
        private string GetTarget()
        {
            if (Listener == 5000000)
            {
                if (ConditionType == 3)
                {
                    var data = Svc.Data.GetExcelSheet<Level>()?.GetRow(ConditionValue);
                    return $"{data} Pos:{data.Territory.Value.PlaceName.Value.Name}{new Vector3(data.X, data.Y, data.Z)} Object:{data.Object.Row}";
                }
                return "Range";
            }
            else if (Listener == 5010000)
            {
                if (Content != 0)
                {
                    ContentFinderCondition? cfc = null;
                    cfc = Svc.Data.GetExcelSheet<ContentFinderCondition>()?.Where(c => c.Content.Row == Content).First(x=>x.TerritoryType.Row == TerritoryType);
                    if(cfc.RowId  is 735 or 778)
                    {
                        var pc = Svc.Data.GetExcelSheet<PublicContent>().GetRow(Content);
                        var bozya = Svc.Data.GetExcelSheet<DynamicEvent>().GetRow(pc.AdditionalData.Row * 16);
                        if (bozya != null)
                        {
                            return $"{bozya} {bozya.Name}";
                        }
                    }
                    if (cfc != null)
                        return $"{cfc} {cfc.Name}";
                }
                return "null";
            }//5020000 可能是去某个地图
            else if (Listener == 5020000)
            {
                if (TerritoryType != 0)
                {
                    var terr = Svc.Data.GetExcelSheet<TerritoryType>().GetRow(TerritoryType);
                    return $"{terr} {terr.PlaceName.Value.Name}";
                }
                return "TerritoryType";
            }
            else if (Listener > 3000000)
            {
                return "null";
            }
            else if (Listener > 2000000)
            {
                var eobj = Svc.Data.GetExcelSheet<EObj>()?.GetRow(Listener);
                var eobjname = Svc.Data.GetExcelSheet<EObjName>()?.GetRow(Listener);
                return $"{eobj} {eobjname.Singular}";
            }
            else if (Listener > 1000000)
            {
                var enpc = Svc.Data.GetExcelSheet<ENpcBase>()?.GetRow(Listener);
                var enpcname = Svc.Data.GetExcelSheet<ENpcResident>()?.GetRow(Listener);
                return $"{enpc} {enpcname.Singular}";
            }
            else
            {
                var bnpc = Svc.Data.GetExcelSheet<BNpcBase>()?.GetRow(Listener);
                if (ConditionType == 0)
                {
                    return $"{bnpc} BaseId:{Listener}";
                }
                else
                {
                    if(ConditionType == 1)
                    {
                        var bnpcName = Svc.Data.GetExcelSheet<BNpcName>()?.GetRow(ConditionValue);
                        return $"{bnpc} BaseId:{Listener} {bnpcName.Singular}";
                    }
                    if (ConditionType == 3)
                    {
                        var lel = Svc.Data.GetExcelSheet<Level>()?.GetRow(ConditionValue);
                        if (lel == null)
                            return ConditionValue.ToString();
                        return $"EventBNpc#{Listener} Pos:{lel?.Territory.Value?.PlaceName.Value?.Name ?? "unkname"}{new Vector3(lel.X, lel.Y, lel.Z)} Object:{lel.Object.Row}";
                    }
                    return $"Unk:{Listener}";
                }
            }
        }
        public static QuestListenerString FromQuest(QuestListenerParamsStruct listener, QuestWrapper quest)
        {
            var content = 0u;
            var territory = 0u;
            if (listener.Listener == 5010000)
            {
                var index = quest.Quest.QuestListenerParams.Where(l => l.Listener == 5010000).IndexOf(l => l.ActorSpawnSeq == listener.ActorSpawnSeq && l.ActorDespawnSeq == listener.ActorDespawnSeq);
                var dungeons = quest.Quest.QuestParams.Where(s => s.ScriptInstruction.ToString().Contains("INSTANCEDUNGEON")).ToList();
                if (dungeons.Count > index)
                {
                    content = dungeons[index].ScriptArg;
                }

                var allContent = Svc.Data.GetExcelSheet<ContentFinderCondition>().Where(x => x.Content.Row == content);
                foreach(var t in quest.Quest.TodoParams.SelectMany(x => x.ToDoLocation.Where(x => x.Value != null && x.Value.Territory.Row != 0)).Select(x => x.Value.Territory.Row).ToHashSet())
                {
                    if(allContent.Any(x => x.TerritoryType.Row == t))
                    {
                        territory = t;
                        break;
                    }
                }
            }
            if (listener.Listener == 5020000)
            {
                var index = quest.Quest.QuestListenerParams.Where(l => l.Listener == 5020000).IndexOf(l => l.ActorSpawnSeq == listener.ActorSpawnSeq && l.ActorDespawnSeq == listener.ActorDespawnSeq);
                var territorys = quest.Quest.QuestParams.Where(s => s.ScriptInstruction.ToString().Contains("TERRITORYTYPE")).ToList();
                if (territorys.Count > index)
                    territory = territorys[index].ScriptArg;
            }
            return new QuestListenerString(listener, content, territory);
        }
    }
    public enum EventType : byte
    {
        None = 0,
        Normal = 1,
        Emote = 2,
        RangeEnemy = 5,
        UseEventItem = 8,
        Enemy = 9,
        Range = 10,
        Unk11 = 11,
        EnterTerritory = 15,
        Unk18 = 18,
        Unk19 = 19,
        Say = 23
    }
}
