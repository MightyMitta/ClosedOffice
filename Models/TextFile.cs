using System.Text.RegularExpressions;

namespace ClosedOffice.Models;
public class TextFile
{
    public string Name { get; set; }
    public string Path { get; set; }
    private List<string> lines = [];

    public TextFile(string path)
    {
        Console.CursorVisible = true;
        Path = path;
        Name = System.IO.Path.GetFileName(path) ?? "file";
    }

    public void Open()
    {
        // Set the cursor to visible, clear the console and set the cursor to the top left
        Console.Clear();
        Console.SetCursorPosition(0, 0);

        // Try to read the file and store the lines in a list
        try
        {
            using StreamReader sr = new(Path);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                lines.Add(line);
            }
            sr.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
            return;
        }

        // Save the current Y position of the cursor
        (int Left, int Top) cursorPos = Console.GetCursorPosition();
        // Represents the current line in the text file
        int currentFileLine = 0;
        // Represents the current buffer line (the amount of lines that can be displayed on the console)
        int currentBufferLine = 0;
        // Represents the last buffer line (the amount of lines that can be displayed on the console)
        int bufferHeight;

        while (true)
        {
            bufferHeight = Console.WindowHeight - 5;
            Console.Title = $"File: {Name} - Line: {currentFileLine + 1}";
            PrintBuffer(currentFileLine - currentBufferLine, bufferHeight, cursorPos);
            ConsoleKeyInfo typedChar;
            try
            {
                typedChar = Console.ReadKey(true);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("This action is not allowed!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                continue;
            }

            switch (typedChar.Key)
            {
                case ConsoleKey.UpArrow:
                    // Check if the current line is the first line of the buffer
                    if (currentBufferLine == 0)
                    {
                        // Check if the current line is the first line of the file
                        if (currentFileLine != 0)
                        {
                            currentFileLine--;
                        }
                        continue;
                    }
                    // Move cursor to start of line if the next line is empty
                    if (string.IsNullOrEmpty(lines[currentFileLine - 1]))
                    {
                        cursorPos.Left = 0;
                    }
                    // Move the cursor up by one line
                    currentBufferLine--;
                    cursorPos.Top--;
                    currentFileLine--;

                    continue;
                case ConsoleKey.DownArrow:
                    // Check if the current line is the last line of the buffer
                    if (currentBufferLine == bufferHeight - 1)
                    {
                        // Check if the current line is the last line of the file
                        if (currentFileLine != lines.Count - 1)
                        {
                            currentFileLine++;
                        }
                        continue;
                    }
                    // Move cursor to the end of line if the next line
                    if (cursorPos.Left > lines[currentFileLine + 1].Length)
                    {
                        cursorPos.Left = lines[currentFileLine + 1].Length;
                    }
                    // Move the cursor down by one line
                    currentBufferLine++;
                    cursorPos.Top++;
                    currentFileLine++;
                    continue;
                case ConsoleKey.LeftArrow:
                    // Check if the cursor is at the beginning of the line
                    if (cursorPos.Left == 0)
                    {
                        // Check if the current line is the first line of the file
                        if (currentFileLine == 0)
                        {
                            continue;
                        }
                        // Move the cursor to the end of the previous line
                        cursorPos.Left = lines[currentFileLine - 1].Length;
                        currentFileLine--;
                        currentBufferLine--;
                        cursorPos.Top--;
                        continue;
                    }
                    // Move the cursor to the left by one
                    Console.SetCursorPosition(cursorPos.Left--, cursorPos.Top);
                    continue;
                case ConsoleKey.RightArrow:
                    // Check if the cursor is at the end of the line
                    if (cursorPos.Left == lines[currentFileLine].Length)
                    {
                        // Check if the current line is the last line of the file
                        if (currentFileLine == lines.Count - 1)
                        {
                            continue;
                        }
                        // Move the cursor to the beginning of the next line
                        cursorPos.Left = 0;
                        currentFileLine++;
                        currentBufferLine++;
                        cursorPos.Top++;
                        continue;
                    }
                    // Move the cursor to the right by one
                    Console.SetCursorPosition(cursorPos.Left++, cursorPos.Top);
                    continue;
                case ConsoleKey.Backspace:
                    // Check if the cursor is at the beginning of the line
                    if (cursorPos.Left == 0)
                    {
                        if (currentFileLine == 0)
                        {
                            continue;
                        }
                        // Check if the current line is empty
                        if (string.IsNullOrEmpty(lines[currentFileLine]))
                        {
                            lines.RemoveAt(currentFileLine);
                        } 
                        // Check if the previous line is empty
                        else if (string.IsNullOrEmpty(lines[currentFileLine - 1]))
                        {
                            lines.RemoveAt(currentFileLine - 1);
                        }
                        else
                        {
                            // Merge the current line with the previous line
                            cursorPos.Left = lines[currentFileLine - 1].Length;
                            lines[currentFileLine - 1] += lines[currentFileLine];
                            lines.RemoveAt(currentFileLine);
                        }
                        cursorPos.Top--;
                        currentFileLine--;
                        currentBufferLine--;
                        continue;
                    }
                    // Get the current line
                    string curLineValue = lines[currentFileLine];
                    // Remove the character at the current cursor position
                    try
                    {
                        lines[currentFileLine] = curLineValue.Remove(cursorPos.Left - 1, 1);
                        // Move the cursor to the left by one
                        Console.SetCursorPosition(cursorPos.Left--, cursorPos.Top);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Showing the error message is not necessary and can be ignored
                    }
                    continue;
                case ConsoleKey.Enter:
                    // Add new line after currentline
                    if (cursorPos.Left == lines[currentFileLine].Length)
                    {
                        lines.Insert(currentFileLine + 1, "");
                    }
                    else
                    {
                        // Get the current line
                        curLineValue = lines[currentFileLine];
                        // Insert a new line after the current line
                        lines.Insert(currentFileLine + 1, curLineValue[cursorPos.Left..]);
                        // Remove the text after the cursor position
                        lines[currentFileLine] = curLineValue.Remove(cursorPos.Left);
                    }
                    currentFileLine++;
                    currentBufferLine++;
                    cursorPos.Left = 0;
                    cursorPos.Top++;

                    continue;
                case ConsoleKey.Delete:
                    // Check if the cursor is at the end of the line
                    if (cursorPos.Left == lines[currentFileLine].Length)
                    {
                        continue;
                    }
                    // Get the current line
                    curLineValue = lines[currentFileLine];
                    // Remove the character at the current cursor position
                    try
                    {
                        lines[currentFileLine] = curLineValue.Remove(cursorPos.Left, 1);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Showing the error message is not necessary and can be ignored
                    }
                    continue;
                case ConsoleKey.Escape:
                    Save();
                    Console.WriteLine("File saved succesfully");
                    return;
                case ConsoleKey.F1:
                    Console.Clear();
                    Console.Write("Enter word to search: ");
                    string searchWord = Console.ReadLine();
                    Console.WriteLine("Would you like to ignore casing in result? (y/N)");
                    ConsoleKeyInfo ignoreCase = Console.ReadKey(true);
                    bool ignoreCaseBool = ignoreCase.Key == ConsoleKey.Y;

                    int wordCount = 0;
                    foreach (string line in lines)
                    {
                        if (ignoreCaseBool)
                        {
                            wordCount += Regex.Matches(line, searchWord, RegexOptions.IgnoreCase).Count;
                        }
                        else
                        {
                            wordCount += Regex.Matches(line, searchWord).Count;
                        }
                    }

                    Console.WriteLine($"Your word '{searchWord}' was found {wordCount} times");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    continue;
                case ConsoleKey.F2:
                    Console.Clear();
                    Console.Write("Enter path of the file you would like to import: ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.WriteLine("(Leave empty to use the TestImportFile)");
                    Console.ResetColor();
                    string importPath = Console.ReadLine();

                    if (string.IsNullOrEmpty(importPath) || string.IsNullOrWhiteSpace(importPath))
                    {

                        importPath = Directory.GetCurrentDirectory() + "\\import.txt";
                    }

                    if (!File.Exists(importPath))
                    {
                        Console.WriteLine("File does not exist");
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        continue;
                    }

                    string[] importedLines = File.ReadAllLines(importPath);
                    lines.AddRange(importedLines);
                    continue;
                case ConsoleKey.F3:
                    Console.Clear();
                    int fileWordCount = 0;
                    foreach (string line in lines)
                    {
                        // Get word count of the current line without counting empty lines and whitespaces
                        fileWordCount += line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    }

                    Console.WriteLine($"Your file contains of {fileWordCount} words in a total of {lines.Count} lines.");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    continue;
            }

            // Regex match the the keychar with a letter, number or any symbol
            Regex regex = new(@"[a-zA-Z0-9_\W]");
            if (regex.IsMatch(typedChar.KeyChar.ToString()))
            {
                // Get the current line
                string curLineValue = lines[currentFileLine];
                // Insert the character at the current cursor position
                try
                {
                    lines[currentFileLine] = curLineValue.Insert(cursorPos.Left, typedChar.KeyChar.ToString());
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                // Move the cursor to the right by one
                Console.SetCursorPosition(cursorPos.Left++, cursorPos.Top);
            }
        }
    }

    private void PrintBuffer(int startLine, int bufferHeight, (int Left, int Top) cursorPos)
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < bufferHeight; i++)
        {

            if (lines[i + startLine].Length > Console.WindowWidth)
            {
                //Console.WriteLine($"{i + startLine + 1} {lines[i + startLine][..Console.WindowWidth]}");
                Console.WriteLine($"{lines[i + startLine][..Console.WindowWidth]}");
            }
            else
            {
                //Console.WriteLine($"{i + startLine + 1} {lines[i + startLine]}");
                Console.WriteLine($"{lines[i + startLine]}");
            }
        }

        // Move the cursor back to the original position
        Console.SetCursorPosition(cursorPos.Left, cursorPos.Top);
    }


    public void Save()
    {
        Console.WriteLine("Would you like to overwrite or save as new file?");

        Menu menu = new(
        [
            new("Overwrite", () => Overwrite()), 
            new("Save as new file", () => SaveAs() ),
            new("Cancel", () => { })
        ]);

        Console.WriteLine("yessir");
    }

    private void Overwrite()
    {
        try
        {
            StreamWriter sw = new(Path);
            File.WriteAllText(Path, string.Empty);
            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
            return;
        }
    }

    public void SaveAs()
    {
        Name = "";
        while (string.IsNullOrEmpty(Name) && string.IsNullOrWhiteSpace(Name))
        {
            Console.Clear();
            Console.WriteLine("Enter the name of the file you would like to create:");
            Name = Console.ReadLine();
        }
        
        // Combine the current directory with the file name
        Path = System.IO.Path.Combine(Environment.CurrentDirectory, Name + ".txt");

        // Try to save the file
        try
        {
            using StreamWriter sw = new(Path);
            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }
        }
        catch
        {
            Console.WriteLine("An error occurred while saving the file!");
            return;
        }

        // Check if the file was saved successfully
        if (File.Exists(Path))
        {
            Console.WriteLine("File saved successfully!");
        }
        else
        {
            Console.WriteLine("An error occurred while saving the file!");
        }
    }

    //private void PrintBuffer(int startLine, int bufferHeight, (int Left, int Top) cursorPos)
    //{
    //    Console.Clear();
    //    Console.SetCursorPosition(0, 0);
    //    for (int i = 0; i < bufferHeight; i++)
    //    {

    //        if (lines[i + startLine].Length > Console.WindowWidth)
    //        {
    //            //Console.WriteLine($"{i + startLine + 1} {lines[i + startLine][..Console.WindowWidth]}");
    //            //Console.WriteLine($"{lines[i + startLine][..Console.WindowWidth]}");
    //        }
    //        else
    //        {
    //            //Console.WriteLine($"{i + startLine + 1} {lines[i + startLine]}");
    //            Console.WriteLine($"{lines[i + startLine]}");
    //        }
    //    }

    //    // Move the cursor back to the original 
    //    Console.SetCursorPosition(cursorPos.Left, cursorPos.Top);
    //}


    public void Delete()
    {
        File.Delete(Path);
    }

    public void Create()
    {
        File.Create(Path);
    }
}
