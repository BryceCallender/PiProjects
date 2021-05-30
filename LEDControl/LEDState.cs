using LEDControl.Models;

namespace LEDControl
{
    public class LEDState : ILEDState
    {
        private LEDRequest _state;
        public static bool IsDirty;

        private static LEDRequest Empty =>  new LEDRequest { Mode = Mode.None}; 
        
        public void SetState(LEDRequest newState)
        {
            IsDirty = true;
            _state = newState;
        }

        public LEDRequest GetState()
        {
            return _state ?? Empty;
        }

        public void ResetState()
        {
            if (_state.Settings.Loop == null || (_state.Settings.Loop.HasValue && !_state.Settings.Loop.Value))
            {
                _state = Empty;
            }
        }
    }
}