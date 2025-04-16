using static SpeakEZSlots.Game.Machine;

namespace SpeakEZSlots.Game.MachineStates
{
    /*
     This class is responsible for testing bonus symbols in slot machine.
        enum states are used to allow player keyboard input(space bar) to view the winnings of each payline
        before moving the program forward.
    Bonus round ignores paylines and only tests for existence of the symbols.
     */

    public class BonusPayOutState : State
    {
        enum PaylinesStates { IDLE, TESTRARES, TESTUNCOMMONS, TESTCOMMONS, STOP }
        PaylinesStates currentState = PaylinesStates.IDLE;
        PaylinesStates nextState = PaylinesStates.TESTRARES;

        Machine machine { get; set; }
        UIController uiController { get; set; }
        SoundController soundController { get; set; }

        private Queue<Reel> bonusReels { get; set; }
        Symbol[] reel1Results { get; set; }
        Symbol[] reel2Results { get; set; }
        Symbol[] reel3Results { get; set; }
        Symbol[] reel4Results { get; set; }
        Symbol[] reel5Results { get; set; }

        List<Symbol> highlighedList { get; set; }

        private float inputTimer = 0.5f;
        private float inputWaitTime = 0.5f;


        public BonusPayOutState(Machine machine, Queue<Reel> bonusReels, UIController uiController, SoundController soundController)
        {
            this.machine = machine;
            this.uiController = uiController;
            this.soundController = soundController;
            this.bonusReels = bonusReels;
            

            if (machine.winnings > 0)
            {
                machine.playerCredits += machine.winnings;
                machine.winnings = 0;

                uiController.UpdatePlayerCredits(machine.playerCredits);
                uiController.UpdatePlayerWinnings(machine.winnings);
            }

            ImportResults();
        }

        private void ImportResults()
        {
            Dictionary<string, Stack<Symbol>> results = new Dictionary<string, Stack<Symbol>>();
            int reelNumber = 1;

            foreach (Reel reel in bonusReels)
            {
                results["Reel" + reelNumber] = reel.GetResultsForPayLines();
                reelNumber++;
            }

            reel1Results = results["Reel1"].ToArray();
            reel2Results = results["Reel2"].ToArray();
            reel3Results = results["Reel3"].ToArray();
            reel4Results = results["Reel4"].ToArray();
            reel5Results = results["Reel5"].ToArray();

            currentState = PaylinesStates.TESTRARES;
        }

        public override void Update(float deltaTime)
        {
            if (inputTimer >= 0)
            {
                inputTimer -= deltaTime;
            }

            switch (currentState)
            {
                case PaylinesStates.IDLE:
                    HandlePlayerInput(deltaTime);
                    break;

                case PaylinesStates.TESTRARES:
                    TestBonuses("Rare1", "Rare2");
                    break;
                case PaylinesStates.TESTUNCOMMONS:
                    TestBonuses("Uncommon1", "Uncommon2");
                    break;
                case PaylinesStates.TESTCOMMONS:
                    TestBonuses("Free Spin", "Free Spin");
                    break;
                case PaylinesStates.STOP:
                    if (highlighedList != null)
                    {
                        foreach (Symbol symbol in highlighedList)
                        {
                            symbol.isHighlighted = false;
                        }
                    }

                    if (machine.playerCredits < machine.minBet)
                    {
                        machine.menuState = menuStates.GAMEOVER;
                    }

                    machine.ChangeMachineState("Idle");
                    break;
            }
        }

        private void HandlePlayerInput(float deltaTime)
        {
            if (InputController.playerInput[" "] == true && inputTimer <= 0)
            {
                MoveToNextState();
                inputTimer = inputWaitTime;
            }
        }

        private void MoveToNextState()
        {
            currentState = nextState;

            if (highlighedList != null)
            {
                foreach (Symbol symbol in highlighedList)
                {
                    symbol.isHighlighted = false;
                }

                highlighedList = null;
            }
        }

        
        private void TestBonuses(string type1Symbol, string type2Symbol)
        {
            List<Symbol> symbols = new List<Symbol>();

            foreach (Symbol symbol in reel1Results)
            {
                if (symbol.symbol == type1Symbol || symbol.symbol == type2Symbol)
                {
                    symbols.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel2Results)
            {
                if (symbol.symbol == type1Symbol || symbol.symbol == type2Symbol)
                {
                    symbols.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel3Results)
            {
                if (symbol.symbol == type1Symbol || symbol.symbol == type2Symbol)
                {
                    symbols.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel4Results)
            {
                if (symbol.symbol == type1Symbol || symbol.symbol == type2Symbol)
                {
                    symbols.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel5Results)
            {
                if (symbol.symbol == type1Symbol || symbol.symbol == type2Symbol)
                {
                    symbols.Add(symbol);
                }
            }

            int calculatedWinnings = 0;

            foreach (Symbol symbol in symbols)
            {
                calculatedWinnings += GetWinRatio() * machine.bet;
            }

            if (calculatedWinnings > 0)
            {
                highlighedList = symbols;
                soundController.PlayGainSound();
                uiController.UpdateMessageBar($"You won {calculatedWinnings} credits!");
                machine.winnings += calculatedWinnings;
                uiController.UpdatePlayerWinnings(machine.winnings);

                foreach (Symbol symbol in highlighedList)
                {
                    symbol.isHighlighted = true;
                    symbol.SpawnParticle((float)(245 * Game.horizontalScale), (float)(1255 * Game.verticalScale));
                }

                currentState = PaylinesStates.IDLE;
                nextState = FindNextState();
            }
            else
            {
                nextState = FindNextState();
                currentState = nextState;
            }

        }

        private int GetWinRatio()
        {
            if (currentState == PaylinesStates.TESTRARES)
            {
                return 4;
            }
            else if (currentState == PaylinesStates.TESTUNCOMMONS)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        private PaylinesStates FindNextState()
        {
            switch (nextState)
            {
                case PaylinesStates.TESTRARES:
                    return PaylinesStates.TESTUNCOMMONS;
                case PaylinesStates.TESTUNCOMMONS:
                    return PaylinesStates.TESTCOMMONS;
                case PaylinesStates.TESTCOMMONS:
                    return PaylinesStates.STOP;
                default:
                    //if (highlighedList != null)
                    //{
                    //    foreach (Symbol symbol in highlighedList)
                    //    {
                    //        symbol.isHighlighted = false;
                    //    }
                    //}

                    //machine.ChangeMachineState("Idle");
                    return PaylinesStates.IDLE;
            }
        }


    }
}
