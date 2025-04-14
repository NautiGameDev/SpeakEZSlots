namespace SpeakEZSlots.Game
{
    /*
        Handles UI related text: Storing, updating, and rendering
     */

    public class UIController
    {
        TextElement pCredits { get; set; }
        TextElement pBet { get; set; }
        TextElement pWinnings { get; set; }
        TextElement messageBar { get; set; }

        Machine machine { get; set; }

        private float winningsTimer = 0;
        private float winningsDelay = 0.01f;

        public UIController(Machine machine)
        {
            this.machine = machine;
            InstantiateText();
            
        }

        private void InstantiateText()
        {

            int fontSize = (int)(62 * Game.verticalScale);
            string textSize = fontSize.ToString() + "px";

            pCredits = new TextElement(machine.playerCredits.ToString(), textSize, (float)(245 * Game.horizontalScale), (float)(595 * Game.verticalScale));
            pBet = new TextElement(machine.bet.ToString(), textSize, (float)(245 * Game.horizontalScale), (float)(955 * Game.verticalScale));
            pWinnings = new TextElement(machine.winnings.ToString(), textSize, (float)(245 * Game.horizontalScale), (float)(1255 * Game.verticalScale));

            fontSize = (int)(48 * Game.verticalScale);
            textSize = fontSize.ToString() + "px";
            messageBar = new TextElement("Press space bar to spin!", textSize, (float)(1115 * Game.horizontalScale), (float)(470 * Game.verticalScale));
        }

        public void UpdatePlayerCredits(int value)
        {
            pCredits.UpdateMessage(value.ToString());
        }

        public void UpdatePlayerBet(int value)
        {
            pBet.UpdateMessage(value.ToString());
        }

        public void UpdatePlayerWinnings(int value)
        {
            pWinnings.UpdateMessage(value.ToString());
        }

        public void UpdateMessageBar(string message)
        {
            messageBar.UpdateMessage(message);
        }

        public void TransferWinnings(float deltaTime)
        {
            if (winningsTimer > 0)
            {
                winningsTimer -= deltaTime;
            }

            if (winningsTimer <= 0 && machine.winnings > 0)
            {
                winningsTimer = winningsDelay;
                machine.winnings--;
                machine.playerCredits++;
                UpdatePlayerWinnings(machine.winnings);
                UpdatePlayerCredits(machine.playerCredits);
            }
        }

        public void Update(float deltaTime)
        {
            
        }

        public async void Render()
        {
            if (pCredits != null)
            {
                await pCredits.Render();
            }

            if (pBet != null)
            {
                await pBet.Render();
            }

            if (pWinnings != null)
            {
                await pWinnings.Render();
            }

            if (messageBar != null)
            {
                await messageBar.Render();
            }
        }
    }
}
