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

        public void AddNote(Note note) => noteList.Add(note);

        public double GetClusterSize() => noteList.Max(x  => x.position + x.length) - noteList.Min(x => x.position);
    }
}
