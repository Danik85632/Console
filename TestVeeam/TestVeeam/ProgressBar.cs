using System;
using System.Collections.Generic;
using System.Text;

namespace TestVeeam
{
    public class ProgressBar
    {
        public ProgressBar(int totalCount) 
        {
            TotalCount = totalCount;
        }
        private bool IsDead = false;
        private int TotalCount;
        private readonly int ForPercent = 100;
        public void drawTextProgressBar(int currentProgress)
        {
            StartWrite();
            float onechunk = 30.0f / TotalCount;
            var isEnd = (currentProgress / (TotalCount / ForPercent)) == ForPercent;
            int position = 1;
            for (int i = 0; i < onechunk * currentProgress; i++)//draw filled part
            {
                WriteToConsole(ConsoleColor.Green, position++, " ");
            }
            for (int i = position; i < 31; i++)//draw unfilled part
            {
                WriteToConsole(ConsoleColor.Gray, position++, " ");
            }
            WriteToConsole(ConsoleColor.Black, 35, currentProgress.ToString() + " of " + TotalCount.ToString() + "    ");//draw totals    
            if (isEnd)
            {
                Console.WriteLine("\n\nProcess is done");
            }
        }
        public void StopProgessBarAndWriteConsole(ConsoleColor color, string stringToWrite) 
        {
            IsDead = true;
            Console.SetCursorPosition(0,1);
            Console.BackgroundColor = color;
            Console.WriteLine(stringToWrite);
        }
        private void StartWrite() 
        {
            if (!IsDead)
            {
                Console.CursorVisible = false;
                //draw empty progress bar
                Console.CursorLeft = 0;
                Console.Write("["); //start
                Console.CursorLeft = 32;
                Console.Write("]"); //end
                Console.CursorLeft = 1;
            }           
        }
        private void WriteToConsole(ConsoleColor color,int cursorLeft, string stringToWrite)
        {
            if (!IsDead)
            {
                Console.BackgroundColor = color;
                Console.CursorLeft = cursorLeft;
                Console.Write(stringToWrite);
            }
        }
    }
}
