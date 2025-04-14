using Microsoft.AspNetCore.Components;
using SpeakEZSlots.Game.MachineStates;

namespace SpeakEZSlots.Game
{
    public class Machine
    {
        State currentState {  get; set; }
        UIController uiController { get; set; }
        Overlay machineOverlay { get; set; }
        Overlay darkOverlay { get; set; }


        private Queue<Reel> reels = new Queue<Reel>();

        public int freeSpins = 0;
        public int playerCredits = 1000;
        public int bet = 25;
        public int winnings = 0;

        public Machine(ElementReference background, ElementReference backgroundDark, ElementReference symbols, ElementReference star)
        {
            machineOverlay = new Overlay(background);
            darkOverlay = new Overlay(backgroundDark);
            InstantiateReels(symbols, star);

            uiController = new UIController(this);
            currentState = new IdleState(this, uiController);
        }

        private void InstantiateReels(ElementReference symbols, ElementReference star)
        {
            for (int i = 0; i < 5; i++)
            {
                float reelXPos = (float)((500 + (250 * i)) * Game.horizontalScale);
                Reel reel = new Reel(symbols, star, reelXPos, (float)(500 * Game.verticalScale));
                reels.Enqueue(reel);
            }
        }
       
        public void ChangeMachineState(string state)
        {
            switch(state)
            {
                case "Idle":
                    currentState = new IdleState(this, uiController);
                    break;
                case "Spinning":
                    currentState = new SpinState(this, reels, uiController);
                    break;
                case "Payout":
                    currentState = new PayOutState(this, reels, uiController);
                    break;
            }
        }
        
        public void Update(float deltaTime)
        {
            currentState.Update(deltaTime);
            RenderGraphics();
        }

        public async Task DrawBlankBG()
        {
            await Game.context.SetFillStyleAsync("black");
            await Game.context.FillRectAsync(0, 0, Game.canvas.Width, Game.canvas.Height);
        }

        public async void RenderGraphics()
        {
            await DrawBlankBG();
            
            if (darkOverlay != null)
            {
                await darkOverlay.Render();
            }

            foreach (Reel reel in reels)
            {
                await reel.Render();
            }

            if (machineOverlay != null)
            {
                await machineOverlay.Render();
            }
            
            if (uiController != null)
            {
                uiController.Render();
            }
        }
        
    }
}
