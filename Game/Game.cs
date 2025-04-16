using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

/* Game Class is responsible for connecting HTML elements to C# program as well as running and maintaining gameplay loop
 * Current version supports single slot machine. This class may be expanded in the future to support multiple machines.
 */

namespace SpeakEZSlots.Game
{
    public class Game
    {
        public static Machine currentMachine;

        //Webpage Reference
        public static BECanvasComponent canvas {  get; private set; }
        public static Canvas2DContext context { get; private set; }
        public static IJSObjectReference JSModule { get; private set; }

        //Game Calculations
        public static double horizontalScale { get; private set; }
        public static double verticalScale { get; private set; }
        public static Dictionary<string, double> BaseGameParams = new Dictionary<string, double>()
        {
            {"Width", 2376 },
            {"Height", 1344 }
        };

        public static void SetComponents(BECanvasComponent canvas, Canvas2DContext context, IJSObjectReference JSModule)
        {
            Game.canvas = canvas;
            Game.context = context;
            Game.JSModule = JSModule;

            horizontalScale = (canvas.Width / BaseGameParams["Width"]);
            verticalScale = (canvas.Height / BaseGameParams["Height"]);
        }

        public static void SetMachine(ElementReference mainMenuScreen, ElementReference gameOverScreen, ElementReference background, ElementReference backgroundDark, ElementReference backgroundBonus, ElementReference backgroundBonusDark, ElementReference symbols, ElementReference bonusSymbols, ElementReference star, ElementReference starParticle, ElementReference bonusRoundAnnouncement)
        {
            currentMachine = new Machine(mainMenuScreen, gameOverScreen, background, backgroundDark, backgroundBonus, backgroundBonusDark, symbols, bonusSymbols, star, starParticle, bonusRoundAnnouncement);
        }

        public static void Update(float deltaTime)
        {
            if (currentMachine != null)
            {
                currentMachine.Update(deltaTime);
            }
        }
    }
}
