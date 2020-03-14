﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TestVeeam
{
    public class ProgressBar
    {
        private static readonly int forPercent = 100;
        public static void drawTextProgressBar(int progress, int total)
        {
            StartWrite();
            float onechunk = 30.0f / total;
            var isEnd = (progress / (total / forPercent)) == forPercent;
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)//draw filled part
            {
                WriteToConsole(ConsoleColor.Green, position++, " ");
            }
            for (int i = position; i < 31; i++)//draw unfilled part
            {
                WriteToConsole(ConsoleColor.Gray, position++, " ");
            }
            WriteToConsole(ConsoleColor.Black, 35, progress.ToString() + " of " + total.ToString() + "    ");//draw totals    
            if (isEnd)
            {
                Console.WriteLine("\n\nProcess is done");
            }
        }

        private static void StartWrite() 
        {
            Console.CursorVisible = false;
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
        }
        private static void WriteToConsole(ConsoleColor color,int cursorLeft, string stringToWrite)
        {
            Console.BackgroundColor = color;
            Console.CursorLeft = cursorLeft;
            Console.Write(stringToWrite);
        }
    }
}
