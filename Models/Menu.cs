﻿namespace ClosedOffice.Models;
public class Menu
{
    private List<Option> options;

    public Menu(List<Option> options)
    {
        this.options = options;
        Init();
    }

    private void Init()
    {
        Console.CursorVisible = false;
        // Set the index to the first option
        Helper.ExitMenu = 0;

        // Write the menu in the console
        WriteMenu(options[Helper.ExitMenu]);

        // Store the pressed key info
        ConsoleKeyInfo keyinfo;
        do
        {
            keyinfo = Console.ReadKey(true);

            // Handle each key input (down arrow will write the menu again with a different selected item)
            if (keyinfo.Key == ConsoleKey.DownArrow)
            {
                if (Helper.ExitMenu + 1 < options.Count)
                {
                    Helper.ExitMenu++;
                    WriteMenu(options[Helper.ExitMenu]);
                }
            }
            if (keyinfo.Key == ConsoleKey.UpArrow)
            {
                if (Helper.ExitMenu - 1 >= 0)
                {
                    Helper.ExitMenu--;
                    WriteMenu(options[Helper.ExitMenu]);
                }
            }
            
            // Invoke the selected option when the enter key is pressed
            if (keyinfo.Key == ConsoleKey.Enter)
            {
                options[Helper.ExitMenu].Selected.Invoke();
            }
        }
        // TODO: Change this to a more appropriate condition
        while (Helper.ExitMenu != -1);
    }

    public void WriteMenu(Option selectedOption)
    {
        int startRow = Console.WindowHeight - options.Count - 4;

        for (int i = 0; i < options.Count + 2; i++)
        {
            Console.SetCursorPosition(0, startRow + i);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        Console.SetCursorPosition(0, startRow);
        Console.WriteLine(new string('-', Console.WindowWidth));
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.WriteLine("Navigate options using arrow up/down, select option by pressing Enter                                                   ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.BackgroundColor = ConsoleColor.Black;

        foreach (Option option in options)
        {
            if (option == selectedOption)
            {
                Console.Write("> ");
            }
            else
            {
                Console.Write(" ");
            }

            Console.WriteLine(option.Name);
        }
        Console.WriteLine(new string('-', Console.WindowWidth));
    }
}
