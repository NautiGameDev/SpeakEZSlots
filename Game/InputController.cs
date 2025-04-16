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

        public static void ChangeInput(string key, bool value)
        {
            if (playerInput.ContainsKey(key))
            {
                playerInput[key] = value;
            }
            
        }
    }
}
