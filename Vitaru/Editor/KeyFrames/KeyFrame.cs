using Prion.Nucleus.Utilities;
using Prion.Nucleus.Utilities.Interfaces;

namespace Vitaru.Editor.KeyFrames
{
    public class KeyFrame : IHasName
    {
        public virtual string Name { get; set; } = nameof(KeyFrame);

        /// <summary>
        /// Time this KeyFrame's <see cref="Effects"/> start.
        /// </summary>
        public double StartTime;

        /// <summary>
        /// Time this KeyFrame's <see cref="Effects"/> end.
        /// </summary>
        public double EndTime;

        public List<KeyFrameEffect> Effects = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public virtual void ApplyKeyFrame(double current)
        {
            float c = (float)PrionMath.Remap(current, StartTime, EndTime);

            for (int i = 0; i < Effects.Count; i++)
                Effects[i].Apply(c);
        }
    }
}
