using System;
using System.Threading;
using VolumeLibrary;

internal class Program
{
    private static void Main(string[] args)
    {
        try
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
                    if (reader.CanGetLine())
                    {
                        PortCommand command = new PortCommand(reader.GetLine());
                        string reply = adapter.DoCommand(command);
                        reader.SendLine(reply);
                    } 
                    else
                    {
                        Thread.Sleep(100);
                    }

                }
            }
        }
        finally
        {
        
        }
    }
}