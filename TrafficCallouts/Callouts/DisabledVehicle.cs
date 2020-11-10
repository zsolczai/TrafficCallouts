using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Runtime.InteropServices;

namespace TrafficCallouts.Callouts
{
    [CalloutInfo("Disabled Vehicle", CalloutProbability.Medium)]

    public class DisabledVehicle: Callout
    {
        private string calloutMessage = "Disabled Vehicle";
        private Ped driver;
        private Vehicle brokenVehicle;
        private Vector3 vehicleDesiredPosition = new Vector3(0f, 0f, 0f);
        private Blip vehicleBlip;
        private Blip driverBlip;
        private string[] VehiclesToSelectFrom = new string[] {
            "ORACLE", "PRIMO", "PRIMO2", "EMPEROR", "EMPEROR2", "INGOT", "RANCHERXL", "HELLION"
        };
        private bool sceneNeeded = false;
        private bool sceneCreated = false;
        private bool officerFirstArrivedOnScene = false;
        private bool officerApproachedDriver = false;
        private bool calloutIsRunning = false;

        public override bool OnBeforeCalloutDisplayed() {
            // TODO: change distance from spawning scene
            CalloutPosition = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(100f));
            vehicleDesiredPosition = CalloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 30f);
            AddMinimumDistanceCheck(20f, CalloutPosition);

            Game.DisplayHelp(calloutMessage);
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 VEHICLE_CATEGORY_SEDAN_01 ASSISTANCE_REQUIRED_01 IN_OR_ON_POSITION", CalloutPosition);
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "~g~Version 1.0~w~", calloutMessage);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted() {
            Helpers.VoiceHelper.PlayAudioDispatch10_4();
            calloutIsRunning = true;
            sceneNeeded = true;
            GetDirectionsToCall(CalloutPosition);
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            sceneNeeded = false;
            base.OnCalloutNotAccepted();

            if (driver.Exists()) 
            { 
                driver.Delete();
            }
            if (brokenVehicle.Exists()) {
                brokenVehicle.Delete();
            }
            Functions.PlayScannerAudio("OTHER_UNIT_TAKING_CALL");
        }
        public override void Process() {
            base.Process();
            Game.DisplayNotification("CalloutIsRuinning: " + calloutIsRunning);

            if (CalloutShouldEnd() && !sceneNeeded)
            {
                End();
            }

            if (isBrokenVehicleInPosition())
            {
                vehicleBlip.Position = brokenVehicle.Position;
            }

            if (!sceneCreated && sceneNeeded)
            {

                if (Game.LocalPlayer.Character.DistanceTo(CalloutPosition) < 50f)
                {                   
                   CreateScene(CalloutPosition);                    
                }
            }

            if (!officerApproachedDriver)
            {
                if (Game.LocalPlayer.Character.DistanceTo(CalloutPosition) < 10f)
                {
                    officerApproachedDriver = true;
                    HandleOfficerApproachedDrvier();
                }
            }
            if (!officerFirstArrivedOnScene)
            {
                if (Game.LocalPlayer.Character.DistanceTo(CalloutPosition) < 30f)
                {
                    officerFirstArrivedOnScene = true;                    
                    HandleOfficerArrivedAtScene();
                }
            }
        }
        public override void End() {
            base.End();
            calloutIsRunning = false;
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "~g~Version 1.0~w~", "End of Call");

            if (driver.Exists())
            {
                driver.Dismiss();
            }

            if (brokenVehicle.Exists())
            {
                brokenVehicle.Dismiss();
            }

            if (driverBlip.Exists())
            {
                driverBlip.Delete();
            }
        }

        private bool isBrokenVehicleInPosition()
        {
            return (brokenVehicle.Position.DistanceTo(vehicleDesiredPosition) < 10.0f);
        }

        private bool CalloutShouldEnd()
        {
            return !(driver.Exists() || brokenVehicle.Exists());
        }
        private void GetDirectionsToCall(Vector3 location)
        {
            vehicleBlip = new Blip(location);
            vehicleBlip.IsRouteEnabled = true;
        }
        private void CreateScene(Vector3 spawnPoint)
        {
            Random rand = new Random();
            int index = rand.Next(VehiclesToSelectFrom.Length);
            string randomModelName = VehiclesToSelectFrom[index];

            brokenVehicle = new Vehicle(randomModelName, spawnPoint);            
            brokenVehicle.IsPersistent = true;
            VehicleDoor hood = brokenVehicle.Doors[4];
            hood.IsOpen = true;            
            vehicleBlip = brokenVehicle.AttachBlip();
            vehicleBlip.IsFriendly = true;

            /// Finds a point on the side of the road and moves the vehicle to that point. Using this method:
            /// BOOL _GET_ROAD_SIDE_POINT_WITH_HEADING(float x, float y, float z, float heading, Vector3* outPosition);
            NativeFunction.Natives.xA0F8A7517A273C05<bool>(
                spawnPoint.X + 100.0f,
                spawnPoint.Y + 100.0f,
                spawnPoint.Z + 50.0f,
                brokenVehicle.Heading, out Vector3 outPosition);
            vehicleDesiredPosition = outPosition;

            driver = brokenVehicle.CreateRandomDriver();
            driver.IsPersistent = true;
            driver.BlockPermanentEvents = true;
            driver.Tasks.DriveToPosition(vehicleDesiredPosition, 30.0f, VehicleDrivingFlags.FollowTraffic);
            brokenVehicle.EngineHealth = 200.0f;
            sceneCreated = true;
        }
        private void HandleOfficerArrivedAtScene()
        {
            vehicleBlip.IsRouteEnabled = false;
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "Approach the driver", "Officer On Scene");
            driver.Tasks.LeaveVehicle(LeaveVehicleFlags.None);

            if (GetDiceRoll() == 1)
            {
                Game.LogTrivial("diceroll is 1");
                AnimationSet drunkAnimset = new AnimationSet("move_m@drunk@verydrunk");
                drunkAnimset.LoadAndWait();
                driver.MovementAnimationSet = drunkAnimset;
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "Driver smells like alcohol", "Press E twice");
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "Question the driver", "Press E twice");
            }
            else
            {
                if (driver.IsFemale)
                {
                    Game.LogTrivial("driver is female()");
                    AnimationSet textIdleSet = new AnimationSet("amb@code_human_wander_idles@female@idle_a");
                    textIdleSet.LoadAndWait();
                    driver.MovementAnimationSet = textIdleSet;
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "Talk to the driver", "Press E twice");
                }
                else
                {
                    Game.LogTrivial("driver is male()");
                    AnimationSet textIdleSet = new AnimationSet("amb@code_human_wander_idles@male@idle_a");
                    textIdleSet.LoadAndWait();
                    driver.MovementAnimationSet = textIdleSet;
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "Talk to the driver", "Press E twice");
                }
                
            }
        }
        private void HandleOfficerApproachedDrvier()
        {
            Game.LogTrivial("HandleOfficerApproachedDrvier()");
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "Question the driver", "Press E twice");
        }
        private int GetDiceRoll()
        {
            List<int> myValues = new List<int>(new int[] { 1, 2, 3, 4, 5, 6 });
            Random rand = new Random();
            IEnumerable<int> randomInt = myValues.OrderBy(x => rand.Next()).Take(1);
            return randomInt.First();
        }
    }
}