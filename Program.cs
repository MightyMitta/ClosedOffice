using System.Text;
using ClosedOffice.Models;

// Create a temporary text file and open it
string tempFilePath = Path.GetTempFileName();
File.WriteAllText(tempFilePath, " ");

Console.WriteLine($"Temporary file created at: {tempFilePath}");

while (true)
{
    // Use the temporary file instead of "./empty.txt"
    TextFile textFile = new(tempFilePath);
    textFile.Open();
}

void CreateFile()
{
    Console.Clear();
    Console.CursorVisible = true;
    string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    Console.Write("Enter the name for your new txt file: ");
    StringBuilder sb = new StringBuilder();
    string fileName = "";
    
    while (true)
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        // Check if user input is escape key or enter key
        if (keyInfo.Key == ConsoleKey.Escape)
        {
            return;
        }
        else if (keyInfo.Key == ConsoleKey.Enter)
        {
            fileName = sb.ToString();
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrWhiteSpace(fileName))
            {
                break;
            }
        }
        else if (keyInfo.Key == ConsoleKey.Backspace)
        {
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                Console.Write("\b \b");
            }
        }
        else
        {
            sb.Append(keyInfo.KeyChar);
            Console.Write(keyInfo.KeyChar); // Write the character to the console
        }
    }

    if (File.Exists(@$"{docPath}/{fileName}.txt"))
    {
        Console.WriteLine("File already exists.");
        Console.WriteLine("Press any key to return to the main menu.");
        Console.ReadKey(true);
        return;
    }

    TextFile textFile = new(@$"{docPath}\{fileName}.txt");
    textFile.Create();
    Console.WriteLine($"File {fileName}.txt created in:");
    Console.WriteLine($@"{docPath}\{fileName}.txt");
    Console.WriteLine("Press any key to open the file.");
    Console.ReadKey(true);
    Console.CursorVisible = false;
    textFile.Open();
}
