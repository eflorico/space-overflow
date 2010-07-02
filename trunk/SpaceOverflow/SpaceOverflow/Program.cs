using System;

namespace SpaceOverflow
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        [STAThread()]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            using (App game = new App())
            {
                game.Run();
            }
        }
    }
}

