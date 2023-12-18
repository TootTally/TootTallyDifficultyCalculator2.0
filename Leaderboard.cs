namespace TootTallyDifficultyCalculator2._0
{
    public class Leaderboard
    {
        public int count;
        public string next;
        public string previous;
        public SongInfoFromDB song_info;
        public List<ScoreDataFromDB> results;


        [Serializable]
        public class ScoreDataFromDB
        {
            public int score;
            public int max_combo;
            public string player;
            public string grade;
            public float percentage;
            public float tt;
            public bool is_rated;
            public int song_id;
            public float replay_speed;
            public string[] modifiers;
        }

        [Serializable]
        public class SongInfoFromDB
        {
            public int id;
            public string file_hash;
            public string track_ref;
        }


    }
}
