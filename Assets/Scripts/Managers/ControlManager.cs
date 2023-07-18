using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class ControlManager : MonoBehaviour
    {
        public enum CurrentControllerType
        {
            KeyboardMouse,
            Xbox,
            Playstation,
            Switch,
            Count
        }


        PlayerInput C_playerInputs;
        CurrentControllerType e_currentControlDevice = CurrentControllerType.KeyboardMouse;

        private void OnEnable()
        {
            C_playerInputs = FindObjectOfType<PlayerInput>();
            ControlManager control = FindObjectOfType<ControlManager>();
            if (control != this)
            {
                Destroy(control.transform);
            }
        }

        private void EnableListeners()
        {
            InputSystem.onDeviceChange += InputDeviceChanged;
        }

        private void InputDeviceChanged(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    string deviceType = device.description.manufacturer;
                    switch (deviceType)
                    {
                        //correct name
                        case "Sony Interactive Entertainement":
                            e_currentControlDevice = CurrentControllerType.Playstation;
                            return;
                        //correct name
                        case "Nintendo Co., Ltd.":
                            e_currentControlDevice = CurrentControllerType.Switch;
                            return;
                        default:
                            //xbox manufacturer returns empty but so do keyboard and mice
                            if (InputSystem.devices[0].description.product.Contains("XBox"))
                            {
                                e_currentControlDevice = CurrentControllerType.Xbox;
                            }
                            else
                            {
                                e_currentControlDevice = CurrentControllerType.KeyboardMouse;
                            }
                            return;
                    }

                case InputDeviceChange.Disconnected:
                    return;

                case InputDeviceChange.Reconnected:
                    return;
                default:

                    break;
            }
        }

        public void ResetDevices()
        {
            for (int i = 0; i < InputSystem.devices.Count - 1; i++)
            {
                InputSystem.RemoveDevice(InputSystem.devices[0]);
            }
        }
        public void CheckFirstInputDevice()
        {
            if (InputSystem.devices[0] == null)
                return;

            switch (InputSystem.devices[0].description.manufacturer)
            {
                //correct name
                case "Sony Interactive Entertainement":
                    e_currentControlDevice = CurrentControllerType.Playstation;
                    return;
                //correct name
                case "Nintendo Co., Ltd.":
                    e_currentControlDevice = CurrentControllerType.Switch;
                    return;
                default:
                    //xbox manufacturer returns empty but so do keyboard and mice
                    if (InputSystem.devices[0].description.product.Contains("XBox"))
                    {
                        e_currentControlDevice = CurrentControllerType.Xbox;
                    }
                    else
                    {
                        e_currentControlDevice = CurrentControllerType.KeyboardMouse;
                    }
                    return;
            }
        }
        public CurrentControllerType GetCurrentControllerType()
        {
            return e_currentControlDevice;
        }

        public void ChangeInputDevice(string controlScheme)
        {

            switch (controlScheme)
            {
                //correct name
                case "Playstation":
                    e_currentControlDevice = CurrentControllerType.Playstation;
                    return;
                //correct name
                case "Nintendo":
                    e_currentControlDevice = CurrentControllerType.Switch;
                    return;
                case "Xbox":
                    e_currentControlDevice = CurrentControllerType.Xbox;
                    return;
                case "KeyboardMouse":
                    e_currentControlDevice = CurrentControllerType.KeyboardMouse;
                    return;
            }
        }

        private void ChangeControlImages(CurrentControllerType type)
        {

        }
    }
}
