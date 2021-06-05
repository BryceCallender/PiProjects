using System.Collections.Generic;
using LEDControl.Models;

namespace LEDControl
{
    public class LEDState : ILEDState
    {
        private LEDRequest _state = Empty;

        public static bool IsDirty;

        private static LEDRequest Empty => new LEDRequest { Mode = Mode.None}; 
        
        public void SetState(LEDRequest newState)
        {
            IsDirty = true;
            _state = newState;
        }

        public LEDRequest GetState()
        {
            return _state;
        }

        public void ResetState()
        {
            if ((IsDirty || !IsSingleUseState()) && !(!_state.Settings?.Loop ?? true)) 
                return;
            
            _state = Empty;
            IsDirty = false;
        }

        private bool IsSingleUseState()
        {
            var singleStates = new List<Mode> { Mode.StaticColor, Mode.Rainbow, Mode.StaticColor };
            
            return singleStates.Contains(_state.Mode);
        }
    }
}