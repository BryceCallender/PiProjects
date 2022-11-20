using LEDControl.Models;

namespace LEDControl;

public class LEDState : ILEDState
{
    public LEDRequest State { get; set; } = Empty;
    
    public static bool IsDirty;

    private static LEDRequest Empty => new() { Mode = LEDMode.None }; 
    
    public void SetState(LEDRequest newState)
    {
        IsDirty = true;
        State = newState;
    }

    public void ResetState()
    {
        if (IsDirty) 
            return;
        
        State = Empty;
        IsDirty = false;
    }
}
