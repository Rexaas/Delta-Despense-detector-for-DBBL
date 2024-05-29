using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // Get the current directory
        string currentDirectory = Directory.GetCurrentDirectory();

        // Get all text files in the current directory
        string[] files = Directory.GetFiles(currentDirectory, "*.*");
        // Create a new text file named "output.txt" in the same directory
        string outputFilePath = Path.Combine(currentDirectory, "DeltaDispenseReport.txt");
        int count = 0;
        string MId=" ";
        // Iterate through each text file
        foreach (string file in files)
        {
            int flag = 0;
            // Read all lines from the current text file
            string[] lines = File.ReadAllLines(file);
            string fileName = System.IO.Path.GetFileName(file);
            // Iterate through each line
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("XXXXXXXXXXXXXXX") && flag==0)
                {
                    MId = lines[i].Substring(23, 4);
                    flag = 1;
                }
                    // Check if the line contains "NOTES DISPENSED (DELTA)"
                    if (lines[i].Contains("NOTES DISPENSED (DELTA)"))
                {
                    // Extract the integers from the next two lines
                    int[] extractedIntegers1 = ExtractIntegers(lines[i + 1]);
                    int[] extractedIntegers2 = ExtractIntegers(lines[i + 2]);
                    Console.WriteLine(extractedIntegers1);
                    Console.WriteLine(extractedIntegers2);

                    // Check if all extracted integers are 0
                    bool allZero1 = CheckAllZero(extractedIntegers1);
                    bool allZero2 = CheckAllZero(extractedIntegers2);

                    if (!allZero1 || !allZero2)
                    {
                        count++;
                        //string extractedSubstring = fileName.Substring(2, 6);
                        //string machineId = "Terminal ID: " + extractedSubstring;
                        string machineId = "Terminal ID: " + MId;
                        // Copy lines from 3 lines above to 2 lines below "NOTES DISPENSED (DELTA)"
                        int startIndex = Math.Max(0, i - 3);
                        int endIndex = Math.Min(lines.Length - 1, i + 2);


                        // Write the selected lines to the output file
                        using (StreamWriter writer = new StreamWriter(outputFilePath, true))
                        {
                            writer.WriteLine(machineId);
                            for (int j = startIndex; j <= endIndex; j++)
                            {
                                writer.WriteLine(lines[j]);
                            }
                            writer.WriteLine("\n");
                        }

                        // Exit the loop since we found and processed "NOTES DISPENSED (DELTA)"
                        break;
                    }
                }
            }
        }
        string newText = "ToTal Number of Delta Dispense: " + count + "\n\n";
        if (File.Exists(outputFilePath))
        {
            string existingContent = File.ReadAllText(outputFilePath);
            string combinedText = newText + existingContent + "\n\n-Rexaas";
            File.WriteAllText(outputFilePath, combinedText);
        }
        else
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath, true))
            {
                writer.WriteLine(newText);
            }
        }
        

        Console.WriteLine("Process complete.");
    }
        // Method to extract integers from the next two lines
        static int[] ExtractIntegers(string input)
    {
        // Use regular expression to match integers in the input string
        MatchCollection matches = Regex.Matches(input, @"\d+");

        // Initialize an array to store the integers
        int[] integers = new int[matches.Count];

        // Convert each matched string to an integer and store it in the array
        for (int i = 0; i < matches.Count; i++)
        {
            integers[i] = int.Parse(matches[i].Value);
        }

        return integers;
    }

    // Method to check if all extracted integers are 0
    static bool CheckAllZero(int[] numbers)
    {
        if (numbers[1] != 0 || numbers[3] != 0)
        {
            return false;
        }
        return true;
    }
}

