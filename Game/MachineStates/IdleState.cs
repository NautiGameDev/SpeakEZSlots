namespace SpeakEZSlots.Game.MachineStates
{
    /*
     Idle state is responsible for handling player input to spin reels and change bet size.
     Idle state is responsible for deducting the bet cost from player's credits and checking for free spins
        as well as changing the state of the machine to spinning.
     */

    public class IdleState : State
    {
        Machine machine {  get; set; }
        UIController uiController { get; set; }

        private float inputTimer = 0.5f;
        private float inputWaitTime = 0.5f;

        public IdleState(Machine machine, UIController uiController)
        {
            this.machine = machine;
            this.uiController = uiController;

            if (machine.freeSpins > 0)
            {
                uiController.UpdateMessageBar($"You have {machine.freeSpins} free spins remaining.");
            }
            else
            {
                uiController.UpdateMessageBar("Press space bar to spin, or use the arrow keys to change your bet.");
            }
        }

        public override void Update(float deltaTime)
        {
            if (inputTimer > 0)
            {
                inputTimer -= deltaTime;
            }

            HandlePlayerInput();
            uiController.TransferWinnings(deltaTime);
        }

        private void HandlePlayerInput()
        {
            if (InputController.playerInput[" "] == true && inputTimer <= 0)
            {
                if (machine.freeSpins > 0)
                {
                    machine.freeSpins--;
                }
                else
                {
                    machine.playerCredits -= machine.bet;
                    uiController.UpdatePlayerCredits(machine.playerCredits);
                }

                machine.ChangeMachineState("Spinning");
            }
        }
    }
}
