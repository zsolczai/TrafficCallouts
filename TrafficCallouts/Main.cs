using Rage;
using LSPD_First_Response.Mod.API;

namespace TrafficCallouts
{
    public class Main: Plugin
    {
        public static string versionNumber;
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            versionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public override void Finally()
        {
            Game.LogTrivial("TrafficCallouts has been cleaned up");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallouts();
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "~y~v" + versionNumber + " ~b~by Normann~b~", "~g~Loaded!");
            } else
            {
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "~y~v" + versionNumber + " ~b~by Normann~b~", "~g~Offloaded");
            }

        }

        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(Callouts.DisabledVehicle));
        }
    }
}