using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
//using System.Threading;
using System.Threading.Tasks;
using System.Threading;

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

        public TaskScheduler ui;

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
            ui = TaskScheduler.FromCurrentSynchronizationContext();
            
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
            Task.Run(async () => 
            {
                var time = DateTime.Now.TimeOfDay;
                Parallel.Invoke(new ParallelOptions()
                {
                    TaskScheduler = ui,
                    CancellationToken = CancellationToken.None,
                    MaxDegreeOfParallelism = 1
                }, () => { this.tB.Text += "Создан " + e.Name + " Время:" + DateTime.Now.ToString() + "\n"; });
                //Task.Factory.StartNew(() => { this.tB.Text += "Создан " + e.Name +" Время:"+ DateTime.Now.ToString() + "\n"; }, CancellationToken.None, TaskCreationOptions.None, ui);
                if (Parsing.Run(e.FullPath))
                {
                    //TimeSpan CurrentTime = DateTime.Now.TimeOfDay;
                    var delta = 5000 - Math.Abs(( DateTime.Now.TimeOfDay-time).Milliseconds);
                    await Task.Delay(delta);
                    Parallel.Invoke(new ParallelOptions()
                    {
                        TaskScheduler = ui,
                        CancellationToken = CancellationToken.None,
                        MaxDegreeOfParallelism = 1
                    }, () => { this.tB.Text += "Удаление " + e.Name + " Время:" + DateTime.Now.ToString() + "\n"; });
                    //Task.Factory.StartNew(() => { this.tB.Text += "Удаление " + e.Name + " Время:" + DateTime.Now.ToString() + "\n"; }, CancellationToken.None, TaskCreationOptions.None, ui);
                    if (File.Exists(e.FullPath))
                    {
                        File.Delete(e.FullPath);
                    }
                }
                else
                {

                }
            });
            

     
            //Dispatcher.Invoke(() => this.tB.Text +="Создан " + e.Name + "\n");
            //Dispatcher.BeginInvoke(new Action(() => tB.Text += "Создан " + e.Name + "\n"));

            //Parallel.Invoke(() => this.tB.Text += "Создан " + e.Name + "\n");
            //this.tB.Text += "Изменён \n";
            //System.Threading.Tasks.Task.Run(() => tB.Text += "Создан " + e.Name + "\n");

            //new Task(() =>
            //{
            //    this.tB.Text += "Создан " + e.Name + "\n";
            //}).RunSynchronously();

            //lock (locker)
            //{
            //    this.tB.Text += "Создан " + e.Name + "\n";
            //}

        }
    }
}
