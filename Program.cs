using ClosedOffice.Models;

while (true)
{
    Console.CursorVisible = false;
    Console.Write("What would you like to do?");

    /*
     * create [bestandsnaam]: Maak een nieuw tekstbestand met de opgegeven bestandsnaam.
     * open [bestandsnaam]: Open een bestaand tekstbestand om te bewerken.
     * save: Sla de wijzigingen op in het geopende bestand.
     * saveas [nieuwe_bestandsnaam]: Sla het geopende bestand op onder een nieuwe naam.
     * edit: Bewerk het geopende tekstbestand.
     * close: Sluit het geopende tekstbestand en keer terug naar het hoofdmenu.
     * exit: Sluit de ClosedOffice-applicatie.)
     */
    Menu menu = new(
    [
        new("Create file", CreateFile),
        new("Open file", OpenFile),
        new("Exit", () => Environment.Exit(0))
    ]);
}

void OpenFile()
{
    Console.Clear();
    // Get the path of the file
    Console.CursorVisible = true;
    Console.Write("Enter the path of the text file you would like to open: ");
    Console.ForegroundColor = ConsoleColor.Black;
    Console.BackgroundColor = ConsoleColor.Gray;
    Console.WriteLine("(Leave empty to use the TestFile)");
    Console.ResetColor();
    
    string path = Console.ReadLine();
    if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
    {
        path = "./Testfile.txt";
    }

    // TODO: Replace with actual file path
    TextFile textFile = new(path);
    textFile.Open();
}

void CreateFile()
{
    string currentDirectory = Environment.CurrentDirectory;

    if (!Directory.Exists(@$"{currentDirectory}/temp"))
    {
        Directory.CreateDirectory(@$"{currentDirectory}/temp");
    }
    TextFile textFile = new(@$"{currentDirectory}/temp/temp.txt");
    textFile.Create();
    textFile.Open();
}