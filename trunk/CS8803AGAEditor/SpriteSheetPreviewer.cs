using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetroidAIGameLibrary;
using System.Drawing.Drawing2D;

namespace MetroidAIEditor
{
    public delegate void AnimationExportEventHandler(object sender, AnimationSet aset);

    public partial class SpriteSheetPreviewer : Form
    {
        public event AnimationExportEventHandler AnimationExported;

        enum Mode
        {
            None, Draw, MoveCenter, MoveShootPoint
        }

        static readonly Point s_invalidPoint = new Point(-1, -1);

        private Mode m_mode = Mode.None;
        private SpriteBox m_grabbedSpriteBox = null;
        private Point m_dragStart = s_invalidPoint;

        #region Construction and Loading

        public SpriteSheetPreviewer()
        {
            InitializeComponent();
            LoadImageFile();
        }

        public SpriteSheetPreviewer(AnimationSet animationSet) : this()
        {
            m_pictureBox.AnimationSet = animationSet;
        }

        private void LoadImageFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files (*.png;*.bmp;*.jpg;*.gif)|*.png;*.bmp;*.jpg;*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bitmap image = new Bitmap(ofd.FileName);
                m_pictureBox.Size = image.Size;
                m_pictureBox.Image = image;
            }
        }

        #endregion

        #region Mouse Events

        private void m_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point moveCoords = e.Location;

            Point scaledPt = m_pictureBox.scalePoint(e.Location);

            m_tbsCursorPos.Text = String.Format("Pos: {0},{1}", scaledPt.X, scaledPt.Y);

            if (m_mode == Mode.None)
            {
                SpriteBox temp = m_pictureBox.findSpriteBox(scaledPt);
                if (temp != null)
                {
                    this.Cursor = Cursors.SizeAll;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }

            if (m_mode == Mode.Draw)
            {
                m_pictureBox.TemporarySpriteBox =
                    new SpriteBox(m_bColor.BackColor,
                        m_pictureBox.scaleInvertRectangle(calculateRectangle(m_dragStart, moveCoords)));

                m_tbsCurRect.Text =
                    String.Format("Corner: {0},{1}  Size: {2},{3}",
                        m_pictureBox.TemporarySpriteBox.Location.X,
                        m_pictureBox.TemporarySpriteBox.Location.Y,
                        m_pictureBox.TemporarySpriteBox.Location.Width + 1,
                        m_pictureBox.TemporarySpriteBox.Location.Height + 1);
            }

            if (m_mode == Mode.MoveCenter)
            {
                m_grabbedSpriteBox.Center = scaledPt;
                m_pictureBox.Refresh();
            }

            if (m_mode == Mode.MoveShootPoint)
            {
                m_grabbedSpriteBox.ShootPoint = scaledPt;
                m_pictureBox.Refresh();
            }
        }

        private void m_pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Point scaledPt = m_pictureBox.scalePoint(e.Location);
            SpriteBox temp = m_pictureBox.findSpriteBox(scaledPt);
            if (temp != null)
            {
                m_grabbedSpriteBox = temp;
                this.Cursor = Cursors.SizeAll;

                if (m_grabbedSpriteBox.Center == scaledPt)
                {
                    m_mode = Mode.MoveCenter;
                }
                else if (m_grabbedSpriteBox.ShootPoint == scaledPt)
                {
                    m_mode = Mode.MoveShootPoint;
                }
            }
            else
            {
                m_dragStart = e.Location;
                m_mode = Mode.Draw;
            }  
        }

        private void m_pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            Point dragEnd = e.Location;

            if (m_mode == Mode.Draw)
            {
                if (m_dragStart != s_invalidPoint && m_dragStart != dragEnd)
                {
                    Rectangle r = calculateRectangle(m_dragStart, dragEnd);
                    r = m_pictureBox.scaleInvertRectangle(r);

                    m_pictureBox.addSpriteBox(new SpriteBox(m_bColor.BackColor, r));
                    m_pictureBox.TemporarySpriteBox = null;

                    m_dragStart = s_invalidPoint;
                }
            }

            if (m_mode == Mode.MoveCenter)
            {
                m_grabbedSpriteBox.Center = m_pictureBox.scalePoint(e.Location);
                m_grabbedSpriteBox = null;
            }

            if (m_mode == Mode.MoveShootPoint)
            {
                m_grabbedSpriteBox.ShootPoint = m_pictureBox.scalePoint(e.Location);
                m_grabbedSpriteBox = null;
            }

            m_mode = Mode.None;
        }

        #endregion

        #region Button and Tool Events

        private void m_bColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = m_bColor.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                m_bColor.BackColor = cd.Color;
            }
        }

        private void m_tbScale_Scroll(object sender, EventArgs e)
        {
            int pow = m_tbScale.Value;
            float scale = (float)Math.Pow(2, pow);
            m_pictureBox.ImageScale = scale;
        }

        private void m_bUndo_Click(object sender, EventArgs e)
        {
            m_pictureBox.popSpriteBox();
        }

        private void m_bExport_Click(object sender, EventArgs e)
        {
            AnimationSet animSet = m_pictureBox.makeAnimationSet();
            AnimationExported(this, animSet);
        }

        #endregion

        #region Helpers

        private Rectangle calculateRectangle(Point p1, Point p2)
        {
            return new Rectangle(
                    Math.Min(p1.X, p2.X),
                    Math.Min(p1.Y, p2.Y),
                    Math.Abs(p1.X - p2.X),
                    Math.Abs(p1.Y - p2.Y)
                    );
        }

        #endregion

    }

    public class SpriteBox
    {
        #region Properties

        public Color Color {
            get { return m_color; }
            set { m_color = value; }
        }
        public Rectangle Location {
            get { return m_location; }
            set {
                m_location = value;

                m_sprite.box = new Microsoft.Xna.Framework.Rectangle(
                        m_location.X,
                        m_location.Y,
                        m_location.Width + 1,
                        m_location.Height + 1); ;
            }
        }
        public Point Center {
            get { return m_center; }
            set {
                m_center = value;

                Point locCenter = new Point(
                        m_location.X + (m_location.Width + 1) / 2,
                        m_location.Y + (m_location.Height + 1) / 2
                        );
                m_sprite.offset = new Microsoft.Xna.Framework.Point(
                    locCenter.X - Center.X,
                    locCenter.Y - Center.Y);
            }
        }
        public Point ShootPoint
        {
            get { return m_shootPoint; }
            set
            {
                m_shootPoint = value;

                Point locCenter = new Point(
                        m_location.X + (m_location.Width + 1) / 2,
                        m_location.Y + (m_location.Height + 1) / 2
                        );
                m_sprite.shootPoint = new Microsoft.Xna.Framework.Point(
                    locCenter.X - m_shootPoint.X,
                    locCenter.Y - m_shootPoint.Y);
            }
        }
        public Sprite Sprite { get { return m_sprite; } }

        #endregion

        #region Fields

        private Color m_color;
        private Rectangle m_location;
        private Point m_center;
        private Point m_shootPoint;
        private Sprite m_sprite;

        #endregion

        #region Constructors

        public SpriteBox(Color color, Rectangle location)
        {
            m_sprite = new Sprite();
            m_sprite.duration = 1;

            Color = color;
            Location = location;
            Center = new Point(
                Location.X + (Location.Width+1)/2,
                Location.Y + (Location.Height+1)/2);
            ShootPoint = new Point(
                Location.X + (Location.Width + 1) / 2 - 5,
                Location.Y + (Location.Height + 1) / 2 - 5);
        }

        public SpriteBox(Color color, Sprite sprite)
        {
            m_sprite = sprite;

            Color = color;
            Location = new Rectangle(
                m_sprite.box.X,
                m_sprite.box.Y,
                m_sprite.box.Width - 1,
                m_sprite.box.Height - 1
                );
            m_center = new Point(
                Location.X + (Location.Width + 1) / 2 - m_sprite.offset.X,
                Location.Y + (Location.Height + 1) / 2 - m_sprite.offset.Y);
            m_shootPoint = new Point(
                Location.X + (Location.Width + 1) / 2 - m_sprite.shootPoint.X,
                Location.Y + (Location.Height + 1) / 2 - m_sprite.shootPoint.Y);
        }

        #endregion

    }

    public class BetterPictureBox : PictureBox
    {
        [Browsable(false)]
        public AnimationSet AnimationSet
        {
            set
            {
                m_animationSet = value;

                Random r = new Random();

                int i = 0;
                foreach (Animation a in value.anims)
                {
                    Color c = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
                    colorLookup[c] = i;
                    sbLookup[c] = new List<SpriteBox>();

                    foreach (Sprite s in a.sprites)
                    {
                        SpriteBox sb = new SpriteBox(c, s);
                        addSpriteBox(sb);

                        //sbLookup[c].Add(sb);
                    }

                    ++i;
                }
            }
        }
        private AnimationSet m_animationSet;
        private Dictionary<Color, int> colorLookup = new Dictionary<Color, int>();
        private Dictionary<Color, List<SpriteBox>> sbLookup = new Dictionary<Color, List<SpriteBox>>();

        [Browsable(false)]
        public new Image Image
        {
            get
            {
                return m_originalImage;
            }

            set
            {
                m_originalImage = value;
                base.Image = value;
            }
        }
        private Image m_originalImage;

        [Browsable(false)]
        public SpriteBox TemporarySpriteBox
        {
            get
            {
                return m_temporarySpriteBox;
            }
            set
            {
                m_temporarySpriteBox = value;
                Refresh();
            }
        }
        private SpriteBox m_temporarySpriteBox;

        [Browsable(false)]
        private List<SpriteBox> SpriteBoxes { get; set; }

        [Browsable(false)]
        public float ImageScale
        {
            get
            {
                return m_imageScale;
            }
            set
            {
                float oldValue = m_imageScale;
                m_imageScale = value;

                if (Image != null)
                {

                    Size s = new Size((int)(m_originalImage.Width * value),
                                      (int)(m_originalImage.Height * value));
                    try
                    {
                        Image result = new Bitmap(s.Width, s.Height);
                        using (Graphics graphics = Graphics.FromImage(result))
                        {
                            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            graphics.DrawImage(m_originalImage, 0, 0, result.Width, result.Height);
                        }

                        base.Image = result;
                        this.Size = s;
                    }
                    catch
                    {
                        m_imageScale = oldValue;
                    }
                }

                Refresh();
            }
        }
        private float m_imageScale = 1.0f;

        public void addSpriteBox(SpriteBox spriteBox)
        {
            SpriteBoxes.Add(spriteBox);

            // if AnimationSet exists, make sure to update it

            // known Animation, so update it
            if (colorLookup.ContainsKey(spriteBox.Color))
            {
                int animIdx = colorLookup[spriteBox.Color];
                List<SpriteBox> lst = sbLookup[spriteBox.Color];

                lst.Add(spriteBox);

                List<Sprite> sprites = new List<Sprite>();
                foreach (SpriteBox sb in lst)
                {
                    sprites.Add(sb.Sprite);
                }
                m_animationSet.anims[animIdx].sprites = sprites.ToArray();
            }

            // unknown Animation, create it
            else if (m_animationSet != null)
            {
                Animation a = new Animation();


                colorLookup.Add(spriteBox.Color, m_animationSet.anims.Length);
                
                List<SpriteBox> lst = new List<SpriteBox>();
                lst.Add(spriteBox);
                sbLookup.Add(spriteBox.Color, lst);

                a.sprites = new Sprite[1];
                a.sprites[0] = spriteBox.Sprite;

                List<Animation> anims = new List<Animation>(m_animationSet.anims);
                anims.Add(a);
                m_animationSet.anims = anims.ToArray();
            }
            Refresh();
        }

        public void popSpriteBox()
        {
            if (SpriteBoxes.Count > 0)
            {
                SpriteBox deleted = SpriteBoxes.Last();
                SpriteBoxes.RemoveAt(SpriteBoxes.Count - 1);

                if (colorLookup.ContainsKey(deleted.Color))
                {
                    List<SpriteBox> lst = sbLookup[deleted.Color];
                    lst.Remove(deleted);

                    if (m_animationSet != null)
                    {
                        List<Sprite> sprites = new List<Sprite>();
                        foreach (SpriteBox sb in lst)
                        {
                            sprites.Add(sb.Sprite);
                        }

                        Animation a = m_animationSet.anims[colorLookup[deleted.Color]];
                        a.sprites = sprites.ToArray();
                    }
                }
            }
            this.RaisePaintEvent(this, null);
            Refresh();
        }

        public SpriteBox findSpriteBox(Point centerValue)
        {
            SpriteBox sb = SpriteBoxes.Find(i => i.Center == centerValue);
            if (sb != null)
            {
                return sb;
            }
            else
            {
                return SpriteBoxes.Find(i => i.ShootPoint == centerValue);
            }
        }

        public AnimationSet makeAnimationSet()
        {
            AnimationSet animSet = new AnimationSet();
            List<Animation> animations = new List<Animation>();

            List<SpriteBox> sbs = new List<SpriteBox>(SpriteBoxes.ToArray());

            while (sbs.Count > 0)
            {
                Animation a = new Animation();
                List<Sprite> sprites = new List<Sprite>();
                animations.Add(a);

                Color c = sbs[0].Color;
                List<SpriteBox> matches = sbs.FindAll(i => i.Color == c);
                sbs.RemoveAll(i => i.Color == c);

                foreach (SpriteBox box in matches)
                {
                    sprites.Add(box.Sprite);
                }
                a.sprites = sprites.ToArray();
            }

            animSet.anims = animations.ToArray();

            return animSet;
        }

        public BetterPictureBox()
        {
            SpriteBoxes = new List<SpriteBox>();
        }

        public Point scalePoint(Point point)
        {
            return new Point(
                (int)((point.X + ImageScale / 2) / ImageScale),
                (int)((point.Y + ImageScale / 2) / ImageScale)
                );
        }

        public Rectangle scaleRectangle(Rectangle r)
        {
            return scaleRectangle(r, this.ImageScale);
        }

        public Rectangle scaleInvertRectangle(Rectangle r)
        {
            int mod = 1;
            if (ImageScale >= 1)
            {
                mod = (int)ImageScale;
            }

            return new Rectangle(
                (int)((r.X + ImageScale / 2) / ImageScale),
                (int)((r.Y + ImageScale / 2) / ImageScale),
                (int)((r.Width + (r.X + ImageScale / 2) % mod) / ImageScale),
                (int)((r.Height + (r.Y + ImageScale / 2) % mod) / ImageScale)
                );
        }

        public Rectangle scaleRectangle(Rectangle r, float scale)
        {
            return new Rectangle(
                (int)(r.X * scale),
                (int)(r.Y * scale),
                (int)(r.Width * scale),
                (int)(r.Height * scale)
                );
        }

        public Rectangle scaleCenter(Point center)
        {
            return new Rectangle(
                (int)(center.X * ImageScale - ImageScale / 2 + 1),
                (int)(center.Y * ImageScale - ImageScale / 2 + 1),
                (int)(ImageScale),
                (int)(ImageScale));
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (SpriteBoxes.Count > 0)
            {
                foreach (SpriteBox sb in SpriteBoxes)
                {
                    Pen p = new Pen(sb.Color, this.ImageScale);
                    pe.Graphics.DrawRectangle(p, scaleRectangle(sb.Location));

                    Brush b = new SolidBrush(sb.Color);
                    pe.Graphics.FillRectangle(b, scaleCenter(sb.Center));

                    Rectangle crosshairHoriz = scaleCenter(sb.ShootPoint);
                    Rectangle crosshairVert = crosshairHoriz;
                    crosshairHoriz.Width *= 3;
                    crosshairHoriz.X -= (int)this.ImageScale;
                    crosshairVert.Height *= 3;
                    crosshairVert.Y -= (int)this.ImageScale;
                    pe.Graphics.FillRectangle(b, crosshairHoriz);
                    pe.Graphics.FillRectangle(b, crosshairVert);
                }
            }

            if (TemporarySpriteBox != null)
            {
                Pen p = new Pen(TemporarySpriteBox.Color, this.ImageScale);
                pe.Graphics.DrawRectangle(p, scaleRectangle(TemporarySpriteBox.Location));
            }

            if (ImageScale >= 8.0f)
            {
                Pen p = new Pen(Color.Black, 1.0f);
                for (int i = (int)ImageScale/2; i < this.Size.Width; i += (int)ImageScale)
                {
                    pe.Graphics.DrawLine(p, new Point(i, 0), new Point(i, this.Size.Height - 1));
                }
                for (int j = (int)ImageScale / 2; j < this.Size.Height; j += (int)ImageScale)
                {
                    pe.Graphics.DrawLine(p, new Point(0, j), new Point(this.Size.Width - 1, j));
                }
            }
        }
    }
}
