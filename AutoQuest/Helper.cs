using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel.GeneratedSheets;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AutoQuest
{
    internal unsafe static class Helper
    {
        public static uint baseIdx = 1221;
        /// <summary>
        /// 24*7 uint
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static uint GetLevelRowId(this Lumina.Excel.GeneratedSheets.Quest quest, int row, int col)
        {
            if (row >= 24 || col >= 8)
                throw new ArgumentOutOfRangeException();
            if (col == 0 && row <8)
                return ((quest.GetType().GetProperty($"Unknown{baseIdx}")?.GetValue(quest) as uint[])?[row] ?? 0);
            return (uint?)(quest.GetType().GetProperty($"Unknown{baseIdx + (col * 24) + row}")?.GetValue(quest)) ?? 0;
        }
        public static Level? GetLevel(this Lumina.Excel.GeneratedSheets.Quest quest, int row, int col)
        {
            var id = quest.GetLevelRowId(row, col);
            return id == 0 ? null : Svc.Data.GetExcelSheet<Level>()?.GetRow(id);
        }
        public static object? GetValue(this Level level, Lumina.Excel.GeneratedSheets.Quest quest)
        {
            return level.Type switch
            {
                8 => Svc.Data.GetExcelSheet<ENpcBase>()?.GetRow(level.Object),
                45 => Svc.Data.GetExcelSheet<EObj>()?.GetRow(level.Object),
                9 => Svc.Data.GetExcelSheet<BNpcBase>()?.GetRow(level.Object),
                51 => GetContentFinderCondition(level, quest),
                _ => null
            } ;
        }
        public static object? GetContentFinderCondition(Level level, Lumina.Excel.GeneratedSheets.Quest quest)
        {
            if(level.Type == 51)
            {
                foreach (var cfc in Svc.Data.GetExcelSheet<ContentFinderCondition>())
                {
                    var UnlockQuestId = cfc.UnlockQuest.Value?.RowId;
                    if ((UnlockQuestId != null || UnlockQuestId == 0) && UnlockQuestId == quest.RowId)
                        return cfc;
                    if (cfc.TerritoryType.Value == null)
                        continue;
                    if (cfc.TerritoryType.Value.RowId == level.Territory.Value.RowId)
                        return cfc;
                }
            }
            return null;
        }
        public static void Todo(this EObj obj)
        {

        }
        public static T GetVirtualFunction<T>(IntPtr address, int vtableOffset, int count) where T : class
        {
            IntPtr ptr = Marshal.ReadIntPtr(address, vtableOffset);
            IntPtr ptr2 = Marshal.ReadIntPtr(ptr, IntPtr.Size * count);
            return Marshal.GetDelegateForFunctionPointer<T>(ptr2);
        }
        public delegate uint VtGetEventList(nint ptr, long* pArry);
        /// <summary>
        /// 获取目标拥有的事件数
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="pEventHandles">List<EventHandle*> <see cref="EventHandler*"/></param>
        /// <returns>List<EventHandle*>.Count <see cref="EventHandler*"/><</returns>
        public static uint GetEventList(this Dalamud.Game.ClientState.Objects.Types.GameObject gameObject, out List<long> pEventHandles)
        {
            var v = GetVirtualFunction<VtGetEventList>(gameObject.Address, 0, 34);
            var p = (long*)Marshal.AllocHGlobal(0x100);
            var count = v(gameObject.Address, p);
            pEventHandles = new();
            for (var i = 0; i < count; i++)
            {
                pEventHandles.Add(p[i]);
            }
            Marshal.FreeHGlobal((nint)p);
            return count;
        }

        public static bool DistanceCanStartEvent(this Dalamud.Game.ClientState.Objects.Types.GameObject gameObject)
        {
            var dis = (Svc.ClientState.LocalPlayer.Position - gameObject.Position).Length() - gameObject.HitboxRadius - Svc.ClientState.LocalPlayer.HitboxRadius;
            return gameObject.ObjectKind switch
            {
                Dalamud.Game.ClientState.Objects.Enums.ObjectKind.EventNpc => dis < 26f,
                Dalamud.Game.ClientState.Objects.Enums.ObjectKind.BattleNpc => dis < 6f,
                Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Aetheryte => dis < 10f,
                _ => false
            };
        }
        public static bool HasEvent(this Dalamud.Game.ClientState.Objects.Types.GameObject gameObject) => gameObject.GetEventList(out _) > 0;
        public static List<string> GetPropertyNameAndValueString(this object obj)
        {
            var list = new List<string>();
            
            foreach(var pro in obj.GetType().GetTypeInfo().DeclaredProperties)
            {
                list.Add($"{pro.Name}:{pro.GetValue(obj)} ");
            }
            return list;
        }
        private static nint fpGetData => Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 ?? ?? 74 ?? 44 ?? ?? ?? 4D");
        public static long GetData(uint id)
        {
            return ((delegate* unmanaged[Stdcall]<byte, uint, uint, long>)fpGetData)(0x29, id, 0);
        }
    }
}
