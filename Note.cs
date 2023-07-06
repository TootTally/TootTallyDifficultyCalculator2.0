using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TootTallyDifficultyCalculator2._0
{
    public class Note
    {
        public int count;
        public float pitchStart, pitchDelta, pitchEnd;
        public double position, length;
        public bool isSlider = false;

        public Note(int count, double position, double length, float pitchStart, float pitchDelta, float pitchEnd)
        {
            this.count = count;
            this.position = position;
            this.length = length;
            this.pitchStart = pitchStart;
            this.pitchDelta = pitchDelta;
            this.pitchEnd = pitchEnd;
        }

        public void SetIsSlider(bool isSlider) => this.isSlider = isSlider;

    }
}
