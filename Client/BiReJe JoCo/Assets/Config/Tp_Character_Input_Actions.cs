// GENERATED AUTOMATICALLY FROM 'Assets/Config/Tp_Character_Input_Actions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Tp_Character_Input_Actions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Tp_Character_Input_Actions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Tp_Character_Input_Actions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""2ba344ff-b9d1-4bdd-80b0-e39fd3e211b3"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""10f7f6be-e6cc-4255-afb7-137265df4dd2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookAround"",
                    ""type"": ""Value"",
                    ""id"": ""294fde9f-6f97-47c7-9ec0-1929f91f34dc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jumping"",
                    ""type"": ""Button"",
                    ""id"": ""0853c5ea-40cb-421f-9907-c47166eeeace"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprinting"",
                    ""type"": ""Button"",
                    ""id"": ""fd14a497-3a66-4887-acef-0617dbc0e7f1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""b5e79723-7215-4f15-9bc4-29c0e51d6262"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Trigger"",
                    ""type"": ""Button"",
                    ""id"": ""31d42936-fd5f-4fc6-b6b8-fcca9b30c0b5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""a22d508e-7c6c-4b5e-9461-b3bdae7485ea"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Special1"",
                    ""type"": ""Button"",
                    ""id"": ""345fe285-9088-4d20-a207-4c99896c2159"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Special2"",
                    ""type"": ""Button"",
                    ""id"": ""eb25ac4f-c25c-4a1d-a1af-a84b723f3cb5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""66bf75b4-10b5-4bc3-8716-5972ade48227"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jumping"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd7b2c45-41a2-43fd-8728-ddd268338d27"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jumping"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""250ff464-d606-46ab-a5d0-6427c2efaa86"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jumping"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""3773f12b-1ea2-4879-ae45-b9d1f30043b8"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""df11d74e-9727-40ed-8bf4-653adc6232d7"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""84c679df-c4a3-4516-8944-fb7361db433e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ed3c76db-ea9a-4de2-a505-ac344684caad"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0d268aa9-7aad-42d1-87b9-e5a5d303a0d0"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""45215c05-b67e-44aa-90aa-24223e9fc310"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6105e80-a4d4-4c4d-8cd5-0beb0cceb151"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1d87afd-c3e6-44f9-829f-c6843ac4f4c1"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""748ecac0-b0a9-4c13-bf97-2db79ab6edba"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Sprinting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d9753891-6c37-4156-ba3a-16f08f6749c1"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Sprinting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9839a8ef-552e-43aa-8863-f082cd982c54"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4384995e-75b9-4700-8505-4a9ccfea8adb"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ec5e13d-7f40-480a-a59d-9a2cce3cb748"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Trigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4784e977-5561-431e-a3e5-2ceca768c8f5"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Trigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79a7f162-6dcc-433b-84f4-708cb7080622"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""04658d23-e971-4ecc-b570-624bcf8a6529"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee1fbb21-8ba8-4aae-b770-0edbd7b3c818"",
                    ""path"": ""<Keyboard>/#(Q)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Special1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b833d37a-3ca6-4f17-9c37-8417bdda56c6"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Special1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3cf50b26-ef84-42a2-a90a-563537e2e5ac"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Special2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Ui"",
            ""id"": ""11cd237e-b08d-4a67-b429-1bd07dc03368"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""5c48a9be-203d-4a18-b019-43f831e3fccf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""540c9e22-bb44-445a-93be-a6ff03464ab9"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_LookAround = m_Player.FindAction("LookAround", throwIfNotFound: true);
        m_Player_Jumping = m_Player.FindAction("Jumping", throwIfNotFound: true);
        m_Player_Sprinting = m_Player.FindAction("Sprinting", throwIfNotFound: true);
        m_Player_Menu = m_Player.FindAction("Menu", throwIfNotFound: true);
        m_Player_Trigger = m_Player.FindAction("Trigger", throwIfNotFound: true);
        m_Player_Shoot = m_Player.FindAction("Shoot", throwIfNotFound: true);
        m_Player_Special1 = m_Player.FindAction("Special1", throwIfNotFound: true);
        m_Player_Special2 = m_Player.FindAction("Special2", throwIfNotFound: true);
        // Ui
        m_Ui = asset.FindActionMap("Ui", throwIfNotFound: true);
        m_Ui_Newaction = m_Ui.FindAction("New action", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_LookAround;
    private readonly InputAction m_Player_Jumping;
    private readonly InputAction m_Player_Sprinting;
    private readonly InputAction m_Player_Menu;
    private readonly InputAction m_Player_Trigger;
    private readonly InputAction m_Player_Shoot;
    private readonly InputAction m_Player_Special1;
    private readonly InputAction m_Player_Special2;
    public struct PlayerActions
    {
        private @Tp_Character_Input_Actions m_Wrapper;
        public PlayerActions(@Tp_Character_Input_Actions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @LookAround => m_Wrapper.m_Player_LookAround;
        public InputAction @Jumping => m_Wrapper.m_Player_Jumping;
        public InputAction @Sprinting => m_Wrapper.m_Player_Sprinting;
        public InputAction @Menu => m_Wrapper.m_Player_Menu;
        public InputAction @Trigger => m_Wrapper.m_Player_Trigger;
        public InputAction @Shoot => m_Wrapper.m_Player_Shoot;
        public InputAction @Special1 => m_Wrapper.m_Player_Special1;
        public InputAction @Special2 => m_Wrapper.m_Player_Special2;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @LookAround.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookAround;
                @LookAround.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookAround;
                @LookAround.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookAround;
                @Jumping.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJumping;
                @Jumping.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJumping;
                @Jumping.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJumping;
                @Sprinting.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprinting;
                @Sprinting.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprinting;
                @Sprinting.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprinting;
                @Menu.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
                @Menu.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
                @Menu.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
                @Trigger.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTrigger;
                @Trigger.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTrigger;
                @Trigger.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTrigger;
                @Shoot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Special1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecial1;
                @Special1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecial1;
                @Special1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecial1;
                @Special2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecial2;
                @Special2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecial2;
                @Special2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecial2;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @LookAround.started += instance.OnLookAround;
                @LookAround.performed += instance.OnLookAround;
                @LookAround.canceled += instance.OnLookAround;
                @Jumping.started += instance.OnJumping;
                @Jumping.performed += instance.OnJumping;
                @Jumping.canceled += instance.OnJumping;
                @Sprinting.started += instance.OnSprinting;
                @Sprinting.performed += instance.OnSprinting;
                @Sprinting.canceled += instance.OnSprinting;
                @Menu.started += instance.OnMenu;
                @Menu.performed += instance.OnMenu;
                @Menu.canceled += instance.OnMenu;
                @Trigger.started += instance.OnTrigger;
                @Trigger.performed += instance.OnTrigger;
                @Trigger.canceled += instance.OnTrigger;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @Special1.started += instance.OnSpecial1;
                @Special1.performed += instance.OnSpecial1;
                @Special1.canceled += instance.OnSpecial1;
                @Special2.started += instance.OnSpecial2;
                @Special2.performed += instance.OnSpecial2;
                @Special2.canceled += instance.OnSpecial2;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Ui
    private readonly InputActionMap m_Ui;
    private IUiActions m_UiActionsCallbackInterface;
    private readonly InputAction m_Ui_Newaction;
    public struct UiActions
    {
        private @Tp_Character_Input_Actions m_Wrapper;
        public UiActions(@Tp_Character_Input_Actions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Ui_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Ui; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UiActions set) { return set.Get(); }
        public void SetCallbacks(IUiActions instance)
        {
            if (m_Wrapper.m_UiActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_UiActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_UiActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_UiActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_UiActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public UiActions @Ui => new UiActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLookAround(InputAction.CallbackContext context);
        void OnJumping(InputAction.CallbackContext context);
        void OnSprinting(InputAction.CallbackContext context);
        void OnMenu(InputAction.CallbackContext context);
        void OnTrigger(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnSpecial1(InputAction.CallbackContext context);
        void OnSpecial2(InputAction.CallbackContext context);
    }
    public interface IUiActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
