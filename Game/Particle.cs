using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace SpeakEZSlots.Game
{
    public class Particle
    {
        ElementReference starParticle {  get; set; }
        private int spriteBaseWidth { set; get; }
        private int spriteBaseHeight { set; get; }

        float xPos { get; set; }
        float yPos { get; set; }
        float moveSpeed = 2000f;

        float targetXPos { get; set; }
        float targetYPos { get; set; }

        public bool setForDeletion = false;

        public Particle(ElementReference starParticle, float xPos, float yPos, float xTarget, float yTarget)
        {
            this.starParticle = starParticle;
            spriteBaseWidth = (int)(155 * 0.5f * Game.horizontalScale);
            spriteBaseHeight = (int)(163 * 0.5f * Game.verticalScale);
            targetXPos = xTarget;
            targetYPos = yTarget;


            this.xPos = xPos;
            this.yPos = yPos;
        }

        public void Update(float deltaTime)
        {
            MoveParticle(deltaTime);
        }

        private void MoveParticle(float deltaTime)
        {
            float dx = targetXPos - xPos;
            float dy = targetYPos - yPos;

            double distance = GetDistance(dx, dy);

            double averageScale = (Game.horizontalScale +  Game.verticalScale)/2;

            if (distance > (moveSpeed * deltaTime * averageScale))
            {
                double angleRadians = Math.Atan2(dy, dx);

                float xVel = (float)(Math.Cos(angleRadians) * moveSpeed * deltaTime * Game.horizontalScale);
                float yVel = (float)(Math.Sin(angleRadians) * moveSpeed * deltaTime * Game.verticalScale);

                xPos += xVel;
                yPos += yVel;
            }
            else
            {
                setForDeletion = true;
            }
        }

        private double GetDistance(float dx, float dy)
        {
            return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        }

        public async Task Render()
        {
            await Game.context.DrawImageAsync(starParticle, xPos, yPos, spriteBaseWidth, spriteBaseHeight);
        }

    }
}
