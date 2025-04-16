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
        SoundController soundController { get; set; }

        private float inputTimer = 0.5f;
        private float inputWaitTime = 0.5f;

        public IdleState(Machine machine, UIController uiController, SoundController soundController)
        {
            this.machine = machine;
            this.uiController = uiController;
            this.soundController = soundController;

            if (machine.totalSpins >= machine.spinsForBonus)
            {
                uiController.UpdateMessageBar($"Bonus round! You have {machine.bonusSpins} spins remaining.");
            }
            else if (machine.freeSpins > 0)
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
                soundController.PlaySpinSound();

                if (machine.totalSpins >= machine.spinsForBonus)
                {
                    if (machine.bonusSpins > 0)
                    {
                        machine.bonusSpins--;
                        machine.ChangeMachineState("Bonus Spinning");

                        if (machine.bonusSpins == 0)
                        {
                            machine.CalculateBonusState();
                        }
                    }
                }
                else if (machine.freeSpins > 0)
                {
                    machine.freeSpins--;
                    uiController.UpdateFreeSpins(machine.freeSpins);
                    machine.ChangeMachineState("Spinning");
                }
                else
                {
                    machine.playerCredits -= machine.bet;
                    machine.totalSpins += 1;
                    uiController.UpdatePlayerCredits(machine.playerCredits);
                    machine.ChangeMachineState("Spinning");
                }
            }

            if (InputController.playerInput["ArrowUp"] == true && inputTimer <= 0)
            {
                inputTimer = inputWaitTime;
                if (machine.bet < machine.maxBet)
                {
                    machine.bet += machine.betStep;
                    uiController.UpdatePlayerBet(machine.bet);
                }
            }

            if (InputController.playerInput["ArrowDown"] == true && inputTimer <= 0)
            {
                inputTimer = inputWaitTime;
                if (machine.bet > machine.minBet)
                {
                    machine.bet -= machine.betStep;
                    uiController.UpdatePlayerBet(machine.bet);
                }
            }

            if (InputController.playerInput["ArrowLeft"] == true && inputTimer <= 0)
            {
                inputTimer = inputWaitTime;
                machine.bet = machine.minBet;
                uiController.UpdatePlayerBet(machine.bet);
            }

            if (InputController.playerInput["ArrowRight"] == true && inputTimer <= 0)
            {
                inputTimer = inputWaitTime;
                machine.bet = machine.maxBet;
                uiController.UpdatePlayerBet(machine.bet);
            }

        }
    }
}
