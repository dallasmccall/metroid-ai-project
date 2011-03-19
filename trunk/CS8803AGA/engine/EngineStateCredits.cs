using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.ui;
using CS8803AGA.devices;
using Microsoft.Xna.Framework;

namespace CS8803AGA.engine
{
    class EngineStateCredits : AEngineState
    {
        private MenuList m_menuList;

        public EngineStateCredits(Engine engine) : base (engine)
        {
            Point center = m_engine.GraphicsDevice.Viewport.TitleSafeArea.Center;
            int top = m_engine.GraphicsDevice.Viewport.TitleSafeArea.Bottom;

            List<String> credits = new List<String>();
            credits.Add("CREDITS");
            credits.Add("");
            credits.Add("");
            credits.Add("SUPER METROID");
            credits.Add("Nintendo");
            credits.Add("");
            credits.Add("METROID GENERATIONS");
            credits.Add("Ken Hartsook");
            credits.Add("Dallas McCall");
            credits.Add("Alexander Zook");
            credits.Add("");
            credits.Add("TEAM COMMANDO");
            credits.Add("Eric Barnes");
            credits.Add("Andrew Pitman");
            credits.Add("Jared Segal");
            credits.Add("Ken Hartsook");
            credits.Add("");
            credits.Add("CODE SUPPORT");
            credits.Add("ecassidy");
            credits.Add("");
            credits.Add("SPRITES");
            credits.Add("Tommy Lee (superiorLordTommy@hotmail.com)");
            credits.Add("jathys");
            credits.Add("");
            credits.Add("MUSIC");
            credits.Add("Metroid Metal");
            credits.Add("Stemage");
            credits.Add("");
            credits.Add("SPECIAL THANKS");
            credits.Add("Mark Riedl");
            credits.Add("Boyang \"Albert\" Li");
            credits.Add("");

            m_menuList = new MenuList(credits, new Vector2(center.X, top));
            m_menuList.Font = FontEnum.Kootenay14;
            m_menuList.ItemSpacing = 50;
            m_menuList.SpaceAvailable = 20000;
        }

        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            m_menuList.Position = new Vector2(
                m_menuList.Position.X, m_menuList.Position.Y - 3);

            if (InputSet.getInstance().getButton(InputsEnum.CONFIRM_BUTTON) ||
                InputSet.getInstance().getButton(InputsEnum.CANCEL_BUTTON) ||
                InputSet.getInstance().getButton(InputsEnum.BUTTON_1) ||
                InputSet.getInstance().getButton(InputsEnum.BUTTON_2))
            {
                InputSet.getInstance().setAllToggles();

                EngineManager.replaceCurrentState(new EngineStateMainMenu(m_engine));
            }
        }

        public override void draw()
        {
            m_menuList.draw();
        }
    }
}
