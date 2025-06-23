using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using UnityEngine.InputSystem;

namespace LethalBots.Inputs
{
    public class LethalBotsInputs : LcInputActions
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [InputAction(KeyboardControl.E, Name = "Lead Bot/Send Bot", GamepadPath = "<Gamepad>/dpad/up")]
        public InputAction LeadBot { get; set; }

        [InputAction(KeyboardControl.G, Name = "Drop item", GamepadControl = GamepadControl.ButtonEast)]
        public InputAction DropItem { get; set; }

        [InputAction(KeyboardControl.X, Name = "Change suit of bot", GamepadPath = "<Gamepad>/dpad/right")]
        public InputAction ChangeSuitBot { get; set; }

        [InputAction(KeyboardControl.C, Name = "Make bot look at position", GamepadPath = "<Gamepad>/dpad/up")]
        public InputAction MakeBotLookAtPosition { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
