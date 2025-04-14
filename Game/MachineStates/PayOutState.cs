namespace SpeakEZSlots.Game.MachineStates
{
    /*
     This class is responsible for testing all paylines of the slot machine.
        enum states are used to allow player keyboard input(space bar) to view the winnings of each payline
        before moving the program forward.
     */

    public class PayOutState : State
    {
        enum PaylinesStates { IDLE, FREESPINTEST, TOPHORIZONTAL, MIDHORIZONTAL, BOTHORIZONTAL, VSHAPE, UPSIDEDOWNV, ZIGZAG, BACKWARDZIGZAG, STOP }
        PaylinesStates currentState = PaylinesStates.IDLE;
        PaylinesStates nextState = PaylinesStates.TOPHORIZONTAL;

        Machine machine {  get; set; }
        UIController uiController { get; set; }

        private Queue<Reel> reels {  get; set; }
        Symbol[] reel1Results { get; set; }
        Symbol[] reel2Results { get; set; }
        Symbol[] reel3Results { get; set; }
        Symbol[] reel4Results { get; set; }
        Symbol[] reel5Results { get; set; }

        List<Symbol> highlighedList { get; set; }

        private float inputTimer = 0.5f;
        private float inputWaitTime = 0.5f;


        public PayOutState(Machine machine, Queue<Reel> reels, UIController uiController) 
        {
            this.machine = machine;
            this.uiController = uiController;
            this.reels = reels;

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

                case PaylinesStates.ZIGZAG:
                    CompileResults(0, 0, 1, 2, 2);
                    break;

                case PaylinesStates.BACKWARDZIGZAG:
                    CompileResults(2, 2, 1, 0, 0);
                    break;
                case PaylinesStates.STOP:
                    if (highlighedList != null)
                    {
                        foreach (Symbol symbol in highlighedList)
                        {
                            symbol.isHighlighted = false;
                        }
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

        /*
            Results are compiled into a list of Symbols according to indices of each reel, passed in through parameters.
            List is then iterated through and organized into Lists according to symbol type.
            Symbol lists are tested for count (3, 4, or 5) to test for payout events
         */
        private void TestFreeSpins()
        {
            List<Symbol> rubies = new List<Symbol>();

            foreach (Symbol symbol in reel1Results)
            {
                if (symbol.symbol == "Ruby")
                {
                    rubies.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel2Results)
            {
                if (symbol.symbol == "Ruby")
                {
                    rubies.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel3Results)
            {
                if (symbol.symbol == "Ruby")
                {
                    rubies.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel4Results)
            {
                if (symbol.symbol == "Ruby")
                {
                    rubies.Add(symbol);
                }
            }
            foreach (Symbol symbol in reel5Results)
            {
                if (symbol.symbol == "Ruby")
                {
                    rubies.Add(symbol);
                }
            }

            if (rubies.Count >= 3)
            {
                uiController.UpdateMessageBar($"You won {rubies.Count} free spins!");
                currentState = PaylinesStates.IDLE;

                highlighedList = rubies;
                foreach (Symbol symbol in highlighedList)
                {
                    symbol.isHighlighted = true;
                }


                machine.freeSpins += rubies.Count();
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
            List<Symbol> cherries = new List<Symbol>();
            List<Symbol> strawberries = new List<Symbol>();
            List<Symbol> emerald = new List<Symbol>();
            List<Symbol> diamond = new List<Symbol>();

            foreach (Symbol symbol in results)
            {
                if (symbol.symbol == "Cherries")
                {
                    cherries.Add(symbol);
                }
                else if (symbol.symbol == "Strawberries")
                {
                    strawberries.Add(symbol);
                }
                else if (symbol.symbol == "Emerald")
                {
                    emerald.Add(symbol);
                }
                else if (symbol.symbol == "Diamond")
                {
                    diamond.Add(symbol);
                }
            }

            TestForWinnings(cherries, strawberries, emerald, diamond);
        }

        private void TestForWinnings(List<Symbol> cherries, List<Symbol> strawberries, List<Symbol> emerald, List<Symbol> diamond)
        {
            int calculatedWinnings = 0;

            //Cherries
            if (cherries.Count == 3)
            {
                calculatedWinnings = machine.bet * 2;
                highlighedList = cherries;
            }
            else if (cherries.Count == 4)
            {
                calculatedWinnings = machine.bet * 8;
                highlighedList = cherries;
            }
            else if (cherries.Count == 5)
            {
                calculatedWinnings = machine.bet * 30;
                highlighedList = cherries;
            }

            //Strawberries
            else if (strawberries.Count == 3)
            {
                calculatedWinnings = machine.bet * 3;
                highlighedList = strawberries;
            }
            else if (strawberries.Count == 4)
            {
                calculatedWinnings = machine.bet * 10;
                highlighedList = strawberries;
            }
            else if (strawberries.Count == 5)
            {
                calculatedWinnings = machine.bet * 40;
                highlighedList = strawberries;
            }

            //Emerald
            else if (emerald.Count == 3)
            {
                calculatedWinnings = machine.bet * 8;
                highlighedList = emerald;
            }
            else if (emerald.Count == 4)
            {
                calculatedWinnings = machine.bet * 25;
                highlighedList = emerald;
            }
            else if (emerald.Count == 5)
            {
                calculatedWinnings = machine.bet * 100;
                highlighedList = emerald;
            }

            //Diamond
            else if (diamond.Count == 3)
            {
                calculatedWinnings = machine.bet * 25;
                highlighedList = diamond;
            }
            else if (diamond.Count == 4)
            {
                calculatedWinnings = machine.bet * 100;
                highlighedList = diamond;
            }
            else if (diamond.Count == 5)
            {
                calculatedWinnings = machine.bet * 500;
                highlighedList = diamond;
            }


            if (calculatedWinnings > 0)
            {
                uiController.UpdateMessageBar($"You won {calculatedWinnings} credits!");
                machine.winnings += calculatedWinnings;
                uiController.UpdatePlayerWinnings(calculatedWinnings);

                foreach (Symbol symbol in highlighedList)
                {
                    symbol.isHighlighted = true;
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
                    return PaylinesStates.ZIGZAG;
                case PaylinesStates.ZIGZAG:
                    return PaylinesStates.BACKWARDZIGZAG;
                case PaylinesStates.BACKWARDZIGZAG:
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
