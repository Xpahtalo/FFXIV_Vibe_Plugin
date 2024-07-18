#nullable enable
namespace FFXIV_Vibe_Plugin.Triggers
{
    public class TriggerDevice
    {
        public string Name = "";
        public bool IsEnabled;
        public bool ShouldVibrate;
        public bool ShouldRotate;
        public bool ShouldLinear;
        public bool ShouldOscillate;
        public bool ShouldStop;
        public FFXIV_Vibe_Plugin.Device.Device? Device;
        public bool[] VibrateSelectedMotors;
        public int[] VibrateMotorsThreshold;
        public int[] VibrateMotorsPattern;
        public bool[] RotateSelectedMotors;
        public int[] RotateMotorsThreshold;
        public int[] RotateMotorsPattern;
        public bool[] LinearSelectedMotors;
        public int[] LinearMotorsThreshold;
        public int[] LinearMotorsPattern;
        public bool[] OscillateSelectedMotors;
        public int[] OscillateMotorsThreshold;
        public int[] OscillateMotorsPattern;

        public TriggerDevice(FFXIV_Vibe_Plugin.Device.Device device)
        {
            this.Name = device.Name;
            this.Device = device;
            this.VibrateSelectedMotors = new bool[device.CanVibrate ? device.VibrateMotors : 0];
            this.VibrateMotorsThreshold = new int[device.CanVibrate ? device.VibrateMotors : 0];
            this.VibrateMotorsPattern = new int[device.CanVibrate ? device.VibrateMotors : 0];
            this.RotateSelectedMotors = new bool[device.CanRotate ? device.RotateMotors : 0];
            this.RotateMotorsThreshold = new int[device.CanRotate ? device.RotateMotors : 0];
            this.RotateMotorsPattern = new int[device.CanRotate ? device.RotateMotors : 0];
            this.LinearSelectedMotors = new bool[device.CanLinear ? device.LinearMotors : 0];
            this.LinearMotorsThreshold = new int[device.CanLinear ? device.LinearMotors : 0];
            this.LinearMotorsPattern = new int[device.CanLinear ? device.LinearMotors : 0];
            this.OscillateSelectedMotors = new bool[device.CanOscillate ? device.OscillateMotors : 0];
            this.OscillateMotorsThreshold = new int[device.CanOscillate ? device.OscillateMotors : 0];
            this.OscillateMotorsPattern = new int[device.CanOscillate ? device.OscillateMotors : 0];
        }

        public override string ToString() => "TRIGGER_DEVICE " + this.Name;
    }
}
