using System;
using System.Linq;
using System.Collections.Generic;
using Netsurit;
using Netsurit.Data;
using Netsurit.Results;

namespace NetsuritFindWords
{
    class Program
    {
        const int MATRIX_SIZE = 5 * 5;//I use a vector instead of a 2D array as access is simpler - converting it to 2D during the later results display is trivial.
        static void Main(string[] args)
        {
            if (args.Length == 1)//For production we would need to do more stringent command-line arguments validation
            {
                String wordsFilePath = args[0];
                Console.WriteLine("Read words file: " + wordsFilePath);
                Result<String> readResult = Files.ReadFileToString(wordsFilePath);
                if(readResult.IsOk) 
                {
                    String fileData = readResult.Unwrap();

                    Console.WriteLine("Create the array of words from the file data");
                    String[] wordsList = ExtractWordsArray(fileData);

                    Console.WriteLine("Convert the array to character byte arrays");
                    CharacterByteData[] characterBytes = ExtractCharacterByteDataFromArray(wordsList);

                    //Generate a random test set
                    MatrixDataSet randomTestSet = new MatrixDataSet(MATRIX_SIZE);

                    MatrixWordsFound wordsFound = new MatrixWordsFound();
                    Console.WriteLine("Started...");
                    for (int testSetCounter = 0; testSetCounter < characterBytes.Length; testSetCounter++)
                    {
                        //Start searching
                        (bool, MatrixDataSet) foundWord = DoSearch(ref characterBytes[testSetCounter], 0, randomTestSet, 0, new byte[MATRIX_SIZE], 0);
                        if (foundWord.Item1)
                        {
                            //We found a word so save the matrix
                            String wordFound = new String(characterBytes[testSetCounter].Characters.Select(b => (char)b).ToArray<char>());
                            wordsFound.Add((MatrixDataSet)foundWord.Item2.Clone(), wordFound);
                        }
                        randomTestSet.Reset();
                        }

                    //Finished searching for words
                    Console.WriteLine("Found {0} words", wordsFound.FoundMatrixes.Count);
                    Console.WriteLine("Print successfull matrices to 'results.html'");
                    wordsFound.Display("results.html");
                    Console.WriteLine("");
                    Console.WriteLine("Goodbye :o)");
                } else
                {
                    Console.WriteLine(readResult.ErroMessage);
                }
            } else
            {
                Console.WriteLine("No file path argument was supplied");
            }
        }

        /// <summary>
        /// Recursive word searches.
        /// </summary>
        /// <param name="wordToSearch">The current word being searched for. We must pass it by ref else a byvalue struct gets duplicated on each recursive method call</param>
        /// <param name="currentRandomTestSet">The current random matrix</param>
        /// <param name="currentCharIndex">The current character being compared</param>
        /// <param name="currentWordBeingBuilt">The successfully built word so far</param>
        /// <returns></returns>
        static (bool, MatrixDataSet) DoSearch(ref CharacterByteData wordToSearch, int currentWordCharIndex, MatrixDataSet currentRandomTestSet, int currentMatrixCharIndex, byte[] currentWordBeingBuilt, int currentNewWordCharIndex)
        {
           if(currentNewWordCharIndex <= wordToSearch.Length && currentMatrixCharIndex < MATRIX_SIZE)
            {
                if (wordToSearch.Characters[currentWordCharIndex] != currentRandomTestSet.RandomMatrix[currentMatrixCharIndex].Character)
                {
                    return DoSearch(ref wordToSearch, currentWordCharIndex, currentRandomTestSet, currentMatrixCharIndex+=1, currentWordBeingBuilt, currentNewWordCharIndex);
                } 
                else
                { 
                    //We have found a matching character...
                    byte[] newCurrentWordBeingBuilt = currentWordBeingBuilt;
                    newCurrentWordBeingBuilt[currentNewWordCharIndex] = currentRandomTestSet.RandomMatrix[currentMatrixCharIndex].Character;
                    currentRandomTestSet.RandomMatrix[currentMatrixCharIndex].AlreadyUsed = true;
                    currentRandomTestSet.RandomMatrix[currentMatrixCharIndex].OriginalIndex = currentMatrixCharIndex;
                    if (currentNewWordCharIndex < wordToSearch.Length && newCurrentWordBeingBuilt[0..(currentNewWordCharIndex+1)] != wordToSearch.Characters)
                    {
                        //...so lets search its neighbours
                        return TryFindWordInNeighbours(true, ref wordToSearch, currentWordCharIndex+1, currentRandomTestSet, currentMatrixCharIndex, currentRandomTestSet, currentMatrixCharIndex, newCurrentWordBeingBuilt, currentNewWordCharIndex + 1);
                    } 
                    else
                    {
                        return (Enumerable.SequenceEqual(currentWordBeingBuilt[0..(currentNewWordCharIndex + 1)],wordToSearch.Characters), currentRandomTestSet); //Check if we found the word
                    }
                }
            }
            else
            {
                return (Enumerable.SequenceEqual(currentWordBeingBuilt[0..(currentNewWordCharIndex + 1)], wordToSearch.Characters), currentRandomTestSet); //Check if we found the word
            }
        }

        static (bool, MatrixDataSet) TryFindWordInNeighbours(bool rebuildMatrix, ref CharacterByteData wordToSearch, int currentWordCharIndex, MatrixDataSet currentRandomTestSet, int currentMatrixCharIndex, MatrixDataSet currentNeighbourMatrix, int neighbourMatrixCharIndex, byte[] currentWordBeingBuilt, int currentNewWordCharIndex)
        {
            MatrixDataSet neighboursMatrix;
            if (rebuildMatrix)
            {
                (MatrixDataSet, int) matrixInfo = GetNeighbouringMatrix(ref currentRandomTestSet, neighbourMatrixCharIndex);
                neighboursMatrix = matrixInfo.Item1;
                neighbourMatrixCharIndex = matrixInfo.Item2;
                rebuildMatrix = false;
            }
            else
            {
                neighboursMatrix = currentNeighbourMatrix;
            }
            
            if(neighboursMatrix.CountOfUsedCharacters < wordToSearch.Length) //If the number of unused characters is less than the length of the word being searched for then a match is impossible...
            {
                return (false, new MatrixDataSet()); //...so exit
            } else
            {
                int nextMatrixCharIndex = neighboursMatrix.RandomMatrix[neighbourMatrixCharIndex].NextCharacterSearchIndex;
                if (neighbourMatrixCharIndex < MATRIX_SIZE && currentMatrixCharIndex < MATRIX_SIZE)
                {
                    if ((neighboursMatrix.RandomMatrix[neighbourMatrixCharIndex].AlreadyUsed )
                        || neighboursMatrix.RandomMatrix[neighbourMatrixCharIndex].Character != wordToSearch.Characters[currentWordCharIndex])
                    {
                        if (nextMatrixCharIndex == 0)
                        {
                            //According to the neighbours matrix we should have found this character but we didn't... so exit
                            return (false, new MatrixDataSet());
                        } else
                        {                            
                            //We didnt match on the character but we still have more to search
                            return TryFindWordInNeighbours(rebuildMatrix, ref wordToSearch, currentWordCharIndex, currentRandomTestSet, currentMatrixCharIndex+1, neighboursMatrix, nextMatrixCharIndex, currentWordBeingBuilt, currentNewWordCharIndex + 1);
                        }
                    }
                    else
                    {
                        //We have found a matching character
                        byte[] newCurrentWordBeingBuilt = currentWordBeingBuilt;
                        newCurrentWordBeingBuilt[currentWordCharIndex] = neighboursMatrix.RandomMatrix[neighbourMatrixCharIndex].Character;
                        bool wordAlreadyMatched = Enumerable.SequenceEqual(newCurrentWordBeingBuilt[0..(currentWordCharIndex+1)], wordToSearch.Characters);
                        currentRandomTestSet.RandomMatrix[neighboursMatrix.RandomMatrix[neighbourMatrixCharIndex].OriginalIndex].AlreadyUsed = true;                       
                        if (currentWordCharIndex < wordToSearch.Length && !wordAlreadyMatched)
                        {
                            return TryFindWordInNeighbours(true, ref wordToSearch, currentWordCharIndex + 1, currentRandomTestSet, 0, neighboursMatrix, neighbourMatrixCharIndex, newCurrentWordBeingBuilt, currentNewWordCharIndex + 1);
                        }
                        else
                        {
                            return (wordAlreadyMatched, currentRandomTestSet);
                        }
                    }
                }
                else
                {
                    return (false, new MatrixDataSet()); //Not found - MatrixDataSet should be nullable
                }
            }
        }

        /// <summary>
        /// Create a String array from the provided data.
        /// Note: I found that the individual lines are seperated by '\r\n'. We can't always assume this is the case and further checks would be wise.
        /// </summary>
        /// <param name="dataToSplit"></param>
        /// <returns></returns>
        static String[] ExtractWordsArray(String dataToSplit)
        {
            return dataToSplit.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Given an array of string return an array of CharacterData structs
        /// </summary>
        /// <param name="arrayToExtractFrom"></param>
        /// <returns></returns>
        static Netsurit.Data.CharacterByteData[] ExtractCharacterByteDataFromArray(String[] arrayToExtractFrom)
        {
            return arrayToExtractFrom.Select(
                x => new Netsurit.Data.CharacterByteData(x)
            ).ToArray<Netsurit.Data.CharacterByteData>();
        }

        /// <summary>
        /// Find all the neighbouring indexes in the matrix for our current index in the matrix
        /// </summary>
        /// <param name="currentRandomTestSet">The current matrix we are searching for words in</param>
        /// <param name="currentIndex">The current index in the matrix to calculate the related diagonal indexes for</param>
        /// <returns>The new MatrixDataset and also the start index to start searches from</returns>
        static (MatrixDataSet, int) GetNeighbouringMatrix(ref MatrixDataSet currentRandomTestSet, int currentIndex)
        {
            int matrixLengthIndexed = currentRandomTestSet.RandomMatrix.Length-1;
            int matrixRange = (int)Math.Sqrt(currentRandomTestSet.RandomMatrix.Length);

            //There can be a maximum of 8 neighbours to our currentIndex, lets calculate their indexes
            int currentRow = GetCurrentRow(currentIndex, matrixRange);
            int[] neighbours = new int[8];
            neighbours[0] = currentIndex - matrixRange - 1;//Top-left
            neighbours[1] = currentIndex - matrixRange;//Top
            neighbours[2] = currentIndex - matrixRange + 1;//top-right
            neighbours[3] = currentIndex - 1; //Left
            neighbours[4] = currentIndex + 1; //Right
            neighbours[5] = currentIndex + matrixRange - 1;//Bottom-left
            neighbours[6] = currentIndex + matrixRange;//Bottom
            neighbours[7] = currentIndex + matrixRange + 1;//Bottom-right

            //Now we remove the invalid indexes
            if (neighbours[0] < 0 || GetCurrentRow(neighbours[0], matrixRange) != currentRow - 1) { neighbours[0] = -1; };//Top-left must be in the previous row and in the matrix
            if (neighbours[1] < 0) { neighbours[1] = -1; };//Top must be in the in the matrix
            if (neighbours[2] < 0 || GetCurrentRow(neighbours[2], matrixRange) != currentRow - 1) { neighbours[2] = -1; };//Top-right must be in the previous row and in the matrix
            if (neighbours[3] < 0 || GetCurrentRow(neighbours[3], matrixRange) != currentRow) { neighbours[3] = -1; };//Left must be in the same row and in the matrix
            if (neighbours[4] > matrixLengthIndexed || GetCurrentRow(neighbours[4], matrixRange) != currentRow) { neighbours[4] = -1; };//Right must be in the same row and in the matrix
            if (neighbours[5] > matrixLengthIndexed || GetCurrentRow(neighbours[5], matrixRange) != currentRow + 1) { neighbours[5] = -1; };//Bottom-left must be in the next row and in the matrix
            if (neighbours[6] > matrixLengthIndexed) { neighbours[6] = -1; };//Bottom must be in the in the matrix
            if (neighbours[7] > matrixLengthIndexed || GetCurrentRow(neighbours[7], matrixRange) != currentRow + 1) { neighbours[7] = -1; };//Bottom-right must be in the next row and in the matrix

            int[] validNeighbours = neighbours.Where(x => x >= 0).ToArray<int>();

            MatrixDataSet neigboursMatrix = new MatrixDataSet(MATRIX_SIZE);
            neigboursMatrix.CreateFromNeighbours(validNeighbours, ref currentRandomTestSet);
            neigboursMatrix.RandomMatrix[currentIndex].AlreadyUsed = true;

            return (neigboursMatrix, validNeighbours[0]);
        }

        /// <summary>
        /// Get the current zero based matrix row number in which the currentIndex is
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="matrixRange">How wide is a row in the matrix</param>
        /// <returns></returns>
        static int GetCurrentRow(int currentIndex, int matrixRange)
        {
            return (int)Math.Floor(currentIndex / (double)matrixRange);
        }
    }
}
