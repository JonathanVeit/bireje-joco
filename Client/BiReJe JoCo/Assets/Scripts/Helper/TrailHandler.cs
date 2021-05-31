using System;
using UnityEngine;
using JoVei.Base.PoolingSystem;
using JoVei.Base.Helper;

namespace BiReJeJoCo
{
    /// <summary>
    /// Control for trails (inspector only) as PoolablePrefab
    /// </summary>
    public class TrailHandler : PoolablePrefab
    {
        [Header("Settings")]
        [SerializeField] TrailRenderer trailRenderer;
        [SerializeField] Option[] options;

        public TrailRenderer TrailRenderer { get { return trailRenderer; } }
        public event Action<Condition> OnConditionMet;

        // get particle system
        void Awake()
        {
            if (!(trailRenderer is null)) trailRenderer = GetComponent<TrailRenderer>();
        }

        // detect changes 
        void Update()
        {
            // has system
            if (!(trailRenderer is null))
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
                case Condition.IsEmitting:
                    return trailRenderer.emitting;
                case Condition.IsNotEmitting:
                    return !trailRenderer.emitting;
                case Condition.IsVisible:
                    return trailRenderer.isVisible;
                case Condition.IsNotVisible:
                    return !trailRenderer.isVisible;

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
            IsEmitting = 1,
            IsNotEmitting = 2,
            IsVisible = 3,
            IsNotVisible = 4,
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