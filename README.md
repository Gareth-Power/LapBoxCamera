# LapBoxCamera

LapBoxCamera is a lightweight browser-based camera viewer for webcam-based laparoscopic simulator boxes. It is aimed at maximizing flexibility for different webcams and capture setups while keeping interaction simple. It is designed for quickly switching cameras, flipping the image, running fullscreen, and biasing capture toward the highest available resolution instead of accidentally favoring unusual high-refresh modes.

The page also includes a built-in record function so you can save footage directly from the active camera stream without extra software. The overall setup is kept minimal to help reduce latency: there is no framework, no bundling step, and the app talks directly to the browser's media APIs.

## Features

- Camera picker for any detected video input device
- Fullscreen mode with persistent overlay controls
- Flip X and Flip Y controls
- HD limit toggle for capping resolution when needed
- Built-in recording to WebM or MP4 when supported by the browser
- Simple single-file setup suitable for low-overhead viewing

## Local Use

Open [index.html](./index.html) in a modern browser and allow camera access when prompted. For best results, use a Chromium-based browser with webcam permissions enabled.

## Deployment

Because the app is a static page, you can either host it yourself as a web app or use the GitHub Pages deployment for this repository.

## Notes

- Camera access requires browser permission.
- Some browsers and webcams may still negotiate different modes depending on driver support.
- Recording format availability depends on what `MediaRecorder` supports in the current browser.

## License

This project is licensed under GPL-3.0-only. See [LICENSE](./LICENSE).