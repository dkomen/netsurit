# Netsurit
Search for words in a random 5*5 2D matrix (but any reasonably sized square can be used). 

This is a stock-standard .Net Core 3.1 solution.

## Opening the solution

Simply open the NetsuritFindWords.sln in Visual Studio 2019

## Building the solution

Build with Ctrl+B (first selecting either Debug or Release mode)

## Running the app

After building the executable can be found at: [Solution Folder]\NetsuritFindWords\bin\[Debug\Release]\netcoreapp3.1\NetsuritFindWords.exe

In the [Solution Folder] you will find the words.txt file needed to extract words to search from. I would suggest copying this file into 
the same folder as the executable binary (NetsuritFindWords.exe) so that the command line argument descibed next is shorter.

From the command-line and in the directory [Solution Folder]\NetsuritFindWords\bin\[Debug\Release]\netcoreapp3.1 run NetsuritFindWords.exe including the path to the words.txt file.

The following example is assuming the words.txt file and the executable binary are in the same directory.

```NetsuritFindWords.exe words.txt```

After successfull execution a file named ```results.html``` will be created in your currently active directory (propably: [Solution Folder]\NetsuritFindWords\bin\[Debug\Release]\netcoreapp3.1) which you can open using your browser.

This html file contains the results of the last word found in the entire test... but all the results are stored in an array and can all be rendered to html if needed.

## Some coding features used

- Custom generic return ```Result``` type so as to minimise coding with exception outcomes and prevents later coding where checking for nulls would have been done
- Generic lists, standard arrays and Enumerables.
- LINQ for various mapping requirements
- Classes as well as structs (structs and primitive data types so as to try to remain on the stack and at least in L1-cache if possble)
- Custom Exception
- Recursion
- Basic text file manipulation (read, write, delete)
- Output last word found to simple html file
- Zero bytes of code was copy-pasted from anywhere
- Coded in a Windows 10 Pro virtual machine

***I did think of multi-threading the Recursion too but it was quick enough - perhaps I should have show have (PS:not sure the TPL would have been the fastest, would have benchmarked it first)***

***With an already quick speed additional threading could suffer due to thread context switches too***




