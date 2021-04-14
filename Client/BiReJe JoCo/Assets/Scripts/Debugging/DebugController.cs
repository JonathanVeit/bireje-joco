﻿using JoVei.Base;
using JoVei.Base.Helper;
using JoVei.Base.Backend;
using JoVei.Base.Data;
using JoVei.Base.UI;
using UnityEngine;
using System.IO;

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

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return))
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
            RegisterCommand(new DebugCommand<bool>("set_debug_stats", "Enables/Disables debug stats", "set_debug_stats <bool>", state =>
            {
                if (state == true)
                    DebugStatsUI.Instance.Show();
                else
                    DebugStatsUI.Instance.Hide();
            }));
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