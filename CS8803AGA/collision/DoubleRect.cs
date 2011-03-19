using Microsoft.Xna.Framework;
namespace CSharpQuadTree
{

    public struct DoubleRect
    {
        private double x;
        private double y;
        private double width;
        private double height;

        public DoubleRect(DoublePoint pt, DoubleSize size)
        {
            this.x = pt.X;
            this.y = pt.Y;
            this.width = size.Width;
            this.height = size.Height;
        }

        public DoubleRect(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public DoubleRect(ref Rectangle rect)
        {
            this.x = rect.X;
            this.y = rect.Y;
            this.width = rect.Width;
            this.height = rect.Height;
        }

        public static DoubleRect operator +(DoubleRect rect, Vector2 dp)
        {
            return new DoubleRect(
                rect.x + dp.X,
                rect.y + dp.Y,
                rect.width,
                rect.height);
        }

        public bool Contains(DoublePoint pt)
        {
            // TODO verify this
            return (pt.X >= this.X &&
                    pt.X <= this.X + this.width &&
                    pt.Y >= this.Y &&
                    pt.Y <= this.Y + this.height);
        }

        public bool Contains(DoubleRect other)
        {
            // TODO verify this
            return (this.x <= other.x &&
                    this.x + this.width >= other.x + other.width &&
                    this.y <= other.y &&
                    this.y + this.height >= other.y + other.height);
        }

        public bool IntersectsWith(DoubleRect other)
        {
            // TODO verify this
            if (this.y + this.height <= other.y) return false;
            if (this.y >= other.y + other.height) return false;
            if (this.x + this.width <= other.x) return false;
            if (this.x >= other.x + other.width) return false;

            return true;
        }

        public Vector2 Center()
        {
            return new Vector2((float)(this.x + this.Width / 2), (float)(this.y + this.Height / 2));
        }

        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public Rectangle truncateToRectangle()
        {
            return new Rectangle((int)this.x, (int)this.y, (int)this.width, (int)this.height);
        }
    }
}