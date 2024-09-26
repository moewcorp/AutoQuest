using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets2;
using System.Numerics;

namespace AutoQuest.Move
{
    internal class MoveControl
    {
        public static MoveControl Instance { get; private set; } = new MoveControl();
        private TaskCompletionSource<bool>? Wait = null;
        private Task<List<Vector3>>? Waypoints = null;
        private CancellationTokenSource Cancell = new();
        private List<Vector3> Waypoints_copy;
        private volatile bool Lock = false;
        private int WaypointsCount = 0;
        private long Time = TimeHelper.Now();
        private Vector3 From;
        private Vector3 To;
        public MoveControl()
        {
            Svc.Framework.Update += Framework_Update;
        }

        private async void Framework_Update(Dalamud.Plugin.Services.IFramework framework)
        {
            if (Lock || Svc.ClientState.LocalPlayer == null )
                return;
            Lock = true;
            try
            {
                if (Waypoints != null)
                {
                    if (!Waypoints.IsCompleted)
                    {
                        await Waypoints.ContinueWith(res =>
                        { 
                            Waypoints_copy = new List<Vector3>(res.Result);
                            if(Waypoints_copy.Count == 0)
                            {
                                LogHelper.Error($"寻路异常 from:{From} to:{To}");
                            }
                        });
                    }
                    else
                    {
                        if(Waypoints.Result.Count == 0)
                        {
                            Wait?.SetResult(true);
                            Waypoints = null;
                            WaypointsCount = 0;
                        }
                    }
                }
                //注意 MoveTo与vnavmesh共享一个List<Vector3>对象 需要判定
                if (Wait != null && Waypoints != null && !Wait.Task.IsCompleted)
                {
                    if (Waypoints.Result.Count == 0 || (Svc.ClientState.LocalPlayer.Position - Waypoints.Result.Last()).Length() < 3f)
                    {
                        Wait.SetResult(true);
                        Waypoints = null;
                        WaypointsCount = 0;
                    }
                }
            }
            finally
            {
                Lock = false;
            }
        }

        public Task<bool> Move(Level l, bool fly) => Move(new Vector3(l.X, l.Y, l.Z), fly, false, new CancellationTokenSource());

        public Task<bool> Move(IGameObject obj, bool fly) => Move(obj.Position, fly, true, new CancellationTokenSource());

        public Task<bool> Move(Vector3 pos, bool fly,bool real = true, CancellationTokenSource? cts = null)
        {
            Cancell.Cancel();
            Cancell.TryReset();
            From = Svc.ClientState.LocalPlayer.Position;
            To = pos;
            Waypoints = IpcSubscriber.Pathfind(From, To, fly, real, cts?.Token ?? Cancell.Token);
            Waypoints.ContinueWith(res =>
            {
                if (res.IsCompletedSuccessfully)
                {
                    WaypointsCount = 0;
                    IpcSubscriber.Stop();
                    IpcSubscriber.MoveTo(res.Result, fly);
                }
            });
            Wait = new TaskCompletionSource<bool>();
            cts?.Token.Register(() =>
            {
                Cancell.Cancel();
                IpcSubscriber.Stop();
                if (!Wait.Task.IsCompleted)
                {
                    Wait.SetResult(false);
                }
            });
            return Wait.Task;
        }
    }
}
