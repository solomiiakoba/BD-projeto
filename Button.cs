using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Component
{
    public class Button : Panel
    {

        private int borderRadius = 30;
        private int borderSize = 0;
        private float gradientAngle = 90F;
        private Color borderColor = Color.Firebrick;
        private Color gradientTopColor = Color.DodgerBlue;
        private Color gradientBottomColor = Color.CadetBlue;
        private Color backgroundColor = Color.Transparent;
        private Color textColor = Color.White;
        private string buttonText = "Button";
        private Image buttonImage = null; private bool isSelected = false;


        public event EventHandler ButtonClick;

        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; this.Invalidate(); }
        }
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                this.Invalidate();
            }
        }

        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; this.Invalidate(); }
        }

        public int BorderSize
        {
            get => borderSize;
            set { borderSize = value; this.Invalidate(); }
        }

        public float GradientAngle
        {
            get => gradientAngle;
            set { gradientAngle = value; this.Invalidate(); }
        }

        public Color GradientTopColor
        {
            get => gradientTopColor;
            set { gradientTopColor = value; this.Invalidate(); }
        }

        public Color GradientBottomColor
        {
            get => gradientBottomColor;
            set { gradientBottomColor = value; this.Invalidate(); }
        }

        public Color BackgroundColor
        {
            get => backgroundColor;
            set { backgroundColor = value; this.Invalidate(); }
        }

        public Color TextColor
        {
            get => textColor;
            set { textColor = value; this.Invalidate(); }
        }

        public string ButtonText
        {
            get => buttonText;
            set { buttonText = value; this.Invalidate(); }
        }

        public Image ButtonImage
        {
            get => buttonImage;
            set { buttonImage = value; this.Invalidate(); }
        }

        public Button()
        {
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
            this.Size = new Size(150, 50);
            this.MouseClick += (s, e) => ButtonClick?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rectSurface = this.ClientRectangle;
            if (rectSurface.Width == 0 || rectSurface.Height == 0) return;

            using (GraphicsPath path = GetFigurePath(rectSurface, borderRadius))
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rectSurface,
                isSelected ? Color.MistyRose : gradientTopColor,
                isSelected ? Color.MistyRose : gradientBottomColor,
                gradientAngle))
            using (Pen penBorder = new Pen(borderColor, borderSize))
            {
                this.Region = new Region(path);
                graphics.FillPath(brush, path);

                if (borderSize > 0)
                    graphics.DrawPath(penBorder, path);
            }

            // Desenhar imagem, se existir
            // Desenhar imagem e texto (imagem à esquerda, texto embaixo)
            if (buttonImage != null)
            {
                Size imageSize = new Size(32, 32); // Tamanho desejado da imagem

                // Centralizar a imagem horizontalmente, deixar no topo
                Point imageLocation = new Point(
                    (this.Width - imageSize.Width) / 2,
                    8 // margem superior
                );

                graphics.DrawImage(buttonImage, new Rectangle(imageLocation, imageSize));

                // Texto abaixo da imagem
                Rectangle textRect = new Rectangle(
                    0,
                    imageLocation.Y + imageSize.Height + 4, // 4px abaixo da imagem
                    this.Width,
                    this.Height - (imageLocation.Y + imageSize.Height + 4)
                );

                using (SolidBrush textBrush = new SolidBrush(textColor))
                using (StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                {
                    graphics.DrawString(buttonText, this.Font, textBrush, textRect, stringFormat);
                }
            }
            else
            {
                // Caso não haja imagem, só desenha o texto centralizado
                using (SolidBrush textBrush = new SolidBrush(textColor))
                using (StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    graphics.DrawString(buttonText, this.Font, textBrush, this.ClientRectangle, stringFormat);
                }
            }


            // Desenhar texto (abaixo da imagem, se houver)
            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                Rectangle textRect;

                if (buttonImage != null)
                {
                    // Deixa espaço abaixo da imagem
                    textRect = new Rectangle(0, this.Height / 2 + 10, this.Width, this.Height / 2);
                }
                else
                {
                    textRect = rectSurface;
                }

                graphics.DrawString(buttonText, this.Font, textBrush, textRect, stringFormat);
            }
        }

        private GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            float r = radius;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
