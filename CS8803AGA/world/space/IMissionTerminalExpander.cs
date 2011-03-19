using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI.world.space
{
    /// <summary>
    /// Encapsulation of logic for determining whether a particular mission
    /// terminal can be mapped onto a portion of a space, and if so the
    /// estimated goodness of such a mapping.
    /// </summary>
    interface IMissionTerminalExpander
    {
        String TerminalName { get; set; }
        String MissionNodeID { get; set; }

        void AddLinearRequirement(string missionID);
        ICollection<string> LinearReqs { get; }

        bool PassesMissionRequirements(ISpace space, Point markedPoint, Point unmarkedPoint);

        /// <summary>
        /// Cost of mapping this terminal onto the connection from markedPoint
        /// to unmarkedPoint.  Lower costs are better, costs greater than one
        /// may be ignored by the algorithm.
        /// </summary>
        /// <param name="space"></param>
        /// <param name="markedPoint"></param>
        /// <param name="unmarkedPoint"></param>
        /// <returns></returns>
        float Evaluate(ISpace space, Point markedPoint, Point unmarkedPoint);

        /// <summary>
        /// Modify the provided space to include the region created by this
        /// mission terminal.
        /// </summary>
        /// <param name="space"></param>
        /// <param name="markedPoint"></param>
        /// <param name="unmarkedPoint"></param>
        void Expand(ISpace space, Point markedPoint, Point unmarkedPoint);

        void InitializeParams();
        void AddParameter(String key, String value);

        int GetHashCode();
    }
}
