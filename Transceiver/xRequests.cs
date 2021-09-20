using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static xLib.Transceiver.xTypes.CK;

namespace xLib.Transceiver
{
    public class xRequests
    {
        public const char REQUEST_START_CHARECTER = '#';
        public const char REQUEST_END_CHARECTER = ':';
        public const string END_PACKET = "\r";
        /*
        public static unsafe class Get
        {
            public const string REQUEST = "GET:";
            public static string Prefix = "" + REQUEST_START_CHARECTER + REQUEST + REQUEST_END_CHARECTER;

            public static xRequest State = new xRequest(Prefix, KEY.CK_GET_STATE, 0, END_PACKET);
        }

        public static unsafe class Set
        {
            public const string REQUEST_SET = "SET:";
            public static string Prefix = "" + REQUEST_START_CHARECTER + REQUEST_SET + REQUEST_END_CHARECTER;

            public static xRequest Options = new xRequest(Prefix, KEY.CK_SET_OPTIONS, sizeof(RequestSetOptionsT), END_PACKET);
            public static xRequest Handler = new xRequest(Prefix, KEY.CK_SET_HANDLER, sizeof(RequestSetHandlerT), END_PACKET);
            public static xRequest MotorState = new xRequest(Prefix, KEY.CK_SET_MOTOR_STATE, sizeof(RequestSetMotorStateT), END_PACKET);
            public static xRequest MotionSteps = new xRequest(Prefix, KEY.CK_SET_MOUTION_STEPS, sizeof(RequestSetMotionStepsT), END_PACKET);
            public static xRequest Requests = new xRequest(Prefix, KEY.CK_SET_REQUESTS, sizeof(RequestSetRequestsT), END_PACKET);
            public static xRequest Height = new xRequest(Prefix, KEY.CK_SET_HEIGHT, sizeof(RequestSetHeightT), END_PACKET);
            public static xRequest MotionPosition = new xRequest(Prefix, KEY.CK_SET_MOTION_POSITION, sizeof(RequestSetMotionPositionT), END_PACKET);
        }

        public static unsafe class Try
        {
            public const string REQUEST_TRY = "TRY:";
            public static string Prefix = "" + REQUEST_START_CHARECTER + REQUEST_TRY + REQUEST_END_CHARECTER;

            public static xRequest Clear = new xRequest(Prefix, KEY.CK_TRY_CLEAR, sizeof(RequestTryClearT), END_PACKET);
            public static xRequest Stop = new xRequest(Prefix, KEY.CK_TRY_STOP, 0, END_PACKET);
            public static xRequest MoveTo = new xRequest(Prefix, KEY.CK_TRY_MOVE_TO, sizeof(RequestTryMoveToT), END_PACKET);
            public static xRequest MotionStart = new xRequest(Prefix, KEY.CK_TRY_MOTION_START, 0, END_PACKET);
        }
        */
    }
}
