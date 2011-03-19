using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidAI.engine;
using Microsoft.Xna.Framework.Graphics;

namespace MetroidAI.controllers.projectiles
{
    class BulletController : ProjectileController
    {
        public static readonly int Speed = 35;
        public static readonly int Damage = 1;

        public BulletController(IGameObject owner, Vector2 position, Vector2 ownerVelocity, Vector2 direction) :
            base(
                owner,
                position,
                CommonFunctions.normalizeNonmutating(direction) * Speed + ownerVelocity,
                ProjectileType.Bullet,
                Damage,
                @"Sprites/Bullet")
        {
            if (GameplayManager.Samus.Inventory.HasItem(Item.IceBeam))
            {
                Tint = Color.CornflowerBlue;
                Freezes = true;
            }
        }
    }
}
