//写cookies
function setCookie(name, value,days) {
    var Days = days; //此 cookie 将被保存 30 天
    var exp = new Date();    //new Date("December 31, 9998");
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
}

//读取cookies
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}

//删除cookie
function delCookie(name)
{
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null) document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();

}

function getTextPosition(id, col, row) {
    var pos = 0;
    var r = row;
    var t = $("#" + id).val().split('\n');
    if (r >= t.length) { r = t.length - 1; }
    for (i = 0; i < r; i++) {
        pos = pos + t[i].length + 1;
    }
    pos = pos - 1;
    if (col > (t[r].length)) {
        pos = pos + t[r].length + 1;
    } else {
        pos = pos + col;
    }
    return pos;
}

function closeMe() {
    //Reference: http://blogs.msdn.com/b/rextang/archive/2008/10/17/9002876.aspx

    try {
        var win = window.open('', '_self', '', true);
        win.opener = true;
        win.close();
    }
    catch (e) {

    }
}

$(function () {
    $('a.has-spinner, button.has-spinner').click(function () {
        //$(this).addClass('active')/*.prop('disabled', true)*/;
        return false;
    });
});

$(document).ready(function () {
    $('a.has-spinner, button.has-spinner').removeClass('active').prop('disabled', false);
});