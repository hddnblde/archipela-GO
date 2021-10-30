using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class ScorableGameModule<T> : GameModule<T>
        where T : ScorableGameModuleConfig
    {
        public abstract int currentScore { get; }
        public abstract int totalScore { get; }

        protected sealed override bool objectiveComplete
        {
            get
            {
                if (config == null)
                    return false;

                return (currentScore >= config.passingScore);
            }
        }

        public int roundedProgress
        {
            get
            {
                if (totalScore <= 0)
                    return 0;

                return Mathf.FloorToInt((currentScore * 1f) / (totalScore * 1f) * 100f);
            }
        }
    }
}