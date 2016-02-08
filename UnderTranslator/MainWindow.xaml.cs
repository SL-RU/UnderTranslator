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
using MahApps.Metro.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;

namespace UnderTranslator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadProject(@"C:\tmp\und");

        }

        public void LoadProject(string s)
        {
            if (Project.Load(@"C:\tmp\und"))
            {
                bindLst = new ObservableCollection<STRDataGridRow>();
                datagrid.ItemsSource = bindLst;
                scr.Maximum = Project.origSTR.Length;
                gotoID.Maximum = Project.origSTR.Length;
                curID = 0;
                SetSelectedId(0);
            }

        }
        
        ObservableCollection<STRDataGridRow> bindLst;
        int selID = 0;
        int maxRows = 0;
        int curID = 0;

        private void datagrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            maxRows = (int)((datagrid.ActualHeight - datagrid.ColumnHeaderHeight + 5) / (datagrid.RowHeight));
            if (!Project.Loaded)
                return;
            bindLst.Clear();
            datagrid.CancelEdit();
            for (int i = 0; i < maxRows; i++)
            {
                bindLst.Add(new STRDataGridRow() { ID = -1 });
            }
            ShowData();

        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            curID = (int)scr.Value;
            ShowData();
            datagrid.CancelEdit();
            if (datagrid.SelectedIndex == -1)
                SetSelectedId(curID);
            else
                SetSelectedId(curID + datagrid.SelectedIndex);
        }

        void ShowData()
        {
            if (!Project.Loaded)
                return;
            //bindLst.Clear();
            int i = Math.Max(curID, 0), c = 0;
            while (c < maxRows && i < Project.origSTR.Length)
            {
                if (!CheckIgnore(Project.origSTR[i]))
                {
                    bindLst[c].ID = i;
                    c++;
                }
                else
                {
                }
                i++;
                if (i >= Project.origSTR.Length)
                    return;
            }
        }
        void SetSelectedId(int id)
        {
            if (!Project.Loaded)
                return;
            if(!(id >= 0 && id < Project.origSTR.Length))
                return;
            selID = id;
            gotoID.Value = id;
            txtViewer.showText(Project.tranSTR[selID]);
            editField.Text = Project.tranSTR[selID];

            editField.Focus();

        }

        void CommitEdit()
        {
            if (!Project.Loaded)
                return;
            if (!(selID >= 0 && selID < Project.origSTR.Length))
                return;

            Project.tranSTR[selID] = editField.Text;

            if (datagrid.SelectedValue is STRDataGridRow)
            {
                if (!Project.Loaded || datagrid.SelectedValue == null || (datagrid.SelectedValue as STRDataGridRow).ID == int.MinValue)
                    return;
            }
            else
                return;

            (datagrid.SelectedValue as STRDataGridRow).Trans = editField.Text;

        }
        void CancelEdit()
        {

        }

        bool CheckIgnore(string s)
        {
            return false;
            //return s == "" || s.StartsWith("gml_") || s.StartsWith("room_") || s.StartsWith("obj_") || s.StartsWith("spr_") ||
            //    s.StartsWith("mus_") || s.StartsWith("bg_") || s.StartsWith("snd_") || s == " ";
        }

        private void datagrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scr.Value -= maxRows * e.Delta / 800;
            e.Handled = true;
        }

        private void datagrid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Up)
            {
                e.Handled = true;
            }
            if(e.Key == Key.Down)
            {
                e.Handled = true;
            }
        }

        private void datagrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                CommitEdit();
                curID = Math.Max(curID - 1, 0);
                ShowData();
                e.Handled = true;

                if (datagrid.SelectedIndex == -1)
                    SetSelectedId(curID);
                else
                    SetSelectedId(curID + datagrid.SelectedIndex);
            }
            if (e.Key == Key.Enter)
            {
                CommitEdit();
                curID = Math.Min(curID + 1,Project.origSTR.Length);
                ShowData();
                e.Handled = true;

                if (datagrid.SelectedIndex == -1)
                    SetSelectedId(curID);
                else
                    SetSelectedId(curID + datagrid.SelectedIndex);
            }
            if (e.Key == Key.Down)
            {
                CancelEdit();
                curID = Math.Min(curID + 1, Project.origSTR.Length);
                ShowData();
                e.Handled = true;

                if (datagrid.SelectedIndex == -1)
                    SetSelectedId(curID);
                else
                    SetSelectedId(curID + datagrid.SelectedIndex);
            }
            if (e.Key == Key.Escape)
            {
                CancelEdit();

                if (datagrid.SelectedIndex == -1)
                    SetSelectedId(curID);
                else
                    SetSelectedId(curID + datagrid.SelectedIndex);
            }
        }

        private void NumericUpDown_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (gotoID.Value >= 0 && gotoID.Value < Project.origSTR.Length)
                {
                    if(datagrid.SelectedIndex == -1)
                        curID = Math.Max(0, (int)gotoID.Value);
                    else
                        curID = Math.Max(0, (int)gotoID.Value - datagrid.SelectedIndex);
                    SetSelectedId((int)gotoID.Value);
                    ShowData();
                }
                else
                {
                    gotoID.Value = curID;
                    datagrid.SelectedIndex = 0;
                }
                e.Handled = true;
            }
        }

        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datagrid.SelectedValue is STRDataGridRow)
            {
                if (!Project.Loaded || datagrid.SelectedValue == null || (datagrid.SelectedValue as STRDataGridRow).ID == int.MinValue)
                    return;
            }
            else
                return;
            CancelEdit();
            //CommitEdit();
            gotoID.Value = (datagrid.SelectedValue as STRDataGridRow).ID;
            SetSelectedId((int)gotoID.Value);
        }

        private void gotoID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (datagrid.SelectedValue is STRDataGridRow)
            {
                if (!Project.Loaded || datagrid.SelectedValue == null || (datagrid.SelectedValue as STRDataGridRow).ID == int.MinValue)
                    return;
            }
            else
                return;
            if (gotoID.Value >= 0 && gotoID.Value < Project.origSTR.Length)
            {
                if (datagrid.SelectedIndex == -1)
                    curID = Math.Max(0, (int)gotoID.Value);
                else
                    curID = Math.Max(0, (int)gotoID.Value - datagrid.SelectedIndex);
                SetSelectedId((int)gotoID.Value);
                ShowData();
            }
        }

        private void MetroWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Middle)
            {
                e.Handled = true;
            }
        }

        private void MetroWindow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (datagrid.SelectedValue is STRDataGridRow)
                {
                    if (!Project.Loaded || datagrid.SelectedValue == null || (datagrid.SelectedValue as STRDataGridRow).ID == int.MinValue)
                        return;
                }
                else
                    return;
                e.Handled = true;
                editField.Text = Clipboard.GetText().Replace('\n', ' ').Replace('\r', 'n');
                if (enterAfterPaste.IsChecked == true)
                {
                    CommitEdit();
                    curID = Math.Min(curID + 1, Project.origSTR.Length);
                    ShowData();
                    e.Handled = true;

                    if (datagrid.SelectedIndex == -1)
                        SetSelectedId(curID);
                    else
                        SetSelectedId(curID + datagrid.SelectedIndex);
                }

            }
        }

        private void editField_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtViewer.showText(editField.Text);
        }

        private void menu_about_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Developer: Alexander Lutsai (SL_RU)\ne-mail: s.lyra@ya.ru");
        }
    }

    public class STRDataGridRow : INotifyPropertyChanged
    {
        // boiler-plate
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Orig
        {
            get {
                if (ID >= 0 && ID < Project.origSTR.Length)
                    return Project.origSTR[ID];
                else
                    return " ";
            }
            set { OnPropertyChanged("ID"); OnPropertyChanged("Trans"); OnPropertyChanged("Orig"); }
        }
        public string Trans
        {
            get
            {
                if (ID >= 0 && ID < Project.tranSTR.Length)
                    return Project.tranSTR[ID];
                else
                    return " ";
            }
            set { Project.tranSTR[ID] = value; OnPropertyChanged("ID"); OnPropertyChanged("Trans"); OnPropertyChanged("Orig"); }
        }
        int id;
        public int ID
        {
            get
            {
                if (id >= 0 && id < Project.origSTR.Length)
                    return id;
                else
                    return int.MinValue;
            }
            set { id = value; OnPropertyChanged("ID"); OnPropertyChanged("Trans"); OnPropertyChanged("Orig"); }
        }
    }

    public class Project
    {
        public static bool Loaded = false;

        public static string PrjPath = "";

        public static string PapFont = "fnt_papyrus.gmx",
            SansFont = "fnt_comicsans.gmx",
            MainFont = "fnt_main.gmx",
            PlainText = "fnt_plain.gmx",
            FontFolder = "FONT",
            NewFontFolder = "FONT_new";

        public static string[] origSTR;
        public static string[] tranSTR;

        public static bool Load(string path)
        {
            if (!LoadSTR(path))
            {
                Loaded = false;
                return false;
            }

            PrjPath = path;
            Loaded = true;

            return true;
        }

        public static bool LoadSTR(string path)
        {
            if(!(File.Exists(System.IO.Path.Combine(path, "STRG.txt")) && File.Exists(System.IO.Path.Combine(path, "translate.txt"))))
                return false;
            origSTR = File.ReadAllLines(System.IO.Path.Combine(path, "STRG.txt"));
            tranSTR = File.ReadAllLines(System.IO.Path.Combine(path, "translate.txt"));
            return true;
        }
        
        public static string getFontPath(string s)
        {
            if(File.Exists(System.IO.Path.Combine(PrjPath, NewFontFolder, s)))
            {
                return System.IO.Path.Combine(PrjPath, NewFontFolder, s);
            }
            return System.IO.Path.Combine(PrjPath, FontFolder, s);
        }
    }
}
