using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGAGameLibrary.actions;

namespace CS8803AGA.actions
{
    /// <summary>
    /// Interface for Actions, which are (intentionally) generic events for
    /// causing something to happen in the game (audio, visual effect, spell,
    /// whatever).
    /// </summary>
    public interface IAction
    {
        void execute(object source);
    }
}
