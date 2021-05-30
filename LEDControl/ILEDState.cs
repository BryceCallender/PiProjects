using LEDControl.Models;

namespace LEDControl
{
    public interface ILEDState
    {
        public void SetState(LEDRequest newState);
        public LEDRequest GetState();
        public void ResetState();
    }
}