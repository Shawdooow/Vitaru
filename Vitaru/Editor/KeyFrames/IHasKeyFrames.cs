using Vitaru.Editor.Editables.Properties.Time;

namespace Vitaru.Editor.KeyFrames
{
    public interface IHasKeyFrames : IHasStartTime, IHasEndTime
    {
        List<KeyFrame> KeyFrames { get; set; }

        protected void ApplyKeyFrames()
        {
            double current = Clock.Current;
            for (int i = 0; i < KeyFrames.Count; i++)
                KeyFrames[i].ApplyKeyFrame(current);
        }
    }
}
