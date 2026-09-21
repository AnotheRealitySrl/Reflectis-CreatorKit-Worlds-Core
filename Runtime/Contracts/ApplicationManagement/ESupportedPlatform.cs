﻿
using System;

namespace Virtuademy.SDK.Core.ApplicationManagement
{
    /// <summary>
    /// The platforms a world can be authored for.
    /// </summary>
    /// <remarks>
    /// Stays in the authoring package while <c>IPlatformSystem</c> moves to the main project: this
    /// is a value a world is authored against - the "Virtuademy Platform: Switch" node branches on
    /// it and <c>CMEnvironment</c> stores it - whereas the system that reports it belongs to the
    /// application. The namespace is unchanged so that no consumer's <c>using</c> moves.
    /// </remarks>
    [Flags]
    public enum ESupportedPlatform
    {
        VR = 1,
        WebGL = 2,
        Mobile = 4,
    }
}
