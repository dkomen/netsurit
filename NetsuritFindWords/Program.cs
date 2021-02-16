using System;
using System.Linq;
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
                        bool foundWord = DoSearch(ref characterBytes[testSetCounter], 0, randomTestSet, 0, new byte[MATRIX_SIZE], 0);
                        if (foundWord)
                        {
                            //We found a word so save the matrix
                            String wordFound = new String(characterBytes[testSetCounter].Characters.Select(b => (char)b).ToArray<char>());
                            wordsFound.Add((MatrixDataSet)randomTestSet.Clone(), wordFound);
                        }
                        randomTestSet.Reset(); 
                    }

                    //Finished searching for words
                    Console.WriteLine("Found {0} words", wordsFound.FoundMatrixes.Count);
                    Console.WriteLine("Print the last successfull matrix to 'results.html'");
                    wordsFound.Display(wordsFound.FoundMatrixes.Count, "results.html");
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
        static bool DoSearch(ref CharacterByteData wordToSearch, int currentWordCharIndex, MatrixDataSet currentRandomTestSet, int currentMatrixCharIndex, byte[] currentWordBeingBuilt, int currentNewWordCharIndex)
        {
           if(currentNewWordCharIndex <= wordToSearch.Length && currentMatrixCharIndex < MATRIX_SIZE)
            {
                if(currentRandomTestSet.RandomMatrix[currentMatrixCharIndex].AlreadyUsed)
                {
                    return DoSearch(ref wordToSearch, currentWordCharIndex, currentRandomTestSet, currentMatrixCharIndex+1, currentWordBeingBuilt, currentNewWordCharIndex);
                }
                else if (wordToSearch.Characters[currentWordCharIndex] != currentRandomTestSet.RandomMatrix[currentMatrixCharIndex].Character)
                {
                    int newMatrixCharIndex = currentMatrixCharIndex+1;
                    return DoSearch(ref wordToSearch, currentWordCharIndex, currentRandomTestSet, newMatrixCharIndex++, currentWordBeingBuilt, currentNewWordCharIndex);
                } 
                else
                { 
                    byte[] newCurrentWordBeingBuilt = currentWordBeingBuilt;
                    newCurrentWordBeingBuilt[currentNewWordCharIndex] = currentRandomTestSet.RandomMatrix[currentMatrixCharIndex].Character;
                    currentRandomTestSet.RandomMatrix[currentMatrixCharIndex].AlreadyUsed = true;
                    if (currentNewWordCharIndex < wordToSearch.Length && newCurrentWordBeingBuilt[0..(currentNewWordCharIndex+1)] != wordToSearch.Characters)
                    {
                        return DoSearch(ref wordToSearch, currentWordCharIndex + 1, currentRandomTestSet, 0, newCurrentWordBeingBuilt, currentNewWordCharIndex + 1);
                    } 
                    else
                    {
                        return Enumerable.SequenceEqual(currentWordBeingBuilt[0..(currentNewWordCharIndex + 1)],wordToSearch.Characters); //Check if we found the word
                    }
                }
            }
            else
            {
                return Enumerable.SequenceEqual(currentWordBeingBuilt[0..(currentNewWordCharIndex + 1)], wordToSearch.Characters); //Check if we found the word
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
    }
}
