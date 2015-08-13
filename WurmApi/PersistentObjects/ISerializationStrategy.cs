namespace AldursLab.WurmApi.PersistentObjects
{
    public interface ISerializationStrategy
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="DeserializationErrorsException{TEntity}">
        /// At least one deserialization error occurred and was not handled. Decision is required. 
        /// A fallback TEntity should be available, with as much data, as was successfully deserialized.
        /// </exception>
        /// <exception cref="DeserializationNullResultException{TEntity}">
        /// Deserialization has returned a null value, which might indicate corrupted or invalid source data.
        /// A fallback TEntity should be available, with an entity initialized to default state.
        /// </exception>
        TEntity Deserialize<TEntity>(string source) where TEntity : Entity, new();

        string Serialize<TEntity>(TEntity entity) where TEntity : Entity, new();
    }
}