using SRTPluginProviderRE2C.Enumerations;
using SRTPluginProviderRE2C.Structs.GameStructs;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProviderRE2C.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct NPCInfo
    {
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

        public static NPCInfo AsStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(NPCInfo*)pb;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                return string.Format("Model: {0} | HP: {1}", ModelType, CurrentHP);
            }
        }

        private short GetMaximumHP(DifficultyEntry difficulty, ScenarioEntry scenario)
        {
            switch (this.ModelType)
            {
                case NPCModelTypeEnumeration.ZombieBrad:
                    {
                        if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                            return 0;
                        else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                            return 250;
                        else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                            return 1250;
                        return 0;
                    }

                case NPCModelTypeEnumeration.Croc:
                    {
                        if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                            return 0;
                        else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                            return 300;
                        else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                            return 300;
                        return 0;
                    }

                case NPCModelTypeEnumeration.MrX1: // Mr. X
                    { // B-scenarios.
                        if (scenario.Scenario == ScenarioEnumeration.B)
                        {
                            if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                                return 0;
                            else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                                return 400;
                            else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                                return 600;
                        }
                        return 0;
                    }

                case NPCModelTypeEnumeration.MrX2: // Final tyrant fight.
                    { // B-scenarios.
                        if (scenario.Scenario == ScenarioEnumeration.B)
                        {
                            if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                                return 0;
                            else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                                return 200; // Verified on Leon B // Estimate (20200 - 19962 = 238, it was probably 200, the hard mode attempt i was probably supposed to move sooner. Ada dropped it right at 200 hp lost on boss when I was in the middle of the room.)
                            else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                                return 200; // Verified on Claire B // Estimate (20201 - 18461 = 1740, it is probably 1700 and my shot brought him lower). *** REVERIFY ***
                        }
                        return 0;
                    }

                case NPCModelTypeEnumeration.GEmbryo: // Chief Iron's Playhouse on Claire A. Chess Plug room on Leon A. 
                    { // A-Scenarios.
                        if (scenario.Scenario == ScenarioEnumeration.A)
                        {
                            if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                                return 400; // Verified on Leon A
                            else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                                return 600; // Verified on Leon A and Claire A
                            else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                                return 1000; // Verified on Leon A
                        }
                        return 0;
                    }

                case NPCModelTypeEnumeration.BirkinG1: // Claire B's Chief Iron's Playhouse Birkin. Leon B's Plug Room Birkin.
                    { // B-scenarios.
                        if (scenario.Scenario == ScenarioEnumeration.B)
                        {
                            if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                                return 0;
                            else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                                return 500; // Verified on Leon B
                            else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                                return 800; // Verified on Claire B
                        }
                        return 0;
                    }

                case NPCModelTypeEnumeration.BirkinG2: // Leon A/Claire A's Train Birkin. Not encountered on Claire B. (Claw through train and when transforming, health is 5000-9000. Ignore drawing logic when HP > 1200)
                    { // A-Scenarios.
                        if (scenario.Scenario == ScenarioEnumeration.A)
                        {
                            if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                                return 0;
                            else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                                return 700; // Verified on Claire A
                            else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                                return 1200; // Verified on Leon A
                        }
                        return 0;
                    }

                case NPCModelTypeEnumeration.BirkinG3: // Claire B's Train Birkin. Not encountered on Leon A.
                    { // B-Scenarios.
                        if (scenario.Scenario == ScenarioEnumeration.B)
                        {
                            if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                                return 0;
                            else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                                return 900; // Verified on Leon B
                            else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                                return 1400; // Verified on Claire B
                        }
                        return 0;
                    }

                case NPCModelTypeEnumeration.BirkinG4: // "DOG" elevator stage.
                    { // A-Scenarios.
                        if (scenario.Scenario == ScenarioEnumeration.A)
                        {
                            if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                                return 0;
                            else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                                return 1100; // Verified on Claire A
                            else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                                return 1300; // Verified on Leon A
                        }
                        return 0;
                    }

                case NPCModelTypeEnumeration.BirkinG5: // Final train blob.
                    { // B-Scenarios.
                        if (scenario.Scenario == ScenarioEnumeration.B)
                        {
                            if (difficulty.Difficulty == DifficultyEnumeration.Easy)
                                return 0;
                            else if (difficulty.Difficulty == DifficultyEnumeration.Normal)
                                return 600; // Verified on Leon B
                            else if (difficulty.Difficulty == DifficultyEnumeration.Hard)
                                return 1000; // Verified on Claire B
                        }
                        return 0;
                    }

                default: // Undefined/Unknown
                    {
                        return 0;
                    }
            }
        }
        public bool GetIsBoss(DifficultyEntry difficulty)
        {
            switch (this.ModelType)
            {
                case NPCModelTypeEnumeration.ZombieBrad:
                case NPCModelTypeEnumeration.Croc:
                case NPCModelTypeEnumeration.MrX1: // Mr. X
                case NPCModelTypeEnumeration.MrX2: // Final tyrant fight.
                case NPCModelTypeEnumeration.GEmbryo: // Chief Iron's Playhouse on Claire A. Chess Plug room on Leon A.
                case NPCModelTypeEnumeration.BirkinG1: // Claire B's Chief Iron's Playhouse Birkin. Leon B's Plug Room Birkin.
                case NPCModelTypeEnumeration.BirkinG2: // Leon A/Claire A's Train Birkin. Not encountered on Claire B. (Claw through train and when transforming, health is 5000-9000. Ignore drawing logic when HP > 1200)
                case NPCModelTypeEnumeration.BirkinG3: // Claire B's Train Birkin. Not encountered on Leon A.
                case NPCModelTypeEnumeration.BirkinG4: // "DOG" elevator stage.
                case NPCModelTypeEnumeration.BirkinG5: // Final train blob.
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }
    }
}