using System;
using VolumeLibrary;

internal class Program
{
    private static void Main(string[] args)
    {
        Config config = new Config();
        AudioMixerAdapter adapter = new AudioMixerAdapter();
        adapter.Init(config);

        using (SerialPortReader reader = new SerialPortReader())
        {
            reader.Init();
            reader.SendLine(adapter.GetLedState());

            while (true)
            {
                PortCommand command = new PortCommand(reader.GetLine());
                string reply = adapter.DoCommand(command);
                reader.SendLine(reply);

            }
        }
    }
}