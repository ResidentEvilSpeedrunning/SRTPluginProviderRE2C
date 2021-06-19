using SRTPluginProviderRE2C.Enumerations;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2C.Structs
{
	[DebuggerDisplay("{_DebuggerDisplay,nq}")]
	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 1, Size = 4)]
	public struct NPCInfo
    {
		public string _DebuggerDisplay => "";

		// In-memory values
		[FieldOffset(0x08)]
		public NPCModelTypeEnumeration ModelType;

		[FieldOffset(0x38)]
		public int X;

		[FieldOffset(0x3C)]
		public int Z;

		[FieldOffset(0x40)]
		public int Y;

		[FieldOffset(0x76)]
		public int RoomID;

		[FieldOffset(0x156)]
		public short CurrentHP;

		// Generated values
		[FieldOffset(0x00)]
		private short MaximumHP;

		[FieldOffset(0x00)]
		public bool IsBoss;
	}
}
