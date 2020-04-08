using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace Cryptography
{
    class RunCryptographer
    {
        public static Cryptographer cryptographer = new Cryptographer();


        static void Main(string[] args)
        {
            RunProgramFromBeginning();
        }

        private static void RunProgramFromBeginning()
        {
            Console.WriteLine("Welcome to your Cryptography generator.\n");
            int dimensions = PromptDimensions();

            Console.WriteLine("\n\nNext, just put in the number that you want to mod each element in your matrix by: ");
            int modVal = PromptMod();

            Matrix<Double> encodingMatrix = PromptEncodingMatrix(dimensions, modVal);

            Console.WriteLine("\n\nNext, Type a string you want to encode.");
            Console.WriteLine($"**Note: The number of Characters in your string must be divisible by {dimensions}");
            string msgToEncode = PromptStringToEncode(dimensions);

            string encodedMsg = cryptographer.Encode(msgToEncode, encodingMatrix, modVal, false);
            Console.WriteLine($"\n\nYour encoded message is: \n {encodedMsg}");

            //Set up stacks for further encryption
            var encodingMatrices = new Stack<Matrix<Double>>();
            encodingMatrices.Push(encodingMatrix);
            var modVals = new Stack<int>();
            modVals.Push(modVal);

            FurtherActions(encodingMatrices, encodedMsg, modVals, dimensions);
        }

        /// <summary>
        /// Get the width of a nxn matrix
        /// </summary>
        private static int PromptDimensions()
        {
            Console.WriteLine("To begin, type the desired width of your square encoding matrix (i.e. 2)");

            string inputtedMsg = Console.ReadLine();

            int dimensions;
            if (!int.TryParse(inputtedMsg, out dimensions))
            {
                Console.WriteLine($"Error: {inputtedMsg} is not a valid number....");
                return PromptDimensions();
            }
            return dimensions;
        }

        /// <summary>
        /// Ask user to input modVal
        /// </summary>
        private static int PromptMod()
        {
            string input = Console.ReadLine();
            int mod;
            if (!int.TryParse(input, out mod))
            {
                Console.WriteLine("Hmmm. That doesn't seem to be a number. Try again:");
                return PromptMod();
            }

            return mod;
        }

        /// <summary>
        /// Asks user to input each row of the nxn matrix. Checks to make sure
        /// inputted matrix is invertible with the modVal.
        /// </summary>
        private static Matrix<Double> PromptEncodingMatrix(int dimensions, int modVal)
        {
            Console.WriteLine($"\n\nInput your {dimensions}x{dimensions} encoding matrix. " +
                "Type each row, seperating each number with a blankspace, then hit enter. ");
            Console.WriteLine("For example, for the first row of a 2x2 matrix, type \"1 2\" and press Enter");

            //Set each row of encoding matrix
            Matrix<Double> encodingMatrix = Matrix<Double>.Build.Dense(dimensions, dimensions);
            for (int rowIndex = 0; rowIndex < dimensions; rowIndex++)
                encodingMatrix.SetRow(rowIndex, GetRowInput(dimensions));

            //Check is valid matrix
            if (!cryptographer.IsValidEncodingMatrix(encodingMatrix, modVal))
            {
                Console.WriteLine("\nWhoops! It looks like your inputted matrix wouldn't be able to invert modularly. Try again");
                return PromptEncodingMatrix(dimensions, modVal);
            }
            Console.WriteLine("\n\n\nGreat! Your encoding matrix is: ");
            MatrixToString(dimensions, encodingMatrix);

            return encodingMatrix;
        }

        /// <summary>
        /// Asks user to input the message they want encrypted. Number of characters
        /// must be divisible by dimension of encoding matrix.
        /// </summary>
        private static string PromptStringToEncode(int dimensions)
        {
            string MsgToEncode = Console.ReadLine();
            if (MsgToEncode.Length % dimensions != 0)
            {
                Console.WriteLine($"Error: The number of characters in your message isn't divisible by {dimensions}. Try again:");
                return PromptStringToEncode(dimensions);
            }

            return MsgToEncode;
        }

        /// <summary>
        /// Allows user to encode, decode, restart, or exit the program
        /// </summary>
        private static void FurtherActions(Stack<Matrix<double>> encodingMatrices, string encodedMsg, Stack<int> modVals, int dimensions)
        {
            bool canDecode = encodingMatrices.Count > 0;

            string options = canDecode ? $"encode/decode/restart/exit" : "encode/restart/exit" ;
            Console.WriteLine($"What would you like to do with this message? {options}");
            string nextAction = PromptNextAction(canDecode);

            if (nextAction.Equals("exit"))
                return;

            else if (nextAction.Equals("restart"))
                RunProgramFromBeginning();

            else if (nextAction.Equals("encode"))
            {
                Console.WriteLine("Okay! First, just put in the number that you want to mod each element by: ");
                int modVal = PromptMod();
                Matrix<Double> newEncodingMatrix = PromptEncodingMatrix(dimensions, modVal);

                encodingMatrices.Push(newEncodingMatrix);
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

        private static string PromptNextAction(bool canDecode)
        {
            string input = Console.ReadLine().Trim().ToLower();
            string decodeOpt = canDecode ? ", \"decode\"" : "";
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
                        Console.WriteLine("Sorry! You've already decoded to your original message! Please type \"encode\", \"restart\", or \"exit\"");
                        return PromptNextAction(canDecode);
                    }
                default:
                    Console.WriteLine($"Sorry, \"{input}\" is not a valid input. Please type \"encode\"{decodeOpt}, \"restart\", or \"exit\"");
                    return PromptNextAction(canDecode);
            }
        }

        #region Helper Methods
        private static double[] GetRowInput(int dimensions)
        {
            string input = Console.ReadLine();
            string[] arr = input.Trim().Split(' ');
            if(arr.Length != dimensions)
            {
                Console.WriteLine($"Invalid format. Your encoding matrix must have {dimensions} elements per row");
                return GetRowInput(dimensions);
            }

            double[] rowNums = new double[dimensions];
            int currColIndex = 0;
            foreach(var el in arr)
            {
                double val;
                if(!double.TryParse(el.Trim(), out val))
                {
                    Console.WriteLine("Invalid format. Try inputting the row again. Only separate numbers by \",\"");
                    return GetRowInput(dimensions);
                }

                rowNums[currColIndex++] = val;
            }

            return rowNums;
        }

        private static void MatrixToString(int dimensions, Matrix<Double> matrix)
        {
            Console.WriteLine("[");
            foreach(var row in matrix.ToRowArrays())
            {
                Console.Write("  [");
                foreach (var el in row)
                    Console.Write(" " + el + " ");

                Console.WriteLine("]");
            }
            Console.WriteLine("]");
        }
        #endregion
    }
}
