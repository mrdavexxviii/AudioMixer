using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeLibrary
{
    public struct PortCommand
    {
        public bool Toggle { get; } 
        public bool Valid { get; }
        public int[] SliderValues { get; }
        public PortCommand(string command)
        {
            SliderValues = new int[4];
            if (command.Equals(CommandStrings.SwitchCommand, StringComparison.InvariantCultureIgnoreCase) )
            {
                Toggle = true;
                Valid = true;
            } else
            {
                Toggle = false;
                string[] volLevels = command.Split(CommandStrings.SliderSplit);
                if (volLevels.Length == SliderValues.Length)
                {
                    Valid = true;
                    for (int i = 0; i < SliderValues.Length; ++i)
                    {
                        Valid &= int.TryParse(volLevels[i], out SliderValues[i]);
                    }
                } else
                {
                    Valid = false;
                }
            }
        }

        public override string ToString()
        {
            return $"HS: {SliderValues[0]}, SP: {SliderValues[1]} MIC: {SliderValues[2]}, Bal: {SliderValues[3]}";
        }
    }
}
