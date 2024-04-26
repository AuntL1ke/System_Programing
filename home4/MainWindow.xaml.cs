using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Home4
{
    public partial class MainWindow : Window
    {
        private Stat stat;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Show(object sender, RoutedEventArgs e)
        {
            string textToAnalyse = text.Text;

            // Виклик асинхронного методу для аналізу тексту та оновлення ListBox
            await AnalyzeTextAndUpdateListBox(textToAnalyse);
        }

        private async Task AnalyzeTextAndUpdateListBox(string text)
        {
            // Створення екземпляра Stat та виклик методу TextAnalyse
            stat = new Stat(text);

            // Очистка ListBox перед виведенням результатів
            MyListBox.Items.Clear();

            // Асинхронно виконуємо аналіз тексту та оновлення ListBox
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MyListBox.Items.Add($"Sentences: {stat.Sentences}");
                    MyListBox.Items.Add($"Symbols: {stat.Symbols}");
                    MyListBox.Items.Add($"Words: {stat.Words}");
                    MyListBox.Items.Add($"Questions: {stat.Questions}");
                    MyListBox.Items.Add($"Exclamations: {stat.Exclamations}");
                });
            });
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            string filePath = "results.txt"; // Вказати шлях до файлу
            await SaveResultsToFileAsync(filePath);
        }

        private async Task SaveResultsToFileAsync(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    await writer.WriteLineAsync($"Sentences: {stat.Sentences}");
                    await writer.WriteLineAsync($"Symbols: {stat.Symbols}");
                    await writer.WriteLineAsync($"Words: {stat.Words}");
                    await writer.WriteLineAsync($"Questions: {stat.Questions}");
                    await writer.WriteLineAsync($"Exclamations: {stat.Exclamations}");
                }
                MessageBox.Show("Результати успішно збережено у файл!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка при збереженні файлу: {ex.Message}");
            }
        }

    }

    public class Stat
    {
        public int Sentences { get; private set; }
        public int Symbols { get; private set; }
        public int Words { get; private set; }
        public int Questions { get; private set; }
        public int Exclamations { get; private set; }

        public Stat(string text) { TextAnalyse(text); }

        private void TextAnalyse(string text)
        {
            // Аналіз тексту та збереження результатів
            Sentences = text.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
            Symbols = text.Length;
            Words = text.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            Questions = text.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries).Length - 1;
            Exclamations = text.Split(new char[] { '!' }, StringSplitOptions.RemoveEmptyEntries).Length - 1;
        }
    }
}
