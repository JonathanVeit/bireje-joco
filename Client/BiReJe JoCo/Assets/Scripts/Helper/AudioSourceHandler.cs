using System;
using UnityEngine;
using JoVei.Base.PoolingSystem;
using JoVei.Base.Helper;

namespace BiReJeJoCo
{
    /// <summary>
    /// Control for audio sources (inspector only) as PoolablePrefab
    /// </summary>
    public class AudioSourceHandler : PoolablePrefab
    {
        [Header("Settings")]
        [SerializeField] AudioSource audioSource;
        [SerializeField] Option[] options;

        public AudioSource AudioSource { get { return audioSource; } }
        public event Action<Condition> OnConditionMet;

        // get particle system
        void Awake()
        {
            if (!(audioSource is null)) audioSource = GetComponent<AudioSource>();
        }

        // detect changes 
        void Update()
        {
            // has system
            if (!(audioSource is null))
            {
                foreach (var curOption in options)
                {
                    // check condition
                    if (ConditionMet(curOption.condition))
                    {
                        // run command
                        RunCommand(curOption.command, curOption.condition);
                    }
                }
            }
        }

        #region Helper
        private bool ConditionMet(Condition condition)
        {
            switch (condition)
            {
                // conditions
                case Condition.IsPlaying:
                    return audioSource.isPlaying;
                case Condition.IsNotPlaying:
                    return !audioSource.isPlaying;

                // default
                default:
                    DebugHelper.PrintFormatted(LogType.Warning, "Condition {0} is not implemented yet", condition.ToString());
                    return false;
            }
        }

        private void RunCommand(Command command, Condition forCondition)
        {
            switch (command)
            {
                case Command.FireEvent:
                    OnConditionMet?.Invoke(forCondition);
                    break;
                case Command.ReturnToPool:
                    RequestReturnToPool();
                    break;
                case Command.ReleaseFromPool:
                    RequestReleaseFromPool();
                    break;
                case Command.Deactivate:
                    this.gameObject.SetActive(false);
                    break;
                case Command.Stop:
                    AudioSource.Stop();
                    break;

                // not implemented
                default:
                    DebugHelper.PrintFormatted(LogType.Warning, "Command {0} is not implemented yet", command.ToString());
                    break;
            }
        }
        #endregion

        #region Commands & Conditions
        [System.Serializable]
        private struct Option
        {
            [SerializeField] string name;
            public Condition condition;
            public Command command;
        }

        public enum Condition
        {
            IsPlaying = 1,
            IsNotPlaying = 2,
        }

        public enum Command
        {
            FireEvent = 1,
            ReturnToPool = 2,
            ReleaseFromPool = 3,
            Deactivate = 4,
            Stop = 5,
        }
        #endregion
    }
}