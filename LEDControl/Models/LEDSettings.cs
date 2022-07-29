using System.Drawing;

namespace LEDControl.Models;

public class LEDSettings
{
    public Color? Color { get; set; }

    public IEnumerable<Color>? Colors { get; set; }

    public int? WaitTime { get; set; }
    public bool? Loop { get; set; }
    public int? Iterations { get; set; }
    public int? Length { get; set; }
    public int? Duration { get; set; }
    public int? Trail { get; set; }
}