using SRTPluginProviderRE2C.Structs;
using System;

namespace SRTPluginProviderRE2C
{
    public interface IGameMemoryRE2C
    {
        int IGT { get; }
        byte PlayerCurrentHealth { get; }
        bool PlayerPoisoned { get; }
        byte PlayerCharacter { get; }
        byte AvailableSlots { get; }
        byte BodyCount { get; }
        byte FASCount { get; }
        byte SaveCount { get; }
        byte EquippedItemId { get; }
        InventoryEntry[] PlayerInventory { get; }
        NPCInfo[] NPCs { get; }

        TimeSpan IGTTimeSpan { get; }
        string IGTFormattedString { get; }
    }
}
