export function getBrowserDimensions() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}

let backgroundMusic = null;

export function playSound (path, volume, loop) {
    try {
        var audio = new Audio(path);
        audio.volume = volume;
        audio.loop = loop;
        audio.play();
        if (loop) {
            backgroundMusic = audio;
        }

    } catch (error) {
        console.error("Error playing sound:", error);
    }
}

export function stopMusic() {
    if (backgroundMusic) {
        backgroundMusic.pause();
        backgroundMusic = null;
    }
}