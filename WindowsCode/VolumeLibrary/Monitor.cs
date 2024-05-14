using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeLibrary
{
    public class Monitor : IDisposable
    {
        readonly SerialPortReader reader;
        private bool Running { get; set; } = false;

        public Monitor(Config config)
        {
            this.config = config;
            this.reader = new SerialPortReader();
        }
        void a()
        {
            AudioMixerAdapter adapter = new AudioMixerAdapter();
            adapter.Init(config);

            
            reader.Init();
            reader.SendLine(adapter.GetLedState());

            while (Running)
            {
                PortCommand command = new PortCommand(reader.GetLine());
                string reply = adapter.DoCommand(command);
                reader.SendLine(reply);
            }
        }


        private bool disposedValue;
        private readonly Config config;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    reader.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Monitor()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
