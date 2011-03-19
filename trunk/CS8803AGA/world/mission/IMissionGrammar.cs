using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidAI.world.mission
{
    /// <summary>
    /// Encapsulation of the rules of a grammar graph for missions.
    /// </summary>
    interface IMissionGrammar
    {
        IMission WalkGrammar(MetroidAIGameLibrary.player.PlayerModel playerModel);
    }
}
