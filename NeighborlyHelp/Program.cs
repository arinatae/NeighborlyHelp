using NeighborlyHelp.Forms;
using System;
using System.Windows.Forms;

namespace NeighborlyHelp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainMenuForm()); 
        }
    }
}