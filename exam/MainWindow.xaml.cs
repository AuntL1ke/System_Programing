using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PropertyChanged;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Reflection.PortableExecutable;

namespace Exam
{
    public partial class MainWindow : Window
    {
  
        ViewModel model;

        public MainWindow()
        {
            InitializeComponent();
            model = new ViewModel();
            this.DataContext = model;
        }

        private void OpenSource_btn(object sender, RoutedEventArgs e)
        {
            model.Directory = OpenDialog();
        }

 
        private string OpenDialog()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            string result = "";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                result = dialog.FileName;
            }
            return result;
        }

        private void Go_btn(object sender, RoutedEventArgs e)
        {
            AllClean();
            Initializer(GetFilePath());
            Parallel.ForEach(model.Stats, Analyze);
        }


        private void AllClean()
        {
            model.ClearStat();
        }


        private void Initializer(List<string> paths)
        {
            foreach (string path in paths)
            {
                string fileName = Path.GetFileName(path);
                string filePath = Path.GetFullPath(path);
                Stat stat = new Stat(fileName, filePath);
                model.AddStats(stat);
            }
        }


        private List<string> GetFilePath()
        {
            string pattern = "*.txt";
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(model.Directory, pattern, SearchOption.AllDirectories));

            return files;
        }


        


        private void Analyze(Stat stat)
        {
            Task.Run(() =>
            {
                using (StreamReader sr = new StreamReader(new FileStream(stat.Path, FileMode.Open, FileAccess.Read)))
                {
                    
                    int totalWordsChecked = 0;
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null)
                            break;
                        string[] words = line.Split(' ');
                        foreach (string currentWord in words)
                        {
                            if (currentWord.ToLower().Contains(model.Word.ToLower()))
                            {
                                stat.Count++;
                            }
                            totalWordsChecked += currentWord.Length + 1;
                            UpdateWord(stat, sr, totalWordsChecked);
                        }
                    }
                    stat.UpdateProgress(100);

                }
            });
        }

       
        private void OpenSavePath_btn(object sender, RoutedEventArgs e)
        {
            model.SaveDirectory = OpenDialog();
        }

        

        private void UpdateWord(Stat stat, StreamReader sr, int wordLengths)
        {
            stat.UpdateProgress((100.0 / sr.BaseStream.Length) * wordLengths);

        }

        private void Save_btn(object sender, RoutedEventArgs e)
        {
            string savePath = Path.Combine(model.SaveDirectory, "result.txt");
            using (StreamWriter sw = new StreamWriter(new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                sw.WriteLine($"Word: {model.Word}");
                sw.WriteLine();
                foreach (Stat stat in model.Stats)
                {
                    sw.WriteLine($"Name: {stat.Name,-40} Path: {stat.Path,-150} Word Count: {stat.Count}");
                }
            }
            MessageBox.Show("Saved!");
        }

       
    }

    [AddINotifyPropertyChangedInterface]
    public class ViewModel
    {
        private ObservableCollection<Stat> stats;
        public string Directory { get; set; }
        public string SaveDirectory { get; set; } = "";
        public string Word { get; set; }
        public double Total { get; set; }

        public IEnumerable<Stat> Stats => stats;
        public ViewModel()
        {
            stats = new ObservableCollection<Stat>();
        }
        public void AddStats(Stat stat)
        {
            stats.Add(stat);
        }
        public void ClearStat()
        {
            stats.Clear();
        }

     
    }

    [AddINotifyPropertyChangedInterface]
    public class Stat
    {
        public string Name { get; set; }

        public string Path { get; set; }
        public int Count { get; set; } = 0;
        public double Progress { get; set; }
        public Stat(string fileName, string filePath) { Name = fileName; Path = filePath; }
        public void UpdateProgress(double progress) { Progress = progress; }
    }
}
