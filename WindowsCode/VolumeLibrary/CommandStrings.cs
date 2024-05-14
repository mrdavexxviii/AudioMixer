using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeLibrary
{
    internal static class CommandStrings
    {
        internal static readonly string Start = "Hello";
        internal static readonly string Ack = "Hi";
        internal static readonly string Stop = "Bye";
        internal static readonly string stayAwake = "Wake";

        internal static readonly string LedOff = "LED0";
        internal static readonly string LedOn = "LED1";
        internal static readonly string Slider0ActiveLed = "Select_SL0";
        internal static readonly string Slider1ActiveLed = "Select_SL1";
        internal static readonly string SwitchCommand = "Toggle";
        internal static readonly char SliderSplit = '|';
        internal static readonly string Refresh = "Get";


    }
}
