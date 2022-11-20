using LEDControl.Models;

namespace LEDControl;

public interface ILEDState
{
    public LEDRequest State { get; set; }
    
    public void SetState(LEDRequest newState);
    public void ResetState();
}