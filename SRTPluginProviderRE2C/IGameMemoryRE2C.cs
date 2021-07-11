using SRTPluginProviderRE2C.Structs;
using SRTPluginProviderRE2C.Structs.GameStructs;
using System;

namespace SRTPluginProviderRE2C
{
    public interface IGameMemoryRE2C
    {
        string GameName { get; }
        string VersionInfo { get; }
        int IGT { get; }
        GamePlayer Player { get; }
        string PlayerName { get; }
        byte AvailableSlots { get; }
        byte BodyCount { get; }
        byte FASCount { get; }
        byte SaveCount { get; }
        byte EquippedItemId { get; }
        GameItemEntry[] PlayerInventory { get; }
        NPCInfo[] EnemyHealth { get; }
        DifficultyEntry CurrentDifficulty { get; }
        TimeSpan IGTTimeSpan { get; }
        string IGTFormattedString { get; }
    }
}
