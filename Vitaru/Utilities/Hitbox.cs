using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vitaru.Utilities
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Hitbox : IHasTeam
    {
        public int Team { get; set; }
    }
}
