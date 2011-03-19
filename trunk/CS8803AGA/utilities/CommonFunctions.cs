/*
***************************************************************************
* Copyright notice removed by a creator for anonymity, please don't sue   *
*                                                                         *
* Licensed under the Apache License, Version 2.0 (the "License");         *
* you may not use this file except in compliance with the License.        *
* You may obtain a copy of the License at                                 *
*                                                                         *
* http://www.apache.org/licenses/LICENSE-2.0                              *
*                                                                         *
* Unless required by applicable law or agreed to in writing, software     *
* distributed under the License is distributed on an "AS IS" BASIS,       *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.*
* See the License for the specific language governing permissions and     *
* limitations under the License.                                          *
***************************************************************************
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA
{
    public static class CommonFunctions
    {
        public static void swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static Vector2 normalizeNonmutating(Vector2 v)
        {
            Vector2 copy = v;
            copy.Normalize();
            return copy;
        }

        public static float distance(Vector2 point1, Vector2 point2)
        {
            point1.X -= point2.X;
            point1.Y -= point2.Y;
            return point1.Length();
            //return (float)Math.Sqrt(point1.X * point1.X + point1.Y * point1.Y);
        }

        /*
        public static float distance(TileIndex tile1, TileIndex tile2)
        {
            return (float)Math.Sqrt(Math.Pow(tile1.x_ - tile2.x_, 2) + Math.Pow(tile1.y_ - tile2.y_, 2));
        }
        */

        public static void rotate(ref Vector2 vector, float angle)
        {
            Vector2 temp = vector;

            vector.X = (float)((temp.X * Math.Cos(angle)) - (temp.Y * Math.Sin(angle)));
            vector.Y = (float)((temp.Y * Math.Cos(angle)) + (temp.X * Math.Sin(angle)));
        }

        public static void rotate(ref Vector2 vector, double angle)
        {
            Vector2 temp = vector;

            vector.X = (float)((temp.X * Math.Cos(angle)) - (temp.Y * Math.Sin(angle)));
            vector.Y = (float)((temp.Y * Math.Cos(angle)) + (temp.X * Math.Sin(angle)));
        }

        public static Vector2 rotate(Vector2 vector, float angle)
        {
            Vector2 temp = vector;

            vector.X = (float)((temp.X * Math.Cos(angle)) - (temp.Y * Math.Sin(angle)));
            vector.Y = (float)((temp.Y * Math.Cos(angle)) + (temp.X * Math.Sin(angle)));

            return vector;
        }

        public static Vector2 rotate(Vector2 vector, double angle)
        {
            Vector2 temp = vector;

            vector.X = (float)((temp.X * Math.Cos(angle)) - (temp.Y * Math.Sin(angle)));
            vector.Y = (float)((temp.Y * Math.Cos(angle)) + (temp.X * Math.Sin(angle)));

            return vector;
        }

        /// <summary>
        /// Returns the angle of a direction or rotation vector in radians
        /// </summary>
        /// <param name="vector">The direction or rotation vector</param>
        /// <returns></returns>
        public static float getAngle(Vector2 vector)
        {
            return (float)Math.Atan2((double)vector.Y, (double)vector.X);
        }

        /// <summary>
        /// Returns the unit vector corresponding to the rotation
        /// </summary>
        /// <param name="rotation">The rotation in radians</param>
        /// <returns></returns>
        public static Vector2 getVector(float rotation)
        {
            double cos = Math.Cos(rotation);
            double sin = Math.Sin(rotation);
            Vector2 result = new Vector2((float)cos, (float)sin);
            return result;
        }

        /// <summary>
        /// Returns the unit vector corresponding to the rotation
        /// </summary>
        /// <param name="rotation">The rotation in radians</param>
        /// <returns></returns>
        public static Vector2 getVector(double rotation)
        {
            double cos = Math.Cos(rotation);
            double sin = Math.Sin(rotation);
            Vector2 result = new Vector2((float)cos, (float)sin);
            return result;
        }

        public static float dotProduct(Vector2 first, Vector2 second)
        {
            return (first.X * second.X) + (first.Y * second.Y);
        }
    }
}
