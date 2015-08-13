namespace AldursLab.WurmApi.PersistentObjects
{
    public interface IPersistentCollectionsLibrary
    {
        IPersistentCollection GetCollection(string collectionId);
        IPersistentCollection DefaultCollection { get; }
    }
}