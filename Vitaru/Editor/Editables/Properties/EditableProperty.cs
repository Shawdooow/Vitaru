namespace Vitaru.Editor.Editables.Properties
{
    public abstract class EditableProperty<T> : EditableProperty
        where T : struct
    {
        public abstract T Value { get; set; }

        protected EditableProperty(IEditable e) : base(e)
        {
        }
    }    
    
    public abstract class EditableProperty
    {
        protected EditableProperty(IEditable e)
        {
        }
    }
}
