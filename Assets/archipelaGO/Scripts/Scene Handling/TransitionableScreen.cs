using System.Collections;
using UnityEngine;

namespace archipelaGO.SceneHandling
{
    public abstract class TransitionableScreen : MonoBehaviour
    {
        public abstract IEnumerator WaitUntilDone();
    }
}