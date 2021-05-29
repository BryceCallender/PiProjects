using LEDControl.Models;

namespace LEDControl
{
    public class LEDState : ILEDState
    {
        public static LEDRequest state;
        public static bool IsDirty;
        
        public void SetState(LEDRequest newState)
        {
            IsDirty = true;
            state = newState;
        }

        public LEDRequest GetState()
        {
            return state ?? new LEDRequest { Mode = Mode.None};
        }
    }
}