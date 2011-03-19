using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.world.space
{
    /// <summary>
    /// Simple class for creating a grid-based world, each object is a cell in the grid and contains
    /// an environment type and whether it is connected to another cell on each side
    /// </summary>
    public class ScreenConstructionParameters
    {
        public Point Location { get; set; }
        internal String Label { get; set; }
        internal Dictionary<Direction, Connection> Connections { get; set; }
        internal IFractalCreator FractalCreator { get; set; }
        internal IScreenPostProcessor PostProcessor { get; set; }
        internal IObjectPopulator ObjectPopulator { get; set; }

        public ScreenConstructionParameters(Point location)
            : this(location, "")
        {
            // nch
        }

        public ScreenConstructionParameters(Point location, String label)
            : this(location, label, new FractalCreatorJagged())
        {
            // nch
        }

        public ScreenConstructionParameters(Point location, String label, IFractalCreator fractalCreator)
        {
            this.Location = location;
            this.Label = label;
            this.Connections = new Dictionary<Direction, Connection>();
            this.FractalCreator = fractalCreator;
            this.PostProcessor = new ScreenPostProcessorEmpty();
            this.ObjectPopulator = new ObjectPopulatorEmpty();

            foreach (Direction d in Direction.All)
            {
                this.Connections[d] = Connection.None;
            }
        }

        public ScreenConstructionParameters DeepCopy()
        {
            ScreenConstructionParameters copy = new ScreenConstructionParameters(
                this.Location,
                this.Label,
                this.FractalCreator);
            copy.PostProcessor = this.PostProcessor;
            copy.ObjectPopulator = this.ObjectPopulator;

            foreach (KeyValuePair<Direction, Connection> kvp in this.Connections)
            {
                copy.Connections[kvp.Key] = kvp.Value;
            }

            return copy;
        }
    }
}
