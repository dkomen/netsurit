using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsurit.Data
{
    public struct MatrixDataSet: ICloneable
    {
        private int _matrixRange;
        /// <summary>
        /// A random character matrix
        /// </summary>
        public MatrixCharacter[] RandomMatrix { get; private set; }

        /// <summary>
        /// Initialise by creating a new random matrix
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public MatrixDataSet(int matrixRange)
        {
            if (matrixRange < 1) // matrix size must a number greater than 1
            {
                throw new ApplicationException("The size of the matrix must be greater than 1");
            }

            _matrixRange = matrixRange;
            this.RandomMatrix = new MatrixCharacter[_matrixRange];
            System.Random rand = new Random();
            for (int index = 0; index < _matrixRange; index++)
            {
                this.RandomMatrix[index] = new MatrixCharacter((byte)rand.Next(97, 97 + 26)); //The range of lower-case ASCII alphabetic characters. It appears the input file only has lower-case characters.
            }
        }

        /// <summary>
        /// Reset the matrix to an unused state
        /// </summary>
        public void Reset()
        {
            for (int index = 0; index < _matrixRange; index++)
            {
                this.RandomMatrix[index].AlreadyUsed = false;
            }
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
