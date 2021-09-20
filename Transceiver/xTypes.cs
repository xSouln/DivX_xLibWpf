using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public class xTypes
    {
        public class CK
        {
            public enum ACTION_SET_HANDLER : uint
            {
                RESET,
                SET
            }

            public enum ACTION_SET_MOTOR_STATE : uint
            {
                RESET,
                SET
            }

            public enum ACTION_SET_MOTION_STEPS : uint
            {
                SET,
                INSERT,
                REMOVE
            }

            public enum ACTION_TRY_MOVE_TO : uint
            {
                MOVE_DOWN,
                MOVE_UPP
            }

            public enum ACTION_TRY_CLEAR : uint
            {
                CLEAR_MOTION,
            }

            public enum HANDLER : uint
            {
                MotorIsEnable = (1 << 0),
                MotorEnabling = (1 << 1),
                MotorDisabling = (1 << 2),

                PositionIsSet = (1 << 3),
                UpdatePeriod = (1 << 4),
                MotionVector = (1 << 5),

                RequestScanStepCount = (1 << 6),
                ScanStepCountIsEnable = (1 << 7),
                SensorUppIsFree = (1 << 8),
                SensorUppAccept = (1 << 9),
                SensorDownAccept = (1 << 10),
            }

            public enum REQUESTS : uint
            {
                StartMotionVector = (1 << 0)
            }

            public enum STATE : uint
            {
                SensorUpp = (1 << 0),
                SensorDown = (1 << 1),

                DrvEnable = (1 << 2),
                DrvDir = (1 << 3),
                DrvDivider = ((1 << 4) | (1 << 5) | (1 << 6)),
                DrvReset = (1 << 7),
            }

            public struct OptionsT
            {
                public float RacingK;
                public float RacingIncrement;
                public float RacingOffset;
                public float RacingStartPeriod;

                public float FallingK;
                public float FallingIncrement;
                public float FallingOffset;
                public float FallingStopPeriod;

                public float MVT_SamplingTime;

                public ushort RacingSteps;
                public ushort FallingSteps;

                public ushort MotorEnableDelay;
                public ushort MotorDisableDelay;

                public ushort SamplingPeriod;
            }

            public struct MotorStateT
            {
                public uint StepsCount;
                public float StepSize;

                public float PeriodTotal;
                public float PeriodRequest;

                public uint PositionTotal;
                public uint PositionRequest;

                public ushort PositionLast;

                public uint MotionTotalIndex;
                public uint MotionPointsCount;

                public uint ReducedHeight;

                public uint MoveTime;
                public uint StepsPassed;
            }

            public struct InfoT
            {
                public uint PWM_Frequency;
                public uint PWM_Prescaler;
                public float PWM_Const;

                public float Height;

                public uint MotionPointsCountMax;
            }

            public struct MotionPropertysT
            {
                public uint PointCount;
                public uint SamplingTime;
                public float TotalScanTime;

                public float MinHeight;
                public float MaxHeight;
            }

            public struct RequestSetOptionsT
            {
                public OptionsT Value;
            }

            public struct RequestSetHandlerT
            {
                public HANDLER Value;
                public ACTION_SET_HANDLER Action;
            }

            public struct RequestSetMotorStateT
            {
                public uint Position;
                public float Period;
                public ACTION_SET_MOTOR_STATE Action;
            }

            public unsafe struct RequestSetMotionStepsT
            {
                public uint StartIndex;
                public uint PointsCount;
                public ACTION_SET_MOTION_STEPS Action;
                public fixed uint Points[32];
            }

            public unsafe struct RequestSetRequestsT
            {
                public REQUESTS Value;
            }

            public unsafe struct RequestTryClearT
            {
                public ACTION_TRY_CLEAR Value;
            }

            public unsafe struct RequestSetHeightT
            {
                public float Value;
            }

            public unsafe struct RequestSetMotionPositionT
            {
                public uint Value;
            }
            public unsafe struct RequestTryMoveToT
            {
                public ACTION_TRY_MOVE_TO Action;
            }
        }
    }
}
