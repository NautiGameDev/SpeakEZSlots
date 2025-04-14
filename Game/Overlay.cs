using Microsoft.AspNetCore.Components;

namespace SpeakEZSlots.Game
{
    public class Overlay
    {
        private ElementReference overlaySprite;
        private int spriteBaseWidth {  get; set; }
        private int spriteBaseHeight { get; set; }


        public Overlay(ElementReference sprite)
        {
            overlaySprite = sprite;
            spriteBaseWidth = (int)(Game.BaseGameParams["Width"] * Game.horizontalScale);
            spriteBaseHeight = (int)(Game.BaseGameParams["Height"] * Game.verticalScale);
        }

        public async Task Render()
        {
            await Game.context.DrawImageAsync(
                overlaySprite,
                0,
                0,
                spriteBaseWidth,
                spriteBaseHeight);
        }
    }
}
