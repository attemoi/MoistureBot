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
        
            ConsoleKey[] FILTERED = { ConsoleKey.Escape, ConsoleKey.Tab };

            var pass = new Stack<char>();

            ConsoleKeyInfo keyInfo;
            while ((keyInfo = System.Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key.Equals(ConsoleKey.Backspace))
                {
                    if (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (!FILTERED.Contains(keyInfo.Key))
                {
                    pass.Push(keyInfo.KeyChar);
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

