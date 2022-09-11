using System;
namespace AutoBattle
{
    /// <summary>
    /// Helper class for reusable resources
    /// </summary>
    public static class Helper
    {
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
    }
}

