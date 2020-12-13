# 斜方向的九宫格图片拉伸

先上最终效果图：

<img src="/Users/heq/Library/Application Support/typora-user-images/image-20201213114213590.png" alt="image-20201213114213590" style="zoom: 33%;" />

阅读本文需要的基础知识：网格，顶点属性

## 一般九宫格拉伸的原理

在ui制作过程中，九宫格技术通常被用来拉伸一些某个维度上没有明显变化区域。比如按钮和一些界面的底图。它的原理是把一个大的矩形分为9个矩形，保留四个角的矩形不变，上下矩形在横向上无变化作横向拉伸，左右矩形在竖向上无变化作竖向拉伸，中间矩形在横向和竖向都无变化作两个方向拉伸。

<img src="/Users/heq/Library/Application Support/typora-user-images/image-20201213111928488.png" alt="image-20201213111928488" style="zoom: 33%;" /><img src="/Users/heq/Library/Application Support/typora-user-images/image-20201213111811726.png" alt="image-20201213111811726" style="zoom: 33%;" />



在unity中，需要设置精灵图片的border来指定拉伸区域。这种做法的好处就是节省图片资源，用更小分辨率的图去完成更大分辨率的图该做的事。

## 斜方向九宫格拉伸

最近接到了个需求：slg中点击地块会有选中格子的示意，现在需要从点击一个地块变为点击25个地块。然而实际情况是地块示意图是不能被正常水平和垂直拉伸的。。只能被斜方向拉伸。。为了这么一个东西去扩大贴图是极其浪费资源的，于是我想是否能通过斜方向拉伸九宫格来实现。百度谷歌了一圈似乎也没有翻到有人造这个轮子，只好自己写了！

首先分析斜方向拉伸的网格分布：

<img src="/Users/heq/Library/Application Support/typora-user-images/image-20201213125719383.png" alt="image-20201213125719383" style="zoom:33%;" />

[推荐该作图软件](https://www.geogebra.org/)

如图矩形ABCD为原始图片资源，中间的红色区域示意实际像素参考。正如九宫格设置border一样，只不过我们这里的border变成了斜方向的，上图中KF、JG、LI、EH就是我们的border line。RKJS、SIHT，TGFQ、QELR、RSTQ就是要拉伸的区域。

首先是顶点三角形分析，把整个矩形分为13个小块，4个角落三角形，上下左右4个不变三角形，被拉伸5个四边形。没有重复的顶点。由于四个角落是空像素，可以被优化掉。

接下来就是uv分析，这里重要的一个概念就是：**不论图片怎样被拉伸，uv是不变的，变化的只有position**。所以可以在一开始就写下所有顶点的uv值 。把上面的ABCD看做unit quad，A为原点，令a=|AE|，b=|AL|，那么每个点的uv可以直接写下来。除了S、Q的y坐标和R、T的横坐标有点麻烦。不过我们可以算出来，比如Q在KF上，KF用截距式可以表示为x/(1-a)+y/(1-b) = 1，代入x=0.5，可以得出y。同理可以得出R的x坐标

```c#
// 在实际coding中用下标写起来会比较方便
var yc  = (1 - a - 0.5f) * (1 - b) / (1 - a);
var xc  = (1 - b - 0.5f) * (1 - a) / (1 - b);
var x0  = new Vector2(0,      0);
var x1  = new Vector2(0,      1);
var x2  = new Vector2(1,      1);
var x3  = new Vector2(1,      0);
var x4  = new Vector2(a,      0);
var x5  = new Vector2(1 - a,  0);
var x6  = new Vector2(a,      1);
var x7  = new Vector2(1 - a,  1);
var x8  = new Vector2(0,      1 - b);
var x9  = new Vector2(0,      b);
var x10 = new Vector2(1,      1 - b);
var x11 = new Vector2(1,      b);
var x12 = new Vector2(0.5f,   yc);
var x13 = new Vector2(xc,     0.5f);
var x14 = new Vector2(0.5f,   1 - yc);
var x15 = new Vector2(1 - xc, 0.5f);
```

上面提到拉伸改变的是position，接下来要分析顶点坐标的变化。想想一下怎样描述拉伸，怎样把拉伸后的坐标赋值到每个顶点上？

停下来，仔细想想。let's think a few moment... 

其实这块还是有点难度的，我也在这里卡了挺久并且历经了几个残次品。后来终于顿悟：把图按BD劈成2半，把右半部分向右上方向拉伸，改变的顶点有哪些？同理，如果按AC劈成2半改变的又有哪些呢~

```c#
// 设实际图片大小为 w*h，右上角方向拉伸距离为trw*trh, 右下角方向拉伸距离为brw*brh
var scale  = new Vector2(w, h);
var vert0  = x0.scale(scale).add(new Vector3(0, 0, 0));
var vert1  = x1.scale(scale).add(new Vector3(0, 0, 0));
var vert2  = x2.scale(scale).add(new Vector3(trw, trh, 0));
var vert3  = x3.scale(scale).add(new Vector3(brw, brh, 0));
var vert4  = x4.scale(scale).add(new Vector3(brw, brh, 0));
var vert5  = x5.scale(scale).add(new Vector3(brw, brh, 0));
var vert6  = x6.scale(scale).add(new Vector3(trw, trh, 0));
var vert7  = x7.scale(scale).add(new Vector3(trw, trh, 0));
var vert8  = x8.scale(scale).add(new Vector3(0, 0, 0));
var vert9  = x9.scale(scale).add(new Vector3(0, 0, 0));
var vert10 = x10.scale(scale).add(new Vector3(trw + brw, trh + brh, 0));
var vert11 = x11.scale(scale).add(new Vector3(trw + brw, trh + brh, 0));
var vert12 = x12.scale(scale).add(new Vector3(brw, brh, 0));
var vert13 = x13.scale(scale).add(new Vector3(0, 0, 0));
var vert14 = x14.scale(scale).add(new Vector3(trw, trh, 0));
var vert15 = x15.scale(scale).add(new Vector3(trw + brw, trh + brh, 0));
```

顶点位置、顶点uv、顶点三角形顺序确定了，就可以确定我们的网格了。可以在unity中调参数验证。

<img src="/Users/heq/Library/Application Support/typora-user-images/image-20201213125136306.png" alt="image-20201213125136306" style="zoom:33%;" /><img src="/Users/heq/Library/Application Support/typora-user-images/image-20201213130130187.png" alt="image-20201213130130187" style="zoom:33%;" />

然后我们还有最后一个问题，那就是原点位置还是以左下角去定义的。这样在赋值坐标的时候还要做一次换算比较麻烦。所以需要对顶点位置进行一个平移变换来校准原点。Tx' = Tx+Vector(UA)

通过不断调整参数也会发现很多有意思的现象。只能说mesh真是挺好玩的！

最后，附上本研究的demo工程地址。希望有机会用上该技术的同学不吝star~！

[demo工程地址](https://github.com/hitheqing/ObliqueStretch/)
