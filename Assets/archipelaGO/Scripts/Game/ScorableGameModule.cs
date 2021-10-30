using UnityEngine;

namespace archipelaGO.Game
{
    public interface IScorableModule
    {
        int currentScore { get; }
        int totalScore { get; }
        int roundedProgress { get; }
    }

    public abstract class ScorableGameModule<T> : GameModule<T>, IScorableModule
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