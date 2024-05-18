using AudioBoxControl.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

        void Quit (object sender, EventArgs args)
        {
            icon.Visible = false;
            Application.Exit();
        }
    }
}
