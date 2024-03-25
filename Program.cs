while (true)
{
    Console.Write("Enter command: ");
    string command = Console.ReadLine();

    switch (command)
    {
        case "open-file":
            Console.WriteLine("Enter file name:");
            OpenFile(@"C:\Users\mitch\source\repos\ClosedOffice\Testfile.txt");
            break;
    }
}

void OpenFile(string fileName)
{
    Console.CursorVisible = false;
    Console.Clear();
    List<string> lines = [];

    try
    {
        using StreamReader sr = new(fileName);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            lines.Add(line);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
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

        switch (typedChar.Key)
        {
            case ConsoleKey.UpArrow:
                if (currentLine != 0)
                    currentLine--;
                break;
            case ConsoleKey.DownArrow:
                if (currentLine < lines.Count - 1 && lastBufferLine !< lines.Count)
                    currentLine++;
                break;
            case ConsoleKey.Escape:
                return;
        }
    }

    void PrintBuffer(int start, int totalLines)
    {
        Console.Clear();
        Console.SetCursorPosition(0,0);
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
            lastBufferLine = i + start;
        }
    }
}
