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
        public Result<String> Display(String htmlFileName) //So we should also check that the index to display is a valid index and the html file is a valid path
        {
            Result<String> deleteResult = Files.DeleteFile(htmlFileName);
            if (deleteResult.IsOk)
            {
                System.Text.StringBuilder html = new System.Text.StringBuilder();
                for (int indexToDisplay = 0; indexToDisplay < this.FoundMatrixes.Count; indexToDisplay++)
                {
                    MatrixDataSet matrixToUse = this.FoundMatrixes[indexToDisplay].Item1;
                    String wordFound = this.FoundMatrixes[indexToDisplay].Item2;
                    
                    int currentIndex = 0;
                    html.Append("<div style='float: left; padding: 8px'><b>" + wordFound + "</b><br /><table style='color: blue; border: 1px solid #333'>");
                    for (int row = 0; row < Math.Sqrt(matrixToUse.RandomMatrix.Length); row++)//Math.Sqrt is assuming we are always working with a perfect square
                    {
                        html.Append("<tr>");
                        for (int column = 0; column < Math.Sqrt(matrixToUse.RandomMatrix.Length); column++)//Math.Sqrt is assuming we are always working with a perfect square
                        {
                            String color = "Green";
                            if (matrixToUse.RandomMatrix[currentIndex].AlreadyUsed == true)
                            {
                                color = "Red";
                            }

                            //html.Append("<td style='width: 12px; color: " + color + "'>" + (char)(matrixToUse.RandomMatrix[currentIndex].Character - 32) + "</td>");
                            html.Append("<td style='width: 12px; color: " + color + "'>" + (char)(matrixToUse.RandomMatrix[currentIndex].Character) + "</td>");
                            currentIndex += 1;
                        }
                        html.Append("</tr>");
                    }
                    html.Append("</table></div>");
                }
                return Files.SaveStringToFile(htmlFileName, html.ToString());
            }
            else
            {
                return deleteResult;
            }
        }
    }
}
