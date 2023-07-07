using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TootTallyDifficultyCalculator2._0.Chart;

namespace TootTallyDifficultyCalculator2._0
{
    public class NoteCluster
    {
        public List<Note> noteList;

        public List<DataVector> aimPerformanceList;
        public List<DataVector> tapPerformanceList;
        public List<DataVector> accPerformanceList;
        public DataVectorAnalytics aimAnalytics;
        public DataVectorAnalytics tapAnalytics;
        public DataVectorAnalytics accAnalytics;

        public NoteCluster()
        {
            noteList = new List<Note>();
            aimPerformanceList = new List<DataVector>();
            tapPerformanceList = new List<DataVector>();
            accPerformanceList = new List<DataVector>();
        }

        public void CalculateDifficulty()
        {

        }

        public void AddNotes(params Note[] note) => noteList.AddRange(note);

        public double GetClusterSize() => GetClusterEnd() - GetClusterStart();

        public double GetClusterEnd() => noteList.Max(x => x.position + x.length);

        public double GetClusterStart() => noteList.Min(x => x.position);

        public double GetDistanceFromCluster(NoteCluster noteCluster) => noteCluster.GetClusterStart() - GetClusterEnd();
    }
}
