using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cryptography
{
    class RunCryptographer
    {
        public static Cryptographer cryptographer = new Cryptographer();


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to your Cryptography generator.");
            int dimensions = StartPrompt();

            Matrix<Double> encodingMatrix = PromptEncodingMatrix(dimensions);

            Console.WriteLine("Next, Type a string you want to encode.");
            Console.WriteLine($"**Note: The number of Characters in your string must be divisible by {dimensions}");
            string msgToEncode = PromptStringToEncode(dimensions);

            Console.WriteLine("Almost Done! Next, just put in the number that you want to mod each element by: ");
            int modVal = GetModVal();

            
            string encodedMsg = cryptographer.Encode(msgToEncode, encodingMatrix, modVal, false);
            Console.WriteLine($"\n\nYour encoded message is: \n {encodedMsg}");

            var encodingMatrices = new Stack<Matrix<Double>>();
            encodingMatrices.Push(encodingMatrix);
            var modVals = new Stack<int>();
            modVals.Push(modVal);

            FurtherActions(encodingMatrices, encodedMsg, modVals, dimensions);
           
        }

        private static void FurtherActions(Stack<Matrix<double>> encodingMatrices, string encodedMsg, Stack<int> modVals, int dimensions)
        {
            bool canDecode = encodingMatrices.Count > 0;

            string options = canDecode ? $"encode/decode/exit" : "encode/decode" ;
            Console.WriteLine("What would you like to do with this message? (encode/decode/exit)");
            string nextAction = GetNextAction(canDecode);
            
            if (nextAction.Equals("exit"))
                return;

            if (nextAction.Equals("encode"))
            {
                Matrix<Double> newEncodingMatrix = PromptEncodingMatrix(dimensions);
                encodingMatrices.Push(newEncodingMatrix);
                
                Console.WriteLine("Almost Done! Next, just put in the number that you want to mod each element by: ");
                int modVal = GetModVal();
                modVals.Push(modVal);


                string newEncodedMsg = cryptographer.Encode(encodedMsg, newEncodingMatrix, modVal, false);
                Console.WriteLine($"\n\nYour encoded message is: \n {newEncodedMsg}");
                FurtherActions(encodingMatrices, newEncodedMsg, modVals, dimensions);
            }
            else //Decode
            {
                Matrix<Double> lastEncodingMatrix = encodingMatrices.Pop();
                bool isLastDecode = encodingMatrices.Count < 1;
                string revertedMsg = cryptographer.Decode(encodedMsg, lastEncodingMatrix, modVals.Pop(), isLastDecode);

                Console.WriteLine($"\n\nYour decoded message is: \n {revertedMsg}");
                FurtherActions(encodingMatrices, revertedMsg, modVals, dimensions);
            }

        }

        private static string GetNextAction(bool canDecode)
        {
            string input = Console.ReadLine();
            string decodeOpt = canDecode ? ", \"decode\"," : "";
            switch (input)
            {
                case "encode":
                case "exit":
                    return input;
                case "decode":
                    if (canDecode)
                        return input;
                    else
                    {
                        Console.WriteLine("Sorry! You've already decoded to your original message! Please type \"encode\" or \"exit\"");
                        return GetNextAction(canDecode);
                    }
                default:
                    Console.WriteLine($"Sorry, \"{input}\" is not a valid input. Please type \"encode\"{decodeOpt} or \"exit\"");
                    return GetNextAction(canDecode);
            }
        }

        private static int StartPrompt()
        {
            Console.WriteLine("To begin, type the width of your square encoding matrix (i.e. 2)");

            string inputtedMsg = Console.ReadLine();
            int dimensions;
            if (!int.TryParse(inputtedMsg, out dimensions))
            {
                Console.WriteLine($"Error: {inputtedMsg} is not a valid number....");
                return StartPrompt();
            }
            return dimensions;
        }

        private static Matrix<Double> PromptEncodingMatrix(int dimensions)
        {
            Console.WriteLine($"Okay! Next, input your {dimensions}x{dimensions} encoding matrix. " +
                "Type each row, seperating each number with a blankspace, then hit enter. ");
            Console.WriteLine("For example, for the first row of a 2x2 matrix, type \"1 2\" and press Enter");

            double[] matrix = new double[dimensions * dimensions];
            int[] tempArr = new int[dimensions];
            for (int i = 0; i < dimensions; i++)
            {
                tempArr = GetRowInput(dimensions);
                for (int j = 0; j < dimensions; j++)
                    matrix[i*dimensions + j] = tempArr[j];
            }

            Console.WriteLine("\n\n\nGreat! Your encoding matrix is: ");
            MatrixToString(dimensions, matrix);
            Matrix<Double> encodingMatrix = Matrix<Double>.Build.Dense(dimensions, dimensions, matrix).Transpose();

            return encodingMatrix;
        }

        private static string PromptStringToEncode(int dimensions)
        {
            string MsgToEncode = Console.ReadLine();
            if(MsgToEncode.Length % dimensions != 0)
            {
                Console.WriteLine($"Error: The number of characters in your message isn't divisible by {dimensions}. Try again:");
                return PromptStringToEncode(dimensions);
            }

            return MsgToEncode;
        }

       


        #region Helper Methods
        private static int[] GetRowInput(int dimensions)
        {
            string input = Console.ReadLine();
            string[] arr = input.Trim().Split(' ');
            if(arr.Length != dimensions)
            {
                Console.WriteLine($"Invalid format. Your encoding matrix must have {dimensions} elements per row");
                return GetRowInput(dimensions);
            }

            int[] rowNums = new int[dimensions];
            int currColIndex = 0;
            foreach(var el in arr)
            {
                int val;
                if(!int.TryParse(el.Trim(), out val))
                {
                    Console.WriteLine("Invalid format. Try inputting the row again. Only separate numbers by \",\"");
                    return GetRowInput(dimensions);
                }

                rowNums[currColIndex++] = val;            
               
            }

            Console.WriteLine($"Inputted {rowNums.ToString()}");

            return rowNums;
        }

        private static int GetModVal()
        {
            string input = Console.ReadLine();
            int mod;
            if (!int.TryParse(input, out mod))
            {
                Console.WriteLine("Hmmm. That doesn't seem to be a number. Try again:");
                return GetModVal();
            }

            return mod;
        }

        private static void MatrixToString(int dimensions, double[] matrix)
        {
            Console.WriteLine("[");
            for (int i = 0; i < dimensions; i++)
            {
                Console.Write("  [");
                for (int j = 0; j < dimensions; j++)
                    Console.Write(matrix[i * dimensions + j] + "  ");
                Console.WriteLine("]");
            }
            Console.WriteLine("]");
        }
        #endregion
    }
}
