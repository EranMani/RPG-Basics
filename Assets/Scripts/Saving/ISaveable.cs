namespace RPG.Saving
{
    // allows to implement saving on any components
    public interface ISaveable
    {
        object CaptureState();
        void RestoreState(object state);
    }
}