using AudioBoxControl.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using VolumeLibrary;

namespace AudioBoxControl
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ABCContext());
        }
    }

    public class ABCContext : ApplicationContext
    {
        private NotifyIcon icon;
        private Monitor volumeMonitor;

        public ABCContext()
        {
            icon = new NotifyIcon()
            {
                Icon = Resources.AudioMixer,
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem ("Quit", Quit)
                }),
                Visible = true
            };
            volumeMonitor = new Monitor(new Config());
            volumeMonitor.Run();
        }

        void Quit (object sender, EventArgs args)
        {
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            icon.Visible = false;
            if (volumeMonitor != null)
            {
                volumeMonitor.Stop();
                volumeMonitor.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
