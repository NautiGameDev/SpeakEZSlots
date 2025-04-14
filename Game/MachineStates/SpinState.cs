namespace SpeakEZSlots.Game.MachineStates
{
    /*
        This class handles the spin timers for each reel and instructs reels to stop spinning when timer reaches 0
            When all reels have stopped spinning, the machine is instructed to change the state to Paylines
     */

    public class SpinState : State
    {
        public Machine machine {  get; set; }
        private UIController uiController { get; set; }
        private Queue<Reel> reels {  get; set; }

        private float spinTimer = 0.75f;
        private float timeToSpin = 0.75f;

        public SpinState(Machine machine, Queue<Reel> reels, UIController uiController)
        {
            this.machine = machine;
            this.uiController = uiController;
            this.reels = reels;

            uiController.UpdateMessageBar("Spinning! Good luck!");

            foreach (Reel reel in reels)
            {
                reel.StartSpinning();
            }
        }

        public override void Update(float deltaTime)
        {
            uiController.TransferWinnings(deltaTime);

            foreach (Reel reel in reels)
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
                Reel firstReel = reels.Peek();

                if (firstReel.isSpinning)
                {
                    firstReel.StopSpinning();
                    reels.Dequeue();
                    reels.Enqueue(firstReel);
                    spinTimer = timeToSpin;
                }
                else
                {
                    machine.ChangeMachineState("Payout");
                }
            }
        }


    }
}
