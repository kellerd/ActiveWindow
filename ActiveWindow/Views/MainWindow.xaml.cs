using ActiveWindow.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ActiveWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
                ((MainViewModel)this.DataContext).Load();
            else
                ((MainViewModel)this.DataContext).Questions.Add(new Work() { What = "Visual Studio", When = DateTime.Now }, new Project(TimeSpan.FromSeconds(15), new Work() { What = "Visual Studio", When = DateTime.Now }));
        }


    }
}
