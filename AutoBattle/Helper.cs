using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using static AutoBattle.Program;

namespace AutoBattle
{
    /// <summary>
    /// Helper class for reusable resources
    /// </summary>
    public static class Helper
    {
        private static Random random;
        /// <summary>
        /// Checks if the console input is an integer value
        /// </summary>
        /// <param name="input">Value taken by console input.</param>
        /// <returns>It always returns an integer value, and if the input is not valid, it returns zero. This test does not require an exception.</returns>
        public static int GetIntParsed(string input)
        {
            int result;
            Int32.TryParse(input, out result);
            return result;
        }

        // Method to use only one instance of the Random class in the project in order to optimize memory consumption.
        public static int GetRandomInt(int min, int max)
        {
            if (random == null)
            {
                random = new Random();
            }
            return random.Next(min, max);
        }

        public static void PrintWellcome()
        {
            Console.Clear();
            Console.ResetColor();
            WriteLineCenter("WELLCOME");
            Console.WriteLine("                 _          ____          _    _    _       ");
            Console.WriteLine("    /\\          | |        |  _ \\        | |  | |  | |      ");
            Console.WriteLine("   /  \\   _   _ | |_  ___  | |_) |  __ _ | |_ | |_ | |  ___ ");
            Console.WriteLine("  / /\\ \\ | | | || __|/ _ \\ |  _ <  / _` || __|| __|| | / _ \\");
            Console.WriteLine(" / ____ \\| |_| || |_| (_) || |_) || (_| || |_ | |_ | ||  __/");
            Console.WriteLine("/_/    \\_\\\\__,_| \\__|\\___/ |____/  \\__,_| \\__| \\__||_| \\___|");
            WriteLineCenter(":::: THE GAME ::::");
            Console.WriteLine(Environment.NewLine);
        }

        public static string AlignToCenter(string text, int screenLength)
        {
            return String.Format("{0," + (int)((screenLength / 2) + (text.Length / 2)) + "}", text).PadRight(screenLength) ;
        }

        public static void WriteLineCenter(string text)
        {
            Console.WriteLine(AlignToCenter(text, defaultScreenSize));
        }
    }
}

