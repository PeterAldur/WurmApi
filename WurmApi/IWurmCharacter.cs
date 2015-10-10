using System;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Wurm.Characters;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Provides means of obtaining wurm character-specific details.
    /// </summary>
    public interface IWurmCharacter
    {
        #region GetHistoricServerAtLogStamp

        /// <summary>
        /// Returns exact server, that the player was on at given stamp.
        /// </summary>
        /// <exception cref="DataNotFoundException"></exception>
        Task<IWurmServer> GetHistoricServerAtLogStampAsync(DateTime stamp);

        /// <summary>
        /// Returns exact server, that the player was on at given stamp.
        /// </summary>
        /// <exception cref="DataNotFoundException"></exception>
        IWurmServer GetHistoricServerAtLogStamp(DateTime stamp);

        /// <summary>
        /// Returns exact server, that the player was on at given stamp.
        /// </summary>
        /// <exception cref="DataNotFoundException"></exception>
        Task<IWurmServer> GetHistoricServerAtLogStampAsync(DateTime stamp, CancellationToken cancellationToken);

        /// <summary>
        /// Returns exact server, that the player was on at given stamp.
        /// </summary>
        /// <exception cref="DataNotFoundException"></exception>
        IWurmServer GetHistoricServerAtLogStamp(DateTime stamp, CancellationToken cancellationToken);

        #endregion

        #region GetCurrentServer

        /// <summary>
        /// Returns exact server, that the player is currently on.
        /// </summary>
        /// <exception cref="DataNotFoundException"></exception>
        Task<IWurmServer> GetCurrentServerAsync();

        /// <summary>
        /// Returns exact server, that the player is currently on.
        /// </summary>
        /// <exception cref="DataNotFoundException"></exception>
        IWurmServer GetCurrentServer();

        /// <summary>
        /// Returns exact server, that the player is currently on.
        /// </summary>
        /// <exception cref="DataNotFoundException"></exception>
        Task<IWurmServer> GetCurrentServerAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns exact server, that the player is currently on.
        /// </summary>
        /// <exception cref="DataNotFoundException"></exception>
        IWurmServer GetCurrentServer(CancellationToken cancellationToken);

        #endregion

        /// <summary>
        /// Triggered, when WurmApi detects, that this character has logged into a server.
        /// This may indicate login into the game or traveling to another server.
        /// </summary>
        event EventHandler<PotentialServerChangeEventArgs> LogInOrCurrentServerPotentiallyChanged;

        /// <summary>
        /// In-game name of this character.
        /// </summary>
        CharacterName Name { get; }

        /// <summary>
        /// Current config for this character. 
        /// Returns null, if config does not exist or could not be read.
        /// </summary>
        IWurmConfig CurrentConfig { get; }

        /// <summary>
        /// Query current skill levels of this character and subscribe for changes.
        /// </summary>
        IWurmCharacterSkills Skills { get; }
    }
}