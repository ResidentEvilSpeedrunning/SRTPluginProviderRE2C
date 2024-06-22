using ProcessMemory;
using System;
using System.Diagnostics;
using SRTPluginProviderRE2C.Structs;
using SRTPluginProviderRE2C.Structs.GameStructs;

namespace SRTPluginProviderRE2C
{
    public unsafe class GameMemoryRE2CScanner : IDisposable
    {
        private static readonly uint MAX_ENTITIES = 32U;
        private static readonly uint MAX_ITEMS = 10U;

        private static DifficultyEnumeration CurrentDifficulty = DifficultyEnumeration.Easy;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE2C gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public uint ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0U;

        // Addresses
        private ulong AddressIGT;
        private ulong AddressPlayerHP;
        private ulong AddressSlots;
        private ulong AddressBodies;
        private ulong AddressFAS;
        private ulong AddressSaves;
        private ulong AddressEquippedItemId;
        private ulong AddressInventory;
        private ulong AddressNPCs;
        private ulong AddressDifficulty;

        private nuint BaseAddress { get; set; }

        private MultilevelPointer[] PointerEnemyList;

        internal GameMemoryRE2CScanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE2C();
            if (process != null)
                Initialize(process);
        }

        internal void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            if (!SelectAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
                return; // Unknown version.

            uint pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = (nuint)process.MainModule.BaseAddress.ToPointer();
            }
            PointerEnemyList = new MultilevelPointer[MAX_ENTITIES];
        }

        private bool SelectAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.re2C_Rebirth_1p10:
                    {
                        AddressIGT = 0x280588;
                        AddressPlayerHP = 0x58A046;
                        AddressSlots = 0x58E9A4;
                        AddressBodies = 0x58E9B8;
                        AddressFAS = 0x58E9BA;
                        AddressSaves = 0x58E9BC;
                        AddressEquippedItemId = 0x291F6A;
                        AddressInventory = 0x58ED34;
                        AddressNPCs = 0x58A114;
                        AddressDifficulty = 0x291B87;
                        return true;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return false;
        }

        internal static bool Reported = false;
        internal IGameMemoryRE2C Refresh()
        {
            gameMemoryValues._igt = memoryAccess.GetIntAt((void*)(BaseAddress + AddressIGT));
            gameMemoryValues._player = memoryAccess.GetAt<GamePlayer>((void*)(BaseAddress + AddressPlayerHP));
            gameMemoryValues._availableSlots = memoryAccess.GetByteAt((void*)(BaseAddress + AddressSlots));
            gameMemoryValues._bodyCount = memoryAccess.GetByteAt((void*)(BaseAddress + AddressBodies));
            gameMemoryValues._fasCount = memoryAccess.GetByteAt((void*)(BaseAddress + AddressFAS));
            gameMemoryValues._saveCount = memoryAccess.GetByteAt((void*)(BaseAddress + AddressSaves));
            gameMemoryValues._equippedItemId = memoryAccess.GetByteAt((void*)(BaseAddress + AddressEquippedItemId));

            // Inventory
            if (gameMemoryValues._playerInventory == null)
                gameMemoryValues._playerInventory = new GameItemEntry[MAX_ITEMS];

            for (uint i = 0U; i < gameMemoryValues.AvailableSlots; ++i)
                gameMemoryValues._playerInventory[i] = memoryAccess.GetAt<GameItemEntry>((void*)(BaseAddress + AddressInventory + (i * 0x4U)));

            // Difficulty
            gameMemoryValues._currentdifficulty = memoryAccess.GetAt<DifficultyEntry>((void*)(BaseAddress + AddressDifficulty));
            CurrentDifficulty = gameMemoryValues.CurrentDifficulty.Difficulty;

            // Enemies
            if (gameMemoryValues._enemyHealth == null)
                gameMemoryValues._enemyHealth = new NPCInfo[MAX_ENTITIES];

            for (uint i = 0U; i < MAX_ENTITIES; ++i)
            {
                if (!Reported)
                {
                    Console.Write($"{nameof(BaseAddress)} = {BaseAddress:X}, ");
                    Console.Write($"{nameof(AddressNPCs)} = {AddressNPCs:X}, ");
                    Console.Write($"i * 0x4U = {i * 0x4U:X}, ");
                    Console.Write($"Result = {BaseAddress + AddressNPCs + (i * 0x4U):X}.");
                    Console.WriteLine();
                    Reported = !Reported;
                }
                gameMemoryValues._enemyHealth[i] = memoryAccess.GetAt<NPCInfo>((void*)(memoryAccess.GetNUIntAt((void*)(BaseAddress + AddressNPCs + (i * 0x4U)))));
            }

            HasScanned = true;
            return gameMemoryValues;
        }

        public static DifficultyEnumeration GetCurrentDifficulty()
        {
            return CurrentDifficulty;
        }

        private uint? GetProcessId(Process process) => (uint?)process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
