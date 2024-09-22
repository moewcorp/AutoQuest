using AutoQuest.Extension;
using Dalamud.Hooking;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Runtime.InteropServices;

namespace AutoQuest
{
    internal unsafe class SkipManager : IDisposable
    {
        public static SkipManager Instance { get; private set; } = new SkipManager();
        private delegate void EventActionDelegate(long a1, ulong a2, uint eventId, ushort sence, long a4, long a5, byte a6);
        private Hook<EventActionDelegate> OnEventAction;
        private delegate void EventFinishDelegate(long a1, uint a2, byte a3, uint a4, uint a5);
        private Hook<EventFinishDelegate> OnEventFinish;
        private unsafe delegate void OpenMapDelegate(long a1, OpenMapInfo* a2);
        private Hook<OpenMapDelegate> OnOpenMap;
        public SkipManager()
        {
            OnEventAction = Svc.Hook.HookFromAddress(Svc.SigScanner.ScanText("48 89 5C 24 ?? 57 48 ?? ?? ?? 8B ?? ?? ?? ?? ?? 41 ?? ?? ?? 48"), new EventActionDelegate(EventActionDetour));
            OnEventAction.Enable();
            OnEventFinish = Svc.Hook.HookFromAddress(Svc.SigScanner.ScanText("40 ?? 55 41 ?? 41 ?? 41 ?? 48 ?? ?? ?? 33 ?? 45"), new EventFinishDelegate(OnEventFinishDetour));
            OnEventFinish.Enable();
            OnOpenMap = Svc.Hook.HookFromAddress(Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 49 8B 45 28 48 8D 8C 24"), new OpenMapDelegate(OnOpenMapDetour));
            OnOpenMap.Enable();
            Svc.GameNetwork.NetworkMessage += GameNetwork_NetworkMessage;
        }

        private unsafe void OnOpenMapDetour(long a1, OpenMapInfo* a2)
        {

            OnOpenMap.Original(a1,a2);
        }
        [StructLayout(LayoutKind.Explicit, Size = 0x8E)]
        public struct OpenMapInfo
        {
            [FieldOffset(0x00)] public MapType Type;
            [FieldOffset(0x04)] public uint AddonId;
            [FieldOffset(0x08)] public uint TerritoryId;
            [FieldOffset(0x0C)] public uint MapId;
            [FieldOffset(0x10)] public uint PlaceNameId;
            [FieldOffset(0x14)] public uint AetheryteId;
            [FieldOffset(0x18)] public uint FateId;
            [FieldOffset(0x1C)] public uint Unk1C;
            [FieldOffset(0x20)] public FFXIVClientStructs.FFXIV.Client.System.String.Utf8String TitleString;
            [FieldOffset(0x88)] public uint Unk88;
            [FieldOffset(0x8C)] public byte Unk8C;
            [FieldOffset(0x8D)] public bool Unk8D; // something for QuestRedoMapMarker
        }

        private void GameNetwork_NetworkMessage(nint dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, Dalamud.Game.Network.NetworkMessageDirection direction)
        {
            if(direction == Dalamud.Game.Network.NetworkMessageDirection.ZoneUp)
            {
                if(opCode == 154)
                {
                    var a = Marshal.PtrToStructure<EventQuestCompleted>(dataPtr - 0x20);
                    LogHelper.Info($"{a.Length} {a.Count} {a.unk2} {a.unk3} {a.unk4} {a.unk5} {a.unk6} {a.unk7} {a.unk8} {a.unk9} {a.unk10}");
                }
            }
        }

        public void Init()
        {

        }
        public unsafe void OnEventFinishDetour(long a1, uint a2, byte a3, uint a4, uint a5)
        {
            OnEventFinish.Original(a1, a2, a3, a4, a5);
        }
        private unsafe void EventActionDetour(long a1, ulong a2, uint eventId, ushort sence, long a4, long data, byte count)
        {
            if (eventId.IsQuestEvent())
            {
                LogHelper.Info($"{eventId} {sence}");
                var quest = QuestWrapper.GetQuestById(eventId);
                if (quest != null)
                {
                    if (!quest.IsQuestRewardScene(sence))   
                    {
                        if (sence > 0 && quest.IsQuestRewardScene((ushort)(sence - 1)))
                        {
                            VoidEvent.SendPackt(new EventQuestCompleted(eventId, sence, quest.Quest.OptionalItemReward[0].Value?.RowId ?? 1,0));
                            return;
                        }
                        else
                        {
                            if (!quest.IsNpcTradeScene(sence))
                            {
                                if (!quest.IsBattleCheck(sence))
                                {
                                    VoidEvent.SendPackt(new EventStartQuest(eventId, sence));
                                    return;
                                }
                            }
                            else
                            {
                                VoidEvent.SendPackt(new EventQuestNpcTrade(eventId, sence, 1));
                                return;
                            }
                        }
                    }
                    else
                    {
                        VoidEvent.SendPackt(new EventQuestCompleted(eventId, sence, 1, quest.Quest.OptionalItemReward[0].Value?.RowId ?? 0));
                        return;
                    }
                }
            }
            OnEventAction.Original(a1, a2, eventId, sence, a4, data, count);
        }

        public void Dispose()
        {
            OnEventAction?.Dispose();
            OnEventFinish?.Dispose();
            OnOpenMap?.Dispose();
            Svc.GameNetwork.NetworkMessage -= GameNetwork_NetworkMessage;
        }
    }
}
