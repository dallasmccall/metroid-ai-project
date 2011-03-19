using System;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MetroidAIGameLibrary.actions;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;

namespace MetroidAIGameLibrary
{
    /// <summary>
    /// A single image in an Animation, as well as metadata about
    /// that image.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Sprite
    {
        /// <summary>
        /// Location of the sprite in the image file.
        /// </summary>
        [Description("Location of the sprite in the image file.")]
        public Rectangle box { get; set; }

        /// <summary>
        /// Number of frames that the sprite plays in the Animation.
        /// </summary>
        [Description("Number of frames that the sprite plays in the Animation.")]
        public int duration { get; set; }

        /// <summary>
        /// AnimationActions that should fire when this sprite is reached.
        /// </summary>
        [Description("AnimationActions that should fire when this sprite is reached.")]
        public AActionInfo[] action { get; set; }

        /// <summary>
        /// Distance from the sprite's visual center to the central point in
        /// the image
        /// </summary>
        [ContentSerializer(Optional=true)]
        [Description("Distance from the sprite's visual center to the central point in the image.")]
        public Point offset { get; set; }

        /// <summary>
        /// Distance from the sprite's weapon point to the central point in the image
        /// </summary>
        /// <returns></returns>
        [ContentSerializer(Optional = true)]
        [Description("Distance from the sprite's weapon point to the central point in the image.")]
        public Point shootPoint { get; set; }

        public override string ToString()
        {
            return String.Format("box:{0}", box.ToString());
        }

        public Sprite()
        {
            shootPoint = new Point(-5, -5);
        }
    }

    /// <summary>
    /// Encapsulation of a set of sprites which should be played in order
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Animation
    {
        /// <summary>
        /// Name of the animation, can be used to tell an AnimationController
        /// to play this animation
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Name used by AnimationController to play this Animation.")]
        public string name { get; set; }

        /// <summary>
        /// Sprites played by this animation, from first to last
        /// </summary>
        [Description("Sprites played by this animation, from first to last.")]
        public Sprite[] sprites { get; set; }

        /// <summary>
        /// Determines which animations can be interrupted by others
        /// 0 = Animation can be interrupted by anything, can go idle anytime
        /// 1+ = Animation can be interrupted only by higher priority anims,
        ///     and it can only go idle when finished
        /// </summary>
        [Description("Determines which animations can be interrupted by others\n0 = Animation can be interrupted by anything, can go idle anytime\n1+ = Animation can be interrupted only by higher priority anims, and it can only go idle when finished")]
        public int priority { get; set; }

        /// <summary>
        /// Whether this animation should loop automatically.
        /// Most 0 priority animations should loop
        /// </summary>
        [Description("Whether this animation should loop automatically.")]
        public bool loops { get; set; }

        /// <summary>
        /// Index of sprite which should be played during idle state
        /// </summary>
        [Description("Index of sprite which should be played during idle state.")]
        public int idleFrame { get; set; }

        /// <summary>
        /// The animation that should be called when this one is done playing,
        /// or "idle" to go idle
        /// </summary>
        [Description("The animation that should be called when this one is done playing, or 'idle' to go idle.")]
        public string whenDone { get; set; }

        public Animation()
        {
            this.sprites = new Sprite[0];
        }

        public Animation(string name, int numSprites, int priority, bool loops, int idleFrame, string whenDone)
        {
            this.name = name;
            this.sprites = new Sprite[numSprites];
            this.priority = priority;
            this.loops = loops;
            this.idleFrame = idleFrame;
            this.whenDone = whenDone;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", name, this.sprites.Length.ToString());
        }
    }

    /// <summary>
    /// A set of Animations
    /// </summary>
    public class AnimationSet
    {
        /// <summary>
        /// Animations contained by the set
        /// </summary>
        [Description("Animations contained by the set.")]
        public Animation[] anims { get; set; }

        public AnimationSet()
        {
            anims = new Animation[0];
        }
    }

    /*public class AnimationSetContentReader : ContentTypeReader<AnimationSet>
    {

        protected override AnimationSet Read(ContentReader input, AnimationSet existingInstance)
        {
            AnimationSet aset = new AnimationSet();
            int animsCount = input.ReadInt32();
            aset.anims = new Animation[animsCount];
            for (int i = 0; i < animsCount; ++i)
            {
                Animation a = new Animation(
                    input.ReadString(),
                    input.ReadInt32(),
                    input.ReadInt32(),
                    input.ReadBoolean(),
                    input.ReadInt32(),
                    input.ReadString()
                    );
                aset.anims[i] = a;

                for (int j = 0; j < a.sprites.Length; ++j)
                {
                    Sprite f = new Sprite();
                    a.sprites[j] = f;

                    f.box = new Rectangle(
                        input.ReadInt32(),
                        input.ReadInt32(),
                        input.ReadInt32(),
                        input.ReadInt32()
                        );
                    //f.center = new Point(
                    //    input.ReadInt32(),
                    //    input.ReadInt32()
                    //    );
                    f.duration = input.ReadInt32();
                    //f.action = input.ReadString();
                    f.offset = input.ReadVector2();
                }
            }

            return aset;
        }
    }*/
}