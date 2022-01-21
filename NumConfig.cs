
public class NumConfig {
    public enum DisplayType {
        DISPLAY_TYPE_SHORT,
        DISPLAY_TYPE_SCIENTIFIC,
        DISPLAY_TYPE_ENGINEERING,
    }

    public DisplayType displayType = DisplayType.DISPLAY_TYPE_SCIENTIFIC;
    public int decimalPlaces = 3;
}