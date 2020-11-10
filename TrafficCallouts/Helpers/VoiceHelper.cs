using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;

namespace TrafficCallouts.Helpers
{
    public class VoiceHelper
    {
        public static void PlayAudioDispatch10_4()
        {
            Functions.PlayScannerAudio("REPORT_RESPONSE_COPY_01");
        }

        public static void PlayAudioDispatchRespondCode1()
        {
            Functions.PlayScannerAudio("UNITS_RESPOND_CODE_01_01");
        }

        public static void PlayAudioDispatchRespondCode2()
        {
            Functions.PlayScannerAudio("UNITS_RESPOND_CODE_02_01");
        }

        public static void PlayAudioDispatchRespondCode99()
        {
            Functions.PlayScannerAudio("UNITS_RESPOND_CODE_99_01");
        }

        public static void PlayAudioDispatchCode4()
        {
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
        }
    }
}
