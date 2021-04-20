using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace AssemblyClass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();        
        }

       

        private List<Regions> Get_Regions(string TNM)
        {
            List<Regions> liste = new List<Regions>();
            using (DataClasses1DataContext db = new DataClasses1DataContext())
            {
                Assembly[] coll = AppDomain.CurrentDomain.GetAssemblies();

                Type typ = null; 
                foreach (var nam in coll)
                {
                    if (nam.FullName.Contains("AssemblyClass")) //ensure find namespace by name if you have similar names in solution
                    {
                        typ = nam.GetType("AssemblyClass." + TNM);// namespace name + table name to connect
                    }
                }

                var tblResult = db.ExecuteQuery(typ, "SELECT * FROM " + TNM).AsQueryable(); // select from the specified table from database

                foreach (var tableItem in tblResult) // get data from tblResult to a list of a specific list type
                {
                    Regions region = new Regions();
                    PropertyInfo[] collTableProps = tableItem.GetType().GetProperties();
                    PropertyInfo[] collCityProps = typeof(Regions).GetProperties();


                    foreach (System.Reflection.PropertyInfo dbTable in collTableProps)
                    {
                        foreach (System.Reflection.PropertyInfo dbRegion in collCityProps)
                        {
                            if (dbTable.Name == dbRegion.Name)
                            {
                                dbRegion.SetValue(region, tableItem.GetType().GetProperty(dbRegion.Name).GetValue(tableItem));
                            }
                        }
                    }
                    liste.Add(region);                   
                }

                listBox.Items.Clear();
                foreach (var item in liste)
                {
                    listBox.Items.Add(item.RegionDescription);
                }

                return liste;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex==1)
            {
                Get_Regions("Region_DE"); // call method with the name of table you want to connect
            }
            if (comboBox.SelectedIndex == 2)
            {
                Get_Regions("Region_RU"); // call method with the name of table you want to connect
            }
        }
    }
}
