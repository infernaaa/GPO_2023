using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Linq;
using System.Collections.Generic;

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
        private void EnterPathBTM_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users\\Eugen\\Desktop\\5_laba";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                PathToTargetFolder.Text = dialog.FileName;
            }
        }

        private void StartProccessBTM_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(PathToTargetFolder.Text))
            {
                DirectoryInfo targetFolder = new DirectoryInfo(PathToTargetFolder.Text);

                // получить список файлов в папке
                IEnumerable<string> data_files, defects_files;
                try
                {
                    data_files = Directory.EnumerateFiles(targetFolder.FullName,
                        "run?_WM32_data.csv",
                        SearchOption.TopDirectoryOnly);

                    defects_files = Directory.EnumerateFiles(targetFolder.FullName, 
                        "run?_WM32_defects.csv",
                        SearchOption.TopDirectoryOnly);

                    if ((data_files.Count() == 1) && (defects_files.Count() == 1))
                    {
                        FileInfo dataFileInfo = new FileInfo(data_files.GetEnumerator().Current),
                            defectsFileInfo = new FileInfo(defects_files.GetEnumerator().Current);

                        if (dataFileInfo.Name[3] == dataFileInfo.Name[3])
                        {
                            preprocess_data(dataFileInfo.FullName, defectsFileInfo.FullName);
                        }
                        else
                        {
                            MessageBox.Show("Файлы вида \"run1_WM32_data.csv\" и " +
                                "\"run1_WM32_defects.csv\" должны относиться к одиному \"run\"." +
                                "Но файл вида \"run1_WM32_data.csv\" относится к: run" + dataFileInfo.Name[3] +
                                ", а файл вида \"run1_WM32_defects.csv\" к : run" + defectsFileInfo.Name[3]);
                            return;
                        }

                    }
                    else
                    {
                        MessageBox.Show("В указанной папке должен находиться" +
                            "1 файл вида \"run1_WM32_data.csv\" и 1 файл вида" +
                            " \"run1_WM32_defects.csv\". Условие не выполнено.");
                        return;
                    }

                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Указанная папка не существует");
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            Instruction.Text = Preprocess_data.Properties.Resources.instruction;
            PathToTargetFolder.Text = "Выберите подходящую папку";
        }


        private void preprocess_data(string dataFilePath, string defectsFilePath)
        {
            String scriptFileName = "preprocess_data.py";

            // Проверка, сохранился ли файл
            if (!File.Exists(scriptFileName))
            {
                MessageBox.Show("Отсутствует файл скрипта обработки данных");
                return;
            }

            MessageBox.Show("Запуск алгоритма обработки");

            // Запуск алгоритма обработки в python
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "CMD.exe";
                startInfo.Arguments = "/C python " + scriptFileName +
                                                    " \"" + dataFilePath +
                                                    "\" \"" + defectsFilePath + "\"";
                process.StartInfo = startInfo;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                process.WaitForExit();

                MessageBox.Show(process.StandardOutput.ReadToEnd());// для второго файла поменять слеш на пробел
                // process.StandardOutput.ReadToEnd() вывыодит массив
            }
            catch (Exception e_i)
            {
                MessageBox.Show(e_i.Message);
            }
        }   
        
    }
}
