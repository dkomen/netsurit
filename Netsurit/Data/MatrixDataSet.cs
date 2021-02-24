using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsurit.Data
{
    public struct MatrixDataSet: ICloneable
    {
        private int _matrixLength;
        /// <summary>
        /// A random character matrix
        /// </summary>
        public MatrixCharacter[] RandomMatrix { get; private set; }

        public int CountOfUsedCharacters { get; private set; }

        /// <summary>
        /// Initialise by creating a new random matrix
        /// </summary>
        /// <param name="matrixLength">The Math.Sqrt of the matrix length</param>
        public MatrixDataSet(int matrixLength)
        {
            if (matrixLength < 1) // matrix size must a number greater than 1
            {
                throw new ApplicationException("The size of the matrix must be greater than 0");
            }

            _matrixLength = matrixLength;
            this.CountOfUsedCharacters = 0; //All characters are currently unused
            this.RandomMatrix = new MatrixCharacter[_matrixLength];
            System.Random rand = new Random();
            for (int index = 0; index < _matrixLength; index++)
            {
                this.RandomMatrix[index] = new MatrixCharacter((byte)rand.Next(97, 97 + 26), 0, index); //The range of lower-case ASCII alphabetic characters. It appears the input file only has lower-case characters.
            }
        }

        public MatrixDataSet(MatrixDataSet fromMatrix)
        {
            _matrixLength = fromMatrix.RandomMatrix.Length;
            this.CountOfUsedCharacters = 0; //All characters are currently unused
            this = (MatrixDataSet)fromMatrix.Clone();
        }
        /// <summary>
        /// Reset the matrix to an unused state
        /// </summary>
        public void Reset()
        {
            for (int index = 0; index < _matrixLength; index++)
            {
                this.RandomMatrix[index].AlreadyUsed = false;
            }
        }

        /// <summary>
        /// Set only the neighbours to useable
        /// </summary>
        /// <param name="neighbours"></param>
        public void CreateFromNeighbours(int[] neighbours, ref MatrixDataSet currentRandomTestSet)
        {
            this = new MatrixDataSet(currentRandomTestSet);

            for (int currentNeigbourIndex = 0; currentNeigbourIndex < neighbours.Length; currentNeigbourIndex++)
            {
                int currentIndex = neighbours[currentNeigbourIndex];
                //this.RandomMatrix[currentIndex].AlreadyUsed = false;
                if (currentNeigbourIndex == neighbours.Length - 1)
                {
                    this.RandomMatrix[currentIndex].NextCharacterSearchIndex = 0;
                }
                else
                {
                    this.RandomMatrix[currentIndex].NextCharacterSearchIndex = neighbours[currentNeigbourIndex + 1];
                }
            }

            this.CountOfUsedCharacters = neighbours.Length - 1;
        }

        /// <summary>
        /// Clone this struct
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            MatrixDataSet clonedDataSet = new MatrixDataSet(this.RandomMatrix.Length);
            for(int index = 0; index < this.RandomMatrix.Length; index++)
            {
                clonedDataSet.RandomMatrix[index] = (MatrixCharacter)this.RandomMatrix[index].Clone();
            }

            return clonedDataSet;
        }
    }
}
