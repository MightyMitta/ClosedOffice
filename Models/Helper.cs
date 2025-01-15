namespace ClosedOffice.Models
{
    public class Config
    {
        private static Config _instance;
        public static Config Instance => _instance ??= new Config();

        public string DefaultPath { get; set; } = "./";  // Standaardpad voor bestanden
    }
}

public static class Helper
{
    public static int ExitMenu = 0;
}