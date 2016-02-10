using System;
using System.Collections.Generic;
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

namespace UnderTranslator
{
    /// <summary>
    /// Логика взаимодействия для UTextViewer.xaml
    /// </summary>
    public partial class UTextViewer : UserControl
    {
        public UTextViewer()
        {
            InitializeComponent();
            fplain1.inverted = true;
            fplain2.inverted = true;
            fplain3.inverted = true;
            fplain4.inverted = true;
        }

        public string Text = "ADFAGFHDFHADFHDF";


        public void showText(string txt)
        {
            Text = txt;
            showWFNormText(txt);
        }

        int Mode = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"> 0 - standart without face; 1 - with face; 2 - plain mode</param>
        public void setMode(int mode)
        {
            withoutFaceNorm.Visibility = System.Windows.Visibility.Collapsed;
            FaceNorm.Visibility = System.Windows.Visibility.Collapsed;
            PlaintText.Visibility = System.Windows.Visibility.Collapsed;
            if(mode == 0)
            {
                withoutFaceNorm.Visibility = System.Windows.Visibility.Visible;
            }
            if (mode == 1)
            {
                FaceNorm.Visibility = System.Windows.Visibility.Visible;
            }
            if (mode == 2)
            {
                PlaintText.Visibility = System.Windows.Visibility.Visible;
            }
            Mode = mode;
        }

        void showWFNormText(string text)
        {
            text = text.Replace(@"\W", "⁰").Replace(@"\X", "⁰").Replace(@"\L", "⁹").Replace(@"\Y", "⁴").Replace(@"\G", "⁵").
                Replace(@"\B", "⁶").Replace(@"\O", "⁷").Replace(@"\R", "⁸").Replace(@"\P", "ⁿ").Replace("^1", "").Replace("^2", "").
                Replace("^3", "").Replace("^4", "").Replace("^5", "").Replace("^6", "").Replace("^7", "").Replace("^8", "").
                Replace("^9", "").Replace(@"\E0", "").Replace(@"\E1", "").Replace(@"\E2", "").Replace(@"\E3", "").Replace(@"\E4", "").
                Replace(@"\E5", "").Replace(@"\E6", "").Replace(@"\E7", "").Replace(@"\E8", "").Replace(@"\E9", "").Replace(@"\C", "").
                Replace(@"\X", "").Replace(@"\%%", "").Replace(@"\E0", "").Replace(@"\E1", "").Replace(@"\E2", "").Replace(@"\E3", "").
                Replace(@"\E4", "").Replace(@"\E5", "").Replace(@"\E6", "").Replace(@"\E7", "").Replace(@"\E8", "").Replace(@"\E9", "").
                Replace(@"\F0", "").Replace(@"\F1", "").Replace(@"\F2", "").Replace(@"\F3", "").Replace(@"\F4", "").Replace(@"\F5", "").
                Replace(@"\F6", "").Replace(@"\F7", "").Replace(@"\F8", "").Replace(@"\F9", "").Replace(@"\TS", "").Replace(@"\Ts", "").
                Replace(@"\TP", "").Replace(@"\TU", "").Replace(@"\TA", "").Replace(@"\TT", "").Replace(@"\Ta", "").Replace(@"\C", "").
                Replace(@"\%%", "").Replace(@"\M1", "").Replace(@"^0", "");  //I'm sorry for this s#&*, but I copypasted this from TransaTale

            wfnorm1.showStr("");
            fplain1.showStr("");
            fnorm1.showStr("");
            wfnorm2.showStr("");
            fplain2.showStr("");
            fnorm2.showStr("");
            wfnorm3.showStr("");
            fplain3.showStr("");
            fnorm3.showStr("");
            fplain4.showStr("");

            List<string> v = new List<string>(text.Split('&'));
            int maxC = 0;
            if(Mode == 0)
            {
                maxC = (int)(wfnorm1.Width / (GMXFontDB.Shift * GMXFontDB.xCoeff));
            }
            else if (Mode == 1)
                {
                    maxC = (int)(fnorm1.Width / (GMXFontDB.Shift * GMXFontDB.xCoeff));
                }
            else if (Mode == 2)
            {
                maxC = (int)(fplain2.Width / (GMXFontDB.Shift * GMXFontDB.xCoeff));
            }
            if (v.Capacity > 0)
            {

                if (v[0].Length > maxC)
                {
                    v.Insert(1, v[0].Substring(maxC));
                    v[0] = v[0].Remove(maxC);
                }
                if (Mode == 0) wfnorm1.showStr(v[0]);
                if (Mode == 2) fplain1.showStr(v[0]);
                if (Mode == 1) fnorm1.showStr(v[0]);
            }
            if (v.Capacity > 1)
            {

                if (v[1].Length > maxC)
                {
                    v.Insert(2, v[1].Substring(maxC));
                    v[1] = v[1].Remove(maxC);
                }
                if (Mode == 0) wfnorm2.showStr(v[1]);
                if (Mode == 2) fplain2.showStr(v[1]);
                if (Mode == 1) fnorm2.showStr(v[1]);
            }
            if (v.Capacity > 2)
            {

                if (v[2].Length > maxC)
                {
                    v.Insert(3, v[2].Substring(maxC));
                    v[2] = v[2].Remove(maxC);
                }
                if (Mode == 0) wfnorm3.showStr(v[2]);
                if (Mode == 2) fplain3.showStr(v[2]);
                if (Mode == 1) fnorm3.showStr(v[2]);
            }
            if (v.Capacity > 3)
            {
                if (Mode == 2) fplain4.showStr(v[3]);
            }
        }

        private void stndrt_Checked(object sender, RoutedEventArgs e)
        {
            if (!Project.Loaded)
                return;
            GMXFontDB.LoadFont(Project.getFontPath(Project.MainFont));
            setMode(faceMode.IsChecked == true ? 1 : 0);
            showText(Text);
        }

        private void pap_Checked(object sender, RoutedEventArgs e)
        {
            if (!Project.Loaded)
                return;
            GMXFontDB.LoadFont(Project.getFontPath(Project.PapFont));
            setMode(faceMode.IsChecked == true ? 1 : 0);
            showText(Text);
        }

        private void sans_Checked(object sender, RoutedEventArgs e)
        {
            if (!Project.Loaded)
                return;
            GMXFontDB.LoadFont(Project.getFontPath(Project.SansFont));
            setMode(faceMode.IsChecked == true ? 1 : 0);
            showText(Text);
        }

        private void plainmode_Checked(object sender, RoutedEventArgs e)
        {
            if (!Project.Loaded)
                return;
            setMode(2);
            GMXFontDB.LoadFont(Project.getFontPath(Project.PlainText));
            showText(Text);
        }

        private void faceMode_Checked(object sender, RoutedEventArgs e)
        {
            if (Mode == 0 || Mode == 1)
            {
                setMode(1);
                showText(Text);
            }
        }

        private void faceMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Mode == 0 || Mode == 1)
            {
                setMode(0);
                showText(Text);
            }
        }



    }
}
