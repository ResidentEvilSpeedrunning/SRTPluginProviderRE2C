using SRTPluginProviderRE2C.Enumerations;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2C.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 1, Size = 4)]
    public struct InventoryItem
    {
        public string _DebuggerDisplay => "";

        [FieldOffset(0x00)]
        public ItemEnumeration Item;

        [FieldOffset(0x01)]
        public byte Quantity;

        [FieldOffset(0x02)]
        public SlotSizeModifierEnumeration SlotModifier;

        [FieldOffset(0x03)]
        public byte Unknown;
    }
}
