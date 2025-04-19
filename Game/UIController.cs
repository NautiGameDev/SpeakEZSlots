using Microsoft.AspNetCore.Components;

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
        TextElement freeSpins { get; set; }

        Button soundButtonOn { get; set; }
        Button soundButtonOff { get; set; }
        Button howToPlayButton { get; set; }

        Machine machine { get; set; }

        private float winningsTimer = 0;
        private float winningsDelay = 0.01f;

        public UIController(Machine machine, ElementReference soundButtonOn, ElementReference soundButtonOff, ElementReference howToPlayButton)
        {
            this.machine = machine;
            InstantiateText();
            InstantiateButtons(soundButtonOn, soundButtonOff, howToPlayButton);
        }

        private void InstantiateText()
        {
            int fontSize = (int)(62 * Game.verticalScale);
            string textSize = fontSize.ToString() + "px";

            pCredits = new TextElement(machine.playerCredits.ToString(), textSize, (float)(245 * Game.horizontalScale), (float)(595 * Game.verticalScale));
            pBet = new TextElement(machine.bet.ToString(), textSize, (float)(165 * Game.horizontalScale), (float)(950 * Game.verticalScale));
            pWinnings = new TextElement(machine.winnings.ToString(), textSize, (float)(245 * Game.horizontalScale), (float)(1255 * Game.verticalScale));
            freeSpins = new TextElement(machine.freeSpins.ToString(), textSize, (float)(320 * Game.horizontalScale), (float)(950 * Game.verticalScale));

            fontSize = (int)(48 * Game.verticalScale);
            textSize = fontSize.ToString() + "px";
            messageBar = new TextElement("Press space bar to spin!", textSize, (float)(1115 * Game.horizontalScale), (float)(470 * Game.verticalScale));
        }

        private void InstantiateButtons(ElementReference soundButtonOn, ElementReference soundButtonOff, ElementReference howToPlayButton)
        {
            this.soundButtonOn = new Button(soundButtonOn, 10, 10);
            this.soundButtonOff = new Button(soundButtonOff, 10, 10);
            this.howToPlayButton = new Button(howToPlayButton, 110, 10);
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

        public void UpdateFreeSpins(int value)
        {
            freeSpins.UpdateMessage(value.ToString());
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
            if (InputController.mouseXCoords > soundButtonOn.xPos &&
                InputController.mouseXCoords < (soundButtonOn.xPos + soundButtonOn.spriteBaseWidth) &&
                InputController.mouseYCoords > soundButtonOn.yPos &&
                InputController.mouseYCoords < (soundButtonOn.yPos + soundButtonOn.spriteBaseHeight) &&
                machine.soundController.soundMuted == false)
            {
                machine.soundController.soundMuted = true;
                machine.soundController.StopMusic();
                InputController.ClearMouseCoords();
            }

            else if (InputController.mouseXCoords > soundButtonOff.xPos &&
                InputController.mouseXCoords < (soundButtonOff.xPos + soundButtonOff.spriteBaseWidth) &&
                InputController.mouseYCoords > soundButtonOff.yPos &&
                InputController.mouseYCoords < (soundButtonOff.yPos + soundButtonOff.spriteBaseHeight) &&
                machine.soundController.soundMuted == true)
            {
                machine.soundController.soundMuted = false;
                machine.soundController.PlayMusic();
                InputController.ClearMouseCoords();
            }

            if (InputController.mouseXCoords > howToPlayButton.xPos &&
                InputController.mouseXCoords < (howToPlayButton.xPos + howToPlayButton.spriteBaseWidth) &&
                InputController.mouseYCoords > howToPlayButton.yPos &&
                InputController.mouseYCoords < (howToPlayButton.yPos + howToPlayButton.spriteBaseHeight) &&
                machine.showHowToPlay == false)
            {
                machine.showHowToPlay = true;
                InputController.ClearMouseCoords();
            }

            if (InputController.mouseXCoords > (float)(1052 * Game.horizontalScale) &&
                InputController.mouseXCoords < (float)(1325 * Game.horizontalScale) &&
                InputController.mouseYCoords > (float)(1052 * Game.verticalScale) &&
                InputController.mouseYCoords < (float)(1118 * Game.verticalScale) &&
                machine.showHowToPlay == true)
            {
                machine.showHowToPlay = false;
                InputController.ClearMouseCoords();
            }

        }

        public async Task Render()
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

            
            if (freeSpins != null)
            {
                await freeSpins.Render();
            }

            if (!machine.soundController.soundMuted && soundButtonOn != null)
            {
                await soundButtonOn.Render();
            }

            if (machine.soundController.soundMuted && soundButtonOff != null)
            {
                await soundButtonOff.Render();
            }

            if (howToPlayButton != null)
            {
                await howToPlayButton.Render();
            }

           
        }
    }
}
