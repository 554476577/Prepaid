var rootUrl = "http://localhost:3893/";
var app = angular.module('app', [], function ($httpProvider) {
    //for (var index in activeIndexs) {
    //    $("#" + activeIndexs[index]).addClass("active");
    //}

    // Use x-www-form-urlencoded Content-Type 修改请求头
    $httpProvider.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';

    /**
     * jquery.post会把Data序列化成字符串而angular不会，这里进行手动转换
     * The workhorse; converts an object to x-www-form-urlencoded serialization.
     * @param {Object} obj
     * @return {String}
     */
    var param = function (obj) {
        var query = '', name, value, fullSubName, subName, subValue, innerObj, i;

        for (name in obj) {
            value = obj[name];

            if (value instanceof Array) {
                for (i = 0; i < value.length; ++i) {
                    subValue = value[i];
                    fullSubName = name + '[' + i + ']';
                    innerObj = {};
                    innerObj[fullSubName] = subValue;
                    query += param(innerObj) + '&';
                }
            }
            else if (value instanceof Object) {
                for (subName in value) {
                    subValue = value[subName];
                    fullSubName = name + '[' + subName + ']';
                    innerObj = {};
                    innerObj[fullSubName] = subValue;
                    query += param(innerObj) + '&';
                }
            }
            else if (value !== undefined && value !== null)
                query += encodeURIComponent(name) + '=' + encodeURIComponent(value) + '&';
        }

        return query.length ? query.substr(0, query.length - 1) : query;
    };

    // Override $http service's default transformRequest
    $httpProvider.defaults.transformRequest = [function (data) {
        return angular.isObject(data) && String(data) !== '[object File]' ? param(data) : data;
    }];
});

// 页面渲染完毕执行的自定义回调
app.directive('onFinishRenderFilters', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) {
                $timeout(function () {
                    scope.$emit('ngRepeatFinished');
                });
            }
        }
    };
});

// 该控制器针对布局页面
app.controller('layoutCtrl', function ($scope, $http) {
    getModules();
    getUserSession();

    // 获取系统所有模块
    function getModules() {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/modules"
        }).success(function (data, status, headers, config) {
            $scope.LayoutModules = data.Items;
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }

    // 获取用户信息
    function getUserSession() {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/user"
        }).success(function (data, status, headers, config) {
            $scope.UserSession = data;
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }

    // 退出系统
    $scope.logout = function () {
        ShowConfirmModal("确定要退出系统吗？", function () {
            $http({
                method: "post",
                withCredentials: true,
                url: "../api/users/logout"
            }).success(function (data, status, headers, config) {
                window.location.href = "../user/login";
            }).error(function (data, status, headers, config) {
                ShowErrModal(data, status);
            });
        });
    };

    // 检测该登录用户是否有访问当前点击模块系统的权限
    $scope.chkAccess = function (moduleUUID) {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/users/access/" + moduleUUID
        }).success(function (data, status, headers, config) {
            $scope.Access = data;
            if (!$scope.Access.CanRead) {
                ShowModal(1, "你没有该系统的访问权限！");
                return;
            } else {
                // 重置系统导航图片为非高亮状态
                resetModuleImg();
                // 让选中图片高亮显示
                highLightModuleImg(moduleUUID);
                // 根据UUID获取模块信息
                getModule(moduleUUID);
                $('#pointContainer').removeClass('nav-right');
                $('#pointContainer').addClass('nav-right-tog');
                $('.nav-left').css("display", 'block');
                $(".menu_body a").each(function () {
                    $(this).removeClass("a-highLight");
                });
            }
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }

    // 重置系统导航图片为非高亮状态
    function resetModuleImg() {
        $(".system").find("img").each(function () {
            var img = $(this);
            var imgSrc = img.attr("src");
            imgSrc = imgSrc.replace(/on/, "off");
            img.attr("src", imgSrc);
        });
    }

    // 高亮显示图片
    function highLightModuleImg(uuid) {
        var img = $("#" + uuid).find("img");
        var imgSrc = img.attr("src");
        imgSrc = imgSrc.replace(/off/, "on");
        img.attr("src", imgSrc);
    }

    // 根据UUID获取模块信息
    function getModule(uuid) {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/modules/" + uuid
        }).success(function (data, status, headers, config) {
            $scope.ModuleItem = data;
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }

    // 在页面显示点位信息
    $scope.displayPoints = function (ModuleID, Floor, GroupName, index) {
        $scope.ModuleID = ModuleID;
        $scope.Floor = Floor;
        $("#floor" + GroupName + index).siblings("a").removeClass("a-highLight");
        console.log($("#floor" + GroupName + index));
        $("#floor" + GroupName + index).addClass("a-highLight");
        getSysSetting();
    }

    //左侧楼层组的样式控制
    $scope.displayGroup = function (index) {
        console.log(index);
        if ($("#group" + index).next("div").css("display")== "none") {
            $("#group" + index).next("div").css("display","block");
            $("#group" + index).removeClass("current");
            $("#group" + index).addClass("h3_highLight");   
        } else {      
            $("#group" + index).next("div").css("display","none");
            $("#group" + index).addClass("current");
        }
        $("#group" + index).parent().siblings("div").children("h3").removeClass("h3_highLight");
        $("#group" + index).parent().siblings("div").children("h3").addClass("current");
        $("#group" + index).parent().siblings("div").children("div").css("display", "none");
        $("#group" + index).parent().siblings("div").children("div").children("a").removeClass("a-highLight");
    }

    // 获取系统设置信息
    function getSysSetting() {
        var params = {
            "ModuleID": $scope.ModuleID
        };
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/settings/system",
            params: params
        }).success(function (data, status, headers, config) {
            var timeInterval = data.RefreshInterval;
            draggable = data.IsDraggable;
            getPoints();
            if (data.IsRefresh) {
                setInterval(getPoints, timeInterval);
            }
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }

    // 获取指定模块系统的相关点位
    function getPoints() {
        var params = {
            "ModuleID": $scope.ModuleID,
            "Floor": $scope.Floor
        };

        $http({
            method: "get",
            withCredentials: true,
            url: "../api/points/factors",
            params: params
        }).success(function (data, status, headers, config) {
            var context = { floor: $scope.Floor, data: data };
            var myTemplate = Handlebars.compile($("#template").html());
            $('#pointContainer').html(myTemplate(context));
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }
});