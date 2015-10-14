using System;
using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    public abstract class ServerGroup
    {
        public abstract ServerGroupId ServerGroupId { get; }

        public virtual string Name
        {
            get { return ServerGroupId.ToString(); }
        }

        public virtual string Description
        {
            get { return ServerGroupId.ToString(); }
        }

        protected bool Equals(ServerGroup other)
        {
            return other.ServerGroupId.Equals(this.ServerGroupId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as ServerGroup;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return ServerGroupId.GetHashCode();
        }
    }

    public enum ServerGroupId
    {
        Unknown = 0,
        Freedom = 1,
        Epic = 2,
        Challenge = 3
    }
}
