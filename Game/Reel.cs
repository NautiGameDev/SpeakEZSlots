using Microsoft.AspNetCore.Components;

namespace SpeakEZSlots.Game
{
    public class Reel
    {
        private Random random = new Random();

        float xPos {  get; set; }
        float yPos { get; set; }

        private int freeSpinSymbolAmt = 4; //Used as a common reward in bonus reels
        private int blankSymbol1Amt = 0;
        private int blankSymbol2Amt = 18;
        private int blankSymbol3Amt = 18;
        private int uncommonSymbol1Amt = 15;
        private int uncommonSymbol2Amt = 8;
        private int rareSymbol1Amt = 5;
        private int rareSymbol2Amt = 4;

        private List<string> reelStrings { get; set; }    
        private List<Symbol> reelSymbols = new List<Symbol>();

        public string topSymbol { get; set; } = "";
        public string midSymbol { get; set; } = "";
        public string botSymbol { get; set; } = "";
        private Queue<string> stops = new Queue<string>();
        private Stack<Symbol> results = new Stack<Symbol>();

        public bool isSpinning = false;
        private float yCutOff {  get; set; }

        public Reel(ElementReference symbols, ElementReference star, ElementReference starParticle, float xpos, float ypos, bool isBonusReel)
        {
            if (isBonusReel)
            {
                ChangeReelProbabilities();
            }

            this.xPos = xpos;
            this.yPos = ypos;
            yCutOff = (float)(1256 * Game.verticalScale);


            SetUpReels();
            InstantiateSymbols(symbols, star, starParticle);
        }

        private void ChangeReelProbabilities()
        {
            freeSpinSymbolAmt = 7; //Used as a common reward in bonus reels
            blankSymbol1Amt = 19;
            blankSymbol2Amt = 19;
            blankSymbol3Amt = 19;
            uncommonSymbol1Amt = 3;
            uncommonSymbol2Amt = 2;
            rareSymbol1Amt = 2;
            rareSymbol2Amt = 1;
        }

        private void SetUpReels()
        {
            List<string> tempReel = new List<string>();

            for (int i = 0; i < freeSpinSymbolAmt; i++)
            {
                tempReel.Add("Free Spin");
            }

            for (int i = 0; i < blankSymbol1Amt; i++)
            {
                tempReel.Add("Blank1");
            }

            for (int i = 0; i < blankSymbol2Amt; i++)
            {
                tempReel.Add("Blank2");
            }

            for (int i = 0; i < blankSymbol3Amt; i++)
            {
                tempReel.Add("Blank3");
            }

            for (int i = 0; i < uncommonSymbol1Amt; i++)
            {
                tempReel.Add("Uncommon1");
            }

            for (int i = 0; i < uncommonSymbol2Amt; i++)
            {
                tempReel.Add("Uncommon2");
            }

            for (int i = 0; i < rareSymbol1Amt; i++)
            {
                tempReel.Add("Rare1");
            }

            for (int i = 0; i < rareSymbol2Amt; i++)
            {
                tempReel.Add("Rare2");
            }

            reelStrings = ShuffleReel(tempReel);
        }

        private List<string> ShuffleReel(List<string> tempReel)
        {
            return tempReel.OrderBy(x => random.Next()).ToList();
        }

        private void InstantiateSymbols(ElementReference symbols, ElementReference star, ElementReference starParticle)
        {
            for (int i = 0; i < 4; i++)
            {
                float symbolYPos = (float)(yPos + (250 * Game.verticalScale) * i);
                Symbol symbol = new Symbol(symbols, star, starParticle, xPos, symbolYPos, i);
                symbol.ChangeSymbol(reelStrings[i]);
                reelSymbols.Add(symbol);
            }
        }

        public async Task Render()
        {
            foreach (Symbol symbol in reelSymbols)
            {
                await symbol.Render();
            }
        }

        public void Update(float deltaTime)
        {
            if(isSpinning || stops.Count > 0)
            {
                SpinReel(deltaTime);
            }
        }

        public void UpdateParticles(float deltaTime)
        {
            foreach (Symbol symbol in reelSymbols)
            {
                if (symbol.particle != null)
                {
                    symbol.particle.Update(deltaTime);

                    if (symbol.particle.setForDeletion)
                    {
                        symbol.particle = null;
                    }
                }
            }
        }

        public async Task RenderParticles()
        {
            foreach (Symbol symbol in reelSymbols)
            {
                if (symbol.particle != null)
                {
                    await symbol.particle.Render();
                }
            }
        }

        private void SpinReel(float deltaTime)
        {
            foreach(Symbol symbol in reelSymbols)
            {
                if(isSpinning)
                {
                    HandleSpinning(deltaTime, symbol);
                }
                else if (!isSpinning)
                {
                    HandleStopping(deltaTime, symbol);
                }
            }
        }

        private void HandleSpinning(float deltaTime, Symbol symbol)
        {
            symbol.yPos += (float)((symbol.fallSpeed * deltaTime) * Game.verticalScale);

            if (symbol.yPos > yCutOff)
            {
                symbol.yPos -= (float)(250 * Game.verticalScale * 4);

                int newSymbolIndex = symbol.symbolIndex;

                if (symbol.symbolIndex + 3 <= 71)
                {
                    symbol.symbolIndex += 3;
                }
                else
                {
                    symbol.symbolIndex += 3;
                    symbol.symbolIndex -= 71;
                }

                symbol.ChangeSymbol(reelStrings[newSymbolIndex]);
            }
        }

        private void HandleStopping(float deltaTime, Symbol symbol)
        {
            if (symbol.isSpinning)
            {
                symbol.yPos += (float)((symbol.fallSpeed / 3 * deltaTime) * Game.verticalScale);
            }
            else if (!symbol.isSpinning && symbol.yPos < symbol.yTarget)
            {
                symbol.yPos += (float)((symbol.fallSpeed / 3 * deltaTime) * Game.verticalScale);
            }

            if (symbol.yPos > yCutOff)
            {
                symbol.yPos -= (float)(250 * Game.verticalScale * 4);

                if (stops.Count > 0)
                {
                    if (stops.Peek() != "")
                    {
                        symbol.ChangeSymbol(stops.Peek());
                        symbol.isSpinning = false;
                        symbol.yTarget = (float)(yPos + (250 * (stops.Count - 1) * Game.verticalScale));
                        stops.Dequeue();
                        results.Push(symbol);
                    }
                    else
                    {
                        stops.Dequeue();
                    }
                }
            }
        }

        public void StartSpinning()
        {
            if(!isSpinning)
            {
                topSymbol = "";
                midSymbol = "";
                botSymbol = "";
                results.Clear();

                isSpinning = true;

                foreach (Symbol symbol in reelSymbols)
                {
                    symbol.isSpinning = true;
                }

                GetNextStop();
            }
        }

        public void StopSpinning()
        {
            if(isSpinning)
            {
                isSpinning = false;
            }
        }

        private void GetNextStop()
        {
            /* 
               Stop is determined by the middle row. String variables are set to symbols before and after middle index.
               Symbols are added to a Queue in order to handle the stopping of the reel.
               As symbols are recycled, the symbol is changed to the first symbol type in the queue then removed until the
               queue reaches 0.
            */

            int randomStop = random.Next(0, reelStrings.Count - 1);

            midSymbol = reelStrings[randomStop];

            if (randomStop > 0)
            {
                topSymbol = reelStrings[randomStop - 1];
            }
            else
            {
                topSymbol = reelStrings[reelStrings.Count - 1];
            }


            if (randomStop < 71)
            {
                botSymbol = reelStrings[randomStop + 1];
            }
            else
            {
                botSymbol = reelStrings[0];
            }

            Console.WriteLine($"| {botSymbol} | {midSymbol} | {topSymbol}");

            stops.Enqueue(botSymbol);
            stops.Enqueue(midSymbol);
            stops.Enqueue(topSymbol);
            stops.Enqueue("");
        }

        public Stack<Symbol> GetResultsForPayLines()
        {
            return results;
        }

    }
}
