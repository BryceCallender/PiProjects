namespace LEDControl;

public interface ILEDEffects
{
    public void HandleRequest();

    public void ColorWipe();
    public void StaticColor();
    public void Rainbow();
    public void RainbowCycle();
    public void TheaterChase();
    public void TheaterChaseRainbow();
    public void AppearFromBack();
    public void Hyperspace();
    public void Breathe();
    public void BreathingRainbow();
    
    public void SelectedColors();
    public void Chaser();

    public void Clear(int delay = 0);
}