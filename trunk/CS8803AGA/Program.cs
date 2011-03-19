using System;

namespace CS8803AGA
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //CS8803AGA.world.mission.MissionGrammar grammar = CS8803AGA.world.mission.MissionGrammar.LoadFromFile("Mission/MissionGrammar.xml");
            //grammar.printGrammar();
            //return;
            using (Engine game = new Engine())
            {
                game.Run();
            }
        }
    }
}

