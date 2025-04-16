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

        private ElementReference starParticle { set; get; }
        public Particle particle { set; get; }

        public string symbol {  set; get; }
        private int spriteIndex { set; get; } = 0; //Sprite index corresponds to the y position on sprite sheet
        public int symbolIndex { set; get; } //Symbol index corresponds to the index on the reel (0-71)
        private float xPos {  set; get; }
        public float yPos { set; get; }
        public float yTarget { set; get; }
        public float fallSpeed { set; get; } = 4000.0f;

        public bool isSpinning = false;
        public bool isHighlighted = false;

        public Symbol(ElementReference symbols, ElementReference star, ElementReference starParticle, float xPos, float yPos, int symbolIndex)
        {
            this.symbolsSprite = symbols;
            spriteBaseWidth = (int)(250 * Game.horizontalScale);
            spriteBaseHeight = (int)(250 * Game.verticalScale);

            starSprite = star;
            starBaseWidth = (int)(250 * Game.horizontalScale);
            starBaseHeight = (int)(250 * Game.verticalScale);

            this.starParticle = starParticle;

            this.symbolIndex = symbolIndex;
            
            this.xPos = xPos;
            this.yPos = yPos;
            this.yTarget = yPos;

        }

        public void ChangeSymbol(string symbolName)
        {
            
            switch(symbolName)
            {
                case ("Free Spin"):
                    spriteIndex = 0;
                    break;
                case ("Blank1"):
                    spriteIndex = 1; 
                    break;
                case ("Blank2"):
                    spriteIndex = 2;
                    break;
                case ("Blank3"):
                    spriteIndex = 3;
                    break;
                case ("Uncommon1"):
                    spriteIndex = 4;
                    break;
                case ("Uncommon2"):
                    spriteIndex = 5;
                    break;
                case ("Rare1"):
                    spriteIndex= 6;
                    break;
                case ("Rare2"):
                    spriteIndex = 7;
                    break;
            }

            symbol = symbolName;

        }

        public void SpawnParticle(float xTarget, float yTarget)
        {
            particle = new Particle(starParticle, (xPos + (spriteBaseWidth/2)), (yPos + (spriteBaseHeight/2)), xTarget, yTarget);
        }

        public void Update(float deltaTime)
        {
            //if (particle != null)
            //{
            //    particle.Update(deltaTime);

            //    if  (particle.setForDeletion)
            //    {
            //        particle = null;
            //    }
            //}
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
