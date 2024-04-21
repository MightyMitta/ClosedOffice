using ClosedOffice.Models;

string superSecretKey = "MitchelTieges";

string textToEnrypt = "Hallo, mijn naam is Mitchel";

Console.WriteLine("Original text: " + textToEnrypt);

string encryptedText = Encrypt(textToEnrypt);
Console.WriteLine("Encrypted text: " + encryptedText);


string Encrypt(string text)
{
    string encryptedText = "";
    int encryptChar = 0;

    foreach (char character in text)
    {
        int newChar;

        if (!char.IsLetter(character))
        {
            encryptedText += character;
            continue;
        }

        if (char.IsUpper(character))
        {
            var x = character + (superSecretKey[encryptChar] - 65);

            if (x > 90)
            {
                x -= 26;
            }

            newChar = x;
        }
        else
        {
            var x = character + (superSecretKey[encryptChar] - 97);

            if (x > 122)
            {
                x -= 26;
            }

            newChar = x;
        }

        encryptedText += (char)newChar;

        encryptChar = encryptChar == superSecretKey.Length ? 0 : encryptChar + 1;
    }
    return encryptedText;
}

Console.ReadLine();


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
    Console.Clear();
    Console.CursorVisible = true;
    string currentDirectory = Environment.CurrentDirectory;
    Console.Write("Enter filename: ");
    string fileName = Console.ReadLine();

    if (File.Exists(@$"{currentDirectory}/{fileName}.txt"))
    {
        Console.WriteLine("File already exists.");
        Console.ReadKey(true);
        return;
    }

    TextFile textFile = new(@$"{currentDirectory}\{fileName}.txt");
    textFile.Create();
    Console.WriteLine($"File {fileName}.txt created in:");
    Console.WriteLine($@"{currentDirectory}\{fileName}.txt");
    Console.ReadKey(true);
    Console.CursorVisible = false;
}