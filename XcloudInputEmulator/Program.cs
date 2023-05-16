using System.Runtime.InteropServices;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace XcloudInputEmulator;

internal class Program
{
    static ViGEmClient client;
    static IDualShock4Controller controller;
    static byte leftThumbX, leftThumbY;
    static byte rightThumbX, rightThumbY;

    static void Main(string[] args)
    {
        try
        {
            using (client = new ViGEmClient())
            {
                controller = client.CreateDualShock4Controller();
                controller.Connect();

                while (true)
                {
                    if (WinApi.IsKeyPressed(Keys.D0))
                        break;

                    UpdateScreenCenter();

                    leftThumbX = 127;
                    leftThumbY = 127;

                    if (WinApi.IsKeyPressed(Keys.W))
                        leftThumbY = 0;
                    else if (WinApi.IsKeyPressed(Keys.S))
                        leftThumbY = 255;

                    if (WinApi.IsKeyPressed(Keys.A))
                        leftThumbX = 0;
                    else if (WinApi.IsKeyPressed(Keys.D))
                        leftThumbX = 255;

                    controller.SetAxisValue(DualShock4Axis.LeftThumbX, leftThumbX);
                    controller.SetAxisValue(DualShock4Axis.LeftThumbY, leftThumbY);

                    UpdateThumbstickPosition();

                    controller.SetAxisValue(DualShock4Axis.RightThumbX, rightThumbX);
                    controller.SetAxisValue(DualShock4Axis.RightThumbY, rightThumbY);

                    UpdateInputBindings();

                    Thread.Sleep(16);
                }

                controller.Disconnect();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(ex);
        }
    }

    static int screenWidth = 0;
    static int screenHeight = 0;
    static int screenCenterX;
    static int screenCenterY;
    static Screen mainScreen;

    static void UpdateScreenCenter()
    {
        mainScreen ??= Screen.PrimaryScreen;
        var bounds = mainScreen.Bounds;
        screenWidth = bounds.Width;
        screenHeight = bounds.Height;
        screenCenterX = screenWidth / 2;
        screenCenterY = screenHeight / 2;
    }

    const int MAX_THUMB_VALUE = 255;
    const int DEFAULT_THUMB_VALUE = 127;
    const float SENSITIVITY = 1.75f;

    static void UpdateThumbstickPosition()
    {
        // Get the current position of the mouse cursor
        Point cursorPos = Cursor.Position;

        // Calculate the relative position of the cursor from the center of the screen
        int relativeX = (int)((cursorPos.X - screenCenterX) / (screenCenterX * SENSITIVITY));
        int relativeY = (int)((cursorPos.Y - screenCenterY) / (screenCenterY * SENSITIVITY));

        // Map the relative coordinates to the range of thumbstick values (-128 to 127)
        rightThumbX = (byte)Math.Max(0, Math.Min(255, (int)Math.Round((double)(MAX_THUMB_VALUE - DEFAULT_THUMB_VALUE) * relativeX / screenCenterX) + DEFAULT_THUMB_VALUE));
        rightThumbY = (byte)Math.Max(0, Math.Min(255, (int)Math.Round((double)(MAX_THUMB_VALUE - DEFAULT_THUMB_VALUE) * relativeY / screenCenterY) + DEFAULT_THUMB_VALUE));

        // Reset the mouse cursor position to the center of the screen
        Cursor.Position = new Point(screenCenterX, screenCenterY);
    }

    static void UpdateInputBindings()
    {
        foreach (var (key, button) in Keybinds)
        {
            var state = WinApi.IsKeyPressed(key);
            controller.SetButtonState(button, state);
        }

        foreach (var (key, slider) in Sliders)
        {
            var state = WinApi.IsKeyPressed(key);
            controller.SetSliderValue(slider, (byte)(state ? 255 : 0));
        }

        foreach (var (key, dir) in DPads)
        {
            var state = WinApi.IsKeyPressed(key);
            controller.SetDPadDirection(state ? dir : DualShock4DPadDirection.None);
        }
    }

    static readonly Dictionary<Keys, DualShock4DPadDirection> DPads = new()
    {
        // Usar cura
        [Keys.E] = DualShock4DPadDirection.South,

        // Trocar poder
        [Keys.G] = DualShock4DPadDirection.North,

        // Trocar arma: anterior
        [Keys.D1] = DualShock4DPadDirection.East,

        // Trocar arma: proxima
        [Keys.D2] = DualShock4DPadDirection.West,

        // Setas do Teclado
        [Keys.Up] = DualShock4DPadDirection.North,
        [Keys.Down] = DualShock4DPadDirection.South,
        [Keys.Left] = DualShock4DPadDirection.East,
        [Keys.Right] = DualShock4DPadDirection.West,
    };

    static readonly Dictionary<Keys, DualShock4Slider> Sliders = new()
    {
        // Atirar
        [Keys.LButton] = DualShock4Slider.RightTrigger,

        // Mirar
        [Keys.RButton] = DualShock4Slider.LeftTrigger
    };

    static readonly Dictionary<Keys, DualShock4Button> Keybinds = new()
    {
        // Pular.
        [Keys.Space] = DualShock4Button.Cross,

        // Recarregar arma, abrir roda de arma, consumir item
        [Keys.R] = DualShock4Button.Square,

        // Usar habilidade do primária (ex. choque)
        [Keys.Q] = DualShock4Button.Triangle,

        // Dar Dash
        [Keys.LShiftKey] = DualShock4Button.Circle,

        // Agaixar
        [Keys.C] = DualShock4Button.ThumbRight,

        // Pausar
        [Keys.P] = DualShock4Button.Options,

        // Mapa + Inventário
        [Keys.M] = DualShock4Button.Share,

        // Interagir
        [Keys.F] = DualShock4Button.ShoulderRight
    };

    public enum KeyBindType
    {
        Slider,
        DPad,
        Button
    }

    public enum DPadDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public class KeyBindInfo
    {
        public KeyBindType Type { get; set; }
        public string Button { get; set; }
        public string Slider { get; set; }
        public DPadDirection DPad { get; set; }
    }
}

static class WinApi
{
    [DllImport("user32")]
    static extern short GetAsyncKeyState(Keys key);

    public static bool IsKeyPressed(Keys key)
        => (GetAsyncKeyState(key) & 0x8000) != 0;
}