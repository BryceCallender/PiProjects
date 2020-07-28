from flask import Flask, request, jsonify
from time import sleep
import time
from rpi_ws281x import *

#import board
#import neopixel

#pixels = []
#pixels = neopixel.NeoPixel(board.D18, 144)

app = Flask(__name__)

enabled = False

# LED strip configuration:
LED_COUNT      = 16      # Number of LED pixels.
LED_PIN        = 18      # GPIO pin connected to the pixels (18 uses PWM!).
#LED_PIN        = 10      # GPIO pin connected to the pixels (10 uses SPI /dev/spidev0.0).
LED_FREQ_HZ    = 800000  # LED signal frequency in hertz (usually 800khz)
LED_DMA        = 10      # DMA channel to use for generating signal (try 10)
LED_BRIGHTNESS = 255     # Set to 0 for darkest and 255 for brightest
LED_INVERT     = False   # True to invert the signal (when using NPN transistor level shift)
LED_CHANNEL    = 0       # set to '1' for GPIOs 13, 19, 41, 45 or 53

pixels = Adafruit_NeoPixel(LED_COUNT, LED_PIN, LED_FREQ_HZ, LED_DMA, LED_INVERT, LED_BRIGHTNESS, LED_CHANNEL)
pixels.begin()


@app.route("/api/led_status", methods=["GET"])
def led_status():
    return jsonify({"enabled": enabled})


@app.route("/api/change_status", methods=["POST"])
def change_status():
    json_data = request.json
    global enabled
    enabled = json_data["enabled"]

    if not enabled:
        turn_off()

    return jsonify({"status": "OK"})


@app.route("/api/color_wipe", methods=["POST"])
@app.route("/api/color_wipe/<wait_ms>", methods=["POST"])
# Define functions which animate LEDs in various ways.
def color_wipe(wait_ms=50):
    """Wipe color across display a pixel at a time."""
    json_data = request.json
    color = color_from_json(json_data)

    for i in range(len(pixels)):
        pixels.setPixelColor(i, color)
        pixels.show()
        sleep(wait_ms/1000.0)

    return jsonify({"status": "OK"})


@app.route("/api/static_color", methods=["POST"])
def static_color():
    json_data = request.json
    color = color_from_json(json_data)

    for i in range(len(pixels)):
        pixels.setPixelColor(i, color)
        pixels.show()

    return jsonify({"status": "OK"})


@app.route("/api/rainbow", methods=["POST"])
def rainbow(wait_ms=20, iterations=1):
    """Draw rainbow that fades across all pixels at once."""
    """Draw rainbow that fades across all pixels at once."""
    for j in range(256 * iterations):
        for i in range(pixels.numPixels()):
            pixels.setPixelColor(i, wheel((i + j) & 255))
        pixels.show()
        sleep(wait_ms / 1000.0)

    return jsonify({"status": "OK"})


@app.route("/api/rainbow_cycle", methods=["POST"])
def rainbow_cycle(wait_ms=20, iterations=5):
    """Draw rainbow that uniformly distributes itself across all pixels."""
    """Draw rainbow that uniformly distributes itself across all pixels."""
    for j in range(256 * iterations):
        for i in range(pixels.numPixels()):
            pixels.setPixelColor(i, wheel((int(i * 256 / pixels.numPixels()) + j) & 255))
        pixels.show()
        sleep(wait_ms / 1000.0)

    return jsonify({"status": "OK"})


@app.route("/api/theater_chase", methods=["POST"])
def theater_chase(wait_ms=50, iterations=10):
    json_data = request.json
    color = color_from_json(json_data)

    """Movie theater light style chaser animation."""
    """Movie theater light style chaser animation."""
    for j in range(iterations):
        for q in range(3):
            for i in range(0, pixels.numPixels(), 3):
                pixels.setPixelColor(i + q, color)
            pixels.show()
            time.sleep(wait_ms / 1000.0)
            for i in range(0, pixels.numPixels(), 3):
                pixels.setPixelColor(i + q, 0)

    return jsonify({"status": "OK"})


@app.route("/api/theater_chase_rainbow", methods=["POST"])
def theater_chase_rainbow(wait_ms=50):
    """Rainbow movie theater light style chaser animation."""
    """Rainbow movie theater light style chaser animation."""
    for j in range(256):
        for q in range(3):
            for i in range(0, pixels.numPixels(), 3):
                pixels.setPixelColor(i + q, wheel((i + j) % 255))
            pixels.show()
            time.sleep(wait_ms / 1000.0)
            for i in range(0, pixels.numPixels(), 3):
                pixels.setPixelColor(i + q, 0)

    return jsonify({"status": "OK"})


@app.route("/api/appear_from_back", methods=["POST"])
def appear_from_back(wait_ms=50):
    json_data = request.json
    color = color_from_json(json_data)

    for i in range(len(pixels)):
        for j in reversed(range(i, len(pixels))):
            # first set all pixels at the begin
            for k in range(i):
                pixels.setPixelColor(k, color)
            # set then the pixel at position j
            pixels.setPixelColor(j, color)
            pixels.show()
            sleep(wait_ms/1000.0)

    return jsonify({"status": "OK"})


@app.route("/api/hyperspace", methods=["POST"])
def hyperspace():
    for i in range(len(pixels)):
        for j in range(i + 5):
            if i + j < len(pixels):
                pixels[i+j] = (255, 255, 255)
                pixels.show()
                sleep(0.01)
        pixels[i] = (0, 0, 0)

    return jsonify({"status": "OK"})


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
        return Color(0, 0, 255)

    return Color(json["r"], json["g"], json["b"])


def turn_off():
    for i in range(len(pixels)):
        pixels[i] = (0, 0, 0)
        pixels.show()


if __name__ == "__main__":
    app.run(host='0.0.0.0', debug=True)
