using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System.Numerics;
using System.Runtime.InteropServices;
using EventHandler = FFXIVClientStructs.FFXIV.Client.Game.Event.EventHandler;

namespace AutoQuest
{
    internal unsafe class VoidEvent
    {
        public EventHandler* EventHandler;
        public VoidEvent(EventHandler* eventHandler)
        {
            EventHandler = eventHandler;
        }

        public void EventStart(ulong ObjectId = 0)
        {
            if (EventHandler != null)
            {
                if (ObjectId == 0)
                    ObjectId = FindEventObject();
                if (ObjectId != 0)
                {
                    SendPackt(new EventStartPackt(ObjectId, EventHandler->Info.EventId.Id));
                }
            }
        }
        private delegate void EventFinishDelegate(EventHandler* ptr, nint pParam, ushort a3, long a4);
        public void EventFinish(long param)
        {
            SendPackt(new EventFinishPackt(EventHandler, param));
        }
        public static VoidEvent? FindEventById(uint id)
        {
            return FindEvent(e => e->Info.EventId.Id == id);
        }
        public delegate bool FindEventPredicateDelegate(EventHandler* eventHandler);
        public static VoidEvent? FindEvent(FindEventPredicateDelegate predicate)
        {
            foreach (var eve in EventFramework.Instance()->EventHandlerModule.EventHandlerMap)
            {
                if (predicate(eve.Item2.Value))
                    return new VoidEvent(eve.Item2.Value);
            }
            return null;
        }
        public static ulong FindEventObject()
        {
            var EventNpclist = Svc.Objects.Where(o => o.HasEvent() && o.DistanceCanStartEvent() && o.Name.ToString() != "").OrderBy(o => Math.Abs((Svc.ClientState.LocalPlayer ?? throw new NullReferenceException(nameof(Svc.ClientState.LocalPlayer))).Position.Y - o.Position.Y)).ToList();
            if (EventNpclist.Count > 0)
            {
                return ((GameObject*)EventNpclist.First().Address)->GetGameObjectId().ObjectId;
            }
            return 0;
        }
        public static void SendPackt<T>(T data) where T : struct
        {
            SendPackt((byte*)&data);
        }
        private static nint fpSendPackt => Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 0F ?? ?? ?? ?? ?? ?? ?? 33 ?? 66 C7 87");
        public static void SendPackt(byte* data)
        {
            ((delegate* unmanaged[Stdcall]<nint, byte*, uint, uint, byte>)fpSendPackt)(GetNetModle(), data, 0, 0);
        }
        private static nint fpGetNetModle => Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 41 81 7F ?? ?? ?? ?? ?? 75");
        //[GameFunction("E8 ?? ?? ?? ?? 41 81 7F ?? ?? ?? ?? ?? 75")]
        private static nint GetNetModle()
        {
            return ((delegate* unmanaged[Stdcall]<nint, nint>)fpGetNetModle)((nint)(FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()));
        }
        public static int eventStart => Marshal.ReadInt32(Svc.SigScanner.ScanText("C7 44 24 ?? ?? ?? ?? ?? 48 C7 44 24 ?? ?? ?? ?? ?? 89 5C 24 ?? 0F 85") + 0x4);
        public static int eventFinish => Marshal.ReadInt32(Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? EB 10 48 8B 0D ?? ?? ?? ??") + 0xCE);//d9
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public struct EventStartPackt
    {
        [FieldOffset(0x00)] public int Opcode = VoidEvent.eventStart;
        [FieldOffset(0x08)] public uint Length = 0x20;
        [FieldOffset(0x20)] public ulong EventObjectId;
        [FieldOffset(0x28)] public uint EventId;

        public EventStartPackt(ulong objectId, uint eventId)
        {
            EventId = eventId;
            EventObjectId = objectId;
        }
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public unsafe struct EventFinishPackt
    {
        [FieldOffset(0x00)] public int Opcode = VoidEvent.eventFinish;
        [FieldOffset(0x08)] public uint Length = 0x20;
        [FieldOffset(0x20)] public uint EventId;
        [FieldOffset(0x24)] public ushort UnkData1;
        [FieldOffset(0x26)] public byte UnkData2;
        [FieldOffset(0x27)] public byte UnkData3 = 2;
        [FieldOffset(0x28)] public long Param;
        public EventFinishPackt(EventHandler* eventHandler, long param)
        {
            EventId = eventHandler->Info.EventId.Id;
            Param = param;
        }
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public struct EventStartQuest
    {
        [FieldOffset(0x00)] public int Opcode = 584;
        [FieldOffset(0x08)] public uint Length = 0x20;
        [FieldOffset(0x20)] public uint QuestId;
        [FieldOffset(0x24)] public uint unk;
        [FieldOffset(0x28)] public uint unk2 = 0;

        public EventStartQuest(uint questId,ushort scene)
        {
            QuestId = questId;
            unk = 0x2000000u | scene;
        }
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x38)]
    public struct EventQuestNpcTrade
    {
        [FieldOffset(0x00)] public int Opcode = 154;
        [FieldOffset(0x08)] public uint Length = 0x28;
        [FieldOffset(0x20)] public uint QuestId;
        [FieldOffset(0x24)] public uint unk;
        [FieldOffset(0x28)] public uint Count;
        [FieldOffset(0x2C)] public uint unk2 = 0;
        [FieldOffset(0x30)] public uint unk3 = 0;
        [FieldOffset(0x34)] public uint unk4 = 0;

        public EventQuestNpcTrade(uint questId, ushort scene,uint count)
        {
            QuestId = questId;
            unk = 0x3000000u | scene;
            Count = count;
        }
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    public struct EventQuestChase
    {
        [FieldOffset(0x00)] public int Opcode = 154;
        [FieldOffset(0x08)] public uint Length = 0x38;
        [FieldOffset(0x20)] public uint unk1 = 3;
        [FieldOffset(0x24)] public uint unk2 = 0xe0000000;
        [FieldOffset(0x28)] public uint unk3 = 0;
        [FieldOffset(0x2C)] public uint unk4 = 0;
        [FieldOffset(0x30)] public uint unk5 = 0;
        [FieldOffset(0x34)] public uint unk6 = 0;
        [FieldOffset(0x38)] public uint unk7 = 0;
        [FieldOffset(0x3c)] public uint unk8 = 0;

        public EventQuestChase()
        {
        }
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x50)]
    public struct EventQuestCompleted
    {
        [FieldOffset(0x00)] public int Opcode = 154;
        [FieldOffset(0x08)] public uint Length = 0x40;
        [FieldOffset(0x20)] public uint QuestId;
        [FieldOffset(0x24)] public uint unk;
        [FieldOffset(0x28)] public uint Count;
        [FieldOffset(0x2C)] public uint unk2 = 0;
        [FieldOffset(0x30)] public uint unk3 = 0;
        [FieldOffset(0x34)] public uint unk4 = 0;
        [FieldOffset(0x38)] public uint unk5 = 0;
        [FieldOffset(0x3c)] public uint unk6 = 0;
        [FieldOffset(0x40)] public uint unk7 = 0;
        [FieldOffset(0x44)] public uint unk8 = 0;
        [FieldOffset(0x48)] public uint unk9 = 0;
        [FieldOffset(0x4c)] public uint unk10 = 0;
        public EventQuestCompleted(uint questId, ushort scene, uint count, uint itemId = 0)
        {
            QuestId = questId;
            unk = 0x4000000u | scene;
            Count = count;
            unk2 = itemId;
        }
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
}
