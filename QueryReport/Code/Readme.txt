头部样式。
<div class='page-header'>
    <h1 class='pull-left'>
        <span>View Level</span>
    </h1>
    <div class='pull-right'>
        <div class='btn-group'>
            <input type="submit" name="Button1" value="Save" id="Button1" class="btn btn-primary" />
        </div>
    </div>
</div>


常用css
row：
根据它的2个重要样式，所以是作为一个独占一行的方块来显示。
1.display:table 此元素会作为块级表格来显示（类似 <table>），表格前后带有换行符
2. clear: both;在左右两侧均不允许浮动元素。

row，一般后接 col-xs-1 col-sm-1 col-md-1 col-lg-1
3个重要属性
1.display：float: left
2.width: 8.33333%;
3.max-width: 750px;

根据它们的属性，先确定每个占母div的百分比（由后面的数字决定），那么就决定你想放多少个div在一行，。
每个div绝对最大宽带 由xs sm md lg决定。
比如我想在一行上放3个div 。所以 可以选择 col-sm-4 ，以4结尾的css。而sm，xl是确定最大宽度的。

xs,sm ,md,lg 定义了最大的宽度。而1---12，则定义了比例的上升。

col-xs-1   ---》col-xs-12                  8.3333333%  的比例逐级上升。 所以到12就是 100%
@media (min-width: 768px) {
  .container {
    max-width: 750px; }

col-sm-1
@media (min-width: 992px) {
  .container {
    max-width: 970px; }


col-md-1
@media (min-width: 1200px) {
  .container {
    max-width: 1170px; }

col-lg-1


关于表格。
<table class='table table-striped table-hover table-bordered'>


关于文本
<div class='text-right text-contrast subtotal'>


text and textbox inline ，在textbox最后加入inline。

index.html
有个曲线图，页面下边第一块 代码就是 曲线图的代码。
下一个代码就是 日历 的代码。注意有一个全屏的弹出窗口。可以作为程序其他地方使用。

1.可以用加入导入功能来简化用户操作。


1.简化代码。
2.大致了解框架和引用文件。

3.整体框架了解。改变框架风格
4.菜单栏缩进，拉伸js代码。
5.一般的三层结构整理。放入不同文件夹。
6.测试页

*menu方法有点繁琐。简化点。

*application的根目录。

*add lever。应该 real time 得到 value。加入2个方法。（ 最后一个。和某某之后。避免同时操作的问题）
*modify ，Report group 没有赋值， list ，group 等没有转换。
*catagery 没有做。
*rp4 ，没有全部转换为强类型 实体。
*user 没有完善。
*全面检查 ，databaseid是否已经插入进入。
*多数据库测试。
*保存 查询语句没有做完。
*平凑语句还是要正常点 xxx，xxx，
*选择 字段，比如order，要从conent中拿出来。
* <td><%--<%# getLevel((int)DataBinder.Eval(Container.DataItem, "REPORTRIGHT"))%>--%></td>


查看报表，有display的权限。并且有这个组。
修改、删除报表。有view 的 sensitivitily level.和add对应的功能。


菜单的focus。
先读cookie是否有值。如果有值。那么插入js。
click 菜单更改 cookie。
还是写一个读写cookie的方法吧。省得每次都baidu。


* user 字段表
view level 没用。


* 没有检测是否有setup的权限。
*像 viewlevel 那样简化。
*sensitivity 。modify彻底删除 position after选项。
*report detail 页面 错误提示不好看。
*company 暂时屏蔽删除功能。
*pdf 标题的格式。


*修改check， report：databaseid+reportname 组合和 unit key。 user:取消uid unit key。
添加一个 新 view。work_2


*not conatin space 也不行。
*取消重复  最好做法是在order by中。
*avg sum. temp pdf .no ok .
temp html ok. 还没有测试。

1.数据导入   .
2.avg 和sum  .
3.修改 check .


1.两套账户都可以登录。
2.外部只要传uid。
3.对比 view来得到是否给权限。



distributionlevel                          report group
usergrouplevel                             no use
reportright                                reportright
GID                                         USERGROUP
usergroup                                  NO USE
sensitivity                                view level
viewlevel                                  no use
[SETUPUSER]                                USER WHERE FROM 0,NEW ,2 LOGIN.3 QUICK