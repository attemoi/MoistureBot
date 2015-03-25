using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Options;

namespace MoistureBot.Utils
{
    public class ConsoleHelper
    {
        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <param name="mask">a <c>char</c> representing your choice of console mask</param>
        /// <returns>the string the user typed in </returns>

        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <param name="mask">a <c>char</c> representing your choice of console mask</param>
        /// <returns>the string the user typed in </returns>
        public static string ReadPassword(char mask)
        {

            ConsoleKey[] FILTERED = { ConsoleKey.Escape, ConsoleKey.Tab }; // const

            var pass = new Stack<char>();
            ConsoleKeyInfo key;
            while ((key = System.Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key.Equals(ConsoleKey.Backspace))
                {
                    if (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (FILTERED.Count(x => key.Key == x) > 0)
                {
                }
                else
                {
                    pass.Push(key.KeyChar);
                    System.Console.Write(mask);
                }
            }

            System.Console.WriteLine();

            return new string(pass.Reverse().ToArray());
        }

        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <returns>the string the user typed in </returns>
        public static string ReadPassword()
        {
            return ReadPassword('\0');
        }
    }
}

