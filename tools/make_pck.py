"""Godot 4.5 pck packer for CalyrexMod.

Reads files under build/pck_root/ and packs them into build/CalyrexMod.pck
with res://-style virtual paths matching the game's pck format (format 3).
"""
import hashlib
import os
import struct
import sys

HEADER_SIZE = 112
MAGIC = b"GDPC"


def pad4(b: bytes) -> bytes:
    rem = len(b) % 4
    return b + b"\x00" * (4 - rem) if rem else b


def main() -> None:
    root = os.path.join(os.path.dirname(__file__), "..", "build", "pck_root")
    out_pck = os.path.join(os.path.dirname(__file__), "..", "build", "CalyrexMod.pck")
    if not os.path.isdir(root):
        print(f"ERROR: {root} does not exist")
        sys.exit(1)

    files = []
    for dirpath, _dirnames, filenames in os.walk(root):
        for name in filenames:
            full = os.path.join(dirpath, name)
            rel = os.path.relpath(full, root).replace("\\", "/")
            files.append((rel, full))
    files.sort(key=lambda x: x[0])

    if not files:
        print("ERROR: no files found under", root)
        sys.exit(1)

    print(f"Packing {len(files)} files...")

    # Data section: file contents, each 4-byte aligned.
    # NOTE: Godot 4.5 stores per-file offsets RELATIVE to files_base.
    data_offset = HEADER_SIZE
    data = bytearray()
    entries = []  # (path_str, rel_offset, size, md5)
    for rel, full in files:
        payload = open(full, "rb").read()
        rel_offset = len(data)
        entries.append(("res://" + rel, rel_offset, len(payload), hashlib.md5(payload).digest()))
        data += payload
        data += b"\x00" * ((4 - len(payload) % 4) % 4)

    pack_size = data_offset + len(data)

    # Header: mirror the game's format (Godot 4.5.1, format 3, flags 2).
    header = bytearray(HEADER_SIZE)
    header[0:4] = MAGIC
    struct.pack_into("<IIIII", header, 4, 3, 4, 5, 1, 2)
    struct.pack_into("<QQ", header, 24, data_offset, pack_size)
    # 40..111 stay zero (index_size=0 => index stored at pack_size).

    # Index section.
    index = bytearray()
    index += struct.pack("<I", len(entries))
    for path_str, offset, size, md5 in entries:
        path_bytes = path_str.encode("utf-8")
        padded = pad4(path_bytes)
        index += struct.pack("<I", len(padded))
        index += padded
        index += struct.pack("<QQ", offset, size)
        index += md5
        index += b"\x00" * 4  # trailing pad

    with open(out_pck, "wb") as f:
        f.write(header)
        f.write(data)
        f.write(index)

    print(f"Wrote {out_pck} ({pack_size + len(index)} bytes, pack_size={pack_size})")


if __name__ == "__main__":
    main()
