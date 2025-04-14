using Blazor.Extensions.Canvas.Canvas2D;
namespace SpeakEZSlots.Game
{
    public class TextElement
    {
        private string message {  get; set; }
        private string fontSize { get; set; }
        private string fontFamily { get; set; } = "Edwardian Script ITC";
        private TextAlign textAlign { get; set; } = TextAlign.Start;
        
        private float xPos { get; set; }
        private float yPos { get; set; }

        public TextElement(string message, string fontSize, float xPos, float yPos)
        {
            this.message = message;
            this.fontSize = fontSize;
            this.xPos = xPos;
            this.yPos = yPos;
        }

        public async Task Render()
        {
            await Game.context.SetFontAsync(fontSize + " " + fontFamily);
            await Game.context.SetFillStyleAsync("#ffffff");
            await Game.context.SetTextAlignAsync(TextAlign.Center);
            await Game.context.FillTextAsync(message, xPos, yPos);
        }

        public void UpdateMessage(string newMessage)
        {
            message = newMessage;
        }

        public void UpdateAlignment(string alignment)
        {
            if (alignment == "left")
            {
                textAlign = TextAlign.Start;
            }

            else if (alignment == "center")
            {
                textAlign = TextAlign.Center;
            }
        }

    }
}
