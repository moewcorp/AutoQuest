using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets2;
using System.Numerics;

namespace AutoQuest.Extension
{
    internal static class LevelExtension
    {
        public static bool IsInLevel(this Level level)
        {
            if (Svc.ClientState.TerritoryType == level.Territory.Value?.RowId)
            {
                var length = (new Vector3(level.X, level.Y, level.Z) - Svc.ClientState.LocalPlayer?.Position)?.Length();
                return level.Object is ENpcBase ? length < 3 : length < level.Radius;
            }
            return false;
        }
    }
}