using System;
using System.Collections.Generic;
using System.Text;

namespace Vitaru.Server.Server
{
    [Serializable]
    public class Setting<T> : Setting
    {
        public T Value;
    }

    [Serializable]
    public class Setting
    {
        public string Name;

        public Sync Sync;
    }

    public enum Sync
    {
        None,
        Client,
        All,
    }
}
