using Dalamud.Plugin.Ipc;
using ECommons.DalamudServices;
using System.Numerics;

namespace AutoQuest
{
    internal static class IpcSubscriber
    {
        public static class Vnavmesh
        {
            public static ICallGateSubscriber<bool>? IsInProgress => Svc.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.SimpleMove.PathfindInProgress");
        }
        public static bool IsRunning => Svc.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.Path.IsRunning").InvokeFunc();
        public static void PathfindAndMoveTo(Vector3 tar, bool real = true) => Svc.PluginInterface.GetIpcSubscriber<Vector3, bool, bool>("vnavmesh.SimpleMove.PathfindAndMoveTo").InvokeFunc(real ? tar : (PointOnFloor(tar) ?? tar), false);
        public static Vector3? PointOnFloor(Vector3 tar) => Svc.PluginInterface.GetIpcSubscriber<Vector3, bool, float, Vector3?>("vnavmesh.Query.Mesh.PointOnFloor").InvokeFunc(new Vector3(tar.X, 1024, tar.Z), false, 5);
        public static bool IsInProgress => Vnavmesh.IsInProgress?.InvokeFunc() ?? false;
        public static void Stop() => Svc.PluginInterface.GetIpcSubscriber<object>("vnavmesh.Path.Stop").InvokeAction();
        public static Task<List<Vector3>> Pathfind(Vector3 from,Vector3 to, bool fly,bool real) => Svc.PluginInterface.GetIpcSubscriber<Vector3,Vector3,bool,Task<List<Vector3>>>("vnavmesh.Nav.Pathfind").InvokeFunc(from, real ? to : (PointOnFloor(to) ?? to),fly);
        public static void MoveTo(List<Vector3> waypoints,bool fly) => Svc.PluginInterface.GetIpcSubscriber<List<Vector3>,bool,object>("vnavmesh.Path.MoveTo").InvokeAction(waypoints,fly);
        public static Task<List<Vector3>> Pathfind(Vector3 from, Vector3 to, bool fly, bool real,CancellationToken cts) => Svc.PluginInterface.GetIpcSubscriber<Vector3, Vector3, bool, CancellationToken, Task<List<Vector3>>>("vnavmesh.Nav.PathfindCancelable").InvokeFunc(from, real ? to : (PointOnFloor(to) ?? to), fly,cts);
        public static ICallGateSubscriber<TRet> GetIpcSubscriber<TRet>(string name) => Svc.PluginInterface.GetIpcSubscriber<TRet>(name);
        public static TRet IpcSubscriberInvokeFunc<TRet>(this string name) => GetIpcSubscriber<TRet>(name).InvokeFunc();
        public static void IpcSubscriberInvokeAction<TRet>(this string name) => GetIpcSubscriber<TRet>(name).InvokeAction();
    }
}
