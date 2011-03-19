namespace CSharpQuadTree
{

    public struct DoubleSize
    {
        private double width;
        private double height;

        public DoubleSize(double width, double height)
        {
            this.width = width;
            this.height = height;
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
    }
}