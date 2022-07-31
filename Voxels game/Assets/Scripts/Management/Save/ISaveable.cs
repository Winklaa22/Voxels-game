namespace Management.Save
{
    public interface ISaveable
    {
        object CatureState();

        void RestoreState(object state);
    }
}
