using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.controllers.projectiles
{
    class MissileController : ProjectileController
    {
        public static readonly int Speed = 40;
        public static readonly int Damage = 5;

        public MissileController(IGameObject owner, Vector2 position, Vector2 ownerVelocity, Vector2 direction) :
            base(
                owner,
                position,
                CommonFunctions.normalizeNonmutating(direction) * Speed + ownerVelocity,
                ProjectileType.Missile,
                Damage,
                @"Sprites/Missile")
        {
            //  nch
        }
    }
}
