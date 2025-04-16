export function getBrowserDimensions() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}

export function playSound (path, volume, loop) {
    try {
        var audio = new Audio(path);
        audio.volume = volume;
        audio.loop = loop;
        audio.play();
    } catch (error) {
        console.error("Error playing sound:", error);
    }
}