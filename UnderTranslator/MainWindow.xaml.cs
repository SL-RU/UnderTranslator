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
using System.Diagnostics;

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
        }

        
        ObservableCollection<STRDataGridRow> bindLst = new ObservableCollection<STRDataGridRow>();
        int selID = 0;  //element ID selected in datagrid
        int maxRows = 0;//currently displaying rows count
        int curID = 0;  //current ID in zero row in datagrid 
        int lastSelectedID = -1; //for search

        public bool LoadProject(string s)
        {
            if (Project.Load(s))
            {
                datagrid.ItemsSource = bindLst;
                scr.Maximum = Project.origSTR.Length;
                gotoID.Maximum = Project.origSTR.Length;
                curID = 0;
                ShowData();
                datagrid.SelectedIndex = maxRows / 2;
                SetSelectedId(maxRows / 2);
                txtViewer.plainmode.IsChecked = true;
                txtViewer.stndrt.IsChecked = true;
                return true;
            }
            return false;
        }
        public bool saveProject()
        {
            if (!Project.Loaded)
                return false;
            return Project.SaveProject();
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


        //handle datagrid
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
                curID = Math.Min(curID + 1, Project.origSTR.Length);
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
        private void datagrid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                e.Handled = true;
            }
        }
        private void datagrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scr.Value -= maxRows * e.Delta / 800;
            e.Handled = true;
        }
        private void datagrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            maxRows = (int)((datagrid.ActualHeight - datagrid.ColumnHeaderHeight + 5) / (datagrid.RowHeight));

            bindLst.Clear();
            datagrid.CancelEdit();
            for (int i = 0; i < maxRows; i++)
            {
                bindLst.Add(new STRDataGridRow() { ID = -1 });
            }
            ShowData();
            datagrid.SelectedIndex = maxRows / 2;
        }
        //scrollbar
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


        //handle numeric updown
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
        private void NumericUpDown_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (gotoID.Value >= 0 && gotoID.Value < Project.origSTR.Length)
                {
                    if (datagrid.SelectedIndex == -1)
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

        //handle middle mouse button
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
        private void MetroWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                e.Handled = true;
            }
        }


        //handle updates in edit field
        private void editField_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtViewer.showText(editField.Text);
        }


        //handle SEARCH keys 
        private void search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Down) //Down - search forward
            {
                if (datagrid.SelectedValue is STRDataGridRow)
                {
                    if (!Project.Loaded || datagrid.SelectedValue == null || (datagrid.SelectedValue as STRDataGridRow).ID == int.MinValue)
                        return;
                }
                else
                    return;
                e.Handled = true;
                if (!(selID >= 0 && selID < Project.origSTR.Length - 2))
                    return;

                int i = selID + 1;
                while(i < Project.origSTR.Length - 2)
                {
                    if (Project.origSTR[i].ToUpper().Contains(search.Text.ToUpper()) 
                        || Project.tranSTR[i].ToUpper().Contains(search.Text.ToUpper())
                        || i.ToString().ToUpper().Contains(search.Text.ToUpper()))
                    {
                        lastSelectedID = selID;
                        SetSelectedId(i);
                        search.Focus();
                        break;
                    }
                    i++;
                }
            }
            if(e.Key == Key.Enter) //goto edit field
            {
                if (datagrid.SelectedValue is STRDataGridRow)
                {
                    if (!Project.Loaded || datagrid.SelectedValue == null || (datagrid.SelectedValue as STRDataGridRow).ID == int.MinValue)
                        return;
                }
                else
                    return;
                e.Handled = true;
                if (!(selID >= 0 && selID < Project.origSTR.Length - 2))
                    return;

                editField.Focus();
            }
            if (e.Key == Key.Up) //go to last and then search back
            {
                if (datagrid.SelectedValue is STRDataGridRow)
                {
                    if (!Project.Loaded || datagrid.SelectedValue == null || (datagrid.SelectedValue as STRDataGridRow).ID == int.MinValue)
                        return;
                }
                else
                    return;
                e.Handled = true;
                if (!(selID >= 0 && selID < Project.origSTR.Length - 2))
                    return;
                if(lastSelectedID >= 0)
                {
                    SetSelectedId(lastSelectedID);
                    lastSelectedID = -1;
                    search.Focus();
                    return;
                }

                int i = selID - 1;
                while (i >= 0)
                {
                    if (Project.origSTR[i].ToUpper().Contains(search.Text.ToUpper())
                        || Project.tranSTR[i].ToUpper().Contains(search.Text.ToUpper())
                        || i.ToString().ToUpper().Contains(search.Text.ToUpper()))
                    {
                        SetSelectedId(i);
                        search.Focus();
                        break;
                    }
                    i--;
                }
            }
        }



        //about button
        private void menu_about_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Developer: Alexander Lutsai (SL_RU)\ne-mail: s.lyra@ya.ru\nVK: https://vk.com/sl_ru_dev");
        }
        //save button
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (saveProject())
                MessageBox.Show("Saved");
        }
        //dump button
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (!Project.Loaded)
                return;
            string p = "trans_dump" + DateTime.Now.ToShortDateString() + " " + (DateTime.Now.Ticks % 50000).ToString() + ".txt";
            if(Project.SaveProject(p))
                MessageBox.Show("Dumped into: " + p);
        }
        //open button
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                if(LoadProject(dialog.SelectedPath))
                {
                    MessageBox.Show("Loaded");
                }
                else
                {
                    MessageBox.Show("Error, maybe you shoos a wrong path");
                }
            }
        }
        //exit button
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
                Close();
        }
        //on exit
        private void MenuItem_Click_3(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure? Have you save the project?", "Ar u sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;
        }
        //goto github page
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/SL-RU/UnderTranslator");
        }
        //goto pack and extract githubpage
        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/fjay69/UndertaleTools");
        }

        //Extract utility
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("WinExtract.exe"))
            {
                MessageBox.Show("I couldn't find WinExtract.exe please copy it here. You can get it here https://github.com/fjay69/UndertaleTools");
                return;
            }
            //Extract Undertale
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select Undertale folder";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string up = System.IO.Path.Combine(dialog.SelectedPath, "data.win");
                if(!File.Exists(up))
                {
                    MessageBox.Show("This is not Undertale! YOU LITTLE LIAR!111");
                    return;
                }
                var dialog2 = new System.Windows.Forms.FolderBrowserDialog();
                dialog2.Description = "Select destination path where Undertale will be extracted.";
                result = dialog2.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string dest = System.IO.Path.Combine(dialog2.SelectedPath, "UndertaleRes");
                    if (Directory.Exists(dest))
                    {
                        MessageBox.Show("The game has already been extracted here. Choose another path.");
                        return;
                    }
                    Directory.CreateDirectory(dest);

                    var proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "WinExtract.exe",
                            Arguments = up + " " + dest + "",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };
                    proc.Start();
                    string o = "";
                    string line = "";
                    while (!proc.StandardOutput.EndOfStream)
                    {
                        line = proc.StandardOutput.ReadLine();
                        o += line + "\n";
                    }
                    MessageBox.Show(o);
                    if(line.StartsWith("Chunk AUDO offset:"))
                    {
                        MessageBox.Show("* Undertale has been extracted. \n* You are filled with determination.");
                        if (Project.Loaded)
                        {
                            if (MessageBox.Show("Do you want open new project without saving the current?", "Do not save current?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                LoadProject(dest);
                            }
                        }
                        else
                            LoadProject(dest);
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong...");
                    }
                }
                else
                {
                    MessageBox.Show("Please, select destination path.");
                }
            }
            else
            {
                MessageBox.Show("Please, choose Undertale folder.");
            }
        }
        //Pack utility
        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("WinPack.exe"))
            {
                MessageBox.Show("I couldn't find WinPack.exe please copy it here. You can get it here https://github.com/fjay69/UndertaleTools");
                return;
            }
            //Extract Undertale
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select Undertale folder";

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string up = System.IO.Path.Combine(dialog.SelectedPath, "data.win");
                if(!File.Exists(up))
                {
                    MessageBox.Show("This is not Undertale! YOU LITTLE LIAR!111");
                    return;
                }
                var dialog2 = new System.Windows.Forms.FolderBrowserDialog();
                dialog2.Description = "Select project folder with translation.";
                if (Project.Loaded)
                    dialog2.SelectedPath = Project.PrjPath;
                result = dialog2.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string dest = System.IO.Path.Combine(dialog2.SelectedPath);
                    if (!File.Exists(System.IO.Path.Combine(dest, "translate.txt")))
                    {
                        MessageBox.Show("This is not your translation folder! Choose folder, where you extact THE GAME");
                        return;
                    }
                    
                    File.Copy(up, System.IO.Path.Combine(dialog.SelectedPath, "data_backup" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.Hour.ToString() + "." + DateTime.Now.Minute.ToString() + ".win"), true);

                    var proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "WinPack.exe",
                            Arguments = dest + " " + up + " -tt",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };
                    proc.Start();
                    string o = "";
                    string line = "";
                    while (!proc.StandardOutput.EndOfStream)
                    {
                        line = proc.StandardOutput.ReadLine();
                        o += line + "\n";
                    }
                    MessageBox.Show(o);

                }
                else
                {
                    MessageBox.Show("Please, select folder with translated text.");
                }
            }
            else
            {
                MessageBox.Show("Please, choose Undertale folder.");
            }
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
            set { Project.tranSTR[ID] = value; OnPropertyChanged("Trans"); }
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
        public static bool SaveProject()
        {
            return SaveProject("translate.txt");
        }
        public static bool SaveProject(string name)
        {
            if (!Project.Loaded)
                return false;
            File.WriteAllLines(System.IO.Path.Combine(PrjPath, name), tranSTR);
            return true;
        }
        public static bool LoadSTR(string path)
        {
            if(!(File.Exists(System.IO.Path.Combine(path, "STRG.txt"))))
                return false;
            origSTR = File.ReadAllLines(System.IO.Path.Combine(path, "STRG.txt"));

            if (File.Exists(System.IO.Path.Combine(path, "translate.txt")))
            {
                tranSTR = File.ReadAllLines(System.IO.Path.Combine(path, "translate.txt"));
            }
            else
                tranSTR = new string[0];

            if (tranSTR.Length < 5 || tranSTR.Length == 0)
            {
                tranSTR = new string[origSTR.Length];
                for (int i = 0; i < origSTR.Length; i++)
                {
                    tranSTR[i] = origSTR[i];
                }
            }
            if (origSTR.Length != tranSTR.Length || origSTR.Length == 0)
                return false;
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
