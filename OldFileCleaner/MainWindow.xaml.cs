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

        public string PatchStorage
        {
            get
            {
                return Directory.GetCurrentDirectory() + "\\Storage";
            }
        }

        private string tamplate;

        public FileSystemWatcher fsw;

        public TaskScheduler ui;

        public MainWindow()
        {
            //Инициализация директорий
            if (!Directory.Exists("TempStorage"))
            {
                Directory.CreateDirectory("TempStorage");
            }
            if (!Directory.Exists("Storage"))
            {
                Directory.CreateDirectory("Storage");
            }
            //Инициализация и настройка FileSystemWatcher
            fsw = new FileSystemWatcher(PatchTempStorage);
            fsw.Filter = "";
            fsw.Created += Fsw_Created;
            fsw.NotifyFilter = NotifyFilters.FileName;
            fsw.EnableRaisingEvents = true;

            InitializeComponent();
            //Контекст синхронизации для работы с UI елементами в UI потоке
            ui = TaskScheduler.FromCurrentSynchronizationContext();
            //Содержимое шаблонного файла
            tamplate = "A\nB\nC";
        }

        /// <summary>
        /// Создание фалов
        /// </summary>
        private void BCreateFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var random = new Random();
                
                using (FileStream fs = new FileStream(PatchTempStorage + "\\" + System.IO.Path.GetRandomFileName(), FileMode.CreateNew))
                {
                    if (random.Next(0, 100) >49)
                    {
                        //Шаюлонный файл
                        var text = Encoding.Default.GetBytes(tamplate);
                        fs.Write(text, 0, text.Length);
                    }
                    else
                    {
                        //Случайный файл
                        var text = Encoding.Default.GetBytes(System.Guid.NewGuid().ToString());
                        fs.Write(text, 0, text.Length);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Событие создания файла
        /// </summary>
        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            //Задача работы с файлами
            Task.Run(async () => 
            {
                try
                {
                    var time = DateTime.Now.TimeOfDay;
                    WorkWithTextBox(e, new Action(() => { this.tB.Text += "Создан " + e.Name + " Время:" + DateTime.Now.ToString() + "\n"; })); 
                    //Парсинг
                    if (Parsing.Run(e.FullPath, tamplate))
                    {
                        //Время удаления в зависимости от время создания
                        var delta = 5000 - Math.Abs((DateTime.Now.TimeOfDay - time).Milliseconds);
                        await Task.Delay(delta);
                        //Удаление
                        if (File.Exists(e.FullPath))
                        {
                            File.Delete(e.FullPath);
                            WorkWithTextBox(e, new Action(() => { this.tB.Text += "Удалён " + e.Name + " Время:" + DateTime.Now.ToString() + "\n"; }));
                        }
                    }
                    else
                    {
                        //Переименование и перемещение
                        if (File.Exists(e.FullPath))
                        {
                            var newFullPath = PatchStorage + "\\" + System.Guid.NewGuid().ToString() + new FileInfo(e.FullPath).Extension;
                            File.Move(e.FullPath, newFullPath);
                            WorkWithTextBox(e, new Action(() => { this.tB.Text += "Файл " + e.Name+" переименован в " + new FileInfo(newFullPath).Name + " Время:" + DateTime.Now.ToString() + "\n"; }));
                        }
                    }
                }
                //Задача не положит приложение исключением
                catch
                {

                }                
            });
        }

        /// <summary>
        /// Работа с TextBox  в потоке UI
        /// </summary>
        private void WorkWithTextBox(FileSystemEventArgs e,Action action)
        {
            Parallel.Invoke(new ParallelOptions()
            {
                TaskScheduler = ui,
                CancellationToken = CancellationToken.None,
                MaxDegreeOfParallelism = 1
            }, action);
        }
    }
}
