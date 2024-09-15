using ECommons.DalamudServices;
using System.Numerics;

namespace AutoQuest
{
    internal static class GameFunction
    {
        private static nint fpGetData => Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 ?? ?? 74 ?? 44 ?? ?? ?? 4D");
        public static unsafe nint GetData(byte type, uint id, uint unk = 0)
        {
            return ((delegate* unmanaged[Stdcall]<byte, uint, uint,nint>)fpGetData)(type, id,unk);
        }
        public static nint GetRangeData(uint id) => GetData(0x31, id);
        public static unsafe bool TryGetRangePos(uint id,out Vector3 pos,out Vector3 size)
        {
            var obj = GetRangeData(id);
            if (obj != nint.Zero)
            {
                pos = *(Vector3*)(obj + 0x40);
                size = *(Vector3*)(obj + 0x60);
                return true;
            }
            pos = default;
            size = default;
            return false;
        }
    }
}
