// 图标配方表：每个函数在 32x32 网格上作画（pInit 后调用）。
// 风格约定：深色描边 #181222、暖色主色、右下压暗、左上提亮。

var OUTLINE = [24, 18, 34];

function icon_black_white_carrot() {
  // 黑白萝卜：上白下黑、绿缨
  pRect(13, 17, 6, 10, 232, 232, 240);
  pRect(13, 25, 6, 4, 32, 30, 42);
  pRect(14, 15, 4, 3, 214, 214, 226);
  pLine(15, 9, 15, 15, 74, 156, 74);
  pLine(12, 11, 15, 14, 52, 132, 60);
  pLine(18, 11, 15, 14, 52, 132, 60);
  pRect(15, 8, 1, 3, 120, 196, 96);
  pShadeDown(0.18);
  pHighlightLight(0.12);
  pOutline(OUTLINE[0], OUTLINE[1], OUTLINE[2]);
}

function icon_loaded_dice() {
  // 灌铅骰子：白色骰面 + 黑点 + 红色灌铅孔
  pRect(8, 10, 16, 15, 238, 238, 244);
  pRect(8, 10, 16, 2, 250, 250, 255);
  pRect(10, 13, 3, 3, 30, 28, 40);
  pRect(19, 13, 3, 3, 30, 28, 40);
  pRect(14, 17, 4, 4, 30, 28, 40);
  pRect(10, 21, 3, 3, 30, 28, 40);
  pRect(19, 21, 3, 3, 30, 28, 40);
  pRect(12, 25, 8, 2, 196, 62, 62);
  pRect(13, 26, 2, 1, 240, 120, 110);
  pShadeDown(0.2);
  pHighlightLight(0.1);
  pOutline(OUTLINE[0], OUTLINE[1], OUTLINE[2]);
}

function icon_miracle_seed() {
  // 奇迹种子：嫩芽从种子裂口钻出
  pCircle(16, 21, 7, 150, 104, 62);
  pCircle(16, 21, 5, 176, 126, 78);
  pRect(13, 22, 7, 3, 120, 80, 46);
  pLine(16, 15, 16, 19, 88, 168, 80);
  pLine(16, 15, 12, 11, 108, 190, 96);
  pLine(16, 15, 21, 11, 108, 190, 96);
  pCircle(11, 10, 2, 128, 204, 112);
  pCircle(22, 10, 2, 128, 204, 112);
  pShadeDown(0.2);
  pHighlightLight(0.12);
  pOutline(OUTLINE[0], OUTLINE[1], OUTLINE[2]);
}

function icon_figy_berry() {
  // 文柚果：胖果实 + 顶叶
  pCircle(16, 19, 9, 196, 84, 96);
  pCircle(13, 16, 4, 224, 118, 126);
  pRect(15, 8, 2, 4, 96, 74, 52);
  pLine(17, 9, 22, 6, 96, 172, 88);
  pLine(22, 6, 24, 8, 96, 172, 88);
  pLine(12, 26, 20, 26, 150, 56, 70);
  pShadeDown(0.22);
  pHighlightLight(0.12);
  pOutline(OUTLINE[0], OUTLINE[1], OUTLINE[2]);
}

function renderIcon(name) {
  pInit(32, 32);
  var fn = globalThis["icon_" + name];
  if (!fn) throw new Error("no icon: " + name);
  fn();
  pFlush();
}

// ---------------- 遗物 ----------------

function icon_snow_carrot() {
  // 雪萝卜：白身蓝顶、雪花
  pRect(13, 16, 6, 11, 236, 242, 250);
  pRect(13, 16, 6, 3, 150, 200, 240);
  pTriDown(16, 26, 3, 3, 236, 242, 250);
  pLine(15, 9, 15, 15, 90, 160, 210);
  pLine(12, 11, 15, 14, 120, 190, 235);
  pLine(19, 11, 15, 14, 120, 190, 235);
  pStar4(9, 20, 2, 1, 180, 225, 250);
  pStar4(22, 22, 2, 1, 180, 225, 250);
  pShadeDown(0.15);
  pOutline(20, 24, 40);
}

function icon_eviolite() {
  // 进化奇石：灰蓝矿石 + 内部青光
  pDiamond(16, 18, 9, 120, 128, 148);
  pDiamond(16, 18, 6, 150, 160, 182);
  pDiamond(16, 17, 3, 120, 220, 235);
  pRect(15, 12, 2, 2, 235, 245, 255);
  pLine(10, 16, 13, 13, 190, 200, 220);
  pShadeDown(0.25);
  pOutline(22, 24, 32);
}

function icon_exp_share() {
  // 学习装置：双箭头共享 + 绿叶
  pRect(8, 15, 16, 9, 104, 160, 118);
  pRect(8, 15, 16, 2, 138, 190, 146);
  pTriUp(13, 23, 4, 4, 104, 160, 118);
  pTriUp(19, 23, 4, 4, 104, 160, 118);
  pLine(12, 19, 20, 19, 236, 240, 220);
  pLine(15, 9, 15, 13, 96, 170, 96);
  pCircle(12, 10, 2, 128, 200, 118);
  pCircle(19, 10, 2, 128, 200, 118);
  pShadeDown(0.2);
  pOutline(24, 20, 30);
}

function icon_focus_sash() {
  // 气势披带：红色腰带打结
  pRect(6, 14, 20, 5, 198, 58, 64);
  pRect(6, 14, 20, 2, 232, 96, 96);
  pRect(14, 12, 4, 9, 170, 44, 52);
  pRect(15, 13, 2, 7, 232, 120, 120);
  pTriDown(9, 19, 3, 5, 198, 58, 64);
  pTriDown(23, 19, 3, 5, 198, 58, 64);
  pShadeDown(0.2);
  pOutline(36, 18, 24);
}

function icon_king_rock() {
  // 王者之证：灰色石头 + 金冠
  pCircle(16, 20, 8, 118, 118, 128);
  pCircle(14, 18, 4, 150, 150, 160);
  pRect(9, 10, 14, 4, 232, 190, 78);
  pTriUp(9, 14, 2, 4, 232, 190, 78);
  pTriUp(16, 14, 2, 4, 232, 190, 78);
  pTriUp(23, 14, 2, 4, 232, 190, 78);
  pRect(12, 12, 2, 2, 250, 226, 140);
  pShadeDown(0.22);
  pOutline(30, 26, 24);
}

function icon_multiscale() {
  // 多重鳞片：三层鳞甲
  for (var i = 0; i < 3; i++) {
    var y = 12 + i * 5;
    pCircle(16, y, 8 - i, 96 + i * 26, 150 + i * 22, 208 + i * 10);
    pRect(8 + i, y, 16 - i * 2, 2, 70 + i * 20, 120 + i * 20, 180 + i * 10);
  }
  pCircle(16, 26, 3, 150, 200, 232);
  pShadeDown(0.18);
  pOutline(24, 30, 44);
}

function icon_never_melt_ice() {
  // 永不融化的冰：冰块中的蓝色火焰
  pRect(8, 12, 16, 14, 150, 206, 236);
  pRect(8, 12, 16, 2, 210, 240, 252);
  pRect(10, 14, 3, 10, 186, 228, 246);
  pTriUp(16, 23, 4, 6, 96, 160, 232);
  pTriUp(16, 20, 2, 4, 168, 220, 255);
  pShadeDown(0.16);
  pOutline(24, 34, 48);
}

function icon_serene_grace() {
  //  serenity：粉白花朵 + 光点
  for (var k = 0; k < 5; k++) {
    var a = k * 72 * Math.PI / 180;
    pCircle(16 + Math.cos(a) * 5, 17 + Math.sin(a) * 5, 3, 232, 178, 206);
  }
  pCircle(16, 17, 4, 248, 226, 168);
  pStar4(16, 17, 2, 1, 255, 250, 220);
  pStar4(7, 8, 2, 1, 240, 220, 250);
  pShadeDown(0.12);
  pOutline(46, 28, 44);
}

function icon_spell_tag() {
  // 咒符标签：纸签 + 红绳 + 符文
  pRect(9, 10, 14, 16, 232, 220, 186);
  pTriDown(9, 26, 2, 3, 232, 220, 186);
  pTriUp(9, 10, 2, 3, 232, 220, 186);
  pRect(15, 6, 2, 5, 176, 52, 52);
  pCircle(16, 6, 2, 198, 70, 70);
  pLine(13, 15, 19, 15, 96, 72, 52);
  pLine(16, 18, 13, 21, 96, 72, 52);
  pLine(16, 18, 19, 21, 96, 72, 52);
  pShadeDown(0.14);
  pOutline(40, 32, 26);
}

function icon_disguise() {
  // 变身面具：紫色眼罩面具
  pCircle(16, 16, 10, 118, 78, 150);
  pRect(7, 13, 18, 7, 138, 96, 174);
  pCircle(12, 16, 2, 240, 236, 250);
  pCircle(20, 16, 2, 240, 236, 250);
  pRect(14, 20, 4, 2, 84, 52, 112);
  pLine(8, 24, 12, 22, 172, 130, 210);
  pLine(24, 24, 20, 22, 172, 130, 210);
  pShadeDown(0.2);
  pOutline(34, 22, 44);
}

function icon_ability_capsule() {
  // 特性胶囊：红白胶囊
  pRect(10, 12, 12, 8, 236, 238, 244);
  pRect(10, 12, 12, 5, 210, 62, 70);
  pRect(10, 12, 12, 1, 240, 130, 130);
  pRect(15, 12, 2, 8, 180, 44, 54);
  pRect(12, 20, 8, 2, 210, 212, 220);
  pShadeDown(0.18);
  pOutline(30, 24, 30);
}

// ---------------- 药水 ----------------

function icon_gray_carrot() {
  // 灰萝卜：灰色萝卜 + 灰绿缨
  pRect(13, 16, 6, 10, 178, 178, 186);
  pTriDown(16, 25, 3, 3, 178, 178, 186);
  pLine(15, 9, 15, 15, 110, 140, 118);
  pLine(12, 11, 15, 14, 128, 158, 132);
  pLine(19, 11, 15, 14, 128, 158, 132);
  pRect(14, 18, 2, 3, 210, 210, 218);
  pShadeDown(0.18);
  pOutline(30, 30, 36);
}

function icon_galarian_spice() {
  // 伽勒尔香料：广口瓶 + 香料颗粒
  pRect(10, 12, 12, 14, 200, 150, 96);
  pRect(10, 12, 12, 4, 232, 196, 140);
  pRect(12, 8, 8, 4, 120, 86, 58);
  pRect(12, 16, 3, 3, 156, 88, 52);
  pRect(17, 19, 2, 2, 176, 100, 60);
  pRect(14, 21, 2, 2, 156, 88, 52);
  pShadeDown(0.2);
  pOutline(38, 28, 20);
}

function icon_plated_armor() {
  // 金属护甲：盾牌 + 铆钉
  pRect(9, 11, 14, 10, 168, 178, 196);
  pTriDown(16, 21, 6, 7, 168, 178, 196);
  pRect(9, 11, 14, 2, 210, 220, 236);
  pRect(11, 13, 2, 8, 196, 206, 222);
  pCircle(16, 16, 3, 120, 130, 150);
  pCircle(16, 16, 1, 226, 232, 244);
  pShadeDown(0.22);
  pOutline(30, 34, 44);
}

function icon_victors_curry() {
  // 冠军咖喱：碗 + 咖喱 + 热气
  pRect(7, 17, 18, 7, 210, 214, 226);
  pRect(7, 17, 18, 2, 236, 240, 248);
  pTriDown(16, 24, 6, 4, 210, 214, 226);
  pRect(9, 15, 14, 3, 196, 142, 64);
  pRect(11, 14, 3, 2, 220, 168, 82);
  pLine(13, 8, 13, 13, 230, 230, 236);
  pLine(16, 6, 16, 12, 230, 230, 236);
  pLine(19, 8, 19, 13, 230, 230, 236);
  pShadeDown(0.16);
  pOutline(38, 30, 26);
}

function icon_defense_boost() {
  // 防御强化（30x30 用 32x32 网格画，外面会裁到 30）
  pRect(10, 8, 12, 10, 120, 176, 232);
  pTriDown(16, 18, 6, 8, 120, 176, 232);
  pRect(10, 8, 12, 2, 168, 214, 250);
  pLine(16, 10, 16, 21, 232, 244, 255);
  pLine(12, 14, 20, 14, 232, 244, 255);
  pShadeDown(0.2);
  pOutline(26, 40, 60);
}

// ---------------- 状态标记（统一圆徽章 + 字形） ----------------

function mBadge(r, g, b) { pCircle(16, 16, 13, r, g, b); pShadeDown(0.26); }

function icon_abundance() {
  mBadge(58, 118, 66);
  pLine(16, 23, 16, 11, 232, 226, 150);
  pLine(16, 17, 11, 13, 200, 226, 130);
  pLine(16, 15, 21, 11, 200, 226, 130);
  pCircle(10, 12, 2, 226, 232, 140); pCircle(22, 10, 2, 226, 232, 140);
  pCircle(16, 9, 2, 240, 236, 160);
  pOutline(20, 40, 24);
}

function icon_accelerate() {
  mBadge(216, 176, 58);
  pTriDown(14, 8, 5, 9, 255, 246, 188);
  pTriDown(19, 14, 5, 9, 255, 246, 188);
  pOutline(60, 44, 12);
}

function icon_awe() {
  mBadge(196, 120, 52);
  pRect(14, 8, 4, 11, 255, 240, 214);
  pRect(14, 21, 4, 4, 255, 240, 214);
  pOutline(58, 32, 12);
}

function icon_cannot_mount() {
  mBadge(96, 96, 112);
  pRing(16, 17, 7, 3, 220, 224, 236);
  pRect(13, 8, 6, 3, 220, 224, 236);
  pLine(9, 9, 23, 25, 208, 66, 66);
  pLine(9, 10, 22, 25, 208, 66, 66);
  pOutline(28, 28, 38);
}

function icon_defiant() {
  mBadge(178, 62, 66);
  pTriUp(16, 24, 6, 9, 255, 214, 120);
  pRect(13, 12, 6, 6, 255, 228, 150);
  pLine(9, 10, 13, 13, 255, 200, 120);
  pLine(23, 10, 19, 13, 255, 200, 120);
  pOutline(56, 20, 22);
}

function icon_dynamax_form() {
  mBadge(168, 58, 74);
  pCircle(16, 15, 7, 232, 96, 108);
  pCircle(16, 15, 4, 255, 176, 168);
  pTriangleCloud();
  pOutline(56, 18, 26);
}

function pTriangleCloud() {
  pCircle(9, 20, 3, 210, 70, 84);
  pCircle(23, 20, 3, 210, 70, 84);
  pCircle(16, 22, 4, 210, 70, 84);
}

function icon_encore() {
  mBadge(150, 86, 176);
  pRect(15, 9, 2, 12, 248, 236, 255);
  pCircle(12, 21, 3, 248, 236, 255);
  pRect(17, 8, 6, 3, 248, 236, 255);
  pOutline(44, 24, 54);
}

function icon_eternal_whinny() {
  mBadge(84, 78, 110);
  pRect(12, 16, 8, 6, 228, 226, 240);
  pTriDown(12, 22, 4, 5, 228, 226, 240);
  pRect(11, 12, 3, 5, 228, 226, 240);
  pRect(10, 10, 2, 3, 228, 226, 240);
  pRect(16, 14, 2, 5, 200, 198, 220);
  pOutline(30, 26, 44);
}

function icon_eternamax() {
  mBadge(96, 60, 140);
  pStar4(16, 16, 11, 3, 226, 150, 255);
  pCircle(16, 16, 4, 255, 236, 255);
  pCircle(16, 16, 2, 190, 110, 240);
  pOutline(40, 22, 60);
}

function icon_fly() {
  mBadge(70, 132, 196);
  pLine(9, 18, 16, 12, 238, 246, 255);
  pLine(16, 12, 23, 18, 238, 246, 255);
  pLine(12, 20, 16, 15, 200, 224, 250);
  pLine(16, 15, 20, 20, 200, 224, 250);
  pRect(15, 18, 3, 4, 238, 246, 255);
  pOutline(20, 44, 74);
}

function icon_friendly_guard() {
  mBadge(66, 118, 170);
  pRect(10, 10, 12, 8, 240, 242, 250);
  pTriDown(16, 18, 6, 7, 240, 242, 250);
  pCircle(16, 14, 3, 200, 86, 96);
  pOutline(22, 40, 62);
}

function icon_frozen() {
  mBadge(62, 132, 186);
  for (var k2 = 0; k2 < 3; k2++) {
    var a2 = k2 * 60 * Math.PI / 180;
    pLine(16 - Math.cos(a2) * 9, 16 - Math.sin(a2) * 9, 16 + Math.cos(a2) * 9, 16 + Math.sin(a2) * 9, 226, 246, 255);
  }
  pLine(12, 9, 13, 12, 226, 246, 255); pLine(20, 9, 19, 12, 226, 246, 255);
  pOutline(18, 46, 72);
}

function icon_future_sight() {
  mBadge(110, 74, 160);
  pCircle(16, 16, 9, 238, 226, 252);
  pCircle(16, 16, 5, 120, 70, 170);
  pCircle(16, 16, 2, 246, 240, 255);
  pStar4(9, 9, 2, 1, 226, 200, 255);
  pOutline(40, 24, 60);
}

function icon_grassy_terrain() {
  mBadge(70, 128, 70);
  pLine(16, 23, 13, 10, 168, 220, 120);
  pLine(16, 23, 16, 8, 198, 234, 140);
  pLine(16, 23, 20, 11, 168, 220, 120);
  pRect(8, 22, 16, 2, 120, 170, 90);
  pOutline(24, 52, 26);
}

function icon_heavy_lance() {
  mBadge(96, 104, 126);
  pLine(9, 23, 22, 10, 214, 220, 234);
  pTriUp(23, 10, 3, 4, 236, 240, 250);
  pRect(12, 18, 5, 2, 176, 120, 70);
  pOutline(28, 32, 44);
}

function icon_helping_hand() {
  mBadge(196, 132, 62);
  pRect(10, 15, 12, 7, 250, 226, 190);
  pRect(12, 12, 3, 4, 250, 226, 190);
  pRect(16, 11, 3, 5, 250, 226, 190);
  pRect(19, 13, 3, 4, 250, 226, 190);
  pOutline(60, 38, 16);
}

function icon_ice_wall() {
  mBadge(74, 140, 188);
  for (var yy = 0; yy < 3; yy++) {
    var off = (yy % 2) * 3;
    for (var xx = 0; xx < 3; xx++) pRect(9 + xx * 6 + off, 10 + yy * 4, 5, 3, 216, 240, 252);
  }
  pOutline(18, 44, 68);
}

function icon_ingrain() {
  mBadge(92, 118, 66);
  pLine(16, 10, 16, 18, 210, 196, 120);
  pLine(16, 15, 11, 22, 150, 196, 110);
  pLine(16, 15, 21, 22, 150, 196, 110);
  pLine(16, 18, 16, 24, 150, 196, 110);
  pOutline(30, 40, 22);
}

function icon_last_resort() {
  mBadge(110, 96, 96);
  pLine(10, 22, 20, 12, 226, 220, 214);
  pTriUp(21, 11, 3, 4, 240, 232, 224);
  pLine(13, 11, 21, 19, 200, 90, 90);
  pOutline(34, 28, 28);
}

function icon_leech_seed() {
  mBadge(78, 128, 82);
  pCircle(16, 18, 6, 170, 120, 66);
  pCircle(14, 16, 2, 200, 150, 90);
  pLine(16, 12, 16, 7, 150, 200, 110);
  pLine(16, 9, 12, 6, 150, 200, 110);
  pLine(16, 9, 20, 6, 150, 200, 110);
  pOutline(26, 50, 28);
}

function icon_mounted_glastrier() {
  mBadge(72, 122, 176);
  pRect(11, 15, 10, 6, 232, 240, 250);
  pTriDown(11, 21, 4, 4, 232, 240, 250);
  pRect(10, 11, 3, 5, 232, 240, 250);
  pTriUp(10, 11, 2, 3, 232, 240, 250);
  pStar4(21, 11, 3, 1, 220, 240, 255);
  pOutline(20, 42, 66);
}

function icon_mounted_spectrier() {
  mBadge(88, 70, 118);
  pRect(11, 15, 10, 6, 214, 204, 236);
  pTriDown(11, 21, 4, 4, 214, 204, 236);
  pRect(10, 11, 3, 5, 214, 204, 236);
  pTriUp(10, 11, 2, 3, 190, 120, 220);
  pStar4(21, 11, 3, 1, 200, 150, 240);
  pOutline(30, 22, 48);
}

function icon_panic() {
  mBadge(178, 62, 74);
  pRect(14, 8, 4, 11, 255, 244, 236);
  pRect(14, 21, 4, 4, 255, 244, 236);
  pLine(8, 9, 11, 12, 255, 220, 210);
  pLine(24, 9, 21, 12, 255, 220, 210);
  pOutline(56, 18, 24);
}

function icon_pp_restore() {
  mBadge(72, 116, 178);
  pCircle(16, 16, 9, 236, 242, 252);
  pRect(15, 10, 2, 8, 90, 130, 190);
  pRect(15, 16, 2, 4, 90, 130, 190);
  pCircle(16, 21, 1, 90, 130, 190);
  pOutline(20, 40, 66);
}

function icon_pressure() {
  mBadge(120, 64, 90);
  pCircle(16, 15, 8, 240, 226, 232);
  pCircle(16, 15, 4, 120, 54, 74);
  pLine(10, 20, 10, 25, 240, 226, 232);
  pLine(16, 21, 16, 26, 240, 226, 232);
  pLine(22, 20, 22, 25, 240, 226, 232);
  pOutline(40, 18, 26);
}

function icon_psychic_noise() {
  mBadge(146, 70, 150);
  pRing(16, 16, 10, 2, 250, 226, 250);
  pRing(16, 16, 6, 2, 236, 180, 240);
  pCircle(16, 16, 2, 255, 240, 255);
  pRect(21, 8, 3, 3, 240, 190, 245);
  pOutline(44, 20, 44);
}

function icon_quick_sight() {
  mBadge(66, 122, 186);
  pCircle(15, 16, 7, 236, 246, 255);
  pCircle(17, 16, 4, 40, 60, 90);
  pCircle(18, 15, 1, 255, 255, 255);
  pLine(22, 11, 27, 10, 220, 236, 255);
  pLine(22, 16, 27, 16, 220, 236, 255);
  pLine(22, 21, 27, 22, 220, 236, 255);
  pOutline(20, 40, 66);
}

function icon_slow_down() {
  mBadge(110, 110, 130);
  pCircle(16, 17, 9, 236, 236, 244);
  pLine(16, 17, 16, 10, 60, 60, 80);
  pLine(16, 17, 21, 19, 60, 60, 80);
  pCircle(16, 17, 2, 60, 60, 80);
  pOutline(30, 30, 44);
}

function icon_soul_heart() {
  mBadge(136, 72, 120);
  pCircle(12, 13, 4, 244, 154, 196);
  pCircle(20, 13, 4, 244, 154, 196);
  pTriDown(16, 14, 8, 9, 244, 154, 196);
  pCircle(12, 12, 2, 255, 200, 226);
  pOutline(44, 20, 40);
}

function icon_spotlight() {
  mBadge(176, 150, 58);
  pTriUp(16, 25, 3, 12, 255, 244, 190);
  pTriUp(16, 25, 6, 9, 226, 206, 120);
  pCircle(16, 26, 2, 255, 250, 220);
  pOutline(56, 44, 16);
}

function icon_steed_guard() {
  mBadge(96, 108, 128);
  pRect(10, 11, 12, 8, 232, 236, 246);
  pTriDown(16, 19, 6, 7, 232, 236, 246);
  pRect(12, 13, 3, 6, 200, 206, 222);
  pLine(16, 12, 16, 22, 150, 160, 180);
  pOutline(28, 32, 44);
}

function icon_temp_thorns() {
  mBadge(88, 116, 74);
  pCircle(16, 16, 6, 150, 190, 110);
  for (var i2 = 0; i2 < 8; i2++) {
    var a3 = i2 * 45 * Math.PI / 180;
    pLine(16 + Math.cos(a3) * 5, 16 + Math.sin(a3) * 5, 16 + Math.cos(a3) * 10, 16 + Math.sin(a3) * 10, 210, 230, 150);
  }
  pCircle(16, 16, 2, 220, 200, 120);
  pOutline(28, 44, 24);
}

function icon_trick() {
  mBadge(122, 78, 156);
  pLine(9, 22, 15, 16, 244, 226, 255);
  pLine(15, 16, 9, 10, 244, 226, 255);
  pLine(23, 22, 17, 16, 244, 226, 255);
  pLine(17, 16, 23, 10, 244, 226, 255);
  pCircle(16, 16, 2, 255, 240, 255);
  pOutline(40, 24, 52);
}

function icon_truce() {
  mBadge(150, 150, 162);
  pRect(14, 8, 2, 18, 200, 170, 120);
  pTriUp(22, 15, 6, 6, 246, 246, 250);
  pRect(17, 9, 10, 6, 246, 246, 250);
  pOutline(36, 36, 44);
}
