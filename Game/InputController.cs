namespace SpeakEZSlots.Game
{
    public class InputController
    {
        public static Dictionary<string, bool> playerInput = new Dictionary<string, bool>()
        {
            { " ", false },
            { "ArrowUp", false },
            { "ArrowDown", false },
            { "ArrowLeft", false },
            { "ArrowRight", false }
        };

        public static float mouseXCoords { get; set; } = 0;
        public static float mouseYCoords { get; set; } = 0;

        public static void ChangeInput(string key, bool value)
        {
            if (playerInput.ContainsKey(key))
            {
                playerInput[key] = value;
            }
        }

        public static void ChangeMouseCoords(float xPos, float yPos)
        {
            mouseXCoords = xPos;
            mouseYCoords = yPos;
        }

        public static void ClearMouseCoords()
        {
            mouseXCoords = 0;
            mouseYCoords = 0;
        }
    }
}
