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
using System.Windows.Shapes;

namespace Lumpix.Inventar.Verwaltung_
{
    public partial class Gegenstand_einstellungen : Window
    {
        public Gegenstand_einstellungen()
        {
            InitializeComponent();
            
        }

        private void txt_EK_LostFocus(object sender, RoutedEventArgs e)
        {
            switch (cmb_type.Text)
            {
                case "Sonstiges":
                    txt_VK.Text = (float.Parse(txt_EK.Text) * PreisMultiplikatoren.Sonstiges).ToString();
                    break;
                case "Schädlingsbekämpfer":
                    txt_VK.Text = (float.Parse(txt_EK.Text) * PreisMultiplikatoren.Schadlingsbekaempfer).ToString();
                    break;
                case "Blumen":
                    txt_VK.Text = (float.Parse(txt_EK.Text) * PreisMultiplikatoren.Blumen).ToString();
                    break;
                case "Pflanzen":
                    txt_VK.Text = (float.Parse(txt_EK.Text) * PreisMultiplikatoren.Pflanzen).ToString();
                    break;
                case "Fertige Arbeiten":
                    txt_VK.Text = (float.Parse(txt_EK.Text) * PreisMultiplikatoren.FertigeArbeiten).ToString();
                    break;
                default:
                    try
                    {
                        txt_VK.Text = (float.Parse(txt_EK.Text) * PreisMultiplikatoren.Default).ToString();
                    }
                    catch
                    { }
                    break;
            }
        }

        private void txt_name_LostFocus(object sender, RoutedEventArgs e)
        {
            lbl_Ausgewähltes_Item.Content = txt_name.Text + " Ändern / Infos";
        }
    }
}
