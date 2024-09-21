
using AutoQuest.Extension;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets2;
using QuestResolve.QuestStep;
using System.Numerics;

namespace AutoQuest.QuestStep
{
    internal class Step : CancellationTokenSource , IStep
    {
        public StepType Type;
        public Task<bool>? Result;
        private readonly Func<Task<bool>, bool> Next;
        private readonly Func<CancellationTokenSource, Task<bool>> StartFunc;
        private long? StartTime;
        private long? TimeOut;
        private Step(StepType type, Func<CancellationTokenSource, Task<bool>> func, Func<Task<bool>, bool> checkNext,long? timeOut = null) : base()
        {
            Type = type;
            StartFunc = func;
            Next = checkNext;
            TimeOut = timeOut;
            LogHelper.Info($"{Type}");
        }
        public Task<bool> Run()
        {
            StartTime ??= TimeHelper.Now();
            return Result ??= StartFunc.Invoke(this);
        }
        public bool CheckNext
        {
            get
            {
                if (TimeOut != null)
                {
                    if(TimeHelper.Now() - StartTime > TimeOut)
                    {
                        return true;
                    }
                }
                if (Result == null || (Result.IsCompleted && Result.Result == true) || Next.Invoke(Result))
                    return true;
                return false;
            }
        }
        protected override void Dispose(bool disposing)
        {
            Cancel();
            base.Dispose(disposing);
        }
        public static IStep CreateMovePostion(Vector3 pos, bool fly,bool realPos, Func<Task<bool>, bool> next) => new Step(StepType.MovePostion, cts => Move.MoveControl.Instance.Move(pos, fly, realPos, cts), next);
        public static IStep CreateMovePostion(Level pos, bool fly, Func<Task<bool>, bool> next) => CreateMovePostion(new Vector3(pos.X, pos.Y, pos.Z), fly, pos.Object.Row.IsActor() || pos.Object.Row == 5000000, next);
        public static IStep CreateMoveTarget(GameObject Target, bool fly) => new Step(StepType.MoveTarget, cts => Move.MoveControl.Instance.Move(Target.Position, fly, true, cts), res => res.IsCompleted);
        public unsafe static IStep CreateUseEventItem(uint item, GameObject? target) => new Step(StepType.EventItem, cts =>
        {
            return Task.Run(() =>
            {
                Task.Delay(500).Wait();
                if(target != null)
                {
                    return ActionManager.Instance()->UseAction(ActionType.KeyItem, item, target.Struct()->GetObjectID());
                }           
                return ActionManager.Instance()->UseAction(ActionType.KeyItem, item);
            }, cts.Token);
        }, res => false, 10000);
        public unsafe static IStep CreateEventStart(QuestWrapper quest, GameObject gameObject) => new Step(StepType.InteractObject, cts =>
        {
            return Task.Run(() =>
            {
                new VoidEvent((FFXIVClientStructs.FFXIV.Client.Game.Event.EventHandler*)quest.QuestEventHandler).EventStart(gameObject.Struct()->GetObjectID());
                Task.Delay(500).Wait();
                return true;
            }, cts.Token);
        }, res => res.IsCompletedSuccessfully);
        public unsafe static IStep CreateTeleport(uint territoryId, Vector3 pos) => new Step(StepType.MovePostion, cts => Task.Run(() =>
        {
            var ret = false;
            foreach (var aetheryte in Svc.Data.GetExcelSheet<Aetheryte>()!.Where(ae => ae.Territory.Row == territoryId && ae.IsAetheryte)
            .Select(ae=> Svc.Data.GetExcelSheet<MapMarker>().First(ma => ma.DataType == 3 && ma.DataKey.Row == ae.RowId))
            .OrderBy(mm=>new Vector2(mm.X-pos.X,mm.Y-pos.Z).Length()))
            {
                if (aetheryte.DataKey.Row != 0 && (ret = Telepo.Instance()->Teleport(aetheryte.DataKey.Row, 0)))
                {
                    break;
                }
            }
            Task.Delay(1000).Wait();
            return ret;
        }, cts.Token), res => Svc.ClientState.TerritoryType == territoryId);
        public unsafe static IStep CreateEnemy(GameObject obj,QuestWrapper quest,uint id) => new Step(StepType.MovePostion, cts => Task.Run(() =>
        {
            Svc.Targets.Target = obj;
            //Share.Pull = true;
            return false;
        },cts.Token),res => quest.FindObjectWithLayoutIDInMemory(id,out _));
        public unsafe static IStep CreateSay(string str) => new Step(StepType.Say, cts =>
        {
            ECommons.Automation.Chat.Instance.SendMessage(str);
            Task.Delay(1000, cts.Token);
            return Task.FromResult(true);
        }, res => true);
        public unsafe static IStep CreateEmote(Emote emote,GameObject? obj) => new Step(StepType.Emote, cts =>
        {
            Svc.Targets.Target = obj;
            ECommons.Automation.Chat.Instance.SendMessage(emote.TextCommand.Value.Command.ToString());
            Task.Delay(1000, cts.Token);
            return Task.FromResult(true);
        }, res => true);
        public IStep Start()
        {
            LogHelper.Info("start");
            Run();
            return this;
        }
    }
    internal enum StepType
    {
        MovePostion,
        MoveTarget,
        TalkTo,
        Enemy,
        EventItem,
        InteractObject,
        Say,
        Emote
    }
}
