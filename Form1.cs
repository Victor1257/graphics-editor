using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace kg1lab
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            change_size();
        }

        private enum instrument
        {
            pen,
            line,
            rect,
            ellipse,
            erase,
            text,
            fill,
            none
        }

        private float thickness = 5;
        private Point startpoint;
        private Bitmap bitmap;
        private instrument selected_instrument = instrument.pen;
        private Rectangle windowsize;
        private Color selectedColor;
        private List<Point> linePoints = new List<Point>();
        private int pictureboxheight;
        private int pictureboxwidth;
        private double sensitivity = 50;

        private bool isinbounds(int a, int b)
        {
            bool ok = false;
            if (a > 0 && b > 0 && a < pictureboxwidth-1 && b < pictureboxheight-1)
                ok = true;
            return ok;
        }

        private bool colorcmp (Color A, Color B)
        {
            if ( Math.Abs(A.R - B.R) < sensitivity && Math.Abs(A.G - B.G) < sensitivity && Math.Abs(A.B - B.B) < sensitivity)
            {
                if (Math.Abs(A.A - B.A) < sensitivity)
                { return true; }
                else return false;
            }
            else return false;
        }

        private void change_size()
        {
            if (textBox1.Text != "")
            {
                pictureboxwidth = Convert.ToInt32(textBox1.Text);
                if (pictureboxwidth >= 200 && pictureboxwidth <= 1200)
                    pictureBox1.Width = pictureboxwidth;
            }

            if (textBox2.Text != "")
            {
                pictureboxheight = Convert.ToInt32(textBox2.Text);
                if (pictureboxheight >= 200 && pictureboxheight <= 680)
                    pictureBox1.Height = pictureboxheight;
            }
            windowsize = new Rectangle(0, 0, pictureboxwidth, pictureboxheight);
            bitmap = new Bitmap(pictureboxwidth, pictureboxheight);
        }

        private void Fillmethod(Bitmap image, int x, int y)
        {
            bool[,] check = new bool[windowsize.Width, windowsize.Height];
            check.Initialize();

            Color currentColor = image.GetPixel(x, y);
            if ( currentColor != selectedColor )
            {
                Point currentpoint = new Point(x, y);
                List<Point> fillPoints = new List<Point>();

                if (!colorcmp(image.GetPixel(x, y), selectedColor))
                {
                    //image.SetPixel(x, y, selectedColor);
                    fillPoints.Add( new Point(x, y) );
                }

                fillPoints.Add(new Point(x, y));
                while (fillPoints.Count != 0)
                {
                    currentpoint = fillPoints.ElementAt(fillPoints.Count-1);
                    int cx = currentpoint.X;
                    int cy = currentpoint.Y;
                
                        fillPoints.RemoveAt(fillPoints.Count - 1);
                    if (!check[cx + 1, cy])
                        if (colorcmp(image.GetPixel(cx + 1, cy), currentColor) && isinbounds(cx + 1, cy) )
                            {
                                fillPoints.Add(new Point (cx + 1, cy) );
                                check[cx + 1, cy] = true;
                            if (image.GetPixel(cx + 1, cy) == currentColor)
                                image.SetPixel(cx + 1, cy, selectedColor);
                        }
                    if (!check[cx - 1, cy])
                        if (colorcmp(image.GetPixel(cx - 1, cy), currentColor) && isinbounds(cx - 1, cy) )
                            {
                                fillPoints.Add(new Point (cx - 1, cy) );
                                check[cx - 1, cy] = true;
                            if (image.GetPixel(cx - 1, cy) == currentColor)
                                image.SetPixel(cx - 1, cy, selectedColor);
                        }
                    if (!check[cx, cy - 1])
                        if (colorcmp(image.GetPixel(cx, cy - 1), currentColor) && isinbounds(cx, cy - 1) )
                            {
                                fillPoints.Add(new Point (cx, cy - 1) );
                                check[cx, cy - 1] = true;
                            if (image.GetPixel(cx, cy - 1) == currentColor)
                                image.SetPixel(cx, cy - 1, selectedColor);
                        }
                    if (!check[cx, cy + 1])
                        if (colorcmp(image.GetPixel(cx, cy + 1), currentColor) && isinbounds(cx, cy + 1) )
                            {
                                fillPoints.Add(new Point (cx, cy + 1) );
                                check[cx, cy + 1] = true;
                            if (image.GetPixel(cx, cy + 1) == currentColor)
                                image.SetPixel(cx, cy + 1, selectedColor);
                        }
                }
                fillPoints.Clear();
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {   selected_instrument = instrument.pen;       }

        private void button2_Click(object sender, EventArgs e)
        {   selected_instrument = instrument.line;      }

        private void button3_Click(object sender, EventArgs e)
        {   selected_instrument = instrument.fill;      }

        private void button5_Click(object sender, EventArgs e)
        {   selected_instrument = instrument.rect;      }

        private void button7_Click(object sender, EventArgs e)
        {   selected_instrument = instrument.ellipse;   }

        private void button10_Click(object sender, EventArgs e)
        {   selected_instrument = instrument.erase;     }

        private void button11_Click(object sender, EventArgs e)
        {   selected_instrument = instrument.text;      }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureboxwidth = Convert.ToInt32(textBox1.Text);
            pictureboxheight = Convert.ToInt32(textBox2.Text);
            bitmap = new Bitmap(pictureboxwidth, pictureboxheight);
            windowsize = new Rectangle(0, 0, pictureboxwidth, pictureboxheight);
            selectedColor = Color.Black;
        }

        private void button4_Click(object sender, EventArgs e)
        {
                ColorDialog MyDialog = new ColorDialog();
                // Keeps the user from selecting a custom color.
                MyDialog.AllowFullOpen = false;
                // Allows the user to get help. (The default is false.)
                MyDialog.ShowHelp = true;
                // Sets the initial color select to the current text color.
                // Update the text box color if the user clicks OK 
                if (MyDialog.ShowDialog() == DialogResult.OK)
                selectedColor = MyDialog.Color;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (selected_instrument == instrument.pen || selected_instrument == instrument.line)
            {
                if (linePoints.Count > 1)
                using (var pen = new Pen(selectedColor, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
                        e.Graphics.DrawLines(pen, linePoints.ToArray());
            }
            else if (selected_instrument == instrument.erase)
            {
                if (linePoints.Count > 1)
                    using (var pen = new Pen( Color.White , thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
                        e.Graphics.DrawLines(pen, linePoints.ToArray());
            }
            else if (selected_instrument == instrument.rect || selected_instrument == instrument.ellipse)
            {
                if (linePoints.Count > 1)
                {
                    Point[] pointlist = linePoints.ToArray();
                    Point A = pointlist[0];
                    Point B = pointlist[1];
                    int xmin;
                    int xmax;
                    int ymin;
                    int ymax;

                    if (A.X > B.X)
                    { xmin = B.X; xmax = A.X; }
                    else
                    { xmin = A.X; xmax = B.X; }

                    if (A.Y > B.Y)
                    { ymin = B.Y; ymax = A.Y; }
                    else
                    { ymin = A.Y; ymax = B.Y; }
                    if (selected_instrument == instrument.rect)
                    {
                        Rectangle rect_todraw = new Rectangle();

                        rect_todraw = new Rectangle(xmin, ymin, Math.Abs(A.X - B.X), Math.Abs(A.Y - B.Y));

                        using (var pen = new Pen(selectedColor, thickness) )
                                e.Graphics.DrawRectangle (pen, rect_todraw);
                    }
                    else
                        if (selected_instrument == instrument.ellipse)
                        {
                            Rectangle rect_todraw = new Rectangle();

                            rect_todraw = new Rectangle(xmin, ymin, Math.Abs(A.X - B.X), Math.Abs(A.Y - B.Y));

                            using (var pen = new Pen(selectedColor, thickness))
                                e.Graphics.DrawEllipse (pen, rect_todraw);
                        }

                }
            }

            if (selected_instrument == instrument.line || selected_instrument == instrument.rect || selected_instrument == instrument.ellipse)
            {
                linePoints.Clear();
                linePoints.Add(startpoint);
            }

            if (selected_instrument == instrument.text)
            {
                e.Graphics.DrawString(textBox3.Text, new Font("Arial", 14), Brushes.Black, startpoint.X, startpoint.Y);
                linePoints.Clear();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            linePoints.Clear();

            if (selected_instrument == instrument.line || selected_instrument == instrument.rect
                || selected_instrument == instrument.ellipse || selected_instrument == instrument.pen
                || selected_instrument == instrument.erase)
            {
                if (e.Button == MouseButtons.Left)
                {
                    linePoints.Add(e.Location);
                    startpoint = e.Location;
                }
            }

            if (selected_instrument == instrument.text)
            {
                startpoint = e.Location;
                pictureBox1.Invalidate();
            }

            if (selected_instrument == instrument.fill)
            {
                pictureBox1.DrawToBitmap(bitmap, windowsize);
                Fillmethod(bitmap, e.X, e.Y);
                //pictureBox1.Image = bmp;
                pictureBox1.Image = bitmap;
                //bitmap = (Bitmap)pictureBox1.Image.Clone();
                //Invalidate();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selected_instrument == instrument.pen || selected_instrument == instrument.erase
                || selected_instrument == instrument.line
                || selected_instrument == instrument.rect || selected_instrument == instrument.ellipse)
            {
                if (e.Button == MouseButtons.Left)
                {
                    linePoints.Add(e.Location);
                    pictureBox1.Invalidate();
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (selected_instrument == instrument.pen || selected_instrument == instrument.erase)
            {
                pictureBox1.Invalidate();
            }
            else if (selected_instrument == instrument.line)
            {
                if (e.Button == MouseButtons.Left)
                {
                    pictureBox1.DrawToBitmap(bitmap, windowsize);
                    linePoints.Add(e.Location);
                }
            }
            else if (selected_instrument == instrument.rect || selected_instrument == instrument.ellipse)
            {
                linePoints.Add(e.Location);
            }
            pictureBox1.DrawToBitmap(bitmap, windowsize);
            pictureBox1.Image = bitmap;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            change_size();
            //windowsize = new Rectangle(0, 0, pictureboxwidth, pictureboxheight);
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            thickness = hScrollBar1.Value;
            //sensitivity = hScrollBar1.Value
            label2.Text = Convert.ToString(thickness);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar)) return;
            else
                e.Handled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar)) return;
            else
                e.Handled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) //если в pictureBox есть изображение
            {
                //создание диалогового окна "Сохранить как..", для сохранения изображения
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                //отображается ли кнопка "Справка" в диалоговом окне
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {
                        pictureBox1.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap image; //Bitmap для открываемого изображения

            OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
            open_dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    image = new Bitmap(open_dialog.FileName);
                    //вместо pictureBox1 укажите pictureBox, в который нужно загрузить изображение 
                    this.pictureBox1.Size = image.Size;
                    pictureBox1.Image = image;
                    pictureBox1.Invalidate();
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }
    }
}
