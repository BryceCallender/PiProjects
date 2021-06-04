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
            //If we want to loop don't reset
            if (_state.Settings?.Loop ?? false)
            {
                return;
            }

            _state = Empty;
        }
    }
}