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
            ConsoleKeyInfo typedChar = Console.ReadKey(true);

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
                    currentBufferLine++;
                    cursorPos.Top++;
                    currentFileLine++;
                    continue;
                case ConsoleKey.LeftArrow:
                    // Check if the cursor is at the beginning of the line
                    if (cursorPos.Left == 0)
                    {
                        continue;
                    }
                    Console.SetCursorPosition(cursorPos.Left--, cursorPos.Top);
                    continue;
                case ConsoleKey.RightArrow:
                    // Check if the cursor is at the end of the line
                    if (cursorPos.Left == lines[currentFileLine].Length)
                    {
                        continue;
                    }
                    Console.SetCursorPosition(cursorPos.Left++, cursorPos.Top);
                    continue;
                case ConsoleKey.Backspace:
                    // Check if the cursor is at the beginning of the line
                    if (cursorPos.Left == 0)
                    {
                        if (string.IsNullOrEmpty(lines[currentFileLine]))
                        {
                            lines.RemoveAt(currentFileLine);
                        }
                        else
                        {
                            // Get the previous line
                            string prevLineValue = lines[currentFileLine - 1];
                            // Add the current line to the previous line
                            lines[currentFileLine - 1] += lines[currentFileLine];
                            // Remove the current line
                            lines.RemoveAt(currentFileLine);
                            cursorPos.Left = prevLineValue.Length;
                        }
                        currentFileLine--;
                        currentBufferLine--;
                        cursorPos.Top--;
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
                    }
                    continue;
                case ConsoleKey.Escape:
                    
                    Save();
                    Console.WriteLine("File saved succesfully");
                    return;
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

    public void Delete()
    {
        File.Delete(Path);
    }

    public void Create()
    {
        File.Create(Path);
    }
}
