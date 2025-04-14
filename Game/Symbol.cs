using Microsoft.AspNetCore.Components;

namespace SpeakEZSlots.Game
{
    public class Symbol
    {
        private ElementReference symbolsSprite { set; get; }
        private int spriteBaseWidth { set; get; }
        private int spriteBaseHeight { set; get; }

        private ElementReference starSprite { set; get; }
        private int starBaseWidth { set; get; }
        private int starBaseHeight { set; get; }

        public string symbol {  set; get; }
        private int spriteIndex { set; get; } = 0; //Sprite index corresponds to the y position on sprite sheet
        public int symbolIndex { set; get; } //Symbol index corresponds to the index on the reel (0-71)
        private float xPos {  set; get; }
        public float yPos { set; get; }
        public float yTarget { set; get; }
        public float fallSpeed { set; get; } = 4000.0f;

        public bool isSpinning = false;
        public bool isHighlighted = false;

        public Symbol(ElementReference symbols, ElementReference star, float xPos, float yPos, int symbolIndex)
        {
            this.symbolsSprite = symbols;
            spriteBaseWidth = (int)(250 * Game.horizontalScale);
            spriteBaseHeight = (int)(250 * Game.verticalScale);

            starSprite = star;
            starBaseWidth = (int)(250 * Game.horizontalScale);
            starBaseHeight = (int)(250 * Game.verticalScale);
            this.symbolIndex = symbolIndex;
            
            this.xPos = xPos;
            this.yPos = yPos;
            this.yTarget = yPos;

        }

        public void ChangeSymbol(string symbolName)
        {
            
            switch(symbolName)
            {
                case ("Ruby"):
                    spriteIndex = 0;
                    break;
                case ("Red Lotus"):
                    spriteIndex = 1; 
                    break;
                case ("Dandelion"):
                    spriteIndex = 2;
                    break;
                case ("Blue Flower"):
                    spriteIndex = 3;
                    break;
                case ("Cherries"):
                    spriteIndex = 4;
                    break;
                case ("Strawberries"):
                    spriteIndex = 5;
                    break;
                case ("Emerald"):
                    spriteIndex= 6;
                    break;
                case ("Diamond"):
                    spriteIndex = 7;
                    break;
            }

            symbol = symbolName;

        }

        public async Task Render()
        {
            if (isHighlighted)
            {
                await Game.context.DrawImageAsync(
                    starSprite,
                    xPos - (5 * Game.horizontalScale),
                    yPos,
                    starBaseWidth,
                    starBaseHeight
                    );
            }

            await Game.context.DrawImageAsync(
                symbolsSprite,
                0,
                spriteIndex * 250,
                250,
                250,
                xPos,
                yPos,
                spriteBaseWidth,
                spriteBaseHeight);
        }
    }
}
