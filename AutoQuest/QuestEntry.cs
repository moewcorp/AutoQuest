using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace AutoQuest
{
    public unsafe class QuestEntry : IDalamudPlugin
    {
        public string Name => GetType().Assembly.GetName().Name;

        public int LimitLevel => 0;

        public QuestEntry(DalamudPluginInterface pluginInterface)
        {
            ECommonsMain.Init(pluginInterface, this);
            SkipManager.Instance.Init();
            AutoQuestManager.Instance.Init();
            Svc.PluginInterface.UiBuilder.Draw += DrawEntrySetting;
        }
        [StructLayout(LayoutKind.Explicit)]
        struct EventAcrion
        {
            [FieldOffset(0x00)] public long TargetId;
            [FieldOffset(0x08)] public uint id;
            [FieldOffset(0x0c)] public ushort Scene;
            [FieldOffset(0x10)] public long unk3;
            [FieldOffset(0x18)] public byte unk4;
            [FieldOffset(0x1c)] public long unk5;
            public override string ToString()
            {
                return $"TargetId:{TargetId:X} id:{id} Scene:{Scene} unk3:{unk3} unk4:{unk4} unk5:{unk5}";
            }
        }

        public void Dispose()
        {
            AutoQuestManager.Instance.Dispose();
            SkipManager.Instance.Dispose();
            TaskManager.Instance.Dispose();
            Svc.PluginInterface.UiBuilder.Draw -= DrawEntrySetting;
        }
        public unsafe delegate void* TT(FFXIVClientStructs.FFXIV.Component.Excel.ExcelSheet* a, int b, int c);
        public bool a;
        public bool b = true;
        public bool c;
        public void DrawEntrySetting()
        {
            ImGuiEx.EzTabBar("Questddw", ("Setting", SettingWindows.DrawSetting, null, true), ("Debug", SettingWindows.DrawDebug, null, true));
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 0x38)]
        public struct EventEnterRange
        {
            [FieldOffset(0x00)] public int Opcode = 356;
            [FieldOffset(0x08)] public uint Length = 0x28;
            [FieldOffset(0x20)] public uint RangeId;
            [FieldOffset(0x24)] public uint QuestId;
            [FieldOffset(0x28)] public Vector3 Pos = Svc.ClientState.LocalPlayer.Position;
            [FieldOffset(0x34)] public uint unk4 = 0;
            public EventEnterRange(uint rangeId, uint questId)
            {
                RangeId = rangeId;
                QuestId = questId;
            }
        }

        public void OnLoad(AssemblyLoadContext loadContext, DalamudPluginInterface PluginInterface)
        {
            using (var steam = new MemoryStream(Resource.Unluac))
            {
                loadContext.LoadFromStream(steam);
            }
            using (var s = new MemoryStream(Resource.SaintCoinach))
            {
                loadContext.LoadFromStream(s);
            }
        }
    }
}
