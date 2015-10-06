using System;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;

namespace AldursLab.WurmApi
{
    [JsonObject(MemberSerialization.Fields)]
    public sealed class CharacterName : IEquatable<CharacterName>
    {
        private readonly string normalizedName;
        static readonly CharacterName EmptyName = new CharacterName("");

        public static CharacterName Empty
        {
            get
            {
                return EmptyName;
            }
        }

        public CharacterName(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            this.normalizedName = name.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// All uppercase character name
        /// </summary>
        public string Normalized { get { return normalizedName; } }
        /// <summary>
        /// Name with first letter capitalized.
        /// </summary>
        public string Capitalized { get { return UnnormalizeCharacterName(normalizedName); } }

        /// <summary>
        /// Converts a string in such way, that first char is uppercase and remaining chars are lowercase
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string UnnormalizeCharacterName(string source)
        {
            Debug.Assert(source != null);
            source = source ?? string.Empty;
            if (source.Length > 0)
            {
                var result = source.Substring(0, 1).ToUpperInvariant();
                if (source.Length > 1)
                {
                    result += source.Substring(1, source.Length - 1).ToLowerInvariant();
                }
                return result;
            }
            return string.Empty;
        }

        public bool Equals(CharacterName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(normalizedName, other.normalizedName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as CharacterName);
        }

        public override int GetHashCode()
        {
            return (normalizedName != null ? normalizedName.GetHashCode() : 0);
        }

        public static bool operator ==(CharacterName left, CharacterName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CharacterName left, CharacterName right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Capitalized.ToString(CultureInfo.InvariantCulture);
        }

        public bool IsEmpty { get { return string.IsNullOrWhiteSpace(normalizedName); } }
    }
}
