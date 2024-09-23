using AutoQuest.Excel;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets2;
using System.Numerics;
using static Lumina.Excel.GeneratedSheets2.Quest;

namespace AutoQuest.QuestStep
{
    /// <summary>
    /// 以Listener划分的任务步骤
    /// </summary>
    internal class QuestStep
    {
        public static MoveControlType MoveControlType;
        public QuestWrapper Quest;
        public QuestListenerParamsStruct MainListener;
        public EventType Type => (EventType)MainListener.QuestUInt8A;
        public Level Location;
        public Vector3 LocationCenter => new(Location.X, Location.Y, Location.Z);
        public uint ObjectBaseId;
        public QuestStep() { }
        public Task<bool> Move()
        {
            return Task.FromResult(false);
        }
        public bool IsInRange => (LocationCenter - Svc.ClientState.LocalPlayer.Position).Length() < MathF.Min(1.0f, Location.Radius);
    }
    public enum MoveControlType
    {
        Default,
        Vnavmesh,
        Command,
    }
}
