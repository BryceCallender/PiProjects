class Color:
    def __init__(self, r=0, g=0, b=0):
        self.r = r
        self.g = g
        self.b = b

        self.default = (0, 0, 255)

    def default(self):
        return self.default

    def change_default_color(self, r, g, b):
        self.default = (r, g, b)