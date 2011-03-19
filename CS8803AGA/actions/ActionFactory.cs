using System;
using MetroidAIGameLibrary.actions;

namespace MetroidAI.actions
{
    /// <summary>
    /// ActionFactory can be used to construct Actions from ActionInfo
    /// objects; essentially it should map the ActionInfo subclasses to
    /// Action subclasses.
    /// </summary>
    public static class ActionFactory
    {
        /// <summary>
        /// Construct the appropriate Action subclass from an info.
        /// </summary>
        /// <param name="actionInfo">Contains action subclass info.</param>
        /// <returns>An IAction constructed from the provided info.</returns>
        public static IAction construct(AActionInfo actionInfo)
        {
            throw new NotImplementedException("Unimplemented action type");
        }
    }
}
