namespace SpeakEZSlots.Game.MachineStates
{
    /*
        This class handles the spin timers for each reel and instructs reels to stop spinning when timer reaches 0
            When all reels have stopped spinning, the machine is instructed to change the state to Paylines
     */

    public class BonusSpinState : State
    {
        public Machine machine { get; set; }
        private UIController uiController { get; set; }
        private SoundController soundController { get; set; }
        private Queue<Reel> bonusReels { get; set; }

        private float spinTimer = 0.75f;
        private float timeToSpin = 0.75f;

        public BonusSpinState(Machine machine, Queue<Reel> bonusReels, UIController uiController, SoundController soundController)
        {
            this.machine = machine;
            this.uiController = uiController;
            this.soundController = soundController;
            this.bonusReels = bonusReels;

            uiController.UpdateMessageBar("Spinning for bonuses! Good luck!");

            foreach (Reel reel in bonusReels)
            {
                reel.StartSpinning();
            }
        }

        public override void Update(float deltaTime)
        {
            uiController.TransferWinnings(deltaTime);

            foreach (Reel reel in bonusReels)
            {
                reel.Update(deltaTime);
            }

            HandleSpinning(deltaTime);
        }

        private void HandleSpinning(float deltaTime)
        {
            spinTimer -= deltaTime;

            if (spinTimer < 0)
            {
                Reel firstReel = bonusReels.Peek();

                if (firstReel.isSpinning)
                {
                    soundController.PlayStopSound();
                    firstReel.StopSpinning();
                    bonusReels.Dequeue();
                    bonusReels.Enqueue(firstReel);
                    spinTimer = timeToSpin;
                }
                else
                {
                    machine.ChangeMachineState("Bonus Payout");
                }
            }
        }


    }
}
