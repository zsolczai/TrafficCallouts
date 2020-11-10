using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace TrafficCallouts.Callouts
{
    [CalloutInfo("StolenVehicle", CalloutProbability.VeryHigh)]
    public class StolenVehicle: Callout
    {
        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Vector3 SpawnPoint;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private bool PursuitCreated = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            Game.DisplaySubtitle("OnBeforeCalloutDisplayed");

            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(250f));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, SpawnPoint);

            CalloutMessage = "Stolen Vehicle";
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_GRAND_THEFT_AUTO IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Game.DisplaySubtitle("OnCalloutAccepted");

            SuspectVehicle = new Vehicle("ZENTORINO", SpawnPoint);
            SuspectVehicle.IsPersistent = true;

            Suspect = SuspectVehicle.CreateRandomDriver();
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.IsFriendly = true;

            Suspect.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.Emergency);

            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();
            Game.DisplaySubtitle("Process");

            if (Game.LocalPlayer.Character.DistanceTo(Suspect.Position) < 100f)
            {
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "~g~Version 1.0~w~", "Officer On Scene");
            }

            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(Suspect.Position) < 30f)
            {
                Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, Suspect);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                PursuitCreated = true;
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "TrafficCallouts", "~g~Version 1.0~w~", "Officer On Scene");
            }
        }
        public override void End()
        {
            base.End();
            Game.DisplaySubtitle("End");

            if (Suspect.Exists())
            {
                Suspect.Dismiss();
            }

            if (SuspectVehicle.Exists())
            {
                SuspectVehicle.Dismiss();
            }

            if (SuspectBlip.Exists())
            {
                SuspectBlip.Delete();
            }

        }
    }
}
