// 获取指定名称的页面传递参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

// 错误提示框
function ShowErrModal(error, status) {
    ShowModal(2, error, function () {
        if (status == 401) { // 如果未经授权则跳转到登录页面，其他情况则跳转到首页。
            if (window.parent) { // 如果当前窗口存在父窗口，则在父窗口进行跳转
                window.parent.location.href = "../user/login";
            } else {
                window.location.href = "../user/login";
            }
        }
    });
}

// 消息提示框
function ShowTipsModel(tips, callback) {
    ShowModal(0, tips, callback);
}

// 信息提示框
// type 0:消息，1:警告，2:错误
function ShowModal(type, msg, callback) {
    var title;
    switch (type) {
        case 0:
            title = '<i class="fa fa-comment"></i><span style="margin-left: 2px;">提示</span>';
            break;
        case 1:
            title = '<i class="fa fa-exclamation-triangle"></i><span style="margin-left: 2px;">警告</span>';
            break;
        case 2:
            title = '<i class="fa fa-times-circle"></i><span style="margin-left: 2px;">错误</span>';
            break;
        default:
            title = '<i class="fa fa-comment"></i><span style="margin-left: 2px;">提示</span>';
            break;
    }
    $("#model-msg-title").html(title);
    $("#showerrTitle").html(msg);
    $('#commontiperrModal').modal({
        backdrop: 'static',
        show: true
    });
    $("#modelClose").click(function () {
        callback();
    });
}

// 确认提示框
function ShowConfirmModal(title, callback) {
    $("#showConfirmTitle").html(title);
    $('#commontipConfirmModal').modal({
        backdrop: 'static',
        show: true
    });
    $("#confirmPop").click(function () {
        $('#commontipConfirmModal').modal("hide");
        callback();
    });
}

// 弹出默认大小的一级窗口，紧靠悬浮窗
function popDefaultFirstWnd(id, title, url) {
    popFirstLevelWnd(id, title, url, 800, 600);
}

// 弹出一级窗口，紧靠悬浮窗
function popFirstLevelWnd(id, title, url, width, height) {
    var left = document.body.clientWidth - 153 - width;
    popFirstWnd(id, title, url, width, height, left);
}

// 弹出默认大小的二级窗口，紧靠弹出的第一个窗口
function popDefaultSecondWnd() {
    var title = $("#btnTransfer").attr("tag");
    var url = $("#btnTransfer").attr("url");
    var width = $("#btnTransfer").attr("wnd-width");
    popSecondLevelWnd(title, url, width, 600, 800);
}

// 弹出二级窗口，紧靠弹出的第一个窗口
function popSecondLevelWnd(title, url, width, height, parentWidth) {
    var left = window.parent.document.body.clientWidth - 153 - width - parentWidth;
    popSecondWnd(title, url, width, height, left);
}

// 弹出自定义窗口
function popFirstWnd(id, title, url, width, height, left) {
    var img = $("#" + id).find("img");
    var imgSrc = img.attr("src");
    var wnd = $("#firstWnd").data("kendoWindow");
    if (!wnd || wnd._closing) {
        $('#firstWnd').replaceWith('<div id="firstWnd" style="position:fixed;"></div>');
        $("#firstWnd").css("width", width + "px");
        $("#firstWnd").css("height", height + "px");
        wnd = $("#firstWnd").kendoWindow({
            width: width,
            height: height,
            position: {
                top: 80,
                left: left
            },
            title: title,
            actions: ["Close"],
            content: rootUrl + url,
            animation: false,
            visible: false,
            resizable: false,
            draggable: false,
            close: function (e) {
                // 当二级窗口还存在。一级窗口关闭时，二级窗口往右边靠拢
                var subWnd = $("#secondWnd").data("kendoWindow");
                if (subWnd) {
                    var left = subWnd.options.position.left;
                    var top = subWnd.options.position.top;
                    left += width;
                    subWnd.setOptions({
                        position: {
                            top: top,
                            left: left
                        }
                    });
                }

                // 关闭窗口时，取消选项的高亮显示
                img.attr("src", imgSrc);
            }
        }).data("kendoWindow");
        wnd.open();
        // 当成功打开窗体时，让选择项高亮显示
        img.attr("src", imgSrc.replace(/1/, "2"));
    }
}

// 弹出自定义窗口
function popSecondWnd(title, url, width, height, left) {
    var wnd = $('#secondWnd').data("kendoWindow");
    if (!wnd || wnd._closing) {
        $('#secondWnd').replaceWith('<div id="secondWnd" style="position:fixed;"></div>');
        $('#secondWnd').css("width", width + "px");
        $('#secondWnd').css("height", height + "px");
        wnd = $('#secondWnd').kendoWindow({
            width: width,
            height: height,
            position: {
                top: 80,
                left: left
            },
            title: title,
            actions: ["Close"],
            content: rootUrl + url,
            animation: false,
            visible: false,
            resizable: false,
            draggable: false
        }).data("kendoWindow");
        wnd.open();
    }
}

// 关闭第二级窗口，并刷新一级窗口
function closeSecondWnd() {
    var wnd = $('#secondWnd').data("kendoWindow");
    wnd.close();

    var parentWnd = $("#firstWnd").data("kendoWindow");
    if (parentWnd) {
        var content = parentWnd.options.content.url;
        parentWnd.refresh(content);
    }
}

// iframe模拟在父窗口点击
function transferClick(id, title, width) {
    var url = $("#" + id).attr("url");
    $("#btnTransfer", parent.document).attr("tag", title);
    $("#btnTransfer", parent.document).attr("url", url);
    $("#btnTransfer", parent.document).attr("wnd-width", width);
    $("#btnTransfer", parent.document).click();
}