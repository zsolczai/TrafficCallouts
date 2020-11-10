using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using System.Windows.Forms;

namespace TrafficCallouts.Helpers
{
    public static class IsKeyDownWithComputerCheck
    {
        //Call this instead of Game.IsKeyDown
        public static bool IsKeyDownComputerCheck(Keys KeyPressed)
        {
            if (Rage.Native.NativeFunction.CallByName<int>("UPDATE_ONSCREEN_KEYBOARD") != 0)
            {
                return Game.IsKeyDown(KeyPressed);
            }
            else
            {
                return false;
            }
        }

        //Call this instead of Game.IsKeyDownRightNow
        public static bool IsKeyDownRightNowComputerCheck(Keys KeyPressed)
        {
            if (Rage.Native.NativeFunction.CallByName<int>("UPDATE_ONSCREEN_KEYBOARD") != 0)
            {
                return Game.IsKeyDownRightNow(KeyPressed);
            }
            else
            {
                return false;
            }
        }
    }
}
