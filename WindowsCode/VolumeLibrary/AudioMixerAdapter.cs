using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeLibrary
{
    public class AudioMixerAdapter
    {

        CoreAudioController controller;
        IDevice microphone;
        IDevice speakers;
        IDevice headset;
        string chatApp = string.Empty;
        string musicApp = string.Empty;
        public void Init(Config config)
        {
            controller = new CoreAudioController();
            var outputDevices = controller.GetPlaybackDevices();
            var capturedevices = controller.GetCaptureDevices();
            microphone = capturedevices.FirstOrDefault(x => x.FullName.Equals(config.MicrophoneInterface));
            speakers = outputDevices.FirstOrDefault(x => x.FullName.Equals(config.AudioInterface1));
            headset = outputDevices.FirstOrDefault(x => x.FullName.Equals(config.AudioInterface2));
            chatApp = config.ChatApp;
            musicApp = config.MusicApp;
        }
        public string DoCommand(PortCommand command)
        {
            if (command.Valid)
            {
                if (command.Toggle)
                {
                    var currentDefault = controller.GetDefaultDevice(DeviceType.Playback, Role.Console);
                    if (currentDefault == headset)
                    {
                        controller.SetDefaultDevice(speakers);
                    } else if (currentDefault == speakers)
                    {
                        controller.SetDefaultDevice(headset);
                    }
                    return GetLedState();
                }
            //    Debug.WriteLine(command.ToString());
                headset.Volume = command.SliderValues[0];
                speakers.Volume = command.SliderValues[1];
                // microphone.Volume = command.SliderValues[2];
                SetAppVolume(command.SliderValues[2]);
                Rebalance(command.SliderValues[3]);
            }
            return string.Empty;
        }

        public void SetAppVolume(int volume)
        {
            var sessionController = controller.DefaultPlaybackDevice.SessionController;
            var sessions = sessionController.ActiveSessions();
            IAudioSession app = null;
            foreach (var i in sessions)
            {
                if (i.DisplayName.Equals(musicApp, StringComparison.InvariantCultureIgnoreCase))
                {
                    i.Volume = volume;
                }
            }

        }

        public void Rebalance (int balance)
        {
            var sessionController = controller.DefaultPlaybackDevice.SessionController;
            var sessions = sessionController.ActiveSessions();
            IAudioSession discord = null;
            List<IAudioSession> others = new List<IAudioSession>();
            foreach (var i in sessions)
            {
                if (i.DisplayName.Equals(chatApp, StringComparison.InvariantCultureIgnoreCase))
                {
                    discord = i;
                } else  if (!i.DisplayName.Equals(musicApp)) 
                { 
                    others.Add(i); 
                }
            }
           
            if (balance< 50)
            {
                foreach (var i in others)
                {
                    i.Volume = 100;
                }
                if (discord != null)
                {
                    discord.Volume = balance * 2;
                }
            } else
            {
                foreach (var i in others)
                {
                    i.Volume = (100 - balance) * 2;
                }
                if (discord != null)
                {
                    discord.Volume = 100;
                }
            }
        }

        public string GetLedState()
        {
            var currentDefault = controller.GetDefaultDevice(DeviceType.Playback, Role.Console);
            return currentDefault == headset ? CommandStrings.Slider0ActiveLed : CommandStrings.Slider1ActiveLed;
        }
    }
}
