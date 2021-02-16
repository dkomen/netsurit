using System;
using System.Collections.Generic;
using System.Linq;
using Netsurit.Results;
using Netsurit;

namespace Netsurit.Data
{
    /// <summary>
    /// A collection matrixes in which words were found
    /// </summary>
    public class MatrixWordsFound
    {
        /// <summary>
        /// The found matrixes
        /// </summary>
        public List<(MatrixDataSet, String)> FoundMatrixes { get; private set; }

        public MatrixWordsFound()
        {
            this.FoundMatrixes = new List<(MatrixDataSet, String)>();
        }

        /// <summary>
        /// Add a new matrix to the list
        /// </summary>
        /// <param name="newMatrix"></param>
        public void Add(MatrixDataSet newMatrix, String wordFound)
        {
            this.FoundMatrixes.Add((newMatrix, wordFound));
        }

        /// <summary>
        /// Print a specific matrix to a file in simple html formatting
        /// </summary>
        /// <param name="indexToDisplay"></param>
        public Result<String> Display(int indexToDisplay, String htmlFileName) //So we should also check that the index to display is a valid index and the html file is a valid path
        {
            int indexToDisplayWithOffset = indexToDisplay-1;
            MatrixDataSet matrixToUse = this.FoundMatrixes[indexToDisplayWithOffset].Item1;
            String wordFound = this.FoundMatrixes[indexToDisplayWithOffset].Item2;

            Result<String> deleteResult = Files.DeleteFile(htmlFileName);
            if (deleteResult.IsOk)
            {
                int currentIndex = 0;
                System.Text.StringBuilder html = new System.Text.StringBuilder();
                html.Append("Word: " + wordFound + "<br /><table style='color: blue'>");
                for (int row = 0; row < Math.Sqrt(matrixToUse.RandomMatrix.Length); row++)//Math.Sqrt is assuming we are always working with a perfect square
                {
                    html.Append("<tr>");
                    for (int column = 0; column < Math.Sqrt(matrixToUse.RandomMatrix.Length); column++)//Math.Sqrt is assuming we are always working with a perfect square
                    {
                        String color = "Green";
                        if (matrixToUse.RandomMatrix[currentIndex].AlreadyUsed)
                        {
                            color = "Red";
                        }

                        html.Append("<td style='width: 12px; color: " + color + "'>" + (char)(matrixToUse.RandomMatrix[currentIndex].Character-32) + "</td>");
                        currentIndex += 1;
                    }
                    html.Append("</tr>");
                }
                html.Append("</table>");

                return Files.SaveStringToFile(htmlFileName, html.ToString());
            }
            else
            {
                return deleteResult;
            }
        }
    }
}
