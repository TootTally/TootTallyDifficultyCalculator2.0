namespace TootTallyDifficultyCalculator2._0
{
    internal static class Program
    {
        public const string MAIN_DIRECTORY = "testSubject/";
        public const string RATED_DIRECTORY = "RatedCharts/";
        public const string RATED_TMBS_DIRECTORY = "RatedTmbs/";
        public const string REPLAY_DIRECTORY = "replays/";
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