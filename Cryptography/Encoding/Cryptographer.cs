using System;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Cryptography
{
    class Cryptographer
    {

        /// <summary>
        /// Encodes an inputted message using the modval and encoding matrix
        /// </summary>
        public string Encode(string message, Matrix<Double> encodingMatrix, int modVal, bool isLastDecode)
        {
            char[] chars = message.ToUpper().ToCharArray();
            int numRows = encodingMatrix.RowCount;
            int numCols = chars.Length / numRows;

            if (chars.Length % numRows != 0)
                Console.WriteLine("Error: Message length is not a multiple of your matrix dimension....");

            double[] numRepresentation = ConvertToNumbers(chars);

            //Multiply by encoding matrix
            Matrix<Double> msgMatrix = Matrix<Double>.Build.Dense(numRows, numCols, numRepresentation);
            Matrix<Double> resultMatrix = encodingMatrix.Multiply(msgMatrix);

            Console.WriteLine("\n\nThe Resulting number matrix is:");
            MatrixToString(encodingMatrix.ColumnCount, resultMatrix);

            //Mod each element and reconvert back to strings
            StringBuilder sb = new StringBuilder();
            foreach (var col in resultMatrix.ToColumnArrays())
            {
                foreach(var el in col)
                {
                    int valToFetch = (int)Math.Round(el) % modVal;
                    sb.Append(GetCharFromIndex(valToFetch));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Decodes a given message based on the original encoding matrix
        /// </summary>
        public string Decode(string message, Matrix<Double> encodingMatrix, int modVal, bool isLastDecode)
        {
            Matrix<Double> decodingMatrix = GetDecodingMatrix(encodingMatrix, modVal);

            return Encode(message, decodingMatrix, modVal, isLastDecode);
        }

        /// <summary>
        /// Makes sure there is a modular multiplicitive inverse matrix
        /// </summary>
        public bool IsValidEncodingMatrix(Matrix<Double> matrix, int modVal)
        {
            return getModInverse(matrix.Determinant(), modVal) != -1;
        }

        /// <summary>
        /// Converts a Char array to a number array
        /// </summary>
        private double[] ConvertToNumbers(char[] chars)
        {
            // Convert the message into a list of numerical equivalents
            double[] numRepresentation = new double[chars.Length];
            int charIndex = 0; //Makes sure the char is valid
            for (int i = 0; i < chars.Length; i++)
            {
                charIndex = GetIndexFromChar(chars[i]);
                if (charIndex < 0)
                    Console.WriteLine($"Error: the character {chars[i]} is invalid");

                numRepresentation[i] = charIndex;
            }

            return numRepresentation;
        }

        /// <summary>
        /// Finds the decoding matrix based off of the encoding matrix and original mod value
        /// </summary>
        private Matrix<Double> GetDecodingMatrix(Matrix<Double> encodingMatrix, int modVal)
        {
            ///
            /// See https://www.youtube.com/watch?v=JnIHfnaY-_w&t=539s for an explanation of this algorithm
            ///
            double det = encodingMatrix.Determinant();
            int modInverse = getModInverse(det, (double)modVal);
            int dimensions = encodingMatrix.RowCount;

            ////Make sure math is correct
            //Console.WriteLine("\n\nThe inverse matrix is:");
            //MatrixToString(dimensions, encodingMatrix.Inverse());
            //Matrix<Double> inverse = encodingMatrix.Inverse();
            //MatrixToString(dimensions, inverse);
            //Console.WriteLine("\n\nThe inverse matrix after multiplication by determinate:");
            //MatrixToString(dimensions, encodingMatrix.Inverse().Multiply(det));
            //Console.WriteLine("\n\nThe inverse matrix after Modulus:");
            //MatrixToString(dimensions, encodingMatrix.Inverse().Multiply(det).Modulus(modVal));
            //Console.WriteLine("\n\nThe inverse matrix after multiplication by Modular multiplicative inverse:");
            //MatrixToString(dimensions, encodingMatrix.Inverse().Multiply(det).Modulus(modVal).Multiply(modInverse));
            Console.WriteLine("\n\nThe Decoding Matrix is:");
            MatrixToString(dimensions, encodingMatrix.Inverse().Multiply(det).Modulus(modVal).Multiply(modInverse).Modulus(modVal));

            Matrix<Double> decodingMatrix = encodingMatrix.Inverse().Multiply(det).Modulus(modVal).Multiply(modInverse).Modulus(modVal);
            return decodingMatrix;
        }

        /// <summary>
        /// Calculates Modular Multiplicitive inverse, or returns -1 if there isn't an inverse
        /// </summary>
        private int getModInverse(double startVal, double modVal)
        {
            startVal =  Math.Round(startVal);
            startVal = MathMod((int)startVal, (int)modVal);
            for (int x = 1; x < modVal; x++)
                if ((startVal * x) % modVal == 1)
                    return x;
            return -1; //inverse doesn't exist
        }

        /// <summary>
        /// Finds the modulo of any given val
        /// </summary>
        private int MathMod(int val, int mod)
        {
            return (Math.Abs(val * mod) + val) % mod;
        }

        /// <summary>
        /// Converts a char into a numerical equivalent
        /// </summary>
        private int GetIndexFromChar(char c)
        {
            switch (c)
            {
                case 'A': return 0;
                case 'B': return 1;
                case 'C': return 2;
                case 'D': return 3;
                case 'E': return 4;
                case 'F': return 5;
                case 'G': return 6;
                case 'H': return 7;
                case 'I': return 8;
                case 'J': return 9;
                case 'K': return 10;
                case 'L': return 11;
                case 'M': return 12;
                case 'N': return 13;
                case 'O': return 14;
                case 'P': return 15;
                case 'Q': return 16;
                case 'R': return 17;
                case 'S': return 18;
                case 'T': return 19;
                case 'U': return 20;
                case 'V': return 21;
                case 'W': return 22;
                case 'X': return 23;
                case 'Y': return 24;
                case 'Z': return 25;
                case ' ': return 26;
                case '.': return 27;
                case '!': return 28;
                default: return -1;
            }
        }

        /// <summary>
        /// Converts a number into its char equivalent
        /// </summary>
        private char GetCharFromIndex(int i)
        {
            char[] charList = {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', ' ', '.', '!'
        };
            return charList[i];
        }

        /// <summary>
        /// Prints a representation of a matrix to the console
        /// </summary>
        private static void MatrixToString(int dimensions, Matrix<Double> matrix)
        {
            Console.WriteLine("[");
            foreach (var row in matrix.ToRowArrays())
            {
                Console.Write("  [");
                foreach (var el in row)
                    Console.Write(" " + el + " ");

                Console.WriteLine("]");
            }
            Console.WriteLine("]");
        }
    }
}
