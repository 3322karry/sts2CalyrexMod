// 像素画绘制核心：在 LibreSprite 的 JS 脚本环境里运行。
// 坐标是低分辨率网格（默认 32x32），画完由外面放大到 128x128。

var CANVAS_W = 32;
var CANVAS_H = 32;
var _img = null;
var _buf = [];   // 每格 [r,g,b,a] 或 null

function pInit(w, h) {
  CANVAS_W = w;
  CANVAS_H = h;
  _img = app.activeSprite.layer(0).cel(0).image;
  _buf = [];
  for (var y = 0; y < h; y++) {
    var row = [];
    for (var x = 0; x < w; x++) row.push(null);
    _buf.push(row);
  }
}

function pSet(x, y, r, g, b, a) {
  if (a === undefined) a = 255;
  x = Math.round(x); y = Math.round(y);
  if (x < 0 || y < 0 || x >= CANVAS_W || y >= CANVAS_H) return;
  if (a <= 0) { _buf[y][x] = null; return; }
  _buf[y][x] = [r, g, b, a];
}

function pGet(x, y) {
  if (x < 0 || y < 0 || x >= CANVAS_W || y >= CANVAS_H) return null;
  return _buf[y][x];
}

function pRect(x, y, w, h, r, g, b, a) {
  for (var yy = y; yy < y + h; yy++)
    for (var xx = x; xx < x + w; xx++) pSet(xx, yy, r, g, b, a);
}

function pCircle(cx, cy, rad, r, g, b, a) {
  for (var yy = Math.floor(cy - rad); yy <= Math.ceil(cy + rad); yy++)
    for (var xx = Math.floor(cx - rad); xx <= Math.ceil(cx + rad); xx++) {
      var dx = xx - cx, dy = yy - cy;
      if (dx * dx + dy * dy <= rad * rad) pSet(xx, yy, r, g, b, a);
    }
}

function pLine(x0, y0, x1, y1, r, g, b, a) {
  x0 = Math.round(x0); y0 = Math.round(y0); x1 = Math.round(x1); y1 = Math.round(y1);
  var dx = Math.abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
  var dy = -Math.abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
  var err = dx + dy;
  while (true) {
    pSet(x0, y0, r, g, b, a);
    if (x0 === x1 && y0 === y1) break;
    var e2 = 2 * err;
    if (e2 >= dy) { err += dy; x0 += sx; }
    if (e2 <= dx) { err += dx; y0 += sy; }
  }
}

// 把当前缓冲整体下移/上移，用于简单投影
function pShadow(ox, oy, r, g, b, a) {
  var copy = [];
  for (var y = 0; y < CANVAS_H; y++) {
    var row = [];
    for (var x = 0; x < CANVAS_W; x++) row.push(_buf[y][x]);
    copy.push(row);
  }
  for (var y2 = 0; y2 < CANVAS_H; y2++)
    for (var x2 = 0; x2 < CANVAS_W; x2++) {
      var src = copy[y2][x2];
      var tx = x2 + ox, ty = y2 + oy;
      var occupied = (tx >= 0 && ty >= 0 && tx < CANVAS_W && ty < CANVAS_H) ? _buf[ty][tx] : true;
      if (src && !occupied) pSet(tx, ty, r, g, b, a);
    }
}

// 给所有实心区域加 1px 深色描边（4 邻接），outline 只在空白处画
function pOutline(r, g, b) {
  var add = [];
  for (var y = 0; y < CANVAS_H; y++)
    for (var x = 0; x < CANVAS_W; x++) {
      if (_buf[y][x]) continue;
      var n = (x > 0 && _buf[y][x - 1]) || (x < CANVAS_W - 1 && _buf[y][x + 1]) ||
              (y > 0 && _buf[y - 1][x]) || (y < CANVAS_H - 1 && _buf[y + 1][x]);
      if (n) add.push([x, y]);
    }
  for (var i = 0; i < add.length; i++) pSet(add[i][0], add[i][1], r, g, b, 255);
}

// 右下边缘压暗，做出简单体积感；amount 0-1
function pShadeDown(amount) {
  var add = [];
  for (var y = 0; y < CANVAS_H; y++)
    for (var x = 0; x < CANVAS_W; x++) {
      var c = _buf[y][x];
      if (!c) continue;
      var right = x < CANVAS_W - 1 ? _buf[y][x + 1] : null;
      var down = y < CANVAS_H - 1 ? _buf[y + 1] ? _buf[y + 1][x] : null : null;
      if (!right || !down) add.push([x, y, c]);
    }
  for (var i = 0; i < add.length; i++) {
    var cc = add[i][2];
    pSet(add[i][0], add[i][1], Math.round(cc[0] * (1 - amount)), Math.round(cc[1] * (1 - amount)), Math.round(cc[2] * (1 - amount)), 255);
  }
}

// 左上提亮
function pHighlightLight(amount) {
  var add = [];
  for (var y = 0; y < CANVAS_H; y++)
    for (var x = 0; x < CANVAS_W; x++) {
      var c = _buf[y][x];
      if (!c) continue;
      var left = x > 0 ? _buf[y][x - 1] : null;
      var up = y > 0 ? _buf[y - 1][x] : null;
      if (!left || !up) add.push([x, y, c]);
    }
  for (var i = 0; i < add.length; i++) {
    var cc = add[i][2];
    pSet(add[i][0], add[i][1],
      Math.min(255, Math.round(cc[0] + (255 - cc[0]) * amount)),
      Math.min(255, Math.round(cc[1] + (255 - cc[1]) * amount)),
      Math.min(255, Math.round(cc[2] + (255 - cc[2]) * amount)), 255);
  }
}

// 圆形徽章底（带描边和内圈），markers 的统一底板
function pBadge(cx, cy, rad, r, g, b) {
  pCircle(cx, cy, rad, r, g, b, 255);
  pShadeDown(0.22);
  pOutline(24, 18, 34);
}

function pFlush() {
  for (var y = 0; y < CANVAS_H; y++)
    for (var x = 0; x < CANVAS_W; x++) {
      var c = _buf[y][x];
      if (c) _img.putPixel(x, y, app.pixelColor.rgba(c[0], c[1], c[2], 255));
      else _img.putPixel(x, y, app.pixelColor.rgba(0, 0, 0, 0));
    }
}

function pDiamond(cx, cy, rad, r, g, b) {
  for (var y = -rad; y <= rad; y++)
    for (var x = -rad; x <= rad; x++)
      if (Math.abs(x) + Math.abs(y) <= rad) pSet(cx + x, cy + y, r, g, b, 255);
}

function pStar4(cx, cy, rad, thick, r, g, b) {
  for (var i = 0; i <= rad; i++) {
    var t = Math.max(0, thick - Math.floor(i / 3));
    for (var k = -t; k <= t; k++) {
      pSet(cx + i, cy + k, r, g, b, 255);
      pSet(cx - i, cy + k, r, g, b, 255);
      pSet(cx + k, cy + i, r, g, b, 255);
      pSet(cx + k, cy - i, r, g, b, 255);
    }
  }
}

function pRing(cx, cy, rad, thick, r, g, b) {
  for (var y = -rad; y <= rad; y++)
    for (var x = -rad; x <= rad; x++) {
      var d = Math.sqrt(x * x + y * y);
      if (d <= rad && d >= rad - thick) pSet(cx + x, cy + y, r, g, b, 255);
    }
}

function pTriUp(cx, baseY, half, h, r, g, b) {
  for (var i = 0; i < h; i++) {
    var w = Math.round(half * (1 - i / h));
    for (var x = -w; x <= w; x++) pSet(cx + x, baseY - i, r, g, b, 255);
  }
}

function pTriDown(cx, topY, half, h, r, g, b) {
  for (var i = 0; i < h; i++) {
    var w = Math.round(half * (i / h));
    for (var x = -w; x <= w; x++) pSet(cx + x, topY + i, r, g, b, 255);
  }
}
