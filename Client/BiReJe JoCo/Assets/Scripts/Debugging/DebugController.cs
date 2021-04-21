﻿using JoVei.Base;
using JoVei.Base.Helper;
using JoVei.Base.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using BiReJeJoCo.Backend;
using Newtonsoft.Json;

namespace BiReJeJoCo.Debugging
{
    /// <summary>
    /// Debug console with commands 
    /// </summary>
    public class DebugController : BaseDebugController
    {
        private float buttonWidth = 70;
        private float consoleHeight = 30;
        private float inputSpacing = 10;
        private float suggestionSpacing = 5;
        private float helpBoxHeight = 100;
        private int buttonCaptionSize = 20;

        private string curInput;
        private bool displayHelp;
        private Vector2 scroll;
        private bool setFocus;

        private PhotonRoomWrapper photonRoomWrapper => DIContainer.GetImplementationFor<PhotonRoomWrapper>();
        private Player localPlayer => DIContainer.GetImplementationFor<PlayerManager>().LocalPlayer;

        protected override void Update()
        {
            if (Keyboard.current[Key.LeftCtrl].IsPressed() &&
                Keyboard.current[Key.D].wasPressedThisFrame)
            {
                ToggleVisibility();
            }

            if (Keyboard.current[Key.Enter].wasPressedThisFrame)
            {
                RunCommand(curInput);
                curInput = string.Empty;
            }
        }

        private void OnGUI()
        {
            if (!DebugPanelIsOpen) return;

            float y = 0f;

            var inputStyle = new GUIStyle();
            inputStyle.fontSize = (int) consoleHeight - 10;

            var buttonStyle = new GUIStyle();
            buttonStyle.fontSize = buttonCaptionSize;
            buttonStyle.fontStyle = FontStyle.Bold;
         
            // help
            if (displayHelp)
            {
                GUI.Box(new Rect(0, y, Screen.width, helpBoxHeight), "");

                Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * Commands.Count);
                scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scroll, viewport);

                for (int i = 0; i < Commands.Count; i++)
                {
                    var curCommand = Commands[i];

                    string label = $"{curCommand.Format} - {curCommand.Description}";

                    Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, inputStyle.fontSize);

                    GUI.Label(labelRect, label, inputStyle);
                }

                y += helpBoxHeight;
                GUI.EndScrollView();
            }

            // input box
            GUI.Box(new Rect(0, y, Screen.width- buttonWidth * 2, consoleHeight), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            GUI.SetNextControlName("InputField");
            curInput = GUI.TextField(new Rect(10f, y + inputSpacing / 2, Screen.width - buttonWidth * 2, consoleHeight - inputSpacing), curInput, inputStyle);
            GUI.backgroundColor = new Color(0, 0, 0, 1);
            
            if (setFocus)
            {
                GUI.FocusControl("InputField");
                setFocus = false;
            }

            // suggestions
            var matchingCommands = GetCommandsMatching(curInput);

            if (matchingCommands.Length > 0)
            {
                float offset = consoleHeight + suggestionSpacing;
                float size = matchingCommands.Length * inputStyle.fontSize + ((matchingCommands.Length + 2) * suggestionSpacing);
                GUI.Box(new Rect(0, y + consoleHeight, Screen.width, size), "");

                foreach (var curMatchingCommand in matchingCommands)
                {
                    // button
                    if (GUI.Button(new Rect(5, y + offset, Screen.width, inputStyle.fontSize), curMatchingCommand.Format, inputStyle))
                    {
                        curInput = curMatchingCommand.Id;
                    }
                    offset += inputStyle.fontSize + suggestionSpacing;
                }
            }

            // run button
            if (GUI.Button(new Rect(Screen.width - buttonWidth*2, y, buttonWidth, consoleHeight), "Run"))
            {
                RunCommand(curInput);
                curInput = string.Empty;
            }

            if (GUI.Button(new Rect(Screen.width - buttonWidth, y, buttonWidth, consoleHeight), "Help"))
            {
                displayHelp = !displayHelp;
            }
        }

        // load commands
        protected override void LoadCommands()
        {
            SetGlobalVariables();

            RegisterCommand(new DebugCommand<bool>("debug_mode", "Enables/Disables debug stats", "debug_mode <bool>", mode =>
            {
                globalVariables.SetVar("debug_mode", mode);

                if (mode == true)
                    DebugStatsUI.Instance.Show();
                else
                    DebugStatsUI.Instance.Hide();
            }));

            RegisterCommand(new DebugCommand("log_player", "Logs all player serialized in room", "log_player", () =>
            {
                var result = string.Empty;

                foreach (var curPlayer in DIContainer.GetImplementationFor<PlayerManager>().GetAllPlayer())
                {
                    result += JsonConvert.SerializeObject(curPlayer, Formatting.Indented, new JsonSerializerSettings() { }) + "\n";
                }

                DebugHelper.Print(result);
            }));

            RegisterCommand(new DebugCommand<bool>("lock_cursor", "Locks/Unlocks the cursor", "lock_cursor <bool>", (value) =>
            {
                Cursor.lockState = value? CursorLockMode.Locked : CursorLockMode.None;
            }));

            RegisterCommand(new DebugCommand<float>("movement_sync_speed", "Set the overall movement synchroniziation speed", "movement_sync_speed <float>", (value) =>
            {
                globalVariables.SetVar("move_sync_speed", value);
            }));

            RegisterCommand(new DebugCommand<float>("rotation_sync_speed", "Set the overall rotation synchroniziation speed", "rotation_sync_speed <float>", (value) =>
            {
                globalVariables.SetVar("rot_sync_speed", value);
            }));

            RegisterCommand(new DebugCommand<string>("load_scene_together", "Load another game scene in lobby (game_scene, game_scene_2, game_scene_3 ...)", "load_scene_together <string>", (value) =>
            {
                if (photonRoomWrapper.IsInRoom && 
                    localPlayer.IsHost)
                    photonRoomWrapper.LoadLevel(value);
            }));
        }

        private static void SetGlobalVariables()
        {
            globalVariables.SetVar("debug_mode", false);
            globalVariables.SetVar<float>("move_sync_speed", 1); 
            globalVariables.SetVar<float>("rot_sync_speed", 1); 
        }

        // before opening the panel  
        protected override void OpenDebugPanel()
        {
            curInput = string.Empty;
            setFocus = true;
        }

        // run command by its id
        protected override void OnRunCommandFailed(string commandId, string errorMessage)
        {
            Debug.Log(string.Format("Failed to run command {0}\n{1}", commandId, errorMessage));
        }
    }
}