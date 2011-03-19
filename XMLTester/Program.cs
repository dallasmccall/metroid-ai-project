using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework;

using CS8803AGAGameLibrary;
using CS8803AGAGameLibrary.actions;
using CS8803AGAGameLibrary.player;

namespace XMLTester
{
    class Program
    {
        static void XnaSerialize(object data)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create("test.xml", settings))
            {
                IntermediateSerializer.Serialize(writer, data, null);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                ActionInfoAttack ai = new ActionInfoAttack();
                ai.location = new Rectangle(0, 0, 20, 20);

                Sprite sprite = new Sprite();
                sprite.action = new AActionInfo[1];
                sprite.action[0] = ai;


                

                

                


                // demonstration versions
                // note these are total nonsense
                /*
                    {"accuracy", 0.5f},         // (# hit) / (# fired)
                    {"shots", 100f},            // total shots fired
                    {"damageTaken", 10f},       // total damage taken
                    {"damageDone", 8f},         // total damage inflicted to enemies
                    {"roomsExplored", 0.5f},    // % of total space explored
                    {"roomsVisited", 10f}      // total rooms entered
                 */
                PlayerModel testModel = new PlayerModel();
                List<string> explorativityList = new List<string>()
                {
                    "roomsExplored", "30", "*", "roomsVisited", "+"
                };
                ModelFormula explorativityForm = new ModelFormula(explorativityList);
                float explorativityVal = explorativityForm.evalFormula(testModel.getAllStats());

                List<string> killativityList = new List<string>()
                {
                    "damageTaken", "0.5", "*", "damageDone", "2.0", "*", "-", 
                    "accuracy", "30", "*", "shots", "+", "+"
                };
                ModelFormula killativityForm = new ModelFormula(killativityList);
                float killativityVal = killativityForm.evalFormula(testModel.getAllStats());


                ModelFormula testXmlForm = new ModelFormula("C:/mark_lab/branches/project0/CS8803AGA/player/formula/exploreFormula.xml");
                float testXmlVal = testXmlForm.evalFormula(testModel.getAllStats());
                ModelFormula formA = new ModelFormula("postfix", "25+22+*");
                object test = killativityForm;

                object testData = sprite;

                /*
                Dictionary<string, float> testDict = new Dictionary<string, float>()
                {
                    {"a", 100},
                    {"b", 150}
                };

                ModelFormula formA = new ModelFormula("postfix", "2a+22+*");
                string testStrApre = formA.PrefixString();
                string testStrApost = formA.PostfixString();
                float testValA = formA.evalFormula(testDict);
                ModelFormula formB = new ModelFormula("prefix", "+2*3^25");
                string testStrBpre = formA.PrefixString();
                string testStrBpost = formA.PostfixString();
                float testValB = formB.evalFormula(testDict);
                object test = formA;
                */
                /*
                DecorationSetInfo testData = new DecorationSetInfo();
                testData.assetPath = "Sprites/trees";
                testData.decorations = new Dictionary<string, DecorationInfo>();

                DecorationInfo di = new DecorationInfo();
                testData.decorations.Add("forest", di);
                 * */

                /*
                CharacterInfo testData = new CharacterInfo();
                testData.animationDataPath = @"Animation/Data/mask";
                testData.animationTexturePath = @"Animation/Sprites/mask";
                testData.collisionBox = new Rectangle(-14, 22, 28, 10);
                testData.speed = 5;
                 */

                /*
                TileSet testData = new TileSet(15);
                testData.assetPath = "Sprites/grassRock";
                testData.tileWidth = 40;
                testData.tileHeight = 40;
                testData.tileInfos[0].passable = true;
                testData.tileInfos[1].passable = true;
                testData.tileInfos[2].passable = true;
                testData.tileInfos[12].passable = true;
                testData.columnsOnSpritesheet = 5;
                 */

                /*
                PropAnimSet pas = new PropAnimSet();
                pas.props.Add("frameHeight", "32");
                pas.props.Add("frameWidth", "23");
                pas.props.Add("duration", "5");
                pas.anims.Add(new PropAnim("right", 2));
                pas.anims.Add(new PropAnim("left", 2));
                pas.anims.Add(new PropAnim("up", 2));
                pas.anims.Add(new PropAnim("down", 2));
                pas.anims.Add(new PropAnim("upright", 2));
                pas.anims.Add(new PropAnim("downleft", 2));
                pas.anims.Add(new PropAnim("upleft", 2));
                pas.anims.Add(new PropAnim("downright", 2));

                Object testData = pas;
                 */

                /*
                AnimationSet testData = new AnimationSet();
                testData.anims = new List<Animation>();
                Animation walk = new Animation("walk", 2, 1, true, 0, "idle");
                walk.frames[0] = new Frame();
                walk.frames[0].box = new Rectangle(0, 0, 23, 32);
                walk.frames[0].duration = 2;
                walk.frames[0].trigger = "";
                walk.frames[1] = new Frame();
                walk.frames[1].box = new Rectangle(23, 0, 23, 32);
                walk.frames[1].duration = 2;
                walk.frames[1].trigger = "";
                Animation run = new Animation("run", 2, 1, true, 0, "idle");
                run.frames[0] = new Frame();
                run.frames[0].box = new Rectangle(0, 33, 23, 32);
                run.frames[0].duration = 2;
                run.frames[0].trigger = "";
                run.frames[1] = new Frame();
                run.frames[1].box = new Rectangle(23, 33, 23, 32);
                run.frames[1].duration = 2;
                run.frames[1].trigger = "";
                testData.anims.Add(walk);
                testData.anims.Add(run);
                 * */

                XnaSerialize(test);
                
            }
            catch (System.IO.FileNotFoundException fex)
            {
                //fex.Data;
                int a = 7;
            }
        }
    }
}
