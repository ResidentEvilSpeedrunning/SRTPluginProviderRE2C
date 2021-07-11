using ProcessMemory;
using System;
using System.Diagnostics;
using SRTPluginProviderRE2C.Structs;
using SRTPluginProviderRE2C.Structs.GameStructs;

namespace SRTPluginProviderRE2C
{
    public unsafe class GameMemoryRE2CScanner : IDisposable
    {
        private static readonly int MAX_ENTITIES = 32;
        private static readonly int MAX_ITEMS = 10;
        //private static readonly int MAX_BOX_ITEMS = 8;

        private static DifficultyEnumeration CurrentDifficulty = DifficultyEnumeration.Easy;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE2C gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Addresses
        private int AddressIGT;
        private int AddressPlayerHP;
        //private int AddressPlayerMaxHP;
        //private int AddressPlayerPoisoned;
        //private int AddressPlayerCharacter;
        private int AddressSlots;
        private int AddressBodies;
        private int AddressFAS;
        private int AddressSaves;
        private int AddressEquippedItemId;
        private int AddressInventory;
        private int AddressNPCs;
        private int AddressDifficulty;

        private IntPtr BaseAddress { get; set; }

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

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_32BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.
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
                        //AddressPlayerMaxHP = 0x58A052;
                        //AddressPlayerPoisoned = 0x58A109;
                        //AddressPlayerCharacter = 0x58EB24;
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

        internal IGameMemoryRE2C Refresh()
        {
            gameMemoryValues._igt = memoryAccess.GetIntAt(IntPtr.Add(BaseAddress, AddressIGT));
            gameMemoryValues._player = memoryAccess.GetAt<GamePlayer>(IntPtr.Add(BaseAddress, AddressPlayerHP));
            gameMemoryValues._availableSlots = memoryAccess.GetByteAt(IntPtr.Add(BaseAddress, AddressSlots));
            gameMemoryValues._bodyCount = memoryAccess.GetByteAt(IntPtr.Add(BaseAddress, AddressBodies));
            gameMemoryValues._fasCount = memoryAccess.GetByteAt(IntPtr.Add(BaseAddress, AddressFAS));
            gameMemoryValues._saveCount = memoryAccess.GetByteAt(IntPtr.Add(BaseAddress, AddressSaves));
            gameMemoryValues._equippedItemId = memoryAccess.GetByteAt(IntPtr.Add(BaseAddress, AddressEquippedItemId));

            // Inventory
            if (gameMemoryValues._playerInventory == null)
                gameMemoryValues._playerInventory = new GameItemEntry[MAX_ITEMS];

            for (int i = 0; i < gameMemoryValues.AvailableSlots; ++i)
                gameMemoryValues._playerInventory[i] = memoryAccess.GetAt<GameItemEntry>(IntPtr.Add(BaseAddress + AddressInventory, (i * 0x4)));

            // Difficulty
            gameMemoryValues._currentdifficulty = memoryAccess.GetAt<DifficultyEntry>(IntPtr.Add(BaseAddress, AddressDifficulty));
            CurrentDifficulty = gameMemoryValues.CurrentDifficulty.Difficulty;

            // Enemies
            if (gameMemoryValues._enemyHealth == null)
                gameMemoryValues._enemyHealth = new NPCInfo[MAX_ENTITIES];

            for (int i = 0; i < MAX_ENTITIES; ++i)
                gameMemoryValues._enemyHealth[i] = memoryAccess.GetAt<NPCInfo>(IntPtr.Add(BaseAddress + AddressNPCs, (i * 0x4)));

            HasScanned = true;
            return gameMemoryValues;
        }

        public static DifficultyEnumeration GetCurrentDifficulty()
        {
            return CurrentDifficulty;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private unsafe bool SafeReadByteArray(IntPtr address, int size, out byte[] readBytes)
        {
            readBytes = new byte[size];
            fixed (byte* p = readBytes)
            {
                return memoryAccess.TryGetByteArrayAt(address, size, p);
            }
        }

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

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

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
