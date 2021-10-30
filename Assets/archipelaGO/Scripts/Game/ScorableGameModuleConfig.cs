using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class ScorableGameModuleConfig : GameModuleConfig
    {
        [SerializeField]
        private int m_passingScore = 1;

        public int passingScore => m_passingScore;
    }
}