var rootUrl = "http://localhost:3567/";
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
    /*左边伸缩四级菜单的行为*/
    (function ($) {
        $.fn.menu = function (b) {
            var c,
            item,
            b = jQuery.extend({
                Speed: 220,
                autostart: 1,
                autohide: 1
            }, b);
            c = $(this);
            item = c.children("ul").parent("li").children("a");
            item.addClass("inactive");
            function _item() {
                var a = $(this);
                if (b.autohide) {
                    a.parent().parent().find(".active").parent("li").children("ul").slideUp(b.Speed / 1.2,
                    function () {
                        $(this).parent("li").children("a").removeAttr("class");
                        $(this).parent("li").children("a").attr("class", "inactive");
                    });
                }

                if (a.attr("class").indexOf("inactive") >= 0) {
                    a.parent("li").children("ul").slideDown(b.Speed,
                    function () {
                        a.removeAttr("class");
                        a.addClass("active");
                    });
                }

                if (a.attr("class").indexOf("inactive") < 0) {
                    a.removeAttr("class");
                    a.addClass("inactive");
                    a.parent("li").children("ul").slideUp(b.Speed)
                }

            }
            item.unbind('click').click(_item);
        }
    })(jQuery);

    getUserSession();
    getBuildingInfo();

    // 获取用户信息
    function getUserSession() {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/admin"
        }).success(function (data, status, headers, config) {
            $scope.UserSession = data;
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }

    // 获取建筑信息列表
    function getBuildingInfo() {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/cachebuildings"
        }).success(function (data, status, headers, config) {
            $scope.BuildingItems = data;
            // angularjs渲染完毕之后执行的回调函数
            $scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
                $(".menu ul li").menu();
                // 高亮显示左边导航栏
                var buildingNo = getQueryString("buildingNo");
                var floor = getQueryString("floor");
                var roomNo = getQueryString("roomNo");
                if (buildingNo != null && floor != null && roomNo != null) {
                    $(".menu>ul>li>a").each(function () {
                        var a_building = $(this);
                        if (a_building.text() == buildingNo) {
                            a_building.parent("li").children("ul").slideDown(200,
                            function () {
                                a_building.removeAttr("class");
                                a_building.addClass("active");
                            });
                            a_building.parent("li").children("ul").children("li").children("a").each(function () {
                                var a_floor = $(this);
                                if ($.trim(a_floor.text()) == floor + "F") {
                                    a_floor.parent("li").children("ul").slideDown(200,
                                    function () {
                                        a_floor.removeAttr("class");
                                        a_floor.addClass("active");
                                    });

                                    a_floor.parent("li").children("ul").children("li").children("a").each(function () {
                                        var a_room = $(this);
                                        if ($.trim(a_room.text()) == roomNo) {
                                            a_room.removeAttr("class");
                                            a_room.addClass("active");
                                        }
                                    });
                                }
                            });
                        }
                    });
                }

                //鼠标移上或离开a标签时,控制背景色和弹出框  底色"#e7ecea", 转色"#ADD8E6",点击高亮色"#19c68b";
                $scope.mouseOverThing = function (index) {
                    if (typeof ($("#building" + index).attr("attrBuildingColor")) == "undefined") {
                        $("#building" + index).attr("attrBuildingColor", "1");
                    }
                    //alert($("#building" + index).attr("attrBuildingColor"));
                    var attrBuildingColor = $("#building" + index).attr("attrBuildingColor");
                    var block = $("#popDiv" + index).css("display");
                    var ulBlock = $("#building" + index).siblings("ul").css("display");
                    if (attrBuildingColor == "1") {
                        $("#building" + index).css("background-color", "#ADD8E6").attr("attrBuildingColor", "2");
                        $("#popDiv" + index).css("display", "block");
                    }
                    $(".sub_cate_box").mouseover(function () {
                        $(this).css("display", "block");
                        $(this).siblings(".area").css("background-color", "#ADD8E6").attr("attrBuildingColor", "2");
                    });
                    $(".sub_cate_box").mouseleave(function () {
                        $(this).siblings(".area").css("background-color", "#e7ecea").attr("attrBuildingColor", "1");
                        $(this).css("display", "none");
                    });
                    if (attrBuildingColor == "3" && block == "none") {
                        $(this).css("background-color", "#19c68b").attr("attrBuildingColor", "3");
                    }
                    $(document).on("click","#building"+index ,function () {
                        var attrBuildingColor = $("#building" + index).attr("attrBuildingColor");
                        var block = $("#building" + index).siblings(".sub_cate_box").css("display");
                        var ulBlock = $("#building" + index).siblings("ul").css("display");
                        if (attrBuildingColor == "2" && block == "block") {
                            $(this).css("background-color", "#19c68b").attr("attrBuildingColor", "3");
                            $(this).siblings(".sub_cate_box").css("display", "none");
                        }
                       if (attrBuildingColor == "2" && block == "block" && ulBlock == "block") {
                            $(this).css("background-color", "#19c68b").attr("attrBuildingColor", "3");
                        }
                        if (attrBuildingColor == "3" && block == "none" && ulBlock == "block") {
                            $(this).css("background-color", "#e7ecea").attr("attrBuildingColor", "1");
                        }
                        if (attrBuildingColor == "1" && block == "none" && ulBlock == "block") {
                            $(this).css("background-color", "#19c68b").attr("attrBuildingColor", "3");
                        }
                        if (attrBuildingColor == "1" && block == "none") {
                            $(this).css("background-color", "#19c68b").attr("attrBuildingColor", "3");
                        }
                    });
                    
                }

                $scope.mouseLeaveThing = function (index) {
                    var attrBuildingColor = $("#building" + index).attr("attrBuildingColor");
                    var block = $("#popDiv" + index).css("display");
                    var ulBlock = $("#building" + index).siblings("ul").css("display");
                    console.log(attrBuildingColor);
                    console.log(block);
                    console.log(ulBlock);
                    if (attrBuildingColor == "2") {
                        $("#building" + index).css("background-color", "#e7ecea").attr("attrBuildingColor", "1");
                        $("#popDiv" + index).css("display", "none");
                    }
                    if (attrBuildingColor == "3" && ulBlock == "block") {
                        $("#building" + index).css("background-color", "#19c68b").attr("attrBuildingColor","3");
                    }
                }

            });
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    };

    $scope.setPopStyle = function (index) {
            return {
                "top": 39 * index + "px",
                "left": 169 + "px"
        }
    };

    // 退出系统
    $scope.logout = function () {
        ShowConfirmModal("确定要退出系统吗？", function () {
            $http({
                method: "post",
                withCredentials: true,
                url: "../api/admins/logout"
            }).success(function (data, status, headers, config) {
                window.location.href = "../admin/login";
            }).error(function (data, status, headers, config) {
                ShowErrModal(data, status);
            });
        });
    };
});