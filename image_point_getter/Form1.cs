using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace image_point_getter
{
    public partial class Form1 : Form
    {
        private int x1;
        private int y1;
        private int x2;
        private int y2;

        private bool shiftpressed = false;
        private bool controlpressed = false;

        private SolidBrush b1;
        private SolidBrush b2;
        public Form1()
        {
            InitializeComponent();
            b1 = new SolidBrush(Color.Blue);
            b2 = new SolidBrush(Color.Red);
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                shiftpressed = true;
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                controlpressed = true;
            }
            else if (e.KeyCode == Keys.W)
            {
                if (shiftpressed)
                {
                    --y2;
                    setP2Coords(x2, y2);
                }
                else
                {
                    --y1;
                    setP1Coords(x1, y1);
                }
                pictureBox1.Refresh();
            }
            else if (e.KeyCode == Keys.A)
            {
                if (shiftpressed)
                {
                    --x2;
                    setP2Coords(x2, y2);
                }
                else
                {
                    --x1;
                    setP1Coords(x1, y1);
                }
                pictureBox1.Refresh();
            }
            else if (e.KeyCode == Keys.S)
            {
                if (shiftpressed)
                {
                    ++y2;
                    setP2Coords(x2, y2);
                }
                else
                {
                    ++y1;
                    setP1Coords(x1, y1);
                }
                pictureBox1.Refresh();
            }
            else if (e.KeyCode == Keys.D)
            {
                if (shiftpressed)
                {
                    ++x2;
                    setP2Coords(x2, y2);
                }
                else
                {
                    ++x1;
                    setP1Coords(x1, y1);
                }
                pictureBox1.Refresh();
            }
            calcDifference();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                shiftpressed = false;
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                controlpressed = false;
            }
        }

        private void calcDifference()
        {
            int xDiff = x2 - x1;
            int yDiff = y2 - y1;

            l_diff.Text = "Diff: (" + xDiff + ", " + yDiff + ") ";
        }
        private void setClipBoard()
        {
            string s = '\t'.ToString();
            Clipboard.SetText(x1 + s + y1 + s + x2 + s + y2);
        }
        private void setP1Coords(int x, int y)
        {
            x1 = x;
            y1 = y;
            l_point1.Text = "Point 1: (" + x1 + ", " + y1 + ")";
            setClipBoard();
        }
        private void setP2Coords(int x, int y)
        {
            x2 = x;
            y2 = y;
            l_point2.Text = "Point 2: (" + x2 + ", " + y2 + ")";
            setClipBoard();
        }
        private int findBorder(int startX, int startY, int dir, bool isVertical, int nLimit)
        {
            int x;
            int b;
            int limit;

            Bitmap bmp = new Bitmap(pictureBox1.Image);

            if (isVertical)
            {
                x = startY;
                b = startX;
                limit = bmp.Height;
            }
            else
            {
                x = startX;
                b = startY;
                limit = bmp.Width;
            }

            double val = 1.0;
            int n = 0;
            for (; x >= 0 && x < limit && n < nLimit; x += dir)
            {
                if (isVertical)
                {
                    val = bmp.GetPixel(b, x).GetBrightness();
                }
                else
                {
                    val = bmp.GetPixel(x, b).GetBrightness();
                }

                if (val < 0.6)
                {
                    return x-dir;
                }
            }
            return -1;
        }
        int findBorder(int startX, int startY, int dir, bool isVertical)
        {
            return findBorder(startX, startY, dir, isVertical, int.MaxValue);
        }
        private void setBounds(int mouseX, int mouseY)
        {
            int topLeftX = findBorder(mouseX, mouseY, -1, false);
            int topLeftY = findBorder(mouseX, mouseY, -1, true);

            int bottomRightX = findBorder(mouseX, mouseY, +1, false);
            int bottomRightY = findBorder(mouseX, mouseY, +1, true);

            setP1Coords(topLeftX, topLeftY);
            setP2Coords(bottomRightX, bottomRightY);
            this.Refresh();
        }
        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (shiftpressed)
            {
                setP2Coords(e.X, e.Y);
            }
            else if (controlpressed)
            {
                setBounds(e.X, e.Y);
            }
            else
            {
                setP1Coords(e.X, e.Y);
            }
            calcDifference();
            pictureBox1.Refresh();
        }
        private string getFilePath()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string output = ofd.FileName;
            return output;
        }
        private void setPicture(string filepath)
        {
            Image img = pictureBox1.Image;
            pictureBox1.Image = null;
            if (img != null)
            {
                img.Dispose();
            }
            pictureBox1.Image = Image.FromFile(filepath);
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            string filepath = getFilePath();
            setPicture(filepath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void L_point2_Click(object sender, EventArgs e)
        {

        }

        private void drawCross(Graphics g, int x, int y, Brush b)
        {
            int top = y - 5;
            int bottom = y + 5;
            int left = x - 5;
            int right = x + 5;

            top = top < 0 ? 0 : top;
            left = left < 0 ? 0 : left;

            int h = pictureBox1.Height;
            int w = pictureBox1.Width;

            bottom = bottom < h ? bottom : h - 1;
            right = right < w ? right : w - 1;

            g.DrawLine(new Pen(b), x, top, x, bottom);
            g.DrawLine(new Pen(b), left, y, right, y);
        }
        private void drawRectangle(Graphics g, int x1, int y1, int width, int height)
        {
            Pen p = new Pen(new SolidBrush(Color.LightBlue));
            g.DrawRectangle(p, new Rectangle(x1, y1, width, height));
        }
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            drawRectangle(e.Graphics, x1, y1, x2 - x1, y2 - y1);
            drawCross(e.Graphics, x1, y1, b1);
            drawCross(e.Graphics, x2, y2, b2);
            //e.Graphics.FillRectangle(b1, new Rectangle(x1, y1, 1, 1));
            //e.Graphics.FillRectangle(b2, new Rectangle(x2, y2, 1, 1));
        }
    }
}
