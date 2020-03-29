using System;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Cryptography
{
    class Cryptographer
    {
        public string Encode(string message, Matrix<Double> m, int modVal, bool isLastDecode)
        {
            char[] chars = message.ToUpper().ToCharArray();
            int numRows = m.RowCount;
            int numCols = chars.Length / numRows;
            if (chars.Length % numRows != 0) 
                Console.WriteLine("Error: Message length is not a multiple of your matrix dimension....");

            double[] numRepresentation = new double[chars.Length]; 
            int currVal = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                currVal = GetIndexFromChar(chars[i]);
                if (currVal < 0)
                    Console.WriteLine($"Error: the character {chars[i]} is invalid");

                numRepresentation[i] = currVal;
            }

            Matrix<Double> toMultiply = Matrix<Double>.Build.Dense(numCols, numRows, numRepresentation).Transpose();
            Matrix<Double> result = m.Multiply(toMultiply);

            StringBuilder sb = new StringBuilder();
            if (isLastDecode)
                modVal = 29;

            foreach (int el in result.ToArray())
            {
                int valToFetch = el % modVal;
                sb.Append(GetCharFromIndex(valToFetch));
            }

            return sb.ToString();
        }

        public string Decode(string message, Matrix<Double> m, int modVal, bool isLastDecode)
        {

            double det = m.Determinant();
            int modInverse = getModInverse(det, (double)modVal);
            Matrix<Double> decodingMatrix = m.Inverse().Multiply(det).Modulus(modVal).Multiply(modInverse);

            return Encode(message, decodingMatrix, modVal, isLastDecode);
        }

        private int getModInverse(double startVal, double modVal)
        {
            startVal = startVal % modVal;
            for (int x = 1; x < modVal; x++)
                if ((startVal * x) % modVal == 1)
                    return x;
            return 1;
        }

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
    }
}
