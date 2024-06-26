﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                        Debug.WriteLine($"Attempting to init {i}");
                        port = new SerialPort(i, 9600, Parity.None, 8, StopBits.One);
                        port.ReadTimeout = 100;
                        port.WriteTimeout = 100;
                        port.Encoding = Encoding.ASCII;
                        port.Open();
                        port.WriteLine(CommandStrings.Stop);
                        Debug.WriteLine($"{i} Stop Sent");
                        port.DiscardInBuffer();
                        port.WriteLine(CommandStrings.Start);
                        Debug.WriteLine($"{i} Start Sent");
                        port.ReadTimeout = -1;
                        string ack = GetLine();
                        if (ack == CommandStrings.Ack)
                        {
                            Debug.WriteLine($"{i} Acknowledged");
                            break;
                        }
                        Debug.WriteLine($"{i} Failure to Acknowledge");
                        port.Dispose();
                        port = null;
                    }
                    catch
                    {
                        Debug.WriteLine($"{i} threw exception");
                        port.Dispose();
                        port = null;
                    }
                }
            }
            SendLine(CommandStrings.LedOn);
            //SendLine(CommandStrings.Refresh);
        }

        public bool CanGetLine()
        {
            return port.BytesToRead > 0;
        }

        public string GetLine()
        {
            try
            {
                string retVal = port.ReadLine().TrimEnd('\r');
                Debug.WriteLine($"Receiving: {retVal}");
                if (retVal == CommandStrings.stayAwake)// Internal keep alive
                {
                    SendLine(CommandStrings.stayAwake);
                }
                return retVal;
            }
            catch
            {
                Init();
            }
            return String.Empty;
        }

        public void SendLine(string line)
        {
            if (!String.IsNullOrEmpty(line))
            {
                try
                {
                    port.WriteLine(line);
                    Debug.WriteLine($"Send Successful : {line}");
                } catch
                {
                    Debug.WriteLine($"Send failed: {line}");
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
