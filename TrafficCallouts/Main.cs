using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using System.Reflection;
using System.CodeDom;

namespace TrafficCallouts
{
    public class Main: Plugin
    {
        public static string versionNumber;
        public override void Initialize()
        {
            versionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
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
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "~y~v" + versionNumber + " ~b~by Normann~b~", "Loaded!");
            } else
            {
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "~y~v" + versionNumber + " ~b~by Normann~b~", "Offloaded");
            }

        }

        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(Callouts.DisabledVehicle));
            //Functions.RegisterCallout(typeof(Callouts.StolenVehicle));
        }
    }
}