using static SpeakEZSlots.Game.Machine;

namespace SpeakEZSlots.Game.MachineStates
{
    /*
     This class is responsible for testing all paylines of the slot machine.
        enum states are used to allow player keyboard input(space bar) to view the winnings of each payline
        before moving the program forward.
     */

    public class PayOutState : State
    {
        enum PaylinesStates { IDLE, 
            FREESPINTEST, 
            TOPHORIZONTAL, 
            MIDHORIZONTAL, 
            BOTHORIZONTAL, 
            VSHAPE, 
            UPSIDEDOWNV, 
            FORWARDSLASH, 
            BACKSLASH, 
            ZIGZAG,
            BACKWARDZAG,
            TESTBONUS,
            STOP }
        PaylinesStates currentState = PaylinesStates.IDLE;
        PaylinesStates nextState = PaylinesStates.TOPHORIZONTAL;

        Machine machine {  get; set; }
        UIController uiController { get; set; }
        SoundController soundController { get; set; }

        private Queue<Reel> reels {  get; set; }
        Symbol[] reel1Results { get; set; }
        Symbol[] reel2Results { get; set; }
        Symbol[] reel3Results { get; set; }
        Symbol[] reel4Results { get; set; }
        Symbol[] reel5Results { get; set; }

        List<Symbol> highlighedList { get; set; }

        private float inputTimer = 0.5f;
        private float inputWaitTime = 0.5f;


        public PayOutState(Machine machine, Queue<Reel> reels, UIController uiController, SoundController soundController) 
        {
            this.machine = machine;
            this.uiController = uiController;
            this.soundController = soundController;
            this.reels = reels;

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

            foreach (Reel reel in reels)
            {
                results["Reel" + reelNumber] = reel.GetResultsForPayLines();
                reelNumber++;
            }

            reel1Results = results["Reel1"].ToArray();
            reel2Results = results["Reel2"].ToArray();
            reel3Results = results["Reel3"].ToArray();
            reel4Results = results["Reel4"].ToArray();
            reel5Results = results["Reel5"].ToArray();

            currentState = PaylinesStates.FREESPINTEST;
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

                case PaylinesStates.FREESPINTEST:
                    TestFreeSpins();
                    break;

                case PaylinesStates.TOPHORIZONTAL:
                    CompileResults(0, 0, 0, 0, 0);
                    break;

                case PaylinesStates.MIDHORIZONTAL:
                    CompileResults(1, 1, 1, 1, 1);
                    break;

                case PaylinesStates.BOTHORIZONTAL:
                    CompileResults(2, 2, 2, 2, 2);
                    break;

                case PaylinesStates.VSHAPE:
                    CompileResults(0, 1, 2, 1, 0);
                    break;

                case PaylinesStates.UPSIDEDOWNV:
                    CompileResults(2, 1, 0, 1, 2);
                    break;

                case PaylinesStates.FORWARDSLASH:
                    CompileResults(0, 0, 1, 2, 2);
                    break;

                case PaylinesStates.BACKSLASH:
                    CompileResults(2, 2, 1, 0, 0);
                    break;

                case PaylinesStates.ZIGZAG:
                    CompileResults(1, 0, 1, 2, 1);
                    break;

                case PaylinesStates.BACKWARDZAG:
                    CompileResults(1, 2, 1, 0, 1);
                    break;

                case PaylinesStates.TESTBONUS:
                    TestBonus();
                    break;

                case PaylinesStates.STOP:
                    if (highlighedList != null)
                    {
                        foreach (Symbol symbol in highlighedList)
                        {
                            symbol.isHighlighted = false;
                        }
                    }

                    if ((machine.playerCredits + machine.winnings) < machine.minBet)
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

                if (machine.announcingBonus)
                {
                    machine.announcingBonus = false;
                }
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

        /*
            Results are compiled into a list of Symbols according to indices of each reel, passed in through parameters.
            List is then iterated through and organized into Lists according to symbol type.
            Symbol lists are tested for count (3, 4, or 5) to test for payout events
         */
        private void TestFreeSpins()
        {
            List<Symbol> freeSpinSymbols = new List<Symbol>();

            foreach (Symbol symbol in reel1Results)
            {
                if (symbol.symbol == "Free Spin")
                {
                    freeSpinSymbols.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel2Results)
            {
                if (symbol.symbol == "Free Spin")
                {
                    freeSpinSymbols.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel3Results)
            {
                if (symbol.symbol == "Free Spin")
                {
                    freeSpinSymbols.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel4Results)
            {
                if (symbol.symbol == "Free Spin")
                {
                    freeSpinSymbols.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel5Results)
            {
                if (symbol.symbol == "Free Spin")
                {
                    freeSpinSymbols.Add(symbol);
                }
            }

            if (freeSpinSymbols.Count >= 3)
            {
                uiController.UpdateMessageBar($"You won {freeSpinSymbols.Count} free spins!");
                
                soundController.PlayFreeSpinsSound();
                currentState = PaylinesStates.IDLE;

                highlighedList = freeSpinSymbols;
                foreach (Symbol symbol in highlighedList)
                {
                    symbol.isHighlighted = true;
                    symbol.SpawnParticle((float)(325 * Game.horizontalScale), (float)(955 * Game.verticalScale));
                }


                machine.freeSpins += freeSpinSymbols.Count();
                uiController.UpdateFreeSpins(machine.freeSpins);
                currentState = PaylinesStates.IDLE;
            }
            else
            {
                currentState = nextState;
            }

        }


        private void CompileResults(int reel1Index, int reel2Index, int reel3Index, int reel4Index, int reel5Index)
        {
            List<Symbol> results = new List<Symbol>();

            results.Add(reel1Results[reel1Index]);
            results.Add(reel2Results[reel2Index]);
            results.Add(reel3Results[reel3Index]);
            results.Add(reel4Results[reel4Index]);
            results.Add(reel5Results[reel5Index]);

            OrganizeResults(results);
        }

        private void OrganizeResults(List<Symbol> results)
        {
            List<Symbol> uncommon1Symbols = new List<Symbol>();
            List<Symbol> uncommon2Symbols = new List<Symbol>();
            List<Symbol> rare1Symbols = new List<Symbol>();
            List<Symbol> rare2Symbols = new List<Symbol>();

            foreach (Symbol symbol in results)
            {
                if (symbol.symbol == "Uncommon1")
                {
                    uncommon1Symbols.Add(symbol);
                }
                else if (symbol.symbol == "Uncommon2")
                {
                    uncommon2Symbols.Add(symbol);
                }
                else if (symbol.symbol == "Rare1")
                {
                    rare1Symbols.Add(symbol);
                }
                else if (symbol.symbol == "Rare2")
                {
                    rare2Symbols.Add(symbol);
                }
            }

            TestForWinnings(uncommon1Symbols, uncommon2Symbols, rare1Symbols, rare2Symbols);
        }

        private void TestForWinnings(List<Symbol> uncommon1Symbols, List<Symbol> uncommon2Symbols, List<Symbol> rare1Symbols, List<Symbol> rare2Symbols)
        {
            int calculatedWinnings = 0;

            //Uncommon 1 Symbol
            if (uncommon1Symbols.Count == 3)
            {
                calculatedWinnings = (int)(machine.bet * 0.25);
                highlighedList = uncommon1Symbols;
            }
            else if (uncommon1Symbols.Count == 4)
            {
                calculatedWinnings = machine.bet * 2;
                highlighedList = uncommon1Symbols;
            }
            else if (uncommon1Symbols.Count == 5)
            {
                calculatedWinnings = machine.bet * 10;
                highlighedList = uncommon1Symbols;
            }

            //Uncommon 2 Symbol
            else if (uncommon2Symbols.Count == 3)
            {
                calculatedWinnings = machine.bet * 1;
                highlighedList = uncommon2Symbols;
            }
            else if (uncommon2Symbols.Count == 4)
            {
                calculatedWinnings = machine.bet * 5;
                highlighedList = uncommon2Symbols;
            }
            else if (uncommon2Symbols.Count == 5)
            {
                calculatedWinnings = machine.bet * 25;
                highlighedList = uncommon2Symbols;
            }

            //Rare 1 Symbol
            else if (rare1Symbols.Count == 3)
            {
                calculatedWinnings = machine.bet * 5;
                highlighedList = rare1Symbols;
            }
            else if (rare1Symbols.Count == 4)
            {
                calculatedWinnings = machine.bet * 25;
                highlighedList = rare1Symbols;
            }
            else if (rare1Symbols.Count == 5)
            {
                calculatedWinnings = machine.bet * 125;
                highlighedList = rare1Symbols;
            }

            //Rare 2 Symbol
            else if (rare2Symbols.Count == 3)
            {
                calculatedWinnings = machine.bet * 10;
                highlighedList = rare2Symbols;
            }
            else if (rare2Symbols.Count == 4)
            {
                calculatedWinnings = machine.bet * 50;
                highlighedList = rare2Symbols;
            }
            else if (rare2Symbols.Count == 5)
            {
                calculatedWinnings = machine.bet * 250;
                highlighedList = rare2Symbols;
            }


            if (calculatedWinnings > 0)
            {
                uiController.UpdateMessageBar($"You won {calculatedWinnings} credits!");
                soundController.PlayGainSound();
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

        private PaylinesStates FindNextState()
        {
            switch (nextState)
            {
                case PaylinesStates.FREESPINTEST:
                    return PaylinesStates.TOPHORIZONTAL;
                case PaylinesStates.TOPHORIZONTAL:
                    return PaylinesStates.MIDHORIZONTAL;
                case PaylinesStates.MIDHORIZONTAL:
                    return PaylinesStates.BOTHORIZONTAL;
                case PaylinesStates.BOTHORIZONTAL:
                    return PaylinesStates.VSHAPE;
                case PaylinesStates.VSHAPE:
                    return PaylinesStates.UPSIDEDOWNV;
                case PaylinesStates.UPSIDEDOWNV:
                    return PaylinesStates.FORWARDSLASH;
                case PaylinesStates.FORWARDSLASH:
                    return PaylinesStates.BACKSLASH;
                case PaylinesStates.BACKSLASH:
                    return PaylinesStates.ZIGZAG;
                case PaylinesStates.ZIGZAG:
                    return PaylinesStates.BACKWARDZAG;
                case PaylinesStates.BACKWARDZAG:
                    return PaylinesStates.TESTBONUS;
                case PaylinesStates.TESTBONUS:
                    return PaylinesStates.STOP;
                default:
                    return PaylinesStates.IDLE;
            }
        }

        private void TestBonus()
        {
            if(machine.totalSpins >= machine.spinsForBonus)
            {
                machine.announcingBonus = true;
                soundController.PlayBonusRoundSound();
                currentState = PaylinesStates.IDLE;
                nextState = FindNextState();
            }
            else
            {
                nextState = FindNextState();
                currentState = nextState;
            }
        }
    }
}
