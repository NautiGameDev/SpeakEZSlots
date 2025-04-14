using Microsoft.AspNetCore.Components;

namespace SpeakEZSlots.Game
{
    public class Machine
    {
        public enum MachineStates { IDLE, SPINNING, PAYLINES, PAYOUT, BONUS }
        public MachineStates currentState = MachineStates.IDLE;

        Overlay machineOverlay { get; set; }
        Overlay darkOverlay { get; set; }
        private Paylines paylinesCalculator { get; set; }

        private Queue<Reel> reels = new Queue<Reel>();

        private float spinTimer = 0;
        private float timeToSpin = 0.75f;

        private float inputTimer = 0;
        private float inputWaitTime = 0.5f;

        private float winningsTimer = 0;
        private float winningsDelay = 0.01f;

        private int freeSpins = 0;

        private int playerCredits = 1000;
        public int bet = 10;
        private int winnings = 0;

        TextElement pCredits {  get; set; }
        TextElement pBet { get; set; }
        TextElement pWinnings { get; set; }
        TextElement messageBar { get; set; }

        public Machine(ElementReference background, ElementReference backgroundDark, ElementReference symbols, ElementReference star)
        {
            machineOverlay = new Overlay(background);
            darkOverlay = new Overlay(backgroundDark);
            paylinesCalculator = new Paylines(this);
            InstantiateReels(symbols, star);
            InstantiateText();
        }

        #region machine set-up

        private void InstantiateReels(ElementReference symbols, ElementReference star)
        {
            for (int i = 0; i < 5; i++)
            {
                float reelXPos = (float)((500 + (250 * i)) * Game.horizontalScale);
                Reel reel = new Reel(symbols, star, reelXPos, (float)(500 * Game.verticalScale));
                reels.Enqueue(reel);
            }
        }
        
        private void InstantiateText()
        {
            
            int fontSize = (int)(62 * Game.verticalScale);
            string textSize = fontSize.ToString() + "px";

            pCredits = new TextElement(playerCredits.ToString(), textSize, (float)(245 * Game.horizontalScale), (float)(595 * Game.verticalScale));
            pBet = new TextElement(bet.ToString(), textSize, (float)(245 * Game.horizontalScale), (float)(955 * Game.verticalScale));
            pWinnings = new TextElement(winnings.ToString(), textSize, (float)(245 * Game.horizontalScale), (float)(1255 * Game.verticalScale));

            fontSize = (int)(48 * Game.verticalScale);
            textSize = fontSize.ToString() + "px";
            messageBar = new TextElement("Press space bar to spin!", textSize, (float)(1115 * Game.horizontalScale), (float)(470 * Game.verticalScale));
        }

        #endregion

        #region update methods
        public void Update(float deltaTime)
        {
            TestPlayerInput(deltaTime);

            if (currentState == MachineStates.IDLE)
            {
                if(freeSpins > 0)
                {
                    UpdateMessageBar($"Press space bar to spin! Free spins remaining: {freeSpins}.");
                }
                else
                {
                    UpdateMessageBar("Press space bar to spin!");
                }

                TransferWinnings(deltaTime);
            }

            if (currentState == MachineStates.SPINNING) 
            {
                HandleSpinning(deltaTime);
                TransferWinnings(deltaTime);
            }
            if (currentState == MachineStates.PAYLINES)
            {
                paylinesCalculator.TestPayLines();
            }

            UpdateReels(deltaTime);
            RenderGraphics();
        }

        private void UpdateReels(float deltaTime)
        {
            foreach (Reel reel in reels)
            {
                reel.Update(deltaTime);
            }
        }

        #endregion

        #region Spinning and Payouts
        private void TestPlayerInput(float deltaTime) //Called every frame
        {
            
            if (InputController.playerInput[" "] == true && inputTimer <= 0)
            {
                if (currentState == MachineStates.IDLE)
                {
                    foreach (Reel reel in reels)
                    {
                        reel.StartSpinning();
                    }

                    if (freeSpins > 0)
                    {
                        freeSpins--;
                    }
                    else
                    {
                        playerCredits -= bet;
                        pCredits.UpdateMessage(playerCredits.ToString());
                    }
                    
                    currentState = MachineStates.SPINNING;
                    spinTimer = timeToSpin;
                    inputTimer = inputWaitTime;
                }
                if(currentState == MachineStates.PAYOUT)
                {
                    ChangePayoutState("Paylines");
                    paylinesCalculator.ContinueTesting();
                    inputTimer = inputWaitTime;
                }
            }

            if (inputTimer > 0)
            {
                inputTimer -= deltaTime;
            }
        }

        

        private void HandleSpinning(float deltaTime)
        {
            spinTimer -= deltaTime;

            if (spinTimer < 0)
            {
                Reel firstReel = reels.Peek();

                if (firstReel.isSpinning)
                {
                    firstReel.StopSpinning();
                    reels.Dequeue();
                    reels.Enqueue(firstReel);

                    spinTimer = timeToSpin;
                }
                else
                {
                    currentState = MachineStates.PAYLINES;
                    TestPayLines();
                }
            }
        }

        private void TestPayLines()
        {
            Dictionary<string, Stack<Symbol>> results = new Dictionary<string, Stack<Symbol>>();
            int reelNumber = 1;

            foreach (Reel reel in reels)
            {
                results["Reel" + reelNumber] = reel.GetResultsForPayLines();
                reelNumber++;
            }

            paylinesCalculator.UpdateResults(results);
        }

        public void ChangePayoutState(string state)
        {
            switch (state)
            {
                case "Paylines":
                    currentState = MachineStates.PAYLINES;
                    break;
                case "Payout":
                    currentState = MachineStates.PAYOUT;
                    break;
                case "Idle":
                    currentState = MachineStates.IDLE;
                    UpdateMessageBar("Press space bar to spin!");
                    break;
            }
        }

        public void AddFreeSpins(int amount)
        {
            freeSpins += amount;
        }

        public void UpdateMessageBar(string message)
        {
            messageBar.UpdateMessage(message);
        }

        public void PayPlayer(int payout)
        {
            winnings += payout;
            pWinnings.UpdateMessage(winnings.ToString());
            pCredits.UpdateMessage(playerCredits.ToString());
        }

        private void TransferWinnings(float deltaTime)
        {
            if (winningsTimer > 0)
            {
                winningsTimer -= deltaTime;
            }

            if (winningsTimer <= 0 && winnings > 0)
            {
                winningsTimer = winningsDelay;
                winnings--;
                playerCredits++;
                pWinnings.UpdateMessage(winnings.ToString());
                pCredits.UpdateMessage(playerCredits.ToString());
            }
        }

        #endregion

        #region rendering methods ~~~

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
            
            if (pCredits != null) 
            { 
                await pCredits.Render(); 
            }

            if(pBet != null)
            {
                await pBet.Render();
            }

            if(pWinnings != null)
            {
                await pWinnings.Render();
            }

            if(messageBar != null)
            {
                await messageBar.Render();
            }
            
        }
        #endregion
    }
}
