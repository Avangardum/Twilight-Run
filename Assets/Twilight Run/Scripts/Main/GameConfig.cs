using Avangardum.TwilightRun.Models;
using UnityEngine;

namespace Avangardum.TwilightRun.Main
{
    public class GameConfig : ScriptableObject, IGameConfig
    {
        [field: SerializeField] public float CharacterHorizontalSpeed { get; private set; }
        [field: SerializeField] public float CharacterVerticalSpeed { get; private set; }
        [field: SerializeField] public float MinCharacterYPosition { get; private set; }
        [field: SerializeField] public float MaxCharacterYPosition { get; private set; }
    }
}