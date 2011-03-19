using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI.engine
{
    /// <summary>
    /// Manages a stack containing all of the states of the game.
    /// </summary>
    public static class EngineManager
    {
        /// <summary>
        /// The Engine instance of the game.
        /// </summary>
        public static Engine Engine { get { return m_engine; } }
        private static Engine m_engine;

        /// <summary>
        /// Stack containing all states of the current game.
        /// </summary>
        private static Stack<IEngineState> m_stateStack;

        public static void initialize(Engine engine)
        {
            m_engine = engine;
            m_stateStack = new Stack<IEngineState>();
        }

        /// <summary>
        /// Gets the state at the top of the stack.
        /// </summary>
        /// <returns>State at top of stack.</returns>
        public static IEngineState peekAtState()
        {
            return m_stateStack.Peek();
        }

        /// <summary>
        /// Gets the state at a position in the stack.
        /// </summary>
        /// <param name="index">Index of state to get.</param>
        /// <returns>The indexed state.</returns>
        public static IEngineState peekAtState(int index)
        {
            return m_stateStack.ToArray()[index];
        }

        /// <summary>
        /// Gets the state on the stack below the provided state.
        /// </summary>
        /// <param name="esi">State to look below.</param>
        /// <returns>State below esi on the stack.</returns>
        public static IEngineState peekBelowState(IEngineState esi)
        {
            List<IEngineState> list = m_stateStack.ToList();
            int loc = list.FindIndex(i => i == esi);
            if (loc > -1 && loc < list.Count - 1)
            {
                return list[loc + 1];
            }
            throw new Exception("Tried to peek below a state which isn't on the stack or is on the bottom");
        }

        /// <summary>
        /// Adds a state to the top of the stack.
        /// </summary>
        /// <param name="esi">State to push to the top.</param>
        public static void pushState(IEngineState esi)
        {
            m_stateStack.Push(esi);
        }

        /// <summary>
        /// Removes a state from the top of the stack.
        /// </summary>
        /// <returns>State which was just removed from the stack.</returns>
        public static IEngineState popState()
        {
            return m_stateStack.Pop();
        }

        /// <summary>
        /// Removes the current top of the state stack and adds a new state.
        /// </summary>
        /// <param name="esi">State which replaces the top of the stack.</param>
        public static void replaceCurrentState(IEngineState esi)
        {
            m_stateStack.Pop();
            m_stateStack.Push(esi);
        }
    }
}
