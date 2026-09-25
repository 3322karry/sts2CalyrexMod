"""One-command release for CalyrexMod.

Steps:
  1. preconditions (git clean, version format, game closed)
  2. bump version (ModInfo.cs + CalyrexMod.json)
  3. prepend CHANGELOG.md entry
  4. create docs/releases/<ver>.md page
  5. sync Wiki (版本历史 + 版本列表: mod table row + wiki record row)
  6. dotnet build -c Release
  7. deploy (tools/deploy.py: pck pack + copy to game mods)
  8. git commit + push (main repo)
  9. git commit + push (wiki repo)
 10. steam: sync ModUploader content + changeNote + upload

Usage:
  python tools/release.py v1.2.41.107.1 --note "改动一" --note "改动二"
  python tools/release.py v1.2.41.107.1 --notes-file notes.txt --skip-steam
  python tools/release.py v1.2.41.107.1 --note "..." --dry-run

Switches:
  --skip-steam    skip ModUploader upload
  --skip-wiki     skip wiki sync + push
  --skip-deploy   skip pck pack / copy to game
  --skip-push     skip git push (commit only)
  --dry-run       print planned changes, touch nothing
"""
import argparse
import datetime
import json
import os
import re
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
WIKI_DIR = r"D:\vibeprograms\sts2CalyrexMod-wiki"
UPLOADER_DIR = r"D:\db\ModUploader-win-x64"
UPLOADER_WORKSPACE = os.path.join(UPLOADER_DIR, "sts2CalyrexMod")
GAME_MODS = r"D:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CalyrexMod"

VER_RE = re.compile(r"^v\d+\.\d+\.\d+\.\d+\.\d+$")
WIKI_ROW_RE = re.compile(r"`v(\d+\.\d+)\.(\d{4})\.(\d{4})\.(\d+)\.([\d.]+)`")

DRY = False


def log(msg: str) -> None:
    print(msg)


def run(cmd: str, cwd: str = ROOT, check: bool = True) -> subprocess.CompletedProcess:
    if DRY:
        log(f"   [dry-run] {cmd}  (cwd={cwd})")
        return subprocess.CompletedProcess(cmd, 0, "", "")
    r = subprocess.run(cmd, shell=True, cwd=cwd, capture_output=True, text=True, encoding="utf-8", errors="replace")
    if check and r.returncode != 0:
        log(f"   FAILED: {cmd}")
        log(f"   stdout: {(r.stdout or '')[-800:]}")
        log(f"   stderr: {(r.stderr or '')[-800:]}")
        sys.exit(1)
    return r


def step(n: int, total: int, title: str) -> None:
    log(f"[{n}/{total}] {title}")


def write_file(path: str, content: str) -> None:
    if DRY:
        log(f"   [dry-run] write {path}")
        return
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write(content)


def read_file(path: str) -> str:
    with open(path, encoding="utf-8") as f:
        return f.read()


# ---------- steps ----------

def check_preconditions(ver: str) -> None:
    if not VER_RE.match(ver):
        sys.exit(f"版本号格式不对: {ver}（示例 v1.2.41.107.1）")
    st = run("git status --porcelain", check=False).stdout.strip()
    if st:
        n = len(st.splitlines())
        log(f"   工作区有 {n} 个未提交改动，将随本次发版一并提交")
    for p in ["mod_src/ModInfo.cs", "CalyrexMod.json", "CHANGELOG.md"]:
        if not os.path.exists(os.path.join(ROOT, p)):
            sys.exit(f"缺少文件: {p}")
    # 关闭游戏（避免 pck 被占用）
    run("taskkill /F /IM SlayTheSpire2.exe", check=False)


def bump_version(ver: str) -> None:
    p = os.path.join(ROOT, "mod_src", "ModInfo.cs")
    s = read_file(p)
    s2 = re.sub(r'(public const string Version = ")v[\d.]+(";)', rf"\g<1>{ver}\g<2>", s)
    if s2 == s:
        sys.exit("ModInfo.cs 版本号替换失败")
    write_file(p, s2)

    p = os.path.join(ROOT, "CalyrexMod.json")
    d = json.loads(read_file(p))
    d["version"] = ver
    write_file(p, json.dumps(d, ensure_ascii=False, indent=2) + "\n")
    log(f"   版本号 -> {ver}")


def update_changelog(ver: str, date: str, notes: list[str]) -> None:
    p = os.path.join(ROOT, "CHANGELOG.md")
    s = read_file(p)
    body = "\n".join(f"- {n}" for n in notes)
    block = f"## {ver} ({date})\n\n### Changes / 改动\n{body}\n\n"
    anchor = "# Changelog / 更新日志\n\n"
    if anchor not in s:
        sys.exit("CHANGELOG.md 锚点未找到")
    write_file(p, s.replace(anchor, anchor + block, 1))
    log(f"   CHANGELOG 已插入 {len(notes)} 条")


def create_release_page(ver: str, notes: list[str]) -> None:
    body = "\n".join(f"- {n}" for n in notes)
    content = f"# {ver} 发布说明\n\n### Changes / 改动\n{body}\n"
    write_file(os.path.join(ROOT, "docs", "releases", f"{ver}.md"), content)
    log(f"   docs/releases/{ver}.md")


def update_wiki(ver: str, date: str, notes: list[str], wiki_summary: str) -> None:
    # 版本历史
    p = os.path.join(WIKI_DIR, "版本历史.md")
    s = read_file(p)
    body = "\n".join(f"- {n}" for n in notes)
    block = f"## {ver}\n{body}\n\n"
    anchor = "# 版本历史\n\n"
    if anchor not in s:
        sys.exit("版本历史.md 锚点未找到")
    write_file(p, s.replace(anchor, anchor + block, 1))

    # 版本列表
    p = os.path.join(WIKI_DIR, "版本列表.md")
    s = read_file(p)
    # 1) 模组版本表
    tbl_anchor = "| 模组版本 | 说明 |\n|---|---|\n"
    if tbl_anchor not in s:
        sys.exit("版本列表.md 模组版本表未找到")
    s = s.replace(tbl_anchor, tbl_anchor + f"| {ver} | {wiki_summary} |\n", 1)
    # 2) wiki 记录行（序号全局递增）
    nums = [int(m.group(4)) for m in WIKI_ROW_RE.finditer(s)]
    nxt = (max(nums) + 1) if nums else 1
    yy, mmdd = date[:4], date[5:7] + date[8:10]
    row = f"| `v0.3.{yy}.{mmdd}.{nxt:02d}.{ver[1:]}` | {ver} | {wiki_summary}；版本历史/版本列表同步 |\n"
    last = None
    for m in WIKI_ROW_RE.finditer(s):
        last = m
    if last is None:
        sys.exit("版本列表.md wiki 记录表未找到")
    line_end = s.find("\n", last.end()) + 1
    s = s[:line_end] + row + s[line_end:]
    write_file(p, s)
    log(f"   版本历史 + 版本列表（wiki 序号 {nxt:02d}）")


def build() -> None:
    r = run("dotnet build -c Release")
    tail = (r.stdout or "").strip().splitlines()[-2:]
    for line in tail:
        log(f"   {line.strip()}")


def deploy() -> None:
    r = run("python tools/deploy.py")
    for line in (r.stdout or "").strip().splitlines()[-2:]:
        log(f"   {line.strip()}")


def git_commit_push(ver: str, notes: list[str], cwd: str = ROOT, push: bool = True) -> None:
    msg = f"{ver}: " + "；".join(notes[:3])
    run(f'git add -A && git commit -m "{msg}" -q', cwd=cwd)
    if push:
        run("git push", cwd=cwd)
    log(f"   {'commit+push' if push else 'commit'}: {msg}")


def steam_upload(ver: str, notes: list[str]) -> None:
    if not os.path.isdir(UPLOADER_WORKSPACE):
        log("   ModUploader workspace 不存在，跳过")
        return
    # 同步 content
    content = os.path.join(UPLOADER_WORKSPACE, "content")
    for f in ["CalyrexMod.dll", "CalyrexMod.pck", "CalyrexMod.json"]:
        src = os.path.join(ROOT, "build", f)
        if not os.path.exists(src):
            src = os.path.join(ROOT, "bin", "Release", "net9.0", f)
        if DRY:
            log(f"   [dry-run] copy {src} -> {content}")
        else:
            import shutil
            shutil.copy(src, content)
    # changeNote
    p = os.path.join(UPLOADER_WORKSPACE, "workshop.json")
    d = json.loads(read_file(p))
    d["changeNote"] = f"{ver}: " + "；".join(notes[:3])
    write_file(p, json.dumps(d, ensure_ascii=False, indent=2))
    # 上传（长任务，容忍失败——元数据可能成功而内容 Invalid）
    log("   ModUploader 上传中（可能需要几分钟）...")
    r = run(
        f'"{os.path.join(UPLOADER_DIR, "ModUploader.exe")}" upload -w "{UPLOADER_WORKSPACE}"',
        cwd=UPLOADER_DIR,
        check=False,
    )
    out = (r.stdout or "") + (r.stderr or "")
    if "Successfully uploaded" in out:
        log("   上传完成（若含 k_EItemUpdateStatusInvalid 表示内容提交被 Steam 拒绝，稍后重试）")
    else:
        log("   上传未确认成功，请检查 mod-uploader.log")


def main() -> None:
    global DRY
    ap = argparse.ArgumentParser(description="CalyrexMod one-command release")
    ap.add_argument("version", help="新版本号，如 v1.2.41.107.1")
    ap.add_argument("--note", action="append", default=[], help="改动条目（可多次）")
    ap.add_argument("--notes-file", help="从文件读取改动条目（每行一条）")
    ap.add_argument("--wiki-summary", help="版本列表里的简短说明（默认第一条 note）")
    ap.add_argument("--skip-steam", action="store_true")
    ap.add_argument("--skip-wiki", action="store_true")
    ap.add_argument("--skip-deploy", action="store_true")
    ap.add_argument("--skip-push", action="store_true")
    ap.add_argument("--dry-run", action="store_true")
    ap.add_argument("--force-dirty", action="store_true")
    args = ap.parse_args()

    DRY = args.dry_run
    ver = args.version
    notes = list(args.note)
    if args.notes_file:
        notes += [ln.strip() for ln in read_file(args.notes_file).splitlines() if ln.strip()]
    if not notes:
        sys.exit("至少提供一条改动：--note \"...\" 或 --notes-file")
    summary = args.wiki_summary or notes[0][:40]
    date = datetime.date.today().isoformat()

    total = 6 + (0 if args.skip_wiki else 2) + (0 if args.skip_deploy else 1) + (0 if args.skip_steam else 1)
    log(f"== CalyrexMod release {ver} ({date}) =={'  [DRY-RUN]' if DRY else ''}")

    step(1, total, "前置检查")
    check_preconditions(ver)
    step(2, total, "更新版本号")
    bump_version(ver)
    step(3, total, "更新 CHANGELOG")
    update_changelog(ver, date, notes)
    step(4, total, "创建发布页")
    create_release_page(ver, notes)
    n = 4
    if not args.skip_wiki:
        n += 1
        step(n, total, "同步 Wiki")
        update_wiki(ver, date, notes, summary)
    n += 1
    step(n, total, "构建")
    build()
    if not args.skip_deploy:
        n += 1
        step(n, total, "部署到游戏")
        deploy()
    n += 1
    step(n, total, "GitHub 推送（主仓库）")
    git_commit_push(ver, notes, cwd=ROOT, push=not args.skip_push)
    if not args.skip_wiki:
        n += 1
        step(n, total, "GitHub 推送（Wiki）")
        git_commit_push(ver, notes, cwd=WIKI_DIR, push=not args.skip_push)
    if not args.skip_steam:
        n += 1
        step(n, total, "Steam 上传")
        steam_upload(ver, notes)

    log(f"== 完成: {ver} ==")


if __name__ == "__main__":
    main()
