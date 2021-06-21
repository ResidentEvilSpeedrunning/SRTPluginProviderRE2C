using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2C.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]

    public unsafe struct ScenarioEntry
    {
        [FieldOffset(0x0)] public ScenarioEnumeration Scenario;

        public static ScenarioEntry AsStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(ScenarioEntry*)pb;
            }
        }

        // Public Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay => string.Format("Scenario: {0}", Scenario);
    }

    public enum ScenarioEnumeration : byte
    {
        A = 0,
        B = 1
    }
}
