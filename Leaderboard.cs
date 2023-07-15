using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TootTallyDifficultyCalculator2._0
{
    public class Leaderboard
    {
        public int count;
        public string next;
        public string previous;
        public string songHash;
        public List<ScoreDataFromDB> results;


        [Serializable]
        public class ScoreDataFromDB
        {
            public int score;
            public string player;
            public string played_on;
            public string grade;
            public int[] noteTally;
            public string replay_id;
            public int max_combo;
            public float percentage;
            public string game_version;
            public float tt;
            public bool is_rated;
            public float replay_speed;
        }
    }
}
