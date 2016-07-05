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
                window.parent.location.href = "../admin/login";
            } else {
                window.location.href = "../admin/login";
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

// 弹出默认大小的窗口
function popDefaultWnd(id,title) {
    popWnd(id, title, 800, 600);
}

//弹出自定义窗口
function popFirstWnd(id, title,width,height) {
    popWnd(id, title, width,height);
}

// 弹出窗口
function popWnd(id, title, width, height) {
    var url = $("#" + id).attr("url");
    var wnd = $("#wnd").data("kendoWindow");
    if (!wnd || wnd._closing) {
        $("#wnd").css("width", width + "px");
        $("#wnd").css("height", height + "px");
        wnd = $("#wnd").kendoWindow({
            width: width,
            height: height,
            title: title,
            actions: ["Close"],
            content: rootUrl + url,
            animation: false,
            modal: true,
            visible: false,
            resizable: false,
            draggable: true
        }).data("kendoWindow");
        wnd.open().center();
    }
}