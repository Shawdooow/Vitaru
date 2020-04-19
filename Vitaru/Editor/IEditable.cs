namespace Vitaru.Editor
{
    public interface IEditable
    {
        void ParseString(string[] data);

        string[] SerializeToStrings();
    }
}
