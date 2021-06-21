using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2C.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]

    public unsafe struct GameItemEntry
    {
        [FieldOffset(0x0)] public byte ItemId;
        [FieldOffset(0x1)] public byte StackSize;
        [FieldOffset(0x2)] public byte SlotModifier;
        [FieldOffset(0x3)] public byte Unknown;

        public static GameItemEntry AsStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(GameItemEntry*)pb;
            }
        }
    }
}