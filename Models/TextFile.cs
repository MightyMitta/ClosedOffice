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
        Console.CursorVisible = true;
        Console.Clear();

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

        //int curXPos = 0;
        //int curYPos = 0;
        int currentLine = 0;
        int lastBufferLine = 0;

        while (true)
        {
            PrintBuffer(currentLine, lines.Count);
            ConsoleKeyInfo typedChar = Console.ReadKey();

            // check if the key is a letter or a number
            if (char.IsAscii(typedChar.KeyChar))
            {
                if (lines[currentLine].Length < Console.WindowWidth)
                {
                    lines[currentLine] += typedChar.KeyChar;
                }
                else
                {
                    lines[currentLine] = lines[currentLine][..^1] + typedChar.KeyChar;
                }
            }

            switch (typedChar.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentLine != 0)
                        currentLine--;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentLine < lines.Count - 1 && lastBufferLine! < lines.Count)
                        currentLine++;
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    private void PrintBuffer(int start, int totalLines)
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
            //int lastBufferLine = i + start;
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
