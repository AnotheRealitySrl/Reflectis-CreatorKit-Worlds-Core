using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ColorPicker
{
    public interface IColorPickerSystem : ISystem
    {
        Task AssignColorPicker(GameObject obj, bool networkedContext = true);

    }
}

