using Microsoft.JSInterop;

namespace SpeakEZSlots.Game
{
    public class SoundController
    {

        public async Task PlayStopSound()
        {
            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "/Assets/Audio/StopReel.ogg", 0.25, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlaySpinSound()
        {
            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "/Assets/Audio/Spin.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlayGainSound()
        {
            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "/Assets/Audio/Gain.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlayFreeSpinsSound()
        {
            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "/Assets/Audio/FreeSpins.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlayBonusRoundSound()
        {
            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "/Assets/Audio/BonusRound.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlayMusic()
        {

            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "/Assets/Audio/Music.mp3", 0.5, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task StartGame()
        {
            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "/Assets/Audio/StartGame.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
