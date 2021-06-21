using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2C.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]

    public unsafe struct DifficultyEntry
    {
        [FieldOffset(0x0)] private byte difficulty;
        [FieldOffset(0x7)] private byte isEasy;

        public static DifficultyEntry AsStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(DifficultyEntry*)pb;
            }
        }

        // Public Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay => string.Format("Difficulty: {0}", Difficulty);

        public DifficultyEnumeration Difficulty
        {
            get
            {
                if (difficulty == 0 && isEasy == 1)
                    return DifficultyEnumeration.Easy;
                else if (difficulty == 0 && isEasy == 0)
                    return DifficultyEnumeration.Normal;
                else if (difficulty == 2 && isEasy == 0)
                    return DifficultyEnumeration.Hard;
                else
                    return DifficultyEnumeration.Unknown;
            }
        }
    }

    public enum DifficultyEnumeration
    {
        Unknown = 0,
        Easy = 1,
        Normal = 2,
        Hard = 3,
    }
}
