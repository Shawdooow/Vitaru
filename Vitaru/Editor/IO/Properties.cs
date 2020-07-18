namespace Vitaru.Editor.IO
{
    public class EditableDouble : EditableProperty
    {
        public double Value;

        public EditableDouble(double value)
        {
            Value = value;
        }
    }

    public abstract class EditableProperty
    {
    }
}
