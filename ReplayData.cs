namespace TootTallyDifficultyCalculator2._0
{
    public class ReplayData
    {
        public const float NOTE_OFFSET = 0.05f;
        public const float ELECTRO_OFFSET = -0.02f;
        public const float ZERO_X_POS = 60f;

        public string version;
        public string username;
        public string starttime;
        public string endtime;
        public string uuid;
        public string input;
        public string song;
        public float samplerate;
        public float scrollspeed;
        public float gamespeedmultiplier;
        public string gamemodifiers;
        public float audiolatency;
        public int pluginbuilddate;
        public string gameversion;
        public string songhash;
        public int finalscore;
        public int maxcombo;
        public int[] finalnotetallies;
        public List<dynamic[]> framedata;
        public List<dynamic[]> notedata;
        public List<dynamic[]> tootdata;

        private TMBChart _chartData;

        public void OnDeserialize()
        {
            gamemodifiers ??= "None";
            version ??= "0.0.0";
        }

        public void SetChart(TMBChart chart) => _chartData = chart;

        public List<string> ToDisplayData()
        {
            List<string> data = new List<string>
            {
                "___REPLAY DATA___",
                "version: " + version,
                "username: " + username,
                "starttime: " + starttime,
                "endtime: " + endtime,
                "uuid: " + uuid,
                "input: " + input,
                "song: " + song,
                "samplerate: " + samplerate,
                "scrollspeed: " + scrollspeed,
                "gamespeedmultiplier: " + gamespeedmultiplier,
                "gamemodifiers: " + gamemodifiers,
                "audiolatency: " + audiolatency,
                "pluginbuilddate: " + pluginbuilddate,
                "gameversion: " + gameversion,
                "songhash: " + songhash,
                "finalscore: " + finalscore,
                "maxcombo: " + maxcombo,
            };

            data.AddRange(GetToDisplayFrameData(framedata));
            data.AddRange(GetToDisplayTootData(tootdata));
            data.AddRange(GetToDisplayNoteData(notedata));


            return data;
        }

        private List<string> GetToDisplayFrameData(List<int[]> frameData)
        {

            List<string> stringList = new List<string>();

            for (int i = 0; i < frameData.Count; i++)
            {
                var time = ReplayPositionToTime(frameData[i][(int)FrameDataStructure.NoteHolder]);

                stringList.Add($"In:{i} " +
                    $"Po: {frameData[i][(int)FrameDataStructure.NoteHolder]} " +
                    $"Ti: {time} " +
                    $"Pp: {frameData[i][(int)FrameDataStructure.PointerPosition]} " +
                    $"MX: {frameData[i][(int)FrameDataStructure.MousePositionX]} " +
                    $"MY: {frameData[i][(int)FrameDataStructure.MousePositionY]}");
            }
            return stringList;
        }

        private double ReplayPositionToTime(float position)
        {
            position /= GetNoteHolderPrecisionMultiplier();
            var trackmovemult = (_chartData.tempo / 60f) * Math.Floor(_chartData.savednotespacing * scrollspeed);
            var time = (position - ZERO_X_POS) / -trackmovemult;
            return (time + NOTE_OFFSET + Ms2Sec(audiolatency)) / gamespeedmultiplier;
        }

        public static float Ms2Sec(float ms) => ms / 1000f;

        public float GetNoteHolderPrecisionMultiplier() => 10 / (scrollspeed <= 1 ? scrollspeed : 1);

        private List<string> GetToDisplayTootData(List<dynamic[]> tootData)
        {
            List<string> stringList = new List<string>();

            for (int i = 0; i < tootData.Count; i++)
            {
                var time = ReplayPositionToTime(tootData[i][(int)TootDataStructure.NoteHolder]);

                stringList.Add($"In:{i} " +
                    $"Po: {tootData[i][(int)TootDataStructure.NoteHolder]} " +
                    $"Ti: {time} ");
            }
            return stringList;
        }

        private List<string> GetToDisplayNoteData(List<int[]> noteData)
        {
            List<string> stringList = new List<string>();

            for (int i = 0; i < noteData.Count; i++)
            {

                stringList.Add($"Ni: {noteData[i][(int)NoteDataStructure.NoteIndex]} " +
                    $"Ts: {noteData[i][(int)NoteDataStructure.TotalScore]} " +
                    $"Mu: {noteData[i][(int)NoteDataStructure.Multiplier]} " +
                    $"Hp: {noteData[i][(int)NoteDataStructure.CurrentHealth]} " +
                    $"Nj: {noteData[i][(int)NoteDataStructure.NoteJudgement]} " +
                    $"Ns: {noteData[i][(int)NoteDataStructure.NoteScore]}");
            }
            return stringList;
        }

        private enum FrameDataStructure
        {
            NoteHolder = 0,
            PointerPosition = 1,
            MousePositionX = 2,
            MousePositionY = 3,
        }

        private enum TootDataStructure
        {
            NoteHolder = 0,
        }

        private enum NoteDataStructure
        {
            NoteIndex = 0,
            TotalScore = 1,
            Multiplier = 2,
            CurrentHealth = 3,
            NoteJudgement = 4,
            NoteScore = 5,
        }
    }
}
