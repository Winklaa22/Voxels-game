namespace Management.Save_System
{
    public interface ISaveable
    {
        object CaptureState();

        void RestoreState(object state);
    }
}
