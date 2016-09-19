var rootUrl = "http://localhost:3567/";
//var rootUrl = "http://192.168.0.110/";
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
                        $(this).parent("li").children("a").each(function () {
                            var id = $(this).attr("id");
                            if (id != undefined && id.indexOf("building") >= 0) {
                                $(this).attr("flag", "0");
                            }
                        });
                    });
                }

                if (a.attr("class").indexOf("inactive") >= 0) {
                    a.parent("li").children("ul").slideDown(b.Speed,
                    function () {
                        a.removeAttr("class");
                        a.addClass("active");
                        var id = a.attr("id");
                        if (id != undefined && id.indexOf("building") >= 0) {
                            a.attr("flag", "2");
                            a.removeAttr("style");
                            a.siblings(".sub_cate_box").css("display", "none");
                        }
                    });
                }

                if (a.attr("class").indexOf("inactive") < 0) {
                    a.removeAttr("class");
                    a.addClass("inactive");
                    a.parent("li").children("ul").slideUp(b.Speed, function () {
                        var id = a.attr("id");
                        if (id != undefined && id.indexOf("building") >= 0) {
                            a.attr("flag", "0");
                        }
                    });
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

    var buildTypeCharts = new Array();
    var buildEpCharts = new Array();
    var buildFunds = new Array();
    $scope.buildArrears = new Array();
    // 获取建筑信息列表
    function getBuildingInfo() {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/cachebuildings"
        }).success(function (data, status, headers, config) {
            $scope.BuildingItems = data;
            for (var i = 0; i < data.length; i++) {
                buildTypeCharts[i + 1] = null;
            }
            for (var i = 0; i < data.length; i++) {
                buildEpCharts[i + 1] = null;
            }
            for (var i = 0; i < data.length; i++) {
                buildFunds[i + 1] = null;
            }
            for (var i = 0; i < data.length; i++) {
                $scope.buildArrears[i + 1] = null;
            }
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
                var origin_color = "#e7ecea";
                var over_color = "#ADD8E6";
                var click_color = "#19c68b";
                $scope.BuildingMouseOver = function (index, buildingNo) {
                    var a_building = $("#building" + index);
                    var popDiv = $("#popDiv" + index);
                    var flag = a_building.attr("flag");
                    if (flag == "0") {
                        a_building.attr("flag", "1");
                        a_building.css("background-color", over_color);
                        popDiv.css("display", "block");
                        drawBuildingTypeStatis(index, buildingNo);
                        drawBuildMonthEpStatis(index, buildingNo);
                        getBuildingRealtimeFund(index, buildingNo);
                        getWorseRooms(index, buildingNo);
                    }

                    popDiv.mouseleave(function () {
                        a_building.attr("flag", "0");
                        a_building.css("background-color", origin_color);
                        $(this).css("display", "none");
                    });

                    popDiv.mouseover(function () {
                        a_building.attr("flag", "0");
                        a_building.css("background-color", over_color);
                        $(this).css("display", "block");
                    });
                }

                $scope.BuildingMouseLeave = function (index) {
                    var a_building = $("#building" + index);
                    var popDiv = $("#popDiv" + index);
                    var flag = a_building.attr("flag");
                    if (flag == "1") {
                        a_building.attr("flag", "0");
                        a_building.css("background-color", origin_color);
                        popDiv.css("display", "none");
                    }
                }
            });
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    };

    function drawBuildingTypeStatis(index, buildingNo) {
        var typeChart = buildTypeCharts[index];
        if (typeChart == null) {
            // 各设备类型能耗对比图
            typeChart = echarts.init(document.getElementById('buildingType' + index));
            typeChart.clear();
            typeChart.showLoading({
                text: '正在努力的读取各设备类型数据...',
            });
            buildingTypeStatis(typeChart, buildingNo);
            buildTypeCharts[index] = typeChart;
        }
    }

    function drawBuildMonthEpStatis(index, buildingNo) {
        var buildEpChart = buildEpCharts[index];
        if (buildEpChart == null) {
            // 建筑历史能耗时间对比图
            buildEpChart = echarts.init(document.getElementById('buildingMonthEp' + index));
            buildEpChart.clear();
            buildEpChart.showLoading({
                text: '正在努力的读取建筑历史能耗时间数据...',
            });
            buildMonthEpStatis(buildEpChart, buildingNo);
            buildEpCharts[index] = buildEpChart;
        }
    }

    function getBuildingRealtimeFund(index, buildingNo) {
        var buildFund = buildFunds[index];
        if (buildFund == null) {
            $http({
                method: "get",
                withCredentials: true,
                url: "../api/buildrealtimefundstatis/" + buildingNo
            }).success(function (data, status, headers, config) {
                buildFunds[index] = data;
                $("#balance" + index).text(data.TotalBalance);
                $("#expand" + index).text(data.TotalExpend);
                $("#percent" + index).text(data.Percent);
            }).error(function (data, status, headers, config) {
                ShowErrModal(data, status);
            });
        }
    }

    function getWorseRooms(index, buildingNo) {
        var buildArrear = $scope.buildArrears[index];
        if (buildArrear == null) {
            var params = {
                "BuildingNo": buildingNo,
                "PageIndex": 1,
                "PageSize": 10
            };
            $http({
                method: "get",
                withCredentials: true,
                url: "../api/bills/arrearsrooms",
                params: params
            }).success(function (data, status, headers, config) {
                $scope.buildArrears[index] = data.Items;
            }).error(function (data, status, headers, config) {
                ShowErrModal(data, status);
            });
        }
    }

    function buildingTypeStatis(typeChart, buildingNo) {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/buildingtypestatis/" + buildingNo
        }).success(function (data, status, headers, config) {
            typeChart.hideLoading();
            var option = {
                title: {
                    text: '设备类型能耗对比图',
                    x: 'center'
                },
                legend: {
                    orient: 'vertical',
                    left: 'left',
                    data: data.DeviceTypes
                },
                toolbox: {
                    show: true,
                    feature: {
                        mark: { show: true }
                    }
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                series: [
                    {
                        name: '设备类型能耗',
                        type: 'pie',
                        center: ['60%', '55%'],
                        data: (function () {
                            var result = [];
                            for (var index in data.DeviceTypes) {
                                var type = data.DeviceTypes[index];
                                result.push({
                                    value: data.Values[index],
                                    name: type
                                });
                            }
                            return result;
                        })(),
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };

            typeChart.setOption(option);
        }).error(function (data, status, headers, config) {
            ShowErrModal(data, status);
        });
    }

    function buildMonthEpStatis(buildEpChart, buildingNo) {
        $http({
            method: "get",
            withCredentials: true,
            url: "../api/buildmonthstatis/" + buildingNo
        }).success(function (data, status, headers, config) {
            buildEpChart.hideLoading();
            var option = {
                title: {
                    text: '历史能耗数据趋势图',
                    x: 'center'
                },
                toolbox: {
                    show: true,
                    feature: {
                        mark: { show: true },
                        magicType: { show: true, type: ['line', 'bar'] }
                    }
                },
                calculable: false,                          // 是否启用拖拽重计算特性，默认关闭
                tooltip: {                                  // 气泡提示配置
                    trigger: 'item',                        // 触发类型，默认数据触发，可选为：'axis','item'
                },
                xAxis: [                                    // 直角坐标系中横轴数组
                    {
                        type: 'category',                   // 坐标轴类型，横轴默认为类目轴，数值轴则参考yAxis说明
                        data: data.Timelines
                    }
                ],
                yAxis: [                                    // 直角坐标系中纵轴数组
                    {
                        type: 'value'                       // 坐标轴类型，纵轴默认为数值轴，类目轴则参考xAxis说明
                    }
                ],
                series: [
                    {
                        name: '月度耗能',
                        type: 'line',
                        barWidth: '60%',
                        smooth: true,
                        data: data.Values
                    }
                ]
            };
            buildEpChart.setOption(option);
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
                url: "../api/admins/logout"
            }).success(function (data, status, headers, config) {
                window.location.href = "../admin/login";
            }).error(function (data, status, headers, config) {
                ShowErrModal(data, status);
            });
        });
    };
});