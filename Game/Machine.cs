using Microsoft.AspNetCore.Components;
using SpeakEZSlots.Game.MachineStates;

namespace SpeakEZSlots.Game
{
    public class Machine
    {
        public enum menuStates { MAINMENU, STARTING, PLAYING, GAMEOVER }
        public menuStates menuState = menuStates.MAINMENU;

        State currentState {  get; set; }
        UIController uiController { get; set; }
        public SoundController soundController { get; set; }

        Overlay mainMenu { get; set; }
        Overlay gameover {  get; set; }
        Overlay machineOverlay { get; set; }
        Overlay darkOverlay { get; set; }
        Overlay bonusOverlay { get; set; }
        Overlay bonusOverlayDark { get; set; }
        Overlay bonusRoundAnnouncement {  get; set; }
        Overlay howToPlayScreen { get; set; }


        private Queue<Reel> reels = new Queue<Reel>();
        private Queue<Reel> bonusReels = new Queue<Reel>();

        public int freeSpins = 0;
        public int playerCredits = 1000;
        public int bet = 25;
        public int winnings = 0;

        public int minBet = 25;
        public int maxBet = 250;
        public int betStep = 25;

        public int totalSpins = 0;
        public int bonusSpins = 3;
        public int spinsForBonus = 0;

        public bool bonusActive = false;
        public bool announcingBonus = false;
        public bool showHowToPlay = false;

        private Random random = new Random();

        public Machine(ElementReference mainMenuScreen, ElementReference gameoverScreen, ElementReference background, ElementReference backgroundDark, ElementReference backgroundBonusSprite, ElementReference backgroundBonusSpriteDark, ElementReference symbols, ElementReference bonusSymbols, ElementReference star, ElementReference starParticle, ElementReference bonusRoundAnnouncement, ElementReference howToPlayScreen, ElementReference soundButtonOn, ElementReference soundButtonOff, ElementReference howToPlayButton)
        {
            mainMenu = new Overlay(mainMenuScreen);
            gameover = new Overlay(gameoverScreen);
            machineOverlay = new Overlay(background);
            darkOverlay = new Overlay(backgroundDark);
            bonusOverlay = new Overlay(backgroundBonusSprite);
            bonusOverlayDark = new Overlay(backgroundBonusSpriteDark);
            this.howToPlayScreen = new Overlay(howToPlayScreen);
            this.bonusRoundAnnouncement = new Overlay(bonusRoundAnnouncement);

            

            InstantiateReels(symbols, bonusSymbols, star, starParticle);

            CalculateBonusState();

            uiController = new UIController(this, soundButtonOn, soundButtonOff, howToPlayButton);
            soundController = new SoundController();
            currentState = new IdleState(this, uiController, soundController);
        }

        private void InstantiateReels(ElementReference symbols, ElementReference bonusSymbols, ElementReference star, ElementReference starParticle)
        {
            for (int i = 0; i < 5; i++)
            {
                float reelXPos = (float)((500 + (250 * i)) * Game.horizontalScale);

                Reel reel = new Reel(symbols, star, starParticle, reelXPos, (float)(500 * Game.verticalScale), false);
                reels.Enqueue(reel);

                Reel bonusReel = new Reel(bonusSymbols, star, starParticle, reelXPos, (float)(500 * Game.verticalScale), true);
                bonusReels.Enqueue(bonusReel);
            }
        }

        public void CalculateBonusState()
        {
            totalSpins = 0;
            bonusSpins = 3;
            spinsForBonus = random.Next(15, 35);
        }
       
        public void ChangeMachineState(string state)
        {
            switch(state)
            {
                case "Idle":
                    currentState = new IdleState(this, uiController, soundController);

                    if (totalSpins >= spinsForBonus)
                    {
                        bonusActive = true;
                    }
                    else
                    {
                        bonusActive = false;
                    }

                    break;
                case "Spinning":
                    currentState = new SpinState(this, reels, uiController, soundController);
                    bonusActive = false;
                    break;
                case "Payout":
                    currentState = new PayOutState(this, reels, uiController, soundController);
                    break;
                case "Bonus Spinning":
                    currentState = new BonusSpinState(this, bonusReels, uiController, soundController);
                    bonusActive = true;
                    break;
                case "Bonus Payout":
                    currentState = new BonusPayOutState(this, bonusReels, uiController, soundController);
                    break;
            }

            Console.WriteLine($"Changing state to {state}");
        }
        
        public async void Update(float deltaTime)
        {
            if (menuState == menuStates.MAINMENU)
            {
                RenderMainMenu();

                if (InputController.playerInput[" "] == true)
                {
                    menuState = menuStates.STARTING;
                    await soundController.StartGame();
                    await Task.Delay(1000);
                    menuState = menuStates.PLAYING;
                    soundController.PlayMusic();
                }
            }

            else if (menuState == menuStates.PLAYING)
            {
                if (!showHowToPlay)
                {
                    currentState.Update(deltaTime);
                }
                
                uiController.Update(deltaTime);
                RenderPlayingGraphics();

                if (!bonusActive)
                {
                    foreach (Reel reel in reels)
                    {
                        reel.UpdateParticles(deltaTime);
                    }
                }
                else if (bonusActive)
                {
                    foreach (Reel reel in bonusReels)
                    {
                        reel.UpdateParticles(deltaTime);
                    }
                }
            }

            else if (menuState == menuStates.GAMEOVER)
            {
                RenderGameOver();
            }
        }

        public async Task DrawBlankBG()
        {
            await Game.context.SetFillStyleAsync("black");
            await Game.context.FillRectAsync(0, 0, Game.canvas.Width, Game.canvas.Height);
        }

        public async void RenderPlayingGraphics()
        {
            await DrawBlankBG();


            if (darkOverlay != null && !bonusActive)
            {
                await darkOverlay.Render();
            }
            else if (bonusOverlayDark != null && bonusActive)
            {
                await bonusOverlayDark.Render();
            }

            if (!bonusActive)
            {
                foreach (Reel reel in reels)
                {
                    await reel.Render();
                }
            }
            else if (bonusActive)
            {
                foreach (Reel reel in bonusReels)
                {
                    await reel.Render();
                }
            }

            if (machineOverlay != null && !bonusActive)
            {
                await machineOverlay.Render();
            }
            else if (bonusOverlay != null && bonusActive)
            {
                await bonusOverlay.Render();
            }

            if (uiController != null)
            {
                await uiController.Render();
            }

            if (reels != null)
            {
                foreach (Reel reel in reels)
                {
                    await reel.RenderParticles();
                }
            }

            if (bonusReels != null)
            {
                foreach (Reel reel in bonusReels)
                {
                    await reel.RenderParticles();
                }
            }


            if (bonusRoundAnnouncement != null && announcingBonus)
            {
                await bonusRoundAnnouncement.Render();
            }

            if(howToPlayScreen != null && showHowToPlay)
            {
                await howToPlayScreen.Render();
            }
        }

        private async void RenderMainMenu()
        {
            if (mainMenu != null)
            {
                await mainMenu.Render();
            }
        }

        private async void RenderGameOver()
        {
            if (gameover != null)
            {
                await gameover.Render();
            }
        }

    }
}
