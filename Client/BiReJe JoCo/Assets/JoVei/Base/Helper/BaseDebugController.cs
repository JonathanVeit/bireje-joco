using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoVei.Base.Helper
{
    /// <summary>
    /// Allows debug commands within the build/editor
    /// </summary>
    public abstract class BaseDebugController : BaseSystemBehaviour, IDebugController, IInitializable
    {
        protected bool DebugPanelIsOpen { get; private set; }
        protected List<BaseDebugCommand> Commands { get; private set; } = new List<BaseDebugCommand>();

        #region Show/Hide
        protected virtual void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
            {
                ToggleVisibility();
            }
            else if (Input.touchCount > 1)
            {
                if (Input.GetTouch(0).deltaPosition.x > 1 && Input.GetTouch(1).deltaPosition.x < -1 ||
                    Input.GetTouch(0).deltaPosition.x < -1 && Input.GetTouch(1).deltaPosition.x > 1)
                {
                    ToggleVisibility();
                }
            }
        }

        private void ToggleVisibility() 
        {
            if (DebugPanelIsOpen)
            {
                CloseDebugPanel();
                DebugPanelIsOpen = false;
            }
            else
            {
                OpenDebugPanel();
                DebugPanelIsOpen = true;
            }
        }

        protected virtual void OpenDebugPanel() { }
        protected virtual void CloseDebugPanel() { }
        #endregion

        #region Run Commands
        public void RunCommand(string rawCommand)
        {
            if (string.IsNullOrEmpty(rawCommand)) return;

            var commandParts = rawCommand.Split(' ');
            string commandId = string.Empty;

            foreach (var curCommand in Commands)
            {
                if (rawCommand.Contains(curCommand.Id))
                {
                    commandId = curCommand.Id;

                    // default command
                    if (curCommand is DebugCommand defaultCommand)
                    {
                        defaultCommand.Invoke();
                    }
                    // missing parameter
                    else if (commandParts.Length < 2)
                    {
                        OnRunCommandFailed(commandId, "Parameter is missing");
                        return;
                    }
                    // single generic command
                    else if (curCommand is DebugCommand<string> stringCommand)
                    {
                        stringCommand.Invoke(commandParts[1]);
                    }
                    else if (curCommand is DebugCommand<int> intCommand)
                    {
                        int parameter;
                        try { parameter = int.Parse(commandParts[1]); } 
                        catch 
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        intCommand.Invoke(parameter);
                    }
                    else if (curCommand is DebugCommand<float> floatCommand)
                    {
                        float parameter;
                        try { parameter = float.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        floatCommand.Invoke(parameter);
                    }
                    else if (curCommand is DebugCommand<bool> boolCommand)
                    {
                        bool parameter;
                        try { parameter = bool.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        boolCommand.Invoke(parameter);
                    }                    
                    // missing parameter
                    else if (commandParts.Length < 3)
                    {
                        OnRunCommandFailed(commandId, "Parameter is missing");
                        return;
                    }
                    // double generic command string + x
                    else if (curCommand is DebugCommand<string, string> stringStringCommand)
                    {
                        stringStringCommand.Invoke(commandParts[1], commandParts[2]);
                    }
                    else if (curCommand is DebugCommand<string, int> stringIntCommand)
                    {
                        string parameter1 = commandParts[1];
                        int parameter2;
                        try { parameter2 = int.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        stringIntCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<string, float> stringFloatCommand)
                    {
                        string parameter1 = commandParts[1];
                        float parameter2;
                        try { parameter2 = float.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        stringFloatCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<string, bool> stringBoolCommand)
                    {
                        string parameter1 = commandParts[1];
                        bool parameter2;
                        try { parameter2 = bool.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        stringBoolCommand.Invoke(parameter1, parameter2);
                    }
                    // double generic command int + x
                    else if (curCommand is DebugCommand<int, string> intStringCommand)
                    {
                        int parameter1;
                        string parameter2 = commandParts[2];
                        try { parameter1 = int.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        intStringCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<int, int> intIntCommand)
                    {
                        int parameter1;
                        int parameter2;
                        try { parameter1 = int.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }
                        try { parameter2 = int.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        intIntCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<int, float> intFloatCommand)
                    {
                        int parameter1;
                        float parameter2;
                        try { parameter1 = int.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }
                        try { parameter2 = float.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        intFloatCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<int, bool> intBoolCommand)
                    {
                        int parameter1;
                        bool parameter2;
                        try { parameter1 = int.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }
                        try { parameter2 = bool.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        intBoolCommand.Invoke(parameter1, parameter2);
                    }
                    // double generic command float + x
                    else if (curCommand is DebugCommand<float, string> floatStringCommand)
                    {
                        float parameter1;
                        string parameter2 = commandParts[2];
                        try { parameter1 = float.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        floatStringCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<float, int> floatIntCommand)
                    {
                        float parameter1;
                        int parameter2;
                        try { parameter1 = float.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }
                        try { parameter2 = int.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        floatIntCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<float, float> floatFloatCommand)
                    {
                        float parameter1;
                        float parameter2;
                        try { parameter1 = float.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }
                        try { parameter2 = float.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        floatFloatCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<float, bool> floatBoolCommand)
                    {
                        float parameter1;
                        bool parameter2;
                        try { parameter1 = float.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }
                        try { parameter2 = bool.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        floatBoolCommand.Invoke(parameter1, parameter2);
                    }
                    // double generic command bool + x
                    else if (curCommand is DebugCommand<bool, string> boolStringCommand)
                    {
                        bool parameter1;
                        string parameter2 = commandParts[2];
                        try { parameter1 = bool.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        boolStringCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<bool, int> boolIntCommand)
                    {
                        bool parameter1;
                        int parameter2;
                        try { parameter1 = bool.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }
                        try { parameter2 = int.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        boolIntCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<bool, float> boolFloatCommand)
                    {
                        bool parameter1;
                        float parameter2;
                        try { parameter1 = bool.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }
                        try { parameter2 = float.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        boolFloatCommand.Invoke(parameter1, parameter2);
                    }
                    else if (curCommand is DebugCommand<bool, bool> boolBoolCommand)
                    {
                        bool parameter1;
                        bool parameter2;
                        try { parameter1 = bool.Parse(commandParts[1]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }
                        try { parameter2 = bool.Parse(commandParts[2]); }
                        catch
                        {
                            OnRunCommandFailed(commandId, "Parameter is invalid");
                            return;
                        }

                        boolBoolCommand.Invoke(parameter1, parameter2);
                    }
                }
            }

            OnRunCommandSucceed(commandId);
        }

        public BaseDebugCommand[] GetCommandsMatching(string input)
        {
            var result = new List<BaseDebugCommand>();
            if (string.IsNullOrEmpty(input)) return result.ToArray();

            foreach (var curCommand in Commands)
            {
                if (curCommand.Id.ToUpper().Contains(input.ToUpper()))
                    result.Add(curCommand);
            }

            return result.ToArray();
        }

        protected virtual void OnRunCommandSucceed(string commandId) { }
        protected virtual void OnRunCommandFailed(string commandId, string errorMessage) { }
        #endregion

        #region Register Commands
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<IDebugController>(this);
            LoadCommands();
            yield return null;
        }

        protected abstract void LoadCommands();

        public void RegisterCommand(BaseDebugCommand command)
        {
            Commands.Add(command);
        }

        public void CleanUp()
        {

        }
        #endregion
    }
}