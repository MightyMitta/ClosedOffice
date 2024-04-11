namespace ClosedOffice.Models;
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
        int index = 0;

        // Write the menu in the console
        WriteMenu(options[index]);

        // Store the pressed key info
        ConsoleKeyInfo keyinfo;
        do
        {
            keyinfo = Console.ReadKey(true);

            // Handle each key input (down arrow will write the menu again with a different selected item)
            if (keyinfo.Key == ConsoleKey.DownArrow)
            {
                if (index + 1 < options.Count)
                {
                    index++;
                    WriteMenu(options[index]);
                }
            }
            if (keyinfo.Key == ConsoleKey.UpArrow)
            {
                if (index - 1 >= 0)
                {
                    index--;
                    WriteMenu(options[index]);
                }
            }
            
            // Invoke the selected option when the enter key is pressed
            if (keyinfo.Key == ConsoleKey.Enter)
            {
                options[index].Selected.Invoke();
                index = 0;
            }
        }
        // TODO: Change this to a more appropriate condition
        while (index != -1);
    }

    public void WriteMenu(Option selectedOption)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.WriteLine("Navigate options using arrow up/down, select option by pressing Enter");
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
    }
}
