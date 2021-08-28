using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class GameElementController : MonoBehaviour
    {
        public abstract void Initialize(GameConfig config);
    }
}