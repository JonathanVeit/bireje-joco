using JoVei.Base.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using UnityEngine;

namespace JoVei.Base
{
    /// <summary>
    /// Creates and initializes all given Systems
    /// </summary>
    [CreateAssetMenu(fileName = "SystemsLoader", menuName = "JoVei/SystemsLoader")]
    public class SystemsLoader : ScriptableObject, ISystemsLoader
    {
        public enum TargetPlattform { All , OnlyAndroid, OnlyIOS }

        #region Events
        public event Action OnAllSystemsLoaded;
        public event Action<string> OnStartLoadingSystem;
        public event Action<string> OnErrorOccured;
        public event Action<object> OnSystemLoaded;
        #endregion

        #region Exposed Properties
        public int TotalElementsToSetup { get; private set; }
        public float CurrentProgress { get { return ((float)ElementsFinishedToSetup / (float)TotalElementsToSetup); } }
        public bool Finished => CurrentProgress >= 1;
        #endregion

        #region Exposed Member in Inspector
        [SerializeField] private bool InitializeAtStart;

        [SerializeField] private List<LoadableSystemDrawer> SystemsToInitialize;
        #endregion

        #region Private Member
        private int ElementsFinishedToSetup;
        private List<IInitializable> InitializedSystems = new List<IInitializable>();
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnBeforeSceneLoad()
        {
            SystemsLoader initializer = Resources.Load<SystemsLoader>("SystemsLoader");
            initializer.Initialize();
        }

        #region Setup
        public virtual void Initialize()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            ElementsFinishedToSetup = 0;
            TotalElementsToSetup = 0;
            InitializedSystems.Clear();

            CalculateTotalSystemsCount(SystemsToInitialize);
            DebugHelper.Print(LogType.Log, "Setup DIContainer");
            DIContainer.Setup();
            DIContainer.RegisterImplementation<ISystemsLoader>(this);
            DebugHelper.Print(LogType.Log, "Finished setup DIContainer");

            if (InitializeAtStart)
            {
                LoadSystems();
            }
        }

        private void CalculateTotalSystemsCount(List<LoadableSystemDrawer> system)
        {
            foreach (var entry in system)
            {
                if (entry.InitializeOnPlatformsOptions == TargetPlattform.All)
                {
                    TotalElementsToSetup++;
                    continue;
                }

                if (entry.InitializeOnPlatformsOptions == TargetPlattform.OnlyIOS &&
                    Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    TotalElementsToSetup++;
                    continue;
                }

                if (entry.InitializeOnPlatformsOptions == TargetPlattform.OnlyAndroid&&
                     Application.platform == RuntimePlatform.Android)
                {
                    TotalElementsToSetup++;
                    continue;
                }
            }
        }
        #endregion

        #region Functions
        protected virtual void LoadSystems()
        {
            CoroutineHelper.Instance.StartCoroutine(LoadSystemsAsync(SystemsToInitialize));
        }

        private IEnumerator LoadSystemsAsync(List<LoadableSystemDrawer> systems)
        {
            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < systems.Count; i++)
            {
                LoadableSystemDrawer entry = systems[i];

                // check target platforms
                if (entry.InitializeOnPlatformsOptions != TargetPlattform.All)
                {
                    if (Application.platform == RuntimePlatform.IPhonePlayer && 
                        entry.InitializeOnPlatformsOptions == TargetPlattform.OnlyIOS) continue;

                    if (Application.platform == RuntimePlatform.Android && 
                        entry.InitializeOnPlatformsOptions == TargetPlattform.OnlyAndroid) continue;
                }

                // check if system has a monobehaviour
                if (entry.SystemBehaviour == null)
                {
                    DebugHelper.Print(LogType.Error, "No prefab given!");
                    OnErrorOccured?.Invoke(entry.SystemBehaviour.name);
                    yield return 0;
                    continue;
                }

                string systemName = entry.SystemBehaviour.name;
                OnStartLoadingSystem?.Invoke(systemName);

                Type typeOfSystem = GetSystemTypeByName(systemName);
                bool isMonoBehaviour = typeOfSystem.IsSubclassOf(typeof(MonoBehaviour));
                IInitializable loadableSystem;

                // monobehaviours
                if (isMonoBehaviour)
                {
                    loadableSystem = LoadSystemAsMonobehaviour(systemName);
                }
                // None monobehaviours
                else
                {
                    loadableSystem = LoadSystemAsNoneMonobehaviour(systemName);
                }

                if (loadableSystem == null)
                {
                    DebugHelper.PrintFormatted(LogType.Error, "Created system with name '{0}' is not initializable! " +
                        "Make sure your system implements IInitializable.", systemName);
                    OnErrorOccured?.Invoke(systemName);
                    yield return 0;
                    continue;
                }
                
                yield return loadableSystem.Initialize(entry.SerializedParameters.ToArray());
                DebugHelper.PrintFormatted(LogType.Log, "Finished loading system <color=yellow>{0}</color>.", systemName);

                InitializedSystems.Add(loadableSystem);
                OnSystemLoaded?.Invoke(loadableSystem);
                ElementsFinishedToSetup++;
                yield return 0;
            }

            stopWatch.Stop();
            DebugHelper.PrintFormatted(LogType.Log, "Finished Loading Systems in {0} seconds!", stopWatch.Elapsed.Seconds);
            OnAllSystemsLoaded?.Invoke();
        }

        private static IInitializable LoadSystemAsMonobehaviour(string systemName)
        {
            GameObject newGO = new GameObject();
            newGO.name = systemName;
            DontDestroyOnLoad(newGO);
            newGO.AddComponent(GetSystemTypeByName(systemName));

            return newGO.GetComponent<IInitializable>();
        }

        private static IInitializable LoadSystemAsNoneMonobehaviour(string systemName)
        {
            IInitializable loadableSystem;
            object newSystem = CreateInstanceByName(systemName);

            loadableSystem = newSystem as IInitializable;
            return loadableSystem;
        }
        #endregion

        #region Helper
        private static object CreateInstanceByName(string typeName)
        {
            Type typeOfSystem = GetSystemTypeByName(typeName);
            if (typeOfSystem == null)
            {
                return null;
            }

            return Activator.CreateInstance(typeOfSystem);
        }

        private static Type GetSystemTypeByName(string typeName)
        {
            Type foundType = null;
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in asm.GetTypes())
                {
                    if (t.Name == typeName)
                    {
                        foundType = t;
                    }
                }
            }

            return foundType;
        }

        [Serializable]
        private struct LoadableSystemDrawer
        {
            public string Name;
            public UnityEngine.Object SystemBehaviour;
            public TargetPlattform InitializeOnPlatformsOptions;
            public List<UnityEngine.Object> SerializedParameters;
        }
        #endregion
    }
}
