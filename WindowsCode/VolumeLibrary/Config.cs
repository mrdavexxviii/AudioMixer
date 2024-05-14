using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeLibrary
{
    public class Config
    {
        public string AudioInterface1 { get; } = "Speakers (Realtek(R) Audio)";
        public string AudioInterface2 { get; } = "Headphones (HyperX Cloud Alpha Wireless)";
        public string MicrophoneInterface { get; } = "Microphone (HyperX Cloud Alpha Wireless)";
        public string ChatApp { get; } = "Discord";
        public string MusicApp { get; } = "Spotify";


    }
}
