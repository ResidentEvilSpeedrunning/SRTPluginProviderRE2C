using ProcessMemory;
using System;
using System.Diagnostics;
using SRTPluginProviderRE2C.Enumerations;
using SRTPluginProviderRE2C.Structs;
using SRTPluginProviderRE2C.Structs.GameStructs;

namespace SRTPluginProviderRE2C
{
    public unsafe class GameMemoryRE2CScanner : IDisposable
    {
        private static readonly int MAX_ENTITIES = 32;
        private static readonly int MAX_ITEMS = 10;
        private static readonly int MAX_BOX_ITEMS = 8;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE2C gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Addresses
        private int* AddressIGT = (int*)0;
        private int* AddressPlayerHP = (int*)0;
        private int* AddressPlayerMaxHP = (int*)0;
        private int* AddressPlayerPoisoned = (int*)0;
        private int* AddressPlayerCharacter = (int*)0;
        private int* AddressSlots = (int*)0;
        private int* AddressBodies = (int*)0;
        private int* AddressFAS = (int*)0;
        private int* AddressSaves = (int*)0;
        private int* AddressEquippedItemId = (int*)0;
        private int* AddressInventory = (int*)0;
        private int* AddressNPCs = (int*)0;

        internal GameMemoryRE2CScanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE2C();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            if (!SelectAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
                return; // Unknown version.

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
        }

        private bool SelectAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.re2C_Rebirth_1p10:
                    {
                        AddressIGT = (int*)0x680588;
                        AddressPlayerHP = (int*)0x98A046;
                        AddressPlayerMaxHP = (int*)0x98A052;
                        AddressPlayerPoisoned = (int*)0x98A109;
                        AddressPlayerCharacter = (int*)0x98EB24;
                        AddressSlots = (int*)0x98E9A4;
                        AddressBodies = (int*)0x98E9B8;
                        AddressFAS = (int*)0x98E9BA;
                        AddressSaves = (int*)0x98E9BC;
                        AddressEquippedItemId = (int*)0x691F6A;
                        AddressInventory = (int*)0x98ED34;
                        AddressNPCs = (int*)0x98A114;

                        return true;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return false;
        }

        internal unsafe IGameMemoryRE2C Refresh()
        {
            // IGT
            fixed (int* p = &gameMemoryValues._igt)
                memoryAccess.TryGetIntAt(AddressIGT, p);

            // Player HP
            fixed (byte* p = &gameMemoryValues._playerCurrentHealth)
                memoryAccess.TryGetByteAt(AddressPlayerHP, p);

            fixed (byte* p = &gameMemoryValues._playerMaxHealth)
                memoryAccess.TryGetByteAt(AddressPlayerMaxHP, p);

            // Player Poison
            fixed (byte* p = &gameMemoryValues._playerPoisoned)
                memoryAccess.TryGetByteAt(AddressPlayerPoisoned, p);

            // Player Character
            fixed (byte* p = &gameMemoryValues._playerCharacter)
                memoryAccess.TryGetByteAt(AddressPlayerCharacter, p);

            // Stats
            fixed (byte* p = &gameMemoryValues._availableSlots)
                memoryAccess.TryGetByteAt(AddressSlots, p);

            fixed (byte* p = &gameMemoryValues._bodyCount)
                memoryAccess.TryGetByteAt(AddressBodies, p);

            fixed (byte* p = &gameMemoryValues._fasCount)
                memoryAccess.TryGetByteAt(AddressFAS, p);

            fixed (byte* p = &gameMemoryValues._saveCount)
                memoryAccess.TryGetByteAt(AddressSaves, p);

            fixed (byte* p = &gameMemoryValues._equippedItemId)
                memoryAccess.TryGetByteAt(AddressEquippedItemId, p);

            // Inventory
            if (gameMemoryValues._playerInventory == null)
            {
                gameMemoryValues._playerInventory = new InventoryEntry[MAX_ITEMS];
                for (int i = 0; i < gameMemoryValues._playerInventory.Length; ++i)
                    gameMemoryValues._playerInventory[i] = new InventoryEntry();
            }
            for (int i = 0; i < gameMemoryValues.AvailableSlots; ++i)
            {
                if (SafeReadByteArray(IntPtr.Add((IntPtr)AddressInventory, (i * 0x4)), sizeof(GameItemEntry), out byte[] ItemBytes))
                {
                    var inventoryEntry = GameItemEntry.AsStruct(ItemBytes);
                    gameMemoryValues._playerInventory[i]._itemID = inventoryEntry.ItemId;
                    gameMemoryValues._playerInventory[i]._quantity = inventoryEntry.StackSize;
                    gameMemoryValues._playerInventory[i]._slotModifier = inventoryEntry.SlotModifier;
                }
            }

            // NPCs
            //if (gameMemoryValues._npcs == null)
            //{
            //    gameMemoryValues._npcs = new NPCInfo[MAX_ENTITIES];
            //    for (int i = 0; i < gameMemoryValues._npcs.Length; ++i)
            //        gameMemoryValues._npcs[i] = new NPCInfo();
            //}
            //for (int i = 0; i < MAX_ENTITIES; ++i)
            //{
            //    fixed (NPCInfo* p = &gameMemoryValues._npcs[i])
            //        memoryAccess.TryGetByteArrayAt(AddressNPCs + (i * 0x4), 342, p);
            //}

            HasScanned = true;
            return gameMemoryValues;
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
