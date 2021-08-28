using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class GameElement : MonoBehaviour
    {
        public abstract void Initialize(GameConfig config);
    }
}