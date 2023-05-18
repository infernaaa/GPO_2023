using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Diagnostics;
using Preprocess_data.Properties;

namespace Preprocess_data
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы CSV|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {

                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void StartProccessBTM_Click(object sender, RoutedEventArgs e)
        {


            if (IsStringPath(FilePathTextBox.Text))
            {
                if (!File.Exists(FilePathTextBox.Text))
                {
                    MessageBox.Show("Файла не существует");

                }
                else
                {
                    


                    if (option1.IsChecked == true)
                    {

                        input_data(FilePathTextBox.Text);
                        MessageBox.Show(FilePathTextBox.Text);
                    }
                    else if (option2.IsChecked == true)
                    {

                       output_data(FilePathTextBox.Text);
                    }


                }
            }

        }
        private bool IsStringPath(String path)
        {
            // Проверка, является ли поданная в функцию строка путем 
            // к файлу или папке
            Regex regex = new Regex(@"[a-zA-Z]:(\\.+)+(.csv\b)");
            path = regex.Match(path).Value;
            if (path != "")
            {

                return true;
            }
            MessageBox.Show("путь нулевой");
            return false;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            inst.Text = "Шаг 1. Нажмите на кнопку <Обзор>. В открывшемся окне найдите нужную папку, в которой выберите интересующий файл с расширением .csv и нажмите кнопку <Открыть>. После выбора необходимого файла для обработки путь к нему отобразится в текстовом поле <Путь к обрабатываемому файлу>." +
"В открывшемся окне будут показаны только файлы с расширением.csv, файлы иного расширения не будут отображены\n" +
                "Шаг 2. Из списка, расположенного ниже, необходимо выбрать нужный тип данных: входные или выходные. Можно выбрать только один вариант из предложенных.Выбор другого варианта приведёт к отмене варианта, который был выбран до этого действия.\n" +
                "Шаг 3. Нажмите на кнопку <Обработать файл>. Появится всплывающие окно, что данные успешно обработаны." +
" Если в текстовом поле <Путь к обрабатываемому файлу> допущена ошибка в написании названия, пути или расширения обрабатываемого файла, то выйдет окно с оповещением об ошибке.В таком случае необходимо повторить шаги для выбора файла для обработки данных(назад к шагу 1).\n" +
                "Шаг 4. После обработки файла откроется окно с графиком, представляющий обработанные данные." +
"Для того чтобы передвинуть файл по горизонтали и / или вертикали необходимо навести курсор мыши на график и зажать левую кнопку мыши." +
" Для того чтобы изменить масштаб графика необходимо навести курсор мыши на график, прокручивая колёсико мыши, выбрать подходящий масштаб.\n";
        }

        /*
         private void input_data(string path)
         {

             MessageBox.Show($"Успешно, входные данные путь к файлу {path}");
         }
         private void output_data(string path)
         {
             MessageBox.Show($"Успешно, выходные данные данные путь к файлу {path}");
         }
        */

        private void input_data(string path)
        {
            // выполнение кода, когда выбрана кнопка "Входные данные"
            //////////////////////////// КОНСТАТЫ ДЛЯ PYTHON АЛГОРИТМА
            // путь к файлу для обработки
            String path_to_data = path;// сделать путь к файлу
            // путь для сохранения обработанного файла
            String path_to_save_data_array_like = "\"C:\\Users\\infernaaa\\Desktop\\X_data.xlsx\"";


            //////////////////////////// ОСТАЛЬНОЕ

            // путь, куда сохранять алгоритм из ресурсов
            //String X_preprocess_file_path = "X_preprocess.py";

            // Переменная, хранящая всю инфу о файле python алгоритма
            //FileInfo X_preprocess = new FileInfo(X_preprocess_file_path);

            // Сохранение python алгоритма из ресурсов в файл
            //File.WriteAllBytes(X_preprocess_file_path, Resources.prepare_X_data);//prepare_X_data.py

            // Проверка, сохранился ли файл
            if (!File.Exists("Python_scripts//preprocess_X_data.py"))
            {

                Console.WriteLine("Ошибка создания скрипта обработки X выборки");
                return;
            }

            // Запуск алгоритма обработки в python
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "CMD.exe";
                startInfo.WorkingDirectory = "Python_scripts";
                startInfo.Arguments = "/C python " + "preprocess_X_data.py" +
                                                    " " + path_to_data +
                                                    " " + path_to_save_data_array_like;
                process.StartInfo = startInfo;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                process.WaitForExit();

                MessageBox.Show(process.StandardOutput.ReadToEnd());

                
            }
            catch (Exception e_i)
            {
                MessageBox.Show(e_i.Message);
            }
        }

        private void output_data(string path)
        {
            // выполнение кода, когда выбрана кнопка "Выходные данные"
            //////////////////////////// КОНСТАТЫ ДЛЯ PYTHON АЛГОРИТМА
            // путь к файлу для обработки
            String path_to_defects = path;
            // путь для сохранения обработанного файла
            String path_to_save_defects = "\"C:\\Users\\infernaaa\\Desktop\\Y_data.xlsx\"";

            // для Run1 115
            // для Run2 119
            int rows_number = 115;
            int detectors_number = 400;

            //////////////////////////// ОСТАЛЬНОЕ

            // путь, куда сохранять алгоритм из ресурсов
           // String Y_preprocess_file_path = "Y_preprocess.py";

            // Переменная, хранящая всю инфу о файле python алгоритма
           // FileInfo Y_preprocess = new FileInfo(Y_preprocess_file_path);

            // Сохранение python алгоритма из ресурсов в файл
            //File.WriteAllBytes(Y_preprocess_file_path, Resources.prepare_Y_data.py);//prepare_Y_data.py

            // Проверка, сохранился ли файл
            if (!File.Exists("Python_scripts//preprocess_Y_data.py"))
            {
                Console.WriteLine("Ошибка создания скрипта обработки X выборки");
                return;
            }

            // Запуск алгоритма обработки в python
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "CMD.exe";
                startInfo.WorkingDirectory = "Python_scripts";
                startInfo.Arguments = "/C python " + "preprocess_Y_data.py" +
                                                    " " + path_to_defects +
                                                    " " + path_to_save_defects +
                                                    " " + rows_number +
                                                    " " + detectors_number;
                process.StartInfo = startInfo;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                Console.WriteLine(process.StandardOutput.ReadToEnd());

                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        
    }
}
