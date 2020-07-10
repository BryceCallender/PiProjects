from flask import Flask, request

app = Flask(__name__)


@app.route("/api/static_color/", methods=["GET"])
def static_color():
    params = request.args.to_dict()
    print(params)
    return "static color"


@app.route("/api/rainbow/", methods=["GET"])
def rainbow():
    return "rainbow"


@app.route("/api/rainbow_cycle/", methods=["GET"])
def rainbow_cycle():
    return "rainbow cycle"


@app.route("/api/appear_from_back/", methods=["GET"])
def appear_from_back():
    return "appear from the back"


@app.route("/api/hyperspace/", methods=["GET"])
def hyperspace():
    return "hyperspace"


if __name__ == "__main__":
    app.run()
