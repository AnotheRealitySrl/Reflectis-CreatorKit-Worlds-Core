
using System.Collections.Generic;


using SPACS.Utilities;

namespace Virtuademy.SDK.Environments.Analytics
{
    public abstract class DisplayableContentBase
    {
        public abstract void CheckValidity();

        public abstract void AssignValues(List<Field> args);
    }
}
