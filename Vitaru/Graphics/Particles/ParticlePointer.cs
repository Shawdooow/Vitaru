using System.Numerics;

namespace Vitaru.Graphics.Particles
{
    public class ParticlePointer
    {
        public int Index { get; internal set; }

        public Vector2 StartPosition
        {
            get => new Vector2(model.M11, model.M12);
            set
            {
                model.M11 = value.X;
                model.M12 = value.Y;
            }
        }

        public Vector2 EndPosition
        {
            get => new Vector2(model.M21, model.M22);
            set
            {
                model.M21 = value.X;
                model.M22 = value.Y;
            }
        }

        public Vector3 Color
        {
            get => new Vector3(model.M31, model.M32, model.M33);
            set
            {
                model.M31 = value.X;
                model.M32 = value.Y;
                model.M33 = value.Z;
            }
        }

        public float StartTime
        {
            get => model.M13;
            set => model.M13 = value;
        }

        public float EndTime
        {
            get => model.M14;
            set => model.M14 = value;
        }

        public float Scale
        {
            get => model.M23;
            set => model.M23 = value;
        }

        public float Rotation
        {
            get => model.M24;
            set => model.M24 = value;
        }

        public float Alpha
        {
            get => model.M34;
            set => model.M34 = value;
        }

        private Matrix4x4 model;

        internal ParticlePointer(Matrix4x4 model)
        {
            this.model = model;
        }

        public void Update() => ParticleManager.Master[Index] = model;
    }
}
