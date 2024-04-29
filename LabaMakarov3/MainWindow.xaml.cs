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

namespace LabaMakarov3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    FilesListBox.Items.Add(filename);
                }
            }
        }

        private void ProcessFilesButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedFiles = FilesListBox.Items.Cast<string>().ToList();
            bool removePunctuation = RemovePunctuationCheckBox.IsChecked == true;
            bool removeShortWords = RemoveShortWordsCheckBox.IsChecked == true;
            int wordLengthThreshold = int.Parse(WordLengthTextBox.Text);

            Task.Run(() =>
            {
                ProcessFiles(selectedFiles, removePunctuation, removeShortWords, wordLengthThreshold);
            });
        }

        private void ProcessFiles(List<string> files, bool removePunctuation, bool removeShortWords, int wordLengthThreshold)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string outputDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);

                foreach (string file in files)
                {
                    string content = File.ReadAllText(file);
                    if (removePunctuation)
                    {
                        content = RemovePunctuation(content);
                    }
                    if (removeShortWords)
                    {
                        content = RemoveShortWords(content, wordLengthThreshold);
                    }

                    string newFileName = System.IO.Path.GetFileNameWithoutExtension(file) + "_processed.txt";
                    string outputPath = System.IO.Path.Combine(outputDirectory, newFileName);
                    File.WriteAllText(outputPath, content);
                }

                MessageBox.Show("Готово.");
            }
        }

        private string RemovePunctuation(string text)
        {
            return Regex.Replace(text, "[^a-zA-Z0-9а-яА-Я ]", "");
        }

        private string RemoveShortWords(string text, int minLength)
        {
            string[] words = text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder newText = new StringBuilder();
            foreach (string word in words)
            {
                if (word.Length >= minLength)
                {
                    newText.Append(word);
                    newText.Append(" ");
                }
            }
            return newText.ToString().Trim();
        }
    }
}
