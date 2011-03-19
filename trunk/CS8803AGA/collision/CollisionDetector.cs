using Microsoft.Xna.Framework;
using CSharpQuadTree;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace MetroidAI.collision
{
    /// <summary>
    /// Encapsulation of a QuadTree for collision detection; handles
    /// management of objects placed in the tree.
    /// </summary>
    public class CollisionDetector
    {
        /// <summary>
        /// QuadTree to maintain locations of objects known by the detector.
        /// </summary>
        private QuadTree<Collider> m_tree;

        /// <summary>
        /// Default ctor
        /// </summary>
        public CollisionDetector()
        {
            // TODO: evaluate these numbers
            // sample usage says
            // "Use larger min size, and higher min object values for better performance"
            m_tree = new QuadTree<Collider>(new DoubleSize(25, 25), 0, false);
        }

        /// <summary>
        /// Adds a collider to the detector and marks that collider as being
        /// owned by this detector
        /// </summary>
        /// <param name="ci">Collider to register</param>
        public void register(Collider ci)
        {
            ci.forCollisionDetectorUseOnly(this);
            m_tree.Insert(ci);
        }

        /// <summary>
        /// Queries the CollisionDetector for all colliders which intersect
        /// with the query area.
        /// </summary>
        /// <param name="queryArea">Area to retrieve all intersectors</param>
        /// <returns>List of all Colliders which intersect with QueryArea</returns>
        public List<Collider> query(DoubleRect queryArea)
        {
            return m_tree.Query(queryArea);
        }

        /// <summary>
        /// Removes a particular collider from this CollisionDetector
        /// </summary>
        /// <param name="ci">Collider to remove</param>
        public void remove(Collider ci)
        {
            m_tree.Remove(ci);
            ci.forCollisionDetectorUseOnly(null);
        }

        /// <summary>
        /// Gets all nodes in the underlying QuadTree
        /// </summary>
        /// <returns></returns>
        public List<QuadTree<Collider>.QuadNode> getAllNodes()
        {
            return m_tree.GetAllNodes();
        }

        /// <summary>
        /// Moves a collider in the direction provided until it hits something
        /// </summary>
        /// <param name="mover">Collider which is moving</param>
        /// <param name="deltaPosition">Change in position the collider is trying to make</param>
        public void handleMovement(Collider mover, Vector2 deltaPosition)
        {
            double areaOfMovementX1 = Math.Min(mover.Bounds.X, mover.Bounds.X + deltaPosition.X);
            double areaOfMovementY1 = Math.Min(mover.Bounds.Y, mover.Bounds.Y + deltaPosition.Y);
            double areaOfMovementX2 = Math.Max(mover.Bounds.X + mover.Bounds.Width, mover.Bounds.X + mover.Bounds.Width + deltaPosition.X);
            double areaOfMovementY2 = Math.Max(mover.Bounds.Y + mover.Bounds.Height, mover.Bounds.Y + mover.Bounds.Height + deltaPosition.Y);

            DoubleRect areaOfMovement = new DoubleRect(
                areaOfMovementX1,
                areaOfMovementY1,
                areaOfMovementX2 - areaOfMovementX1,
                areaOfMovementY2 - areaOfMovementY1);

            //DoubleRect newbounds = mover.Bounds + deltaPosition;

            List<Collider> collisions = m_tree.Query(areaOfMovement);
            List<Collider>.Enumerator i = collisions.GetEnumerator();
            Vector2 allowedMovement = deltaPosition;
            Vector2 temp;
            while (i.MoveNext())
            {
                // we will usually collide with our old position - ignore that case
                if (i.Current != mover)
                {
                    bool canMove = CollisionHandler.handleMovement(mover, i.Current, deltaPosition, out temp);
                    if (!canMove)
                    {
                        return; // don't allow movement
                    }
                    if (allowedMovement.X > 0.0f)
                        allowedMovement.X = Math.Min(allowedMovement.X, temp.X);
                    else if (allowedMovement.X < 0.0f)
                        allowedMovement.X = Math.Max(allowedMovement.X, temp.X);
                    if (allowedMovement.Y > 0.0f)
                        allowedMovement.Y = Math.Min(allowedMovement.Y, temp.Y);
                    else if (allowedMovement.Y < 0.0f)
                        allowedMovement.Y = Math.Max(allowedMovement.Y, temp.Y);
                }
            }

            mover.move(allowedMovement);
        }

        public void draw()
        {
            draw(false);
        }

        public void draw(bool drawQuadTree)
        {
            List<QuadTree<Collider>.QuadNode> nodes = m_tree.GetAllNodes();
            foreach (QuadTree<Collider>.QuadNode node in nodes)
            {
                if (drawQuadTree)
                {
                    DoubleRect dr = node.Bounds;
                    LineDrawer.drawLine(new Vector2((float)dr.X, (float)dr.Y),
                                        new Vector2((float)dr.X + (float)dr.Width, (float)dr.Y),
                                        Color.AliceBlue);
                    LineDrawer.drawLine(new Vector2((float)dr.X, (float)dr.Y),
                                        new Vector2((float)dr.X, (float)dr.Y + (float)dr.Height),
                                        Color.AliceBlue);
                    LineDrawer.drawLine(new Vector2((float)dr.X + (float)dr.Width, (float)dr.Y),
                                        new Vector2((float)dr.X + (float)dr.Width, (float)dr.Y + (float)dr.Height),
                                        Color.AliceBlue);
                    LineDrawer.drawLine(new Vector2((float)dr.X, (float)dr.Y + (float)dr.Height),
                                        new Vector2((float)dr.X + (float)dr.Width, (float)dr.Y + (float)dr.Height),
                                        Color.AliceBlue);
                }

                foreach (Collider collider in node.quadObjects)
                {
                    DoubleRect dr2 = collider.Bounds;
                    LineDrawer.drawLine(new Vector2((float)dr2.X, (float)dr2.Y),
                                    new Vector2((float)dr2.X + (float)dr2.Width, (float)dr2.Y),
                                    Color.LimeGreen);
                    LineDrawer.drawLine(new Vector2((float)dr2.X, (float)dr2.Y),
                                        new Vector2((float)dr2.X, (float)dr2.Y + (float)dr2.Height),
                                        Color.LimeGreen);
                    LineDrawer.drawLine(new Vector2((float)dr2.X + (float)dr2.Width, (float)dr2.Y),
                                        new Vector2((float)dr2.X + (float)dr2.Width, (float)dr2.Y + (float)dr2.Height),
                                        Color.LimeGreen);
                    LineDrawer.drawLine(new Vector2((float)dr2.X, (float)dr2.Y + (float)dr2.Height),
                                        new Vector2((float)dr2.X + (float)dr2.Width, (float)dr2.Y + (float)dr2.Height),
                                        Color.LimeGreen);
                }
            }
        }
    }
}