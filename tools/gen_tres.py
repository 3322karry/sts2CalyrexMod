"""Generate .tres (embedded RGBA) from .png for all icon/asset PNGs.

Godot cannot load bare PNGs without .import, so every PNG used by the mod
must be embedded as ImageTexture (.tres) with a PackedByteArray. This script
regenerates all of them from their source PNGs.

Run:  python tools/gen_tres.py
"""
import os
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..", "assets")

TRES_TEMPLATE = '''[gd_resource type="ImageTexture" load_steps=2 format=3]

[sub_resource type="Image" id="Image_1"]
data = {{
"data": PackedByteArray({ba}),
"format": "RGBA8",
"width": {w},
"height": {h},
"mipmaps": false
}}

[resource]
image = SubResource("Image_1")
'''


def write_tres(img: Image.Image, path: str) -> None:
    w, h = img.size
    raw = img.tobytes()
    ba = ", ".join(str(b) for b in raw)
    with open(path, "w", encoding="utf-8") as out:
        out.write(TRES_TEMPLATE.format(ba=ba, w=w, h=h))


def main() -> None:
    generated = 0
    for dirpath, _dirnames, filenames in os.walk(ROOT):
        for name in filenames:
            ext = os.path.splitext(name)[1].lower()
            if ext not in (".png", ".jpg", ".jpeg"):
                continue
            base = os.path.splitext(name)[0]
            tres_path = os.path.join(dirpath, base + ".tres")
            src_path = os.path.join(dirpath, name)
            if os.path.exists(tres_path) and os.path.getmtime(tres_path) >= os.path.getmtime(src_path):
                continue
            img = Image.open(src_path).convert("RGBA")
            write_tres(img, tres_path)
            generated += 1
            print(f"generated {os.path.relpath(tres_path, ROOT)} ({img.size[0]}x{img.size[1]})")
    print(f"done: {generated} .tres generated")


if __name__ == "__main__":
    main()
