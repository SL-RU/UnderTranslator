using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace UnderTranslator
{
    /// <summary>
    /// Логика взаимодействия для GMXFontViewr.xaml
    /// </summary>
    public partial class GMXFontViewr : UserControl
    {
        public GMXFontViewr()
        {
            curcol = Color.FromRgb(255, 255, 255);
            InitializeComponent();
        }


        public string Text = "";
        int x, y;
        int size = 24;
        public bool inverted = false;
        Color curcol;

        public void showStr(string s)
        {
            if (!Project.Loaded)
                return;
            if (!GMXFontDB.Loaded)
                return;
            if (!inverted)
            {
                setColor("⁰");
                cnvs.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else
            {
                setColor("⁹ⁿ");
                cnvs.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            x = 0;
            y = 0;
            float scale = GMXFontDB.xCoeff;//(float)size / (float)GMXFontDB.Size;
            cnvs.Children.Clear();
            while (s.EndsWith("\\") || s.EndsWith("%") || s.EndsWith("/") || s.EndsWith("^"))
                s = s.Remove(s.Length - 1);
            for(int i = 0; i<s.Length; i++)
            {
                Text = s;
                if (GMXFontDB.glyphs.Keys.Contains(s[i]))
                {
                    GMXFontDB.Glyph g = GMXFontDB.glyphs[s[i]];
                    Rectangle rec = new Rectangle();
                    rec.Width = g.w * scale;
                    rec.Height = g.h * scale;
                    ImageBrush img = new ImageBrush(g.bmap);
                    RenderOptions.SetBitmapScalingMode(rec, BitmapScalingMode.NearestNeighbor);
                    RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
                    rec.Fill = new SolidColorBrush(curcol);
                    rec.OpacityMask = img;
                    rec.SnapsToDevicePixels = true;
                    //rec.SnapsToDevicePixels = true;
                    cnvs.Children.Add(rec);
                    Canvas.SetTop(rec, y + g.offset * scale); Canvas.SetLeft(rec, x);

                    x += (int)(g.shift * scale);
                    //img.Source = g.bmap;
                } 
                else
                    setColor(s[i].ToString());
            }
        }

        public void setColor(string col)
        {
            switch(col)
            {
                case "⁰": col = inverted ? "Black" : "White"; break;
                case "⁴": col = "Yellow"; break;
                case "⁵": col = "Green"; break;
                case "⁶": col = "Blue"; break;
                case "⁷": col = "Orange"; break;
                case "⁸": col = "Red"; break;
                case "ⁿ": col = "Purple"; break;
                case "⁹": col = "LightBlue"; break;
                case "⁹ⁿ": col = "Black"; break;
                default: col = ""; break;
            }
            if (col != "") curcol = (Color)ColorConverter.ConvertFromString(col);
        }

    }

    public class GMXFontDB
    {
        public struct Glyph
        {
            public int w, h, x, y, offset, shift;
            public CroppedBitmap bmap;
        }
        public static bool Loaded = false;
        public static string Name;
        public static int Size, xCoeff = 1, Shift = 1;
        public static Dictionary<char, Glyph> glyphs;
        public static bool LoadFont(string path)
        {
            if (!Project.Loaded)
                return false;
            string ipath;
            if (path.EndsWith(".gmx") && File.Exists(path))
            {
                using (var reader = new XmlTextReader(path))
                {
                    XDocument X = XDocument.Load(reader);
                    ipath = X.Root.Element("image").Value;
                    ipath = path.Remove(path.LastIndexOf('\\') + 1) + ipath;

                    Name = X.Root.Element("name").Value;
                    Size = int.Parse(X.Root.Element("size").Value);
                    Shift = 8;
                    if (Name.StartsWith("DotumChe"))
                    {
                        xCoeff = 1;
                    }
                    else
                    {
                        xCoeff = 2;
                    }
                    if (Name.StartsWith("Papyrus"))
                        Shift = 12;

                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(ipath, UriKind.Relative);
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();

                    glyphs = new Dictionary<char, Glyph>();
                    foreach (var v in X.Root.Element("glyphs").Elements())
                    {
                        int w = int.Parse(v.Attribute("w").Value),
                            h = int.Parse(v.Attribute("h").Value),
                            x = int.Parse(v.Attribute("x").Value),
                            y = int.Parse(v.Attribute("y").Value);
                        Glyph g = new Glyph();
                        g.x = x;
                        g.y = y;
                        g.w = w;
                        g.h = h;
                        g.offset = int.Parse(v.Attribute("offset").Value);
                        g.shift = Shift;//int.Parse(v.Attribute("shift").Value);
                        g.bmap = new CroppedBitmap(src, new Int32Rect(x, y, w, h));

                        glyphs.Add((char)int.Parse(v.Attribute("character").Value), g);
                    }
                }
            }
            else
            {
                return false;
            }
            Loaded = true;
            return true;
        }
    }
}
