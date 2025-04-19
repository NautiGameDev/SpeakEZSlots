using Microsoft.AspNetCore.Components;

namespace SpeakEZSlots.Game
{
    public class Button
    {
        private ElementReference sprite;
        public int spriteBaseWidth { get; set; }
        public int spriteBaseHeight { get; set; }

        public float xPos { get; set; }
        public float yPos { get; set; }

        public Button(ElementReference sprite, float xpos, float ypos) 
        {
            this.sprite = sprite;
            spriteBaseWidth = (int)(90 * Game.horizontalScale);
            spriteBaseHeight = (int)(90 * Game.verticalScale);

            this.xPos = (float)(xpos * Game.horizontalScale);
            this.yPos = (float)(ypos * Game.verticalScale);
        }

        public async Task Render()
        {
            await Game.context.DrawImageAsync(
                sprite,
                xPos,
                yPos,
                spriteBaseWidth,
                spriteBaseHeight);
        }

    }
}
