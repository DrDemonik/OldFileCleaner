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
using System.IO;

namespace OldFileCleaner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string PatchTempStorage
        {
            get
            {
                return Directory.GetCurrentDirectory() + "\\TempStorage";
            }
        }

        public FileSystemWatcher fsw;

        public MainWindow()
        {
            fsw = new FileSystemWatcher(PatchTempStorage);
            fsw.Filter = "";
            fsw.Created += Fsw_Created;
            fsw.NotifyFilter = NotifyFilters.FileName;
            fsw.EnableRaisingEvents = true;


            if (!Directory.Exists("TempStorage"))
            {
                Directory.CreateDirectory("TempStorage");
            }

            if (!Directory.Exists("Storage"))
            {
                Directory.CreateDirectory("Storage");
            }
            InitializeComponent();            
        }



        private void BCreateFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream(PatchTempStorage + "\\" + System.IO.Path.GetRandomFileName(), FileMode.CreateNew))
                {
                    var text = Encoding.Default.GetBytes("A\nB\nC");
                    fs.Write(text, 0, text.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(() => this.tB.Text +="Создан " + e.Name + "\n");
        }
    }
}
