using AutoQuest.Extension;
using Dalamud.Hooking;
using ECommons.DalamudServices;
using System.Runtime.InteropServices;

namespace AutoQuest
{
    internal class SkipManager : IDisposable
    {
        public static SkipManager Instance { get; private set; } = new SkipManager();
        private delegate void EventActionDelegate(long a1, ulong a2, uint eventId, ushort sence, long a4, long a5, byte a6);
        private Hook<EventActionDelegate> OnEventAction;
        private delegate void EventFinishDelegate(long a1, uint a2, byte a3, uint a4, uint a5);
        private Hook<EventFinishDelegate> OnEventFinish;
        public SkipManager()
        {
            OnEventAction = Svc.Hook.HookFromAddress(Svc.SigScanner.ScanText("48 89 5C 24 ?? 57 48 ?? ?? ?? 8B ?? ?? ?? ?? ?? 41 ?? ?? ?? 48"), new EventActionDelegate(EventActionDetour));
            OnEventAction.Enable();
            OnEventFinish = Svc.Hook.HookFromAddress(Svc.SigScanner.ScanText("40 ?? 55 41 ?? 41 ?? 41 ?? 48 ?? ?? ?? 33 ?? 45"), new EventFinishDelegate(OnEventFinishDetour));
            OnEventFinish.Enable();
            Svc.GameNetwork.NetworkMessage += GameNetwork_NetworkMessage;
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
                        if (!quest.IsNpcTradeScene(sence))
                        {
                            VoidEvent.SendPackt(new EventStartQuest(eventId, sence));
                            return;
                        }
                        else
                        {
                            VoidEvent.SendPackt(new EventQuestNpcTrade(eventId, sence, 1));
                            return;
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
            Svc.GameNetwork.NetworkMessage -= GameNetwork_NetworkMessage;
        }
    }
}
