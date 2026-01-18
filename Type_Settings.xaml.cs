using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lumpix.Inventar.Verwaltung_
{
    public partial class Type_Settings : Window
    {
        #region variables
        #region scroll sync
        private ScrollViewer? scrollViewer1;
        private ScrollViewer? scrollViewer2;
        private bool isSyncingScroll = false;
        #endregion
        #region strings
        public string pfad_Type_Save = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save", "Type_Save.txt");
        string text = "";
        #endregion
        #region int
        int Index = 0;
        int Item_Count = 0;
        #endregion
        #region dictionaries
        private Dictionary<int, string> Types_dict = new Dictionary<int, string>();
        private Dictionary<int, float> Factor_dict = new Dictionary<int, float>();
        #endregion
        #endregion
        #region scroll sync
        private static ScrollViewer? GetScrollViewer(DependencyObject dep)
        {
            if (dep is ScrollViewer) return dep as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            {
                var child = VisualTreeHelper.GetChild(dep, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }
        
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (isSyncingScroll) return;

            isSyncingScroll = true;

            if (sender == scrollViewer1 && scrollViewer2 != null)
            {
                scrollViewer2.ScrollToVerticalOffset(scrollViewer1.VerticalOffset);
            }
            else if (sender == scrollViewer2 && scrollViewer1 != null)
            {
                scrollViewer1.ScrollToVerticalOffset(scrollViewer2.VerticalOffset);
            }

            isSyncingScroll = false;
        }
        public Type_Settings()
        {
            InitializeComponent();
            Loaded += OnType_Settings_Loaded;
        }
        private void OnType_Settings_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer1 = GetScrollViewer(Types);
            scrollViewer2 = GetScrollViewer(Factor);
            if (scrollViewer1 != null)
                scrollViewer1.ScrollChanged += ScrollViewer_ScrollChanged;

            if (scrollViewer2 != null)
                scrollViewer2.ScrollChanged += ScrollViewer_ScrollChanged;
        }
        #endregion
        #region Types verwaltung
        #region import export data
        private void btn_import_data_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Textdatei (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
            openFileDialog.Title = "Import Type Save Datei";
            openFileDialog.FileName = "Type_Save.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                string dateipfad = openFileDialog.FileName;
                System.Diagnostics.Debug.WriteLine("Dateipfad: " + dateipfad);
                if (dateipfad == "")
                {
                    Environment.Exit(0);
                }
                File.WriteAllText(pfad_Type_Save, File.ReadAllText(dateipfad));
            }
            Load_Types();
        }
        private void btn_export_data_Click(object sender, RoutedEventArgs e)
        {
            Save_Types();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Textdatei (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
            saveFileDialog.Title = "Export Type Save Datei";
            saveFileDialog.FileName = "Type_Save.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                string dateipfad = saveFileDialog.FileName;
                System.Diagnostics.Debug.WriteLine("Dateipfad: " + dateipfad);
                if (dateipfad == "")
                {
                    Environment.Exit(0);
                }
                File.WriteAllText(dateipfad, File.ReadAllText(pfad_Type_Save));
            }
        }
        #endregion
        #region Types load und save
        private void Load_Types()
        {
            string text = File.ReadAllText(pfad_Type_Save);
            string[] Gegenstände = text.Split("\n");
            Index = 0;
            foreach (var Gegenstand in Gegenstände)
            {
                Types_dict.Clear();
                Types.Items.Add("Type Name");
                Types.Items.Add("");
                Factor.Items.Add("Factor");
                Factor.Items.Add("");
                try
                {
                    string[] Items1 = Gegenstand.Split(Trenner.trenner);
                    if (Items1[0] == "")
                    {
                        continue;
                    }
                    Types_dict.Add(Index, Items1[0]);
                    Factor_dict.Add(Index, float.Parse(Items1[1]));
                    Types.Items.Add(Items1[0]);
                    Factor.Items.Add(Items1[1]);
                    Index++;
                }
                catch { }
            }
        }
        private void Save_Types()
        {
            Item_Count = Types_dict.Count();
            File.WriteAllText(pfad_Type_Save, "");
            for (int i = 0; i < Item_Count; i++)
            {
                text += Types_dict[i].ToString() + Trenner.trenner + Factor_dict[i].ToString() + "\n";
            }
            File.WriteAllText(pfad_Type_Save, text);
        }
        #endregion
        #region add remove Types TODO: "remove" funktion
        private void btn_add_type_Click(object sender, RoutedEventArgs e)
        {
            Types.Items.Add("[Neuer Typ]");
            Factor.Items.Add("1.0");
            Types.SelectedIndex = Types_dict.Count + 3;
            txt_type_name.Text = "[Neuer Typ]";
            txt_type_factor.Text = "1.0";
            Types_dict.Add(Types_dict.Count + 1, "[Neuer Typ]");
            Factor_dict.Add(Types_dict.Count + 1, 1.0f);
        }
        #endregion
        #region Change Types TODO: save selections after change
        private void txt_type_name_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Types_dict[Types.SelectedIndex - 2] = txt_type_name.Text;
                Types.Items[Types.SelectedIndex] = txt_type_name.Text;
                
            }
            catch 
            { 
                MessageBox.Show("Bitte wählen sie einen Validen Typ den sie ändern möchten aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Types.SelectedIndex = Types.SelectedIndex;
        }
        private void txt_type_factor_LostFocus(object sender, RoutedEventArgs e)
        {
            // Speichere den aktuellen Index
            int selectedIndex = Types.SelectedIndex;

            try
            {
                Factor_dict[selectedIndex - 2] = float.Parse(txt_type_factor.Text);

                Factor.Items[selectedIndex] = txt_type_factor.Text;
            }
            catch
            {
                MessageBox.Show("Bitte wählen sie einen Validen Typ den sie ändern möchten aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Types.SelectedIndex = selectedIndex;
        }
        #endregion
        #endregion

    }
}
