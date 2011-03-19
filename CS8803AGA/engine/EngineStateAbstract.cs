namespace CS8803AGA.engine
{
    public abstract class AEngineState : IEngineState
    {
        protected Engine m_engine;

        public AEngineState(Engine engine)
        {
            m_engine = engine;
        }

        public abstract void update(Microsoft.Xna.Framework.GameTime gameTime);

        public abstract void draw();

    }
    
}