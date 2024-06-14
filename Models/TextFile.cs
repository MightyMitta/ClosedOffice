using System.Text;
using System.Text.RegularExpressions;

namespace ClosedOffice.Models;
public class TextFile
{
    public string Name { get; set; }
    public string Path { get; set; }
    private List<string> Lines { get; set; } = [];
    
    private string lookUp = string.Empty;
    private bool ignoreCase = false;

    
    private int CurrentFileLine { get; set; } = 0; // Represents the current line in the opened file
    private int CurrentBufferLine { get; set; } = 0; // Represents the current buffer line (the amount of lines that can be displayed on the console)
    private int BufferHeight { get; set; } = Console.BufferHeight; // Represents the last buffer line (the amount of lines that can be displayed on the console)
    private int CursorPosLeft // Represents the current horizontal cursor position in the console
    {
        get
        {
            return _cursorPosLeft;
        }
        set
        {
            if ((value >= 0) && (value < Console.WindowWidth - 1))
            {
                _cursorPosLeft = value;
            } else if (value < 0)
            {
                _cursorPosLeft = 0;
            } else if (value > Console.WindowWidth - 1)
            {
                _cursorPosLeft = Console.WindowWidth - 1;
            }
        }
    } 

    private int _cursorPosLeft = 0; // Represents the current horizontal cursor position in the console

    private int CursorPosTop // Represents the current vertical cursor position in the console
    {
        get
        {
            return _cursorPosTop;
        }
        set
        {
            if ((value >= 3) && (value < Console.WindowHeight - 4))
            {
                _cursorPosTop = value;
            }
            else if (value < 3)
            {
                _cursorPosTop = 3;
            }
            else if (value > Console.WindowHeight - 4)
            {
                _cursorPosTop = Console.WindowHeight - 4;
            }
        }
    }

    private int _cursorPosTop = 3; // Represents the current vertical cursor position in the console

    public TextFile(string path) // Constructor
    {
        Console.CursorVisible = true;
        Path = path;
        Name = System.IO.Path.GetFileName(path) ?? "file";
    }
    
    public bool IsTextExceedingWindowWidth(string text) // Check if the text is exceeding the window width
    {
        return text.Length >= Console.WindowWidth;
    }

    public void Open() // Open the file
    {
        // Set the cursor to visible, clear the console and set the cursor to the top left
        Console.Clear();
        Console.SetCursorPosition(0, 3);
        Console.CursorVisible = true;

        // Try to read the file and store the lines in a list
        try
        {
            using StreamReader sr = new(Path);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Lines.Add(line);
            }
        }
        catch (Exception e) // Catch any exceptions and display an error message
        {
            Console.SetCursorPosition(0, Console.WindowWidth - 3);
            
            Console.WriteLine("An error occurred: " + e.Message);
            return;
        }

        while (true)
        {
            Console.Clear();
            BufferHeight = Console.WindowHeight - 7;
            // if currently opened file is not a .tmp file
            if (!Path.EndsWith(".tmp"))
            {
                Console.Title = $"File: {Name} - Line: {CurrentFileLine + 1}"; // Set the title of the console window to the name of the file
            }
            else
            {
                Console.Title = "ClosedOffice"; // Set the title of the console window to ClosedOffice
            }
            PrintBuffer(CurrentFileLine - CurrentBufferLine, BufferHeight, (CursorPosLeft, CursorPosTop)); // Print the buffer
            ConsoleKeyInfo typedChar; // Store the key that was pressed
            try
            {
                typedChar = Console.ReadKey(true); // Read the key that was pressed
            }
            catch (InvalidOperationException) // Catch any exceptions and display an error message
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 3);
                Console.WriteLine(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.WindowHeight - 3);
                Console.WriteLine("this action is not allowed, press any key to continue");
                Console.CursorVisible = false;
                Console.ReadKey(true);
                Console.CursorVisible = true;
                continue;
            }

            switch (typedChar.Key) // Check which key was pressed
            {
                case ConsoleKey.UpArrow:
                    MoveVertical(-1); // Move the cursor up by one line
                    // Move the cursor to the left if the next line is shorter than the current cursor position
                    if (CursorPosLeft > Lines[CurrentFileLine].Length)
                    {
                        CursorPosLeft = Lines[CurrentFileLine].Length; // Set the cursor to the end of the line
                    }
                    continue;
                case ConsoleKey.DownArrow:
                    MoveVertical(1); // Move the cursor down by one line
                    continue;
                case ConsoleKey.LeftArrow:
                    MoveHorizontal(-1); // Move the cursor to the left by one
                    continue;
                case ConsoleKey.RightArrow:
                    MoveHorizontal(1); // Move the cursor to the right by one
                    continue;
                case ConsoleKey.Backspace:
                    // Check if the cursor is at the beginning of the line
                    if (CursorPosLeft == 0)
                    {
                        if (IsTopLine(CurrentFileLine)) // Check if the current line is the first line of the file
                        {
                            continue; // If it is, continue with the next iteration of the loop
                        }
                        // Check if the current line is empty
                        if (LineIsEmpty(CurrentFileLine))
                        {
                            RemoveLine(CurrentFileLine); // If it is, remove the current line
                        }
                        // Merge the current line with the previous line
                        else
                        {
                            RemoveLine(CurrentFileLine - 1); // Remove the previous line
                        }
                        CursorPosLeft = Lines[CurrentFileLine - 1].Length; // Set the cursor to the end of the previous line
                        CursorUp(); // Move the cursor up by one line
                        continue;
                    }
                    // Get the current line
                    string curLineValue = Lines[CurrentFileLine];
                    // Remove the character at the current cursor position
                    try
                    {
                        Lines[CurrentFileLine] = curLineValue.Remove(CursorPosLeft - 1, 1); // Remove the character at the current cursor position
                        // Move the cursor to the left by one
                        Console.SetCursorPosition(CursorPosLeft--, CursorPosTop); // Move the cursor to the left by one
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Showing the error message is not necessary and can be ignored
                    }
                    continue;
                case ConsoleKey.Enter:
                    // If the Enter key is pressed

                    // Check if the cursor is at the end of the current line
                    if (CursorPosLeft == Lines[CurrentFileLine].Length)
                    {
                        // If it is, insert a new empty line after the current line
                        Lines.Insert(CurrentFileLine + 1, "");
                    }
                    else
                    {
                        // If it's not, get the current line
                        curLineValue = Lines[CurrentFileLine];
                        
                        // Remove the text after the cursor position
                        Lines[CurrentFileLine] = curLineValue.Remove(CursorPosLeft);

                        // Insert a new line after the current line with the remaining text after the cursor position
                        Lines.Insert(CurrentFileLine + 1, curLineValue[CursorPosLeft..]);
                    }

                    // If the current buffer line is not the last line of the buffer
                    if (CurrentBufferLine < BufferHeight - 1)
                    {
                        // Move the cursor down by one line
                        CursorPosTop++;
                    }
                    else
                    {
                        // If the current buffer line is the last line of the buffer
                        // Move the buffer up by one line
                        CurrentBufferLine--;
                    }

                    // If the current file line is not the last line of the file
                    if (CurrentFileLine + 1 < Lines.Count)
                    {
                        // Move to the next line and set the cursor to the beginning of the line
                        CurrentFileLine++;
                        CursorPosLeft = 0;
                    }

                    // Move to the next buffer line
                    CurrentBufferLine++;


                    // Continue with the next iteration of the loop
                    continue;
                
                case ConsoleKey.Delete:
                    // Check if the cursor is at the end of the line
                    if (CursorPosLeft == Lines[CurrentFileLine].Length)
                    {
                        // Check if the current line is the last line of the file
                        if (CurrentFileLine == Lines.Count - 1)
                        {
                            continue;
                        }
                        // Merge the current line with the next line
                        RemoveLine(CurrentFileLine);
                        continue;
                    }
                    // Get the current line
                    curLineValue = Lines[CurrentFileLine];
                    // Remove the character at the current cursor position
                    try
                    {
                        Lines[CurrentFileLine] = curLineValue.Remove(CursorPosLeft, 1); // Remove the character at the current cursor position
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Showing the error message is not necessary and can be ignored
                    }
                    continue;
                case ConsoleKey.Escape:
                    Save(); // show the save menu
                    return;
                case ConsoleKey.F1:
                    OpenFile(); // run the OpenFile method
                    return;
                case ConsoleKey.F2:
                    Console.SetCursorPosition(left: 0 , top: Console.WindowHeight - 3);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(left: 0 , top: Console.WindowHeight - 3);
                    Console.Write("Enter word to search: ");
                    string searchWord = Console.ReadLine(); // Get the word to search for
                    lookUp = searchWord; // store the search word

                    int wordCount = 0;
                    foreach (string line in Lines) // Loop through each line in the file
                    {
                        // ingore casing when searching for the word
                        wordCount += line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Count(x => x.Equals(searchWord, StringComparison.OrdinalIgnoreCase)); // Count the number of times the word appears in the file
                    }

                    // Display the number of times the word appears in the file
                    Console.SetCursorPosition(left: 0 , top: Console.WindowHeight - 3);
                    Console.Write($"Your word '{searchWord}' was found {wordCount} times, press any key to continue...");
                    Console.ReadKey(true);
                    continue; // Continue and mark all the matching words
                case ConsoleKey.F3:
                    Console.SetCursorPosition(left: 0 , top: Console.WindowHeight - 3);
                    Console.Write("Enter path of import file ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.Write("(Leave empty to use the TestImportFile)");
                    Console.ResetColor();
                    Console.Write(": ");
                    string importPath = Console.ReadLine(); // Get the path of the file to import

                    if (string.IsNullOrEmpty(importPath) || string.IsNullOrWhiteSpace(importPath))
                    {

                        importPath = Directory.GetCurrentDirectory() + "\\import.txt"; // Use the TestImportFile if the path is empty
                    }

                    if (!File.Exists(importPath)) // Check if the file exists
                    {
                        Console.SetCursorPosition(0, Console.WindowHeight - 3);
                        Console.WriteLine(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.WindowHeight - 3);
                        Console.WriteLine("File does not exist, press any key to continue");
                        Console.CursorVisible = false;
                        Console.ReadKey(true);
                        Console.CursorVisible = true;
                        return;
                    }

                    string[] importedLines = File.ReadAllLines(importPath); // Read all the lines from the file
                    Lines.AddRange(importedLines); // Add the imported lines to the current file
                    continue;
            }

            // Regex match the the keychar with a letter, number or any symbol
            Regex regex = new(@"[a-zA-Z0-9_\W]");
            if (regex.IsMatch(typedChar.KeyChar.ToString()))
            {
                // Get the current line
                string curLineValue = Lines[CurrentFileLine];

                // Insert the character at the current cursor position
                try
                {
                    Lines[CurrentFileLine] = curLineValue.Insert(CursorPosLeft, typedChar.KeyChar.ToString());
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                
                if (IsTextExceedingWindowWidth(Lines[CurrentFileLine]))
                {
                    curLineValue = Lines[CurrentFileLine];
                    
                    // save the current cursor position
                    (int cursorLeft, int cursorTopBefore) = Console.GetCursorPosition();
                        
                    // Remove the text after the cursor position
                    Lines[CurrentFileLine] = curLineValue.Remove(CursorPosLeft + 1);

                    // Insert a new line after the current line with the remaining text after the cursor position
                    Lines.Insert(CurrentFileLine + 1, curLineValue[CursorPosLeft..]);
                    
                    // remove the first character of the inserted text
                    Lines[CurrentFileLine + 1] = Lines[CurrentFileLine + 1].Remove(0, 1);
                    
                    // set the cursor back to where it was before
                    Console.SetCursorPosition(cursorLeft, cursorTopBefore);
                    
                }
                // Move the cursor to the right by one but check if the cursor is at the end of the line first
                if (CursorPosLeft == Console.WindowWidth -2)
                {
                    // Check if the current line is the last line of the file
                    if (CurrentFileLine == Lines.Count - 1)
                    {
                        Lines.Insert(CurrentFileLine + 1, "");
                        // Console.SetCursorPosition(CursorPosLeft = 0, CursorPosTop++);
                        MoveVertical(1);
                    }
                    // if not Move the cursor to the beginning of the next line
                    else
                    {
                        MoveVertical(1);
                        CursorPosLeft = 0;
                    }
                }
                // Move the cursor to the right by one
                else
                {
                    Console.SetCursorPosition(CursorPosLeft++, CursorPosTop);
                }

            }
        }
    }

    /// <summary>
    /// Move the cursor in the vertical direction. Up is -1 and down is 1.
    /// </summary>
    /// <param name="direction"></param>
    private void MoveVertical(int direction)
    {
        // Check if the direction is up
        if (direction == -1)
        {
            if (CurrentBufferLine == 0 && CurrentFileLine == 0)
            {
                return;
            }

            // Check if the current line is the first line of the buffer
            if (CurrentBufferLine == 0)
            {
                // Check if the current line is the first line of the file
                if (!IsTopLine(CurrentFileLine))
                {
                    CursorUp();
                }
                return;
            }

            // Move cursor to start of line if the next line is empty
            if (string.IsNullOrEmpty(Lines[CurrentFileLine - 1]))
            {
                CursorPosLeft = 0;
            }
            // Move the cursor up by one line
            CursorUp();
            return;
        }

        // Check if the direction is down
        if (direction == 1)
        {
            // Check if the current line is the last line of the buffer
            if (IsBufferBottom(CurrentBufferLine))
            {
                // Check if the current line is the last line of the file
                if (CurrentFileLine != Lines.Count - 1)
                {
                    CurrentFileLine++;
                    // Update the console buffer
                    Console.Clear();
                }
                return;
            }

            if (IsBottomLine(CurrentFileLine))
            {
                return;
            } 
            else 
            {
                // Move cursor to the end of line if the next line
                if (CursorPosLeft > Lines[CurrentFileLine + 1].Length)
                {
                    CursorPosLeft = Lines[CurrentFileLine + 1].Length;
                }
            }
            // Move the cursor down by one line
            CursorDown();
            return;
        }
    }

    /// <summary>
    /// Move the cursor in the horizontal direction. Left is -1 and right is 1.
    /// </summary>
    /// <param name="direction"></param>
    private void MoveHorizontal(int direction)
    {
        // Check if the direction is left
        if (direction == -1)
        {
            // Check if the cursor is at the beginning of the line
            if (CursorPosLeft == 0)
            {
                // Check if the current line is the first line of the file
                if (CurrentFileLine == 0)
                {
                    return;
                }
                // Move the cursor to the end of the previous line
                CursorPosLeft = Lines[CurrentFileLine - 1].Length;
                CurrentFileLine--;
                CurrentBufferLine--;
                CursorPosTop--;
                return;
            }
            // Move the cursor to the left by one
            Console.SetCursorPosition(CursorPosLeft--, CursorPosTop);
            return;
        }

        // Check if the direction is right
        if (direction == 1)
        {
            // Check if the cursor is at the end of the line
            if (CursorPosLeft == (Lines[CurrentFileLine].Length > Console.WindowWidth - 1 ? Console.WindowWidth - 1 : Lines[CurrentFileLine].Length))
            {
                // Check if the current line is the last line of the file
                if (IsBottomLine(CurrentFileLine))
                {
                    return;
                }
                // Move the cursor to the beginning of the next line
                CursorPosLeft = 0;
                CurrentFileLine++;
                CurrentBufferLine++;
                CursorPosTop++;
                return;
            }
            // Move the cursor to the right by one
            Console.SetCursorPosition(CursorPosLeft++, CursorPosTop);
            return;
        }
    }

    private void CursorUp()
    {
        // Check if the current line is the first line of the buffer
        if (!IsBufferTop(CurrentBufferLine))
        {
            CurrentBufferLine--;
            CursorPosTop--;
        }

        // Check if the current line is the first line of the file
        if (!IsTopLine(CurrentFileLine))
        {
            CurrentFileLine--;
        }
    }

    private void CursorDown()
    {
        // Check if the current line is the last line of the buffer
        if (!IsBufferBottom(CurrentBufferLine))
        {
            CurrentBufferLine++;
            CursorPosTop++;
        }

        // Check if the current line is the last line of the file
        if (!IsBottomLine(CurrentFileLine))
        {
            CurrentFileLine++;
        }
    }

    // Remove line, check if the line is empty and if it is the last line and if necessary remove or merge the line
    private void RemoveLine(int currentLine)
    {
        // Check if the current line is empty
        if (string.IsNullOrEmpty(Lines[currentLine]))
        {
            Lines.RemoveAt(currentLine);
        }
        // Check if the current line is the last line
        else if (currentLine == Lines.Count - 1)
        {
            Lines.RemoveAt(currentLine);
        }
        // If the current line is not empty and not the last line
        else
        {
            Lines[currentLine] += Lines[currentLine + 1];
            Lines.RemoveAt(currentLine + 1);
        }
    }

    // Check if the next line exists and if it is empty
    private bool LineIsEmpty(int currentLine)
    {
        return string.IsNullOrEmpty(Lines[currentLine]);
    }

    private bool IsTopLine(int currentLine) // Check if the current line is the first line of the file
    {
        return currentLine == 0;
    }

    private bool IsBufferTop(int currentLine) // Check if the current line is the first line of the buffer
    {
        return currentLine == 0;
    }

    private bool IsBottomLine(int currentLine) // Check if the current line is the last line of the file
    {
        return currentLine == Lines.Count - 1;
    }

    private bool IsBufferBottom(int currentLine) // Check if the current line is the last line of the buffer
    {
        return currentLine == BufferHeight - 1;
    }

    private void PrintBuffer(int startLine, int bufferHeight, (int Left, int Top) cursorPos)
    {
        // Clear the console
        Console.SetCursorPosition(0, 0);

        // Print the top border of the header
        Console.WriteLine(new string('-', Console.WindowWidth)); //problem

        // Display the header with hotkey information
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.WriteLine("F1 Open file | F2 Search | F3 Import | ESC Save");

        // Print the bottom border of the header
        Console.ResetColor();
        Console.WriteLine(new string('-', Console.WindowWidth));

        // Display the footer with a command input area
        Console.SetCursorPosition(0, Console.WindowHeight - 4); // was 3
        Console.WriteLine(new string('-', Console.WindowWidth));
        
        int fileWordCount = 0;
        // Loop through each line in the file
        foreach (string line in Lines)
        {
            // Get word count of the current line without counting empty lines and whitespaces
            fileWordCount += line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
        
        Console.WriteLine($"{fileWordCount} words in a total of {Lines.Count} lines."); // Display the number of words in the file

        // Print the bottom border
        Console.WriteLine(new string('-', Console.WindowWidth));

        Console.ResetColor();

        Console.SetCursorPosition(0, 3);

        // Print the buffer
        for (int i = startLine; i < startLine + bufferHeight && i < Lines.Count; i++)
        {
            if (Lines[i].Length > Console.WindowWidth) // Check if the line is longer than the window width
            {
                Console.WriteLine($"{Lines[i][..(Console.WindowWidth - 1)]}"); // Print the line up to the window width
            }
            else
            {
                Console.WriteLine($"{Lines[i]}"); // Print the line
            } 
            var stringComparison = StringComparison.OrdinalIgnoreCase; // Set the string comparison to ignore case
            var contains = Lines[i].Contains(lookUp, stringComparison); // Check if the line contains the search word
            if (contains && !string.IsNullOrEmpty(lookUp)) // Check if the line contains the search word and the search word is not empty
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                int index = 0;
                while ((index = Lines[i].IndexOf(lookUp, index, stringComparison)) != -1) // Loop through each occurrence of the search word
                {
                    var cursorTop = Console.CursorTop;
                    // save the current cursor position
                    (int cursorLeft, int cursorTopBefore) = Console.GetCursorPosition(); // Save the current cursor position
                    Console.SetCursorPosition(index, cursorTop - 1); // Set the cursor position to the start of the search word
                    Console.Write(lookUp); // Write the search word
    
                    // Move the cursor back to the original position
                    Console.SetCursorPosition(cursorLeft, cursorTopBefore); // Move the cursor back to the original position
    
                    index += lookUp.Length; // Move the index to the next occurrence of the search word
                }
                Console.ResetColor();
            }
        }
        // Move the cursor back to the original position
        if (cursorPos.Left > Console.WindowWidth - 1) // Check if the cursor position is at the end of the line
        {
            cursorPos.Left = Console.WindowWidth - 1; // Set the cursor position to the end of the line
        }
        Console.SetCursorPosition(cursorPos.Left, cursorPos.Top); // Move the cursor to the original position
    }

    public void Save()
    {
        Console.SetCursorPosition(left: 0 , top: Console.WindowHeight - 3);
        Console.WriteLine("Would you like to overwrite or save as new file?"); // Ask the user if they want to overwrite the file or save as a new file

        List<Option> options = new List<Option> // Create a list of options
        {
            new("Save as new file", () => SaveAs() ),
            new("Continue editing", () => Open() )
        };

        // Add the Overwrite option only if the file is not a .tmp file
        if (!Path.EndsWith(".tmp"))
        {
            options.Insert(0, new Option("Overwrite", () => Overwrite()));
        }

        Menu menu = new(options);
    }

    private void Overwrite()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(Path)) // Open the file for writing
            {
                foreach (var line in Lines) // Loop through each line in the file
                {
                    sw.WriteLine(line); // Write the line to the file
                }
            } 
            //clear the menu
            Console.SetCursorPosition(0, Console.WindowHeight - 7);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 6);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 5);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 4);
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            
            Console.WriteLine("File saved successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            Helper.ExitMenu = -1;
        }
        catch (Exception e) // Catch any exceptions and display an error message
        {
            Console.WriteLine("An error occurred: " + e.Message);
            Helper.ExitMenu = 0;
            return;
        }
    }

    public void SaveAs()
    {
        Name = "";
        while (string.IsNullOrEmpty(Name) && string.IsNullOrWhiteSpace(Name)) // Check if the name is empty or whitespace
        {
            //clear the menu
            Console.SetCursorPosition(0, Console.WindowHeight - 7);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 6);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 5);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 4);
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            
            Console.Write("Enter the name of the file you would like to create:"); // Ask the user to enter the name of the file
            Console.CursorVisible = true;
            Name = Console.ReadLine(); // Get the name of the file
            Console.CursorVisible = false;
            Console.WriteLine(new string('-', Console.WindowWidth));
        }
        
        // Combine the current directory with the file name
        Path = System.IO.Path.Combine(Environment.CurrentDirectory, Name + ".txt"); // Combine the current directory with the file name

        // Try to save the file
        try
        {
            using StreamWriter sw = new(Path);
            foreach (var line in Lines)
            {
                sw.WriteLine(line);
            }
        }
        catch // Catch any exceptions and display an error message
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.WriteLine("An error occurred while saving the file!");
            return;
        }

        // Check if the file was saved successfully
        if (File.Exists(Path))
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.WriteLine("File saved successfully!");
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
            Console.CursorVisible = true;
            Helper.ExitMenu = -1;
        }
        else // If the file was not saved successfully
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.WriteLine("An error occurred while saving the file!");
            Console.ReadKey(true);
            Console.CursorVisible = true;
            Helper.ExitMenu = -1;
        }
    }

    public void Delete()
    {
        File.Delete(Path);
    }

    public void Create()
    {
        using (FileStream fs = File.Create(Path))
        {
            // File created
            throw new NotImplementedException();
        }
    }
    
    void OpenFile()
    {
        // Clear the console and set the cursor to the footer
        Console.SetCursorPosition(0, Console.WindowHeight - 3);

        // Get the path of the file
        Console.CursorVisible = true;
        Console.Write("Enter the path ");
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.Write("(Leave empty to use the TestFile)");
        Console.ResetColor();
        Console.Write(": ");

        
        // Get the path of the file
        string path = Console.ReadLine();
        
        // If the path is empty, use the TestFile
        if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
        {
            path = "./Testfile.txt";
        }
        
        // TODO: if the user presses escape key, continue with the current file
        // if (path == "\u001b")
        // {
        //     Console.ReadKey(true);
        //     return;
        // }
        
        //if path does not exist, continue with the current file
        if (!File.Exists(path))
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.WriteLine("File does not exist, press any key to continue");
            Console.CursorVisible = false;
            Console.ReadKey(true);
            Console.CursorVisible = true;
            return;
        }

        // TODO: Replace with actual file path
        TextFile textFile = new(path);
        textFile.Open();
        
        // Clear the console and reset the cursor position after the file is opened
        Console.Clear();
        Console.SetCursorPosition(0, 0);
    }
}
