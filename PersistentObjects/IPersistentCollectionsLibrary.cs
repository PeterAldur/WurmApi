namespace AldursLab.PersistentObjects
{
    public interface IPersistentCollectionsLibrary
    {
        IPersistentCollection GetCollection(string collectionId);
        IPersistentCollection DefaultCollection { get; }
    }
}