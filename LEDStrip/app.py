from flask import Flask, request, jsonify
from time import sleep
from Color import Color

import board
import neopixel

pixels = neopixel.NeoPixel(board.D18, 30)
app = Flask(__name__)


@app.route("/api/color_wipe/", methods=["POST"])
@app.route("/api/color_wipe/<wait_ms>", methods=["POST"])
# Define functions which animate LEDs in various ways.
def color_wipe(wait_ms=50):
    """Wipe color across display a pixel at a time."""
    json_data = request.json
    color = color_from_json(json_data)

    for i in range(len(pixels)):
        pixels[i] = (color.r, color.g, color.b)
        pixels.show()
        sleep(wait_ms/1000.0)

    return jsonify({"status": "OK"})


@app.route("/api/static_color/", methods=["POST"])
def static_color():
    json_data = request.json
    color = color_from_json(json_data)

    # for i in range(len(pixels)):
    #     pixels[i] = (color.r, color.g, color.b)

    return jsonify({"status": "OK"})


@app.route("/api/rainbow/", methods=["POST"])
def rainbow(wait_ms=20, iterations=1):
    """Draw rainbow that fades across all pixels at once."""
    for j in range(256 * iterations):
        for i in range(len(pixels)):
            color = wheel((i + j) & 255)
            pixels[i] = (color.r, color.g, color.b)
        pixels.show()
        sleep(wait_ms / 1000.0)

    return jsonify({"status": "OK"})


@app.route("/api/rainbow_cycle/", methods=["POST"])
def rainbow_cycle(wait_ms=20, iterations=5):
    """Draw rainbow that uniformly distributes itself across all pixels."""
    for j in range(256 * iterations):
        for i in range(len(pixels)):
            color = wheel((int(i * 256 / len(pixels)) + j) & 255)
            pixels[i] = (color.r, color.g, color.b)
        pixels.show()
        sleep(wait_ms/1000.0)

    return jsonify({"status": "OK"})


@app.route("/api/theater_chase/", methods=["POST"])
def theater_chase(wait_ms=50, iterations=10):
    json_data = request.json
    color = color_from_json(json_data)

    """Movie theater light style chaser animation."""
    for j in range(iterations):
        for q in range(3):
            for i in range(0, len(pixels), 3):
                pixels[i+q] = (color.r, color.g, color.b)
            pixels.show()
            sleep(wait_ms/1000.0)
            for i in range(0, len(pixels), 3):
                pixels[i+q] = (0, 0, 0)

    return jsonify({"status": "OK"})


@app.route("/api/theater_chase_rainbow/", methods=["POST"])
def theater_chase_rainbow(wait_ms=50):
    """Rainbow movie theater light style chaser animation."""
    for j in range(256):
        for q in range(3):
            for i in range(0, len(pixels), 3):
                color = wheel((i+j) % 255)
                pixels[i+q] = (color.r, color.g, color.b)
            pixels.show()
            sleep(wait_ms/1000.0)
            for i in range(0, len(pixels), 3):
                pixels[i+q] = (0, 0, 0)

    return jsonify({"status": "OK"})


@app.route("/api/appear_from_back/", methods=["POST"])
def appear_from_back(wait_ms=50):
    json_data = request.json
    color = color_from_json(json_data)

    for i in range(len(pixels)):
        for j in reversed(range(i, len(pixels))):
            # first set all pixels at the begin
            for k in range(i):
                pixels[k] = (color.r, color.g, color.b)
            # set then the pixel at position j
            pixels[j] = (color.r, color.g, color.b)
            pixels.show()
            sleep(wait_ms/1000.0)

    return jsonify({"status": "OK"})


@app.route("/api/hyperspace/")
def hyperspace():
    return "hyperspace"


def wheel(pos):
    """Generate rainbow colors across 0-255 positions."""
    if pos < 85:
        return Color(pos * 3, 255 - pos * 3, 0)
    elif pos < 170:
        pos -= 85
        return Color(255 - pos * 3, 0, pos * 3)
    else:
        pos -= 170
        return Color(0, pos * 3, 255 - pos * 3)


def color_from_json(json):
    if json is None:
        return Color.default

    return Color(json["r"], json["g"], json["b"])


if __name__ == "__main__":
    app.run(host='0.0.0.0', debug=True)
