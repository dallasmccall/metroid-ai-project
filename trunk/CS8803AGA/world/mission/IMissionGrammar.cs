using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS8803AGA.world.mission
{
    /// <summary>
    /// Encapsulation of the rules of a grammar graph for missions.
    /// </summary>
    interface IMissionGrammar
    {
        IMission WalkGrammar(CS8803AGAGameLibrary.player.PlayerModel playerModel);
    }
}
