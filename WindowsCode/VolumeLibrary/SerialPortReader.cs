using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace VolumeLibrary
{
    public class SerialPortReader : IDisposable
    {
        private SerialPort port;
        private bool disposedValue;

        public void Init ()
        {
            if (port != null)
            { 
                port.Dispose();
                port = null;
            }
            
            while (port == null)
            {
                var ports = SerialPort.GetPortNames();
                foreach (string i in ports)
                {
                    try
                    {
                        port = new SerialPort(i, 9600, Parity.None, 8, StopBits.One);
                        port.ReadTimeout = 100;
                        port.WriteTimeout = 100;
                        port.Encoding = Encoding.ASCII;
                        port.Open();
                        port.WriteLine(CommandStrings.Stop);
                        port.DiscardInBuffer();
                        port.WriteLine(CommandStrings.Start);

                        port.ReadTimeout = -1;
                        string ack = GetLine();
                        if (ack == CommandStrings.Ack)
                        {
                            break;
                        }
                        port.Dispose();
                        port = null;
                    }
                    catch
                    {
                        port.Dispose();
                        port = null;
                    }
                }
            }
            SendLine(CommandStrings.LedOn);
            //SendLine(CommandStrings.Refresh);
        }

        public string GetLine()
        {
            try
            {
                return port.ReadLine().TrimEnd('\r');
            } catch
            {
                Init();
                return String.Empty;
            }
        }

        public void SendLine(string line)
        {
            if (!String.IsNullOrEmpty(line))
            {
                try
                {
                    port.WriteLine(line);
                } catch
                {
                    Init();
                }
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        port.WriteLine(CommandStrings.LedOff);
                        port.WriteLine(CommandStrings.Stop);
                    }
                    catch
                    {

                    }
                    port?.Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SerialPortReader()
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
