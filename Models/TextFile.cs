using System.Text.RegularExpressions;

namespace ClosedOffice.Models;
public class TextFile
{
    public string Name { get; set; }
    public string Path { get; set; }
    private List<string> lines = [];
    private List<string> changedLines = [];

    public TextFile(string path)
    {
        Console.CursorVisible = false;
        Path = path;
        Name = System.IO.Path.GetFileName(path) ?? "file";
    }

    public void Open()
    {
        // Set the cursor to visible, clear the console and set the cursor to the top left
        Console.CursorVisible = true;
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
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
            return;
        }

        // Save the current Y position of the cursor
        (int Left, int Top) cursorPos = Console.GetCursorPosition();
        int curLine = 0;
        int currentVirtualLine = 0;
        int lastBufferLine = 0;

        while (true)
        {
            lastBufferLine = Console.WindowHeight - 5;
            PrintBuffer(curLine, lines.Count, cursorPos);
            ConsoleKeyInfo typedChar = Console.ReadKey(true);
            int curLineNumberLength = curLine.ToString().Length;

            switch (typedChar.Key)
            {
                case ConsoleKey.UpArrow:
                    if (cursorPos.Top > 0)
                    {
                        Console.SetCursorPosition(Console.GetCursorPosition().Left, cursorPos.Top--);
                    } 
                    else if (curLine != 0)
                    {
                        curLine--;
                    }
                    currentVirtualLine--;
                    continue;
                case ConsoleKey.DownArrow:
                    if (cursorPos.Top + 1 < lastBufferLine)
                    {
                        Console.SetCursorPosition(Console.GetCursorPosition().Left, cursorPos.Top++);
                    } 
                    else if (curLine < lines.Count - 1 && (curLine + lastBufferLine) != lines.Count)
                    {
                        curLine++;
                    }
                    currentVirtualLine++;
                    continue;
                case ConsoleKey.LeftArrow:
                    if (cursorPos.Left > 0 + curLineNumberLength)
                    {
                        Console.SetCursorPosition(cursorPos.Left--, Console.GetCursorPosition().Top);
                    }
                    continue;
                case ConsoleKey.RightArrow:
                    if (cursorPos.Left < lines[cursorPos.Top].Length + curLineNumberLength)
                    {
                        Console.SetCursorPosition(cursorPos.Left++, Console.GetCursorPosition().Top);
                    }
                    continue;
                case ConsoleKey.Backspace:
                    if (lines[currentVirtualLine].Length == 0)
                    {
                        lines.RemoveAt(currentVirtualLine);
                        cursorPos.Top--;
                        currentVirtualLine--;
                        continue;
                    }
                    if (cursorPos.Left > curLineNumberLength + 1)
                    {
                        string curLineText = lines[currentVirtualLine];
                        lines[currentVirtualLine] = curLineText.Remove(cursorPos.Left - 3, 1);
                        Console.SetCursorPosition(cursorPos.Left--, cursorPos.Top);
                    }
                    continue;
                case ConsoleKey.Enter:
                    lines.Insert(cursorPos.Top + 1, "");
                    cursorPos.Left = curLineNumberLength + 1;
                    cursorPos.Top++;
                    continue;
                case ConsoleKey.Delete:
                    if (cursorPos.Left < lines[cursorPos.Top].Length + curLineNumberLength)
                    {
                        string curLineText = lines[currentVirtualLine];
                        lines[currentVirtualLine] = curLineText.Remove(cursorPos.Left - 2, 1);
                    }
                    continue;
                case ConsoleKey.Escape:
                    return;
            }

            // check if the key is a letter or a number
            // Regex match the the keychar with a letter, number or any symbol
            Regex regex = new(@"[a-zA-Z0-9_\W]");
            if (regex.IsMatch(typedChar.KeyChar.ToString()))
            {
                // Get the current line
                string curLineValue = lines[cursorPos.Top];
                // Insert the character at the current cursor position
                try
                {
                    lines[cursorPos.Top] = curLineValue.Insert(cursorPos.Left - 2, typedChar.KeyChar.ToString());
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                // Move the cursor to the right
                Console.SetCursorPosition(cursorPos.Left++, cursorPos.Top);
            }
        }
    }

    private void PrintBuffer(int start, int totalLines, (int Left, int Top) cursorPos)
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < Console.WindowHeight - 5; i++)
        {
            if (i + start >= totalLines)
            {
                return;
            }

            if (lines[i + start].Length > Console.WindowWidth)
            {
                Console.WriteLine($"{i + start + 1} {lines[i + start][..Console.WindowWidth]}");
            }
            else
            {
                Console.WriteLine($"{i + start + 1} {lines[i + start]}");
            }
        }

        // Move the cursor back to the original position
        Console.SetCursorPosition(cursorPos.Left, cursorPos.Top);
    }

    public void Delete()
    {
        File.Delete(Path);
    }

    public void Create()
    {
        File.Create(Path);
    }

    public void Save()
    {
        using StreamWriter sw = new(Path);
        foreach (var line in lines)
        {
            sw.WriteLine(line);
        }

        Name = "";

        while (string.IsNullOrEmpty(Name) && string.IsNullOrWhiteSpace(Name))
        {
            Console.Clear();
            Console.WriteLine("Enter the name of the file you would like to create:");
            Name = Console.ReadLine();
        }

        FileStream file = null;

        try
        {
            file = File.Create(Path + $"{Name}.txt");
            Path = file.Name;
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
        }
        catch (PathTooLongException e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
        }
        finally
        {
            file.Close();
        }
    }
}
