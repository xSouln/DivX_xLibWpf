using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static xLib.Transceiver.xTypes.CK;

namespace xLib.Transceiver
{
    public class xResponses
    {
        public const char START_CHARECTER = '#';
        public const char END_CHARECTER = ':';
        public const string END_PACKET = "\r";

        public const string RESPONSE = "RES:";
        public const string CONFIRMATION = "ACC:";

        public static string PREFIX_RESPONSE = START_CHARECTER + RESPONSE + END_CHARECTER;
        public static string PREFIX_CONFIRMATION = START_CHARECTER + CONFIRMATION + END_CHARECTER;

        public static unsafe class Get
        {
            public static xResponse State = new xResponse(PREFIX_RESPONSE, KEY.CK_GET_STATE, sizeof(HANDLER) + sizeof(STATE) + sizeof(REQUESTS) + sizeof(InfoT) + sizeof(OptionsT) + sizeof(MotorStateT));
            public static xResponse Options = new xResponse(PREFIX_RESPONSE, KEY.CK_GET_OPTIONS, sizeof(OptionsT));
            public static xResponse Handler = new xResponse(PREFIX_RESPONSE, KEY.CK_GET_HANDLER, sizeof(HANDLER));
            public static xResponse MotorState = new xResponse(PREFIX_RESPONSE, KEY.CK_GET_MOTOR_STATE, sizeof(MotorStateT));
            public static xResponse Info = new xResponse(PREFIX_RESPONSE, KEY.CK_GET_INFO, sizeof(InfoT));
        }

        public static unsafe class Set
        {
            public static xResponse Options = new xResponse(PREFIX_RESPONSE, KEY.CK_SET_OPTIONS, sizeof(RequestSetOptionsT) + sizeof(ERRORS) + sizeof(OptionsT));
            public static xResponse Handler = new xResponse(PREFIX_RESPONSE, KEY.CK_SET_HANDLER, sizeof(RequestSetHandlerT) + sizeof(ERRORS) + sizeof(HANDLER));
            public static xResponse MotorState = new xResponse(PREFIX_RESPONSE, KEY.CK_SET_MOTOR_STATE, sizeof(RequestSetMotorStateT) + sizeof(ERRORS) + sizeof(MotorStateT));
            public static xResponse MotionSteps = new xResponse(PREFIX_RESPONSE, KEY.CK_SET_MOUTION_STEPS, sizeof(RequestSetMotionStepsT) + sizeof(ERRORS) + sizeof(MotorStateT));
            public static xResponse Requests = new xResponse(PREFIX_RESPONSE, KEY.CK_SET_REQUESTS, sizeof(RequestSetRequestsT) + sizeof(ERRORS) + sizeof(REQUESTS));
            public static xResponse Height = new xResponse(PREFIX_RESPONSE, KEY.CK_SET_HEIGHT, sizeof(RequestSetHeightT) + sizeof(ERRORS) + sizeof(MotorStateT));
            public static xResponse MotionPosition = new xResponse(PREFIX_RESPONSE, KEY.CK_SET_MOTION_POSITION, sizeof(RequestSetMotionPositionT) + sizeof(ERRORS) + sizeof(MotorStateT));
        }

        public static unsafe class Try
        {
            public static xResponse Stop = new xResponse(PREFIX_RESPONSE, KEY.CK_TRY_STOP, sizeof(ERRORS) + sizeof(HANDLER) + sizeof(STATE));
            public static xResponse MoveTo = new xResponse(PREFIX_RESPONSE, KEY.CK_TRY_MOVE_TO, sizeof(RequestTryMoveToT) + sizeof(ERRORS) + sizeof(HANDLER));
            public static xResponse Clear = new xResponse(PREFIX_RESPONSE, KEY.CK_TRY_CLEAR, sizeof(RequestTryClearT) + sizeof(ERRORS) + sizeof(MotorStateT));
            public static xResponse MotionStart = new xResponse(PREFIX_RESPONSE, KEY.CK_TRY_MOTION_START, sizeof(ERRORS) + sizeof(HANDLER));
        }

        public static xResponse Confirmation = new xResponse(PREFIX_RESPONSE, KEY.CK_CONFIRMATION, sizeof(KEY) + sizeof(ERRORS));
    }
}
