"""One-shot staging + pck packing + deploy for CalyrexMod.

Steps:
  1. regenerate .tres from .png (tools/gen_tres.py)
  2. stage assets/ into build/pck_root/CalyrexMod with game-path simulation
     (energy icon, card frame material, potion atlas)
  3. pack build/CalyrexMod.pck (tools/make_pck.py)
  4. copy dll + pck to the game's mods folder

Run:  python tools/deploy.py
"""
import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
GAME_MODS = r"D:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CalyrexMod"


def run(cmd: str) -> None:
    print(f">> {cmd}")
    r = subprocess.run(cmd, shell=True, cwd=ROOT)
    if r.returncode != 0:
        sys.exit(f"command failed: {cmd}")


def stage() -> None:
    root = os.path.join(ROOT, "build", "pck_root", "CalyrexMod")
    shutil.rmtree(os.path.join(ROOT, "build", "pck_root"), ignore_errors=True)

    # localization
    for lang in ["eng", "zhs"]:
        dst = os.path.join(root, "localization", lang)
        src = os.path.join(ROOT, "assets", "localization", lang)
        os.makedirs(dst, exist_ok=True)
        for f in os.listdir(src):
            shutil.copy(os.path.join(src, f), os.path.join(dst, f))

    # scenes
    dst = os.path.join(root, "scenes")
    os.makedirs(dst, exist_ok=True)
    for f in os.listdir(os.path.join(ROOT, "assets", "scenes")):
        if f.endswith(".tscn"):
            shutil.copy(os.path.join(ROOT, "assets", "scenes", f), os.path.join(dst, f))

    # icons (includes scene_images/ subfolder -> becomes icons/scene_images/)
    dst = os.path.join(root, "icons")
    os.makedirs(dst, exist_ok=True)
    for item in os.listdir(os.path.join(ROOT, "assets", "icons")):
        src = os.path.join(ROOT, "assets", "icons", item)
        if os.path.isdir(src):
            shutil.copytree(src, os.path.join(dst, item))
        else:
            shutil.copy(src, os.path.join(dst, item))
    # scene_images lives under assets/scene_images -> stage to icons/scene_images
    src = os.path.join(ROOT, "assets", "scene_images")
    if os.path.isdir(src):
        shutil.copytree(src, os.path.join(dst, "scene_images"))

    # simulated game paths (energy icon, card frame, potion atlas) - pck root level
    pck_base = os.path.join(ROOT, "build", "pck_root")
    dst2 = os.path.join(pck_base, "images", "atlases", "ui_atlas.sprites", "card")
    os.makedirs(dst2, exist_ok=True)
    shutil.copy(os.path.join(ROOT, "assets", "icons", "energy_calyrex.tres"), os.path.join(dst2, "energy_calyrex.tres"))

    dst3 = os.path.join(pck_base, "materials", "cards", "frames")
    os.makedirs(dst3, exist_ok=True)
    shutil.copy(os.path.join(ROOT, "assets", "icons", "card_frame_calyrex_mat.tres"),
                os.path.join(dst3, "card_frame_calyrex_mat.tres"))

    dst4 = os.path.join(pck_base, "images", "atlases", "potion_atlas.sprites")
    os.makedirs(dst4, exist_ok=True)
    for pn in ["figy_berry", "gray_carrot", "defense_boost", "galarian_spice", "victors_curry"]:
        shutil.copy(os.path.join(ROOT, "assets", "icons", "potions", f"{pn}.tres"),
                    os.path.join(dst4, f"{pn}.tres"))
    print("staged OK")


def main() -> None:
    run("python tools/gen_tres.py")
    stage()
    run("python tools/make_pck.py")
    os.makedirs(GAME_MODS, exist_ok=True)
    # dll: 最新构建在 bin/Release/net9.0（build/ 只是旧产物目录）
    dll = os.path.join(ROOT, "bin", "Release", "net9.0", "CalyrexMod.dll")
    if not os.path.exists(dll):
        dll = os.path.join(ROOT, "build", "CalyrexMod.dll")
    shutil.copy(dll, GAME_MODS)
    shutil.copy(os.path.join(ROOT, "build", "CalyrexMod.pck"), GAME_MODS)
    shutil.copy(os.path.join(ROOT, "CalyrexMod.json"), GAME_MODS)
    print("DEPLOYED to", GAME_MODS)


if __name__ == "__main__":
    main()
