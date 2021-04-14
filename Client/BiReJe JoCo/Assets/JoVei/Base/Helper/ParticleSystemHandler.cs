using System;
using UnityEngine;
using JoVei.Base.PoolingSystem;

namespace JoVei.Base.Helper
{
    /// <summary>
    /// Control for particle systems (inspector only) as PoolablePrefab
    /// </summary>
    public class ParticleSystemHandler : PoolablePrefab
    {
        [Header("Settings")]
        [SerializeField] ParticleSystem system;
        [SerializeField] Option[] options;

        public ParticleSystem ControlledSystem { get { return system; } }
        public event Action<Condition> OnConditionMet;

        // get particle system
        void Awake()
        {
            if (!(system is null)) system = GetComponent<ParticleSystem>();
        }

        // detect changes 
        void Update()
        {
            // has system
            if (!(system is null))
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
                case Condition.IsAlive:
                    return system.IsAlive(true) == true;
                case Condition.IsNotAlive:
                    return system.IsAlive(true) == false;
                case Condition.isPaused:
                    return system.isPaused == true;
                case Condition.isNotPaused:
                    return system.isPaused == false;

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
            IsAlive     = 1,
            IsNotAlive  = 2,
            isPaused    = 3,
            isNotPaused = 4,
        }

        public enum Command
        {
            FireEvent = 1,
            ReturnToPool = 2,
            ReleaseFromPool = 3,
            Deactivate = 4,
        }
        #endregion
    }
}