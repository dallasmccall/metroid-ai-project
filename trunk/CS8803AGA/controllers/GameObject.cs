
namespace CS8803AGA.controllers
{
    /// <summary>
    /// Interface for any type of object which should be in the game.
    /// </summary>
    public interface IGameObject
    {
        /// <summary>
        /// Whether the object is still alive, as in active, used, etc.
        /// </summary>
        /// <returns>True if the object should be kept in game, false if it
        /// should be removed.</returns>
        bool isAlive();

        /// <summary>
        /// How the object updates each frame.
        /// </summary>
        void update();

        /// <summary>
        /// How the object renders each frame.
        /// </summary>
        void draw();
    }
}
