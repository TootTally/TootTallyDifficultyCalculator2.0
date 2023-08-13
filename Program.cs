namespace TootTallyDifficultyCalculator2._0
{
    internal static class Program
    {
        public const string EXPORT_DIRECTORY = "export/";
        public const string DOWNLOAD_DIRECTORY = "downloads/";
        public const string CACHE_DIRECTORY = "cache/";

        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}