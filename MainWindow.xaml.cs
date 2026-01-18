using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace Lumpix.Inventar.Verwaltung_
{
    public partial class MainWindow : Window
    {
        #region Variablen
        #region Dictionaries
        public Dictionary<int, string> item_titel = new Dictionary<int, string>();
        public Dictionary<int, int> item_anzahl = new Dictionary<int, int>();
        public Dictionary<int, string> item_beschreibung = new Dictionary<int, string>();
        public Dictionary<int, string> item_kategorie = new Dictionary<int, string>();
        public Dictionary<int, float> item_ek = new Dictionary<int, float>();
        #endregion
        #region int, string und bool
        int Item_Count = 0;
        int Item_ID_Settings = 0;
        int Index = 0;

        string text = "";
        string pfad2 = "";
        public string pfad1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save", "PFAD.txt");
        public string pfad = "";

        private bool isSyncingScroll = false;
        #endregion
        #region ScrollViewer
        private ScrollViewer? scrollViewer1;
        private ScrollViewer? scrollViewer2;
        #endregion
        #endregion
        #region Main Window
        public MainWindow()
        {
            InitializeComponent();
            Closing += OnMainWindowClosing;
        }
        private void OnMainWindowClosing(object? sender, CancelEventArgs e)
        {
            Debug.WriteLine("Pfad: " + pfad);
            try
            {
                save_items();
            }
            catch
            {
                MessageBox.Show("Änderungen konnten nicht gespeichert werden!", "Nicht Gespeichert", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pfad = File.ReadAllText(pfad1);
            System.Diagnostics.Debug.WriteLine("TEST", pfad);
            if (pfad != "")
            {
                try
                {
                    text = File.ReadAllText(pfad);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Die Datei wurde nicht gefunden. Bitte erstellen Sie eine neue Datei und wälen die Datei über den -Pfad Ändern- knopf aus", "Datei nicht gefunden", MessageBoxButton.OK, MessageBoxImage.Error);
                    File.WriteAllText(pfad1, "");
                    return;
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Bitte wählen sie eine Datei aus", "Datei nicht gefunden", MessageBoxButton.OK, MessageBoxImage.Error);
                    File.WriteAllText(pfad1, "");
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                System.Diagnostics.Debug.WriteLine(text);
                System.Diagnostics.Debug.WriteLine(pfad);
                load_items();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Textdatei (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
                openFileDialog.Title = "Specher Datei erstellen";
                openFileDialog.FileName = "SPEICHER.txt";
                if (openFileDialog.ShowDialog() == true)
                {
                    string dateipfad = openFileDialog.FileName;
                    System.Diagnostics.Debug.WriteLine("Dateipfad: " + dateipfad);
                    if (dateipfad == "")
                    {
                        Environment.Exit(0);
                    }
                    pfad = dateipfad;
                    File.WriteAllText(pfad1, pfad);
                }
                try
                {
                    text = File.ReadAllText(pfad);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Die Datei wurde nicht gefunden. Bitte erstellen Sie eine neue Datei und wälen die Datei über den -Pfad Ändern- knopf aus.", "Datei nicht gefunden", MessageBoxButton.OK, MessageBoxImage.Error);
                    File.WriteAllText(pfad1, "");
                    return;
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Bitte wählen sie eine Datei aus", "Datei nicht gefunden", MessageBoxButton.OK, MessageBoxImage.Error);
                    File.WriteAllText(pfad1, "");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }

                System.Diagnostics.Debug.WriteLine(text);
                System.Diagnostics.Debug.WriteLine(pfad);
                load_items();
            }
            scrollViewer1 = GetScrollViewer(_list_got_1);
            scrollViewer2 = GetScrollViewer(_list_got_2);

            if (scrollViewer1 != null)
                scrollViewer1.ScrollChanged += ScrollViewer_ScrollChanged;

            if (scrollViewer2 != null)
                scrollViewer2.ScrollChanged += ScrollViewer_ScrollChanged;
        }
        #endregion
        #region Gegenstand Einstellungen
        Gegenstand_einstellungen _gegenstand_Einstellungen;
        Type_Settings _gegenstand_Type_Settings;
        private void _gegenstand_Einstellungen_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _gegenstand_Einstellungen.lbl_Ausgewähltes_Item.Content = item_titel[Item_ID_Settings] + " Ändern / Infos";
                _gegenstand_Einstellungen.txt_name.Text = item_titel[Item_ID_Settings];
                _gegenstand_Einstellungen.txt_Maenge.Text = item_anzahl[Item_ID_Settings].ToString();
                _gegenstand_Einstellungen.txt_Info.Text = item_beschreibung[Item_ID_Settings];
                _gegenstand_Einstellungen.cmb_type.Text = item_kategorie[Item_ID_Settings];
                _gegenstand_Einstellungen.txt_EK.Text = item_ek[Item_ID_Settings].ToString();
                switch (_gegenstand_Einstellungen.cmb_type.Text)
                {
                    case "Sonstiges":
                        _gegenstand_Einstellungen.txt_VK.Text = (item_ek[Item_ID_Settings] * PreisMultiplikatoren.Sonstiges).ToString();
                        break;
                    case "Schädlingsbekämpfer":
                        _gegenstand_Einstellungen.txt_VK.Text = (item_ek[Item_ID_Settings] * PreisMultiplikatoren.Schadlingsbekaempfer).ToString();
                        break;
                    case "Blumen":
                        _gegenstand_Einstellungen.txt_VK.Text = (item_ek[Item_ID_Settings] * PreisMultiplikatoren.Blumen).ToString();
                        break;
                    case "Pflanzen":
                        _gegenstand_Einstellungen.txt_VK.Text = (item_ek[Item_ID_Settings] * PreisMultiplikatoren.Pflanzen).ToString();
                        break;
                    case "Fertige Arbeiten":
                        _gegenstand_Einstellungen.txt_VK.Text = (item_ek[Item_ID_Settings] * PreisMultiplikatoren.FertigeArbeiten).ToString();
                        break;
                    default:
                        _gegenstand_Einstellungen.txt_VK.Text = (item_ek[Item_ID_Settings] * 1).ToString();
                        break;

                }
            }
            catch
            {
            }
        }

        private void _gegenstand_Einstellungen_Closing(object? sender, CancelEventArgs e)
        {
            try
            {
                item_titel[Item_ID_Settings] = _gegenstand_Einstellungen.txt_name.Text;
                item_anzahl[Item_ID_Settings] = int.Parse(_gegenstand_Einstellungen.txt_Maenge.Text);
                item_beschreibung[Item_ID_Settings] = _gegenstand_Einstellungen.txt_Info.Text;
                item_kategorie[Item_ID_Settings] = _gegenstand_Einstellungen.cmb_type.Text;
                item_ek[Item_ID_Settings] = float.Parse(_gegenstand_Einstellungen.txt_EK.Text);
                reload_items();
            }
            catch
            {
            }
        }
        #endregion
        #region Scrole Viewer
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
        #endregion
        #region Button Events
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            Item_Count = item_titel.Count();
            item_titel.Add(Item_Count, "[Name eingeben]");
            item_anzahl.Add(Item_Count, 0);
            item_beschreibung.Add(Item_Count, "");
            item_kategorie.Add(Item_Count, "Sonstiges");
            item_ek.Add(Item_Count, 0);

            Item_ID_Settings = item_titel.Count() - 1;
            _gegenstand_Einstellungen = new Gegenstand_einstellungen();
            _gegenstand_Einstellungen.Closing += _gegenstand_Einstellungen_Closing;
            _gegenstand_Einstellungen.Loaded += _gegenstand_Einstellungen_Loaded;
            _gegenstand_Einstellungen.ShowDialog();
            save_items();
            load_items();
        }
        private void btn_delitm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = $"Soll der gegenstand -{_list_got_1.SelectedItem.ToString()}- wirklich entfernt werden";
                MessageBoxResult should_delet = MessageBox.Show(message, "Entfernt", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (should_delet == MessageBoxResult.No)
                {
                    MessageBox.Show("Der Gegenstand wurde nicht entfernt", "Entfernt", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (should_delet == MessageBoxResult.Yes)
                {
                    int delete_index = _list_got_1.SelectedIndex - 2;
                    item_titel.Remove(delete_index);
                    item_anzahl.Remove(delete_index);
                    item_beschreibung.Remove(delete_index);
                    item_kategorie.Remove(delete_index);
                    item_ek.Remove(delete_index);
                    save_items();
                    load_items();
                    MessageBox.Show("Der Gegenstandwurde entfernt", "Entfernt", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                MessageBox.Show("Bitte ein Item auswählen!", "Nichts Ausgewählt", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btn_changpath_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Pfad: " + pfad);
            save_items();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Textdatei (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
            openFileDialog.Title = "Speicher Datei öffnen";
            openFileDialog.FileName = "";
            if (openFileDialog.ShowDialog() == true)
            {
                string dateipfad = openFileDialog.FileName;
                System.Diagnostics.Debug.WriteLine("Dateipfad: " + dateipfad);
                if (dateipfad == "")
                {
                    Environment.Exit(0);
                }
                pfad = dateipfad;
                File.WriteAllText(pfad1, pfad);
            }
            try
            {
                text = File.ReadAllText(pfad);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Die Datei wurde nicht gefunden oder es wurde keine ausgewählt. Bitte erstellen Sie eine neue Datei oder wählen sie eine Datei aus", "Datei nicht gefunden", MessageBoxButton.OK, MessageBoxImage.Error);
                File.WriteAllText(pfad1, "");
                return;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Bitte wählen sie eine Datei aus", "Datei nicht gefunden", MessageBoxButton.OK, MessageBoxImage.Error);
                File.WriteAllText(pfad1, "");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            load_items();
        }
        private void btn_item_settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Item_ID_Settings = _list_got_1.SelectedIndex - 2;
                _gegenstand_Einstellungen = new Gegenstand_einstellungen();
                _gegenstand_Einstellungen.Closing += _gegenstand_Einstellungen_Closing;
                _gegenstand_Einstellungen.Loaded += _gegenstand_Einstellungen_Loaded;
                _gegenstand_Einstellungen.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Bitte ein Item auswählen!", "Nichts Ausgewählt", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void btn_type_settings_Click(object sender, RoutedEventArgs e)
        {
            _gegenstand_Type_Settings = new Type_Settings();
            _gegenstand_Type_Settings.ShowDialog();
        }
        #endregion
        #region Item Verwaltung
        private void reload_items()
        {
            _list_got_1.Items.Clear();
            _list_got_2.Items.Clear();
            _list_got_1.Items.Add("Gegenstand");
            _list_got_2.Items.Add("Menge");
            _list_got_1.Items.Add("");
            _list_got_2.Items.Add("");
            Item_Count = item_titel.Count();
            for (int i = 0; i < Item_Count; i++)
            {
                _list_got_1.Items.Add(item_titel[i]);
                _list_got_2.Items.Add(item_anzahl[i]);
            }
        }
        private void save_items()
        {
            Item_Count = item_titel.Count();
            File.WriteAllText(pfad, "");
            foreach (var key in item_titel.Keys)
            {
                File.AppendAllText(pfad, item_titel[key] + Trenner.trenner + item_anzahl[key].ToString() + Trenner.trenner + item_kategorie[key] + Trenner.trenner + item_beschreibung[key] + Trenner.trenner + item_ek[key].ToString() + "\n");
            }
            item_titel.Clear();
            item_anzahl.Clear();
            item_beschreibung.Clear();
            item_kategorie.Clear();
            item_ek.Clear();
            _list_got_1.Items.Clear();
            _list_got_2.Items.Clear();
        }
        private void load_items()
        {
            string text = File.ReadAllText(pfad);
            string[] Gegenstände = text.Split("\n");
            Index = 0;
            foreach (var Gegenstand in Gegenstände)
            {
                try
                {
                    string[] Items1 = Gegenstand.Split(Trenner.trenner);
                    if (Items1[0] == "")
                    {
                        continue;
                    }
                    item_titel.Add(Index, Items1[0]);
                    item_anzahl.Add(Index, int.Parse(Items1[1]));
                    item_kategorie.Add(Index, Items1[2]);
                    item_beschreibung.Add(Index, Items1[3]);
                    item_ek.Add(Index, float.Parse(Items1[4]));
                    Index++;
                }
                catch { }

            }
            try
            {
                reload_items();
            }
            catch { }
        }
        #endregion
    }
}   