using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TootTallyDifficultyCalculator2._0
{
    public class ReplayData
    {
        public string username;
        public string starttime;
        public string endtime;
        public string uuid;
        public string input;
        public string song;
        public float samplerate;
        public float scrollspeed;
        public float gamespeedmultiplier;
        public int pluginbuilddate;
        public string gameversion;
        public string songhash;
        public int finalscore;
        public int maxcombo;
        public int[] finalnotetallies;
        public List<int[]> framedata;
        public List<int[]> notedata;
        public List<int[]> tootdata;

        public void OnDeserialize()
        {

        }
    }
}
