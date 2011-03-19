using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel;

namespace MetroidAIGameLibrary
{
    /// <summary>
    /// Encapsulation of metadata used to create a CharacterController
    /// </summary>
    public class CharacterInfo
    {
        /// <summary>
        /// Asset path to the image file with the sprite sheet
        /// </summary>
        [Description("Asset path to the image file with the sprite sheet")]
        public string animationTexturePath { get; set; }

        /// <summary>
        /// Asset path to the AnimationSet xml
        /// </summary>
        [Description("Asset path to the AnimationSet xml")]
        public string animationDataPath { get; set; }

        /// <summary>
        /// Collision area relative to center on *scaled* character
        /// </summary>
        [Description("Collision area relative to center on *scaled* character")]
        public Rectangle collisionBox { get; set; }

        /// <summary>
        /// Movement speed of the character in pixels/frame
        /// </summary>
        [Description("Movement speed of the character in pixels/frame")]
        public int speed { get; set; }

        /// <summary>
        /// Amount of scaling to perform on the texture
        /// </summary>
        [Description("Amount of scaling to perform on the texture")]
        public float scale { get; set; }

        public CharacterInfo()
        {
            speed = 5;
            scale = 1.0f;
        }
    }
}
