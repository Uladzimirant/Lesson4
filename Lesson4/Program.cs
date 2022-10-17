// See https://aka.ms/new-console-template for more information

using System.Collections.Specialized;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;

namespace Lesson4 {
    public class Program
    {
        static CMDHandler handler = new CMDHandler();
        static double[,]? matrix;


        //Asks for width and heigth, if only one received ask for other
        static private void AskDimentions(out int width, out int height)
        {
            string str;
            str = handler.AskForInput("Enter width and height:");
            string[] whStr = str.Split();
            try
            {
                width = Convert.ToInt32(whStr[0]);
                height = Convert.ToInt32(whStr.Length < 2 ? handler.AskForInput("Enter height:") : whStr[1]);
            }
            catch { throw new MessageException("Width and height must be positive numbers"); }
        }
        //matrix input
        static private void FillAskMatrix(double[,] matrix)
        {
            Console.WriteLine("Enter matrix line by line:");
            int spaces = 10; //amount of spaces with number len, input length should not exceed it
            Console.WriteLine();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Console.Write($"{i,-3}: ");
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    while (true)
                    {
                        (int x, int y) cursor = Console.GetCursorPosition();
                        var str = Console.ReadLine() ?? "";
                        CMDHandler.CheckExit(str);
                        //ReadLine inserts \n on enter, that will place it back
                        Console.SetCursorPosition(cursor.x + str.Length, cursor.y);
                        try
                        {
                            matrix[i, j] = Convert.ToDouble(str);
                            if (spaces > str.Length) Console.Write(new String(' ', spaces - str.Length));
                            break;
                        }
                        catch (FormatException)
                        {   //if not correct input, clear inputed number and ask again
                            Console.SetCursorPosition(cursor.x, cursor.y);
                            Console.Write(new string(' ', str.Length));
                            Console.SetCursorPosition(cursor.x, cursor.y);
                        }
                    }
                }
                Console.WriteLine();
            }
        }
        
        static void AskForMatrix()
        {
            AskDimentions(out int width, out int height);
            double[,] tempMatrix = new double[height, width];
            FillAskMatrix(tempMatrix);
            matrix = tempMatrix;
            Console.WriteLine("Matrix created");
        }
        static void PrintMatrix()
        {
            const int spaceW = 9, spaceH = -3;
            if (matrix == null) throw new MessageException("Matrix need to be created first");
            string leftSpace = new string(' ', Math.Abs(spaceH) + 1);
            //print index row
            Console.Write(leftSpace);
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write($"{j,spaceW} ");
            }
            Console.WriteLine();
            //print separating horizontal line
            Console.WriteLine(leftSpace + new string('_', (spaceW + 1) * matrix.GetLength(1)));
            //print matrix
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Console.Write($"{i,spaceH}|");//index with vertical separator
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write($"{matrix[i, j],spaceW} ");
                }
                Console.WriteLine();
            }
        }
        //count positive or negative elements of matrix
        private static int countSignRealization(double[,] m, bool positive)
        {
            int amount = 0;
            foreach (var elem in m) //because matrix is double[,] only one cycle
            {
                if (positive ? elem >= 0 : elem < 0) ++amount;
            }
            return amount;
        }
        private static void swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
        }
        private static void reverseLinesRealization(double[,] m)
        {
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0, k = m.GetLength(1) - 1; j < m.GetLength(1) / 2; j++, k--)
                {
                    swap(ref m[i, j], ref m[i, k]);
                }
            }
        }
        private static void sortLinesRealization(double[,] m, bool isAscending)
        {
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 1; j < m.GetLength(1); j++)
                {
                    for (int k = j; k > 0 && 
                        (isAscending ? m[i, k - 1] > m[i, k] : m[i, k - 1] < m[i, k]);
                        k--)
                    {
                        swap(ref m[i, k], ref m[i, k-1]);
                    }
                }
            }
        }
        //These methods call their realizations
        public static void SortLines(bool isAscending)
        {
            if (matrix == null) throw new MessageException("Matrix need to be created first");
            sortLinesRealization(matrix, isAscending);
            Console.WriteLine("Sorted");
        }
        public static void CountSign(bool positive)
        {
            if (matrix == null) throw new MessageException("Matrix need to be created first");
            Console.WriteLine("Amount of {0} numbers is {1}", positive ? "positive" : "negative", countSignRealization(matrix, positive));
        }
        public static void ReverseLines()
        {
            if (matrix == null) throw new MessageException("Matrix need to be created first");
            reverseLinesRealization(matrix);
            Console.WriteLine("Reversed");
        }
        public static void Main(string[] args)
        {
            //These are console commands that will execute functions
            handler.RegisterCommand("create", AskForMatrix, "Create new matrix");
            handler.RegisterCommand("print", PrintMatrix, "Print matrix");
            handler.RegisterCommand(new string[] { "count", "count positive" }, () => CountSign(true), "Prints amount of positive numbers");
            handler.RegisterCommand("count negative", () => CountSign(false), "Prints amount of negative numbers");
            handler.RegisterCommand(new string[] { "sort", "sort ascending" }, () => SortLines(true), "Sorts every line in ascending order");
            handler.RegisterCommand("sort descenting", () => SortLines(false), "Sorts every line in descenting order");
            handler.RegisterCommand("reverse", ReverseLines, "Reverses every line");

            handler.PrintHelp();
            handler.Run();
        }
    }
}

