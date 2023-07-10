namespace TootTallyDifficultyCalculator2._0
{
    internal static class Program
    {
        public const string MAIN_DIRECTORY = "testSubject/";
        public const string EXPORT_DIRECTORY = "export/";
        public const string REPLAY_DIRECTORY = "replays/";
        public const string DOWNLOAD_DIRECTORY = "downloads/";

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