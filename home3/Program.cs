using System;
using System.IO;
using System.Threading;

namespace _06_practice_1
{
    class Stat
    {
        public static int Letters { get; private set; }
        public static int Digits { get; private set; }
        public static int Punctuations {  get; private set; }
        private static readonly object lockObj = new object();

        public static void TextAnalyse(object text)
        {
            var res = (string)text;
            string result = File.ReadAllText(res);
            int localLetters = 0;
            int localDigits = 0;
            int localPunctuations = 0;
            foreach (char c in result)
            {
                if (char.IsLetter(c))
                    localLetters++;
                else if (char.IsDigit(c))
                    localDigits++;
                else if(char.IsPunctuation(c))
                    localPunctuations++;

            }

            lock (lockObj)
            {
                Letters += localLetters;
                Digits += localDigits;
                Punctuations += localPunctuations;  
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
          
            string currentDirectory = Environment.CurrentDirectory;
            string[] files = Directory.GetFiles(@"C:\Users\semen\source\repos\Thread_Sync\Thread_Sync\06_practice_1\files", "*.txt");
            Thread[] threads = new Thread[files.Length];



            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(Stat.TextAnalyse);
                threads[i].Start(files[i]);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            Console.WriteLine($"Word Count: {Stat.Letters}");
            Console.WriteLine($"Strings Count: {Stat.Digits}");
            Console.WriteLine($"Punctuations Count: {Stat.Punctuations}");


        }
    }
}
