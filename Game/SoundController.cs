using Microsoft.JSInterop;

namespace SpeakEZSlots.Game
{
    public class SoundController
    {

        public bool soundMuted = false;

        public async Task PlayStopSound()
        {
            if (soundMuted) return;

            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "Assets/Audio/StopReel.ogg", 0.25, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlaySpinSound()
        {
            if (soundMuted) return;

            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "Assets/Audio/Spin.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlayGainSound()
        {
            if (soundMuted) return;

            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "Assets/Audio/Gain.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlayFreeSpinsSound()
        {
            if (soundMuted) return;

            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "Assets/Audio/FreeSpins.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlayBonusRoundSound()
        {
            if (soundMuted) return;

            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "Assets/Audio/BonusRound.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task PlayMusic()
        {
            if (soundMuted) return;

            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "Assets/Audio/Music.mp3", 0.5, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task StopMusic()
        {
            try
            {
                await Game.JSModule.InvokeVoidAsync("stopMusic");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task StartGame()
        {
            if (soundMuted) return;

            try
            {
                await Game.JSModule.InvokeVoidAsync("playSound", "Assets/Audio/StartGame.ogg", 0.5, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
