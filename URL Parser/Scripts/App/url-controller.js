﻿angular.module('UrlParser', ['tc.chartjs', 'ngAnimate', 'ngTouch', 'ngSanitize'])
    .controller('UrlController', [
        "$scope", "$http", function($scope, $http) {
            $scope.valid = true;
            $scope.url = "";
            $scope.images = [];
            $scope.words = [];
            $scope.loaded = {
                images: false,
                words: false,
            }
            $scope.loadStarted = false;
            $scope.errors = [];
            $scope.working = {
                images: false,
                words: false,
            }
            $scope.maxReportSize = 10;
            $scope.isDataLoaded = false;
            $scope.invalidUrl = true;

            // Chart options
            $scope.chartData = {
                labels: [],
                datasets: [
                    {
                        label: $scope.maxReportSize + ' Most used words',
                        fillColor: 'rgba(151,187,205,0.5)',
                        strokeColor: 'rgba(151,187,205,0.8)',
                        highlightFill: 'rgba(151,187,205,0.75)',
                        highlightStroke: 'rgba(151,187,205,1)',
                        data: []
                    }
                ]
            }

            $scope.chartOptions = {
                // Sets the chart to be responsive
                responsive: true,

                //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
                scaleBeginAtZero: true,

                //Boolean - Whether grid lines are shown across the chart
                scaleShowGridLines: true,

                //String - Colour of the grid lines
                scaleGridLineColor: "rgba(0,0,0,.05)",

                //Number - Width of the grid lines
                scaleGridLineWidth: 1,

                //Boolean - If there is a stroke on each bar
                barShowStroke: true,

                //Number - Pixel width of the bar stroke
                barStrokeWidth: 2,

                //Number - Spacing between each of the X value sets
                barValueSpacing: 5,

                //Number - Spacing between data sets within X values
                barDatasetSpacing: 1
            };

            $scope.errorSummary = null;

            function createErrorSummary() {
                $scope.errorSummary = null;
                var errors = $scope.errors;
                if (errors.length === 0) return "";
                var errorMessage = "<ul>";
                var is404Found = false;
                for (var i = 0; i < errors.length; i++) {
                    if (errors[i].code !== 404) {
                        errorMessage += "<li><span>" + errors[i].message + "</span></li>";
                    } else {
                        if (!is404Found) errorMessage += "<li><span>" + errors[i].message + "</span></li>";
                    }
                    if (errors[i].code === 404 && !is404Found) {
                        is404Found = true;
                        break;
                    }
                }
                errorMessage += "</ul>";
                if (is404Found) {
                    errorMessage += "<span>Please try another one.</span>";
                } else {
                    errorMessage += "<span>A team of trained monkeys has been dispatched to troubleshoot. Please try a different URL.</span>";
                }
                $scope.errorSummary = errorMessage;
            }

            $scope.hasErrors = function() {
                return $scope.errors.length > 0;
            }

            $scope.showSlideshow = function() {
                var shouldShow = $scope.dataHasBeenLoaded() && $scope.images.length > 0;
                return shouldShow;
            }

            $scope.startsWithHttp = function(text) {
                if (typeof url === 'undefined' || !text)return false;
                if (text.length === 1 && text.toLowerCase().substring(0, 1) === "h") return true;
                if (text.length === 2 && text.toLowerCase().substring(0, 2) === "ht") return true;
                if (text.length === 3 && text.toLowerCase().substring(0, 3) === "htt") return true;
                if (text.length === 4 && text.toLowerCase().substring(0, 4) === "http") return true;
                if (text.length === 5 && (text.toLowerCase().substring(0, 5) === "http:" || text.toLowerCase().substring(0, 5) === "https")) return true;
                if (text.length === 6 && (text.toLowerCase().substring(0, 6) === "http:/" || text.toLowerCase().substring(0, 6) === "https:")) return true;
                if (text.length === 7 && (text.toLowerCase().substring(0, 7) === "http://" || text.toLowerCase().substring(0, 7) === "https:/")) return true;
                if (text.length >= 8 && (text.toLowerCase().substring(0, 7) === "http://" || text.toLowerCase().substring(0, 8) === "https://")) return true;
                return false;
            };

            $scope.showWordReport = function() {
                var shouldShow = $scope.dataHasBeenLoaded() && $scope.chartData.datasets[0].data.length > 0;
                return shouldShow;
            }

            $scope.isWorking = function() {
                var isWorking = ($scope.working.images && $scope.working.words);
                return isWorking;
            }

            $scope.dataHasBeenLoaded = function() {
                return $scope.isDataLoaded; // $scope.loaded.all;
            }

            //#region Slideshow


            $scope.direction = 'left';
            $scope.currentIndex = 0;

            $scope.setCurrentSlideIndex = function(index) {
                $scope.direction = (index > $scope.currentIndex) ? 'left' : 'right';
                $scope.currentIndex = index;
            };

            $scope.isCurrentSlideIndex = function(index) {
                return $scope.currentIndex === index;
            };

            $scope.prevSlide = function() {
                $scope.direction = 'left';
                $scope.currentIndex = ($scope.currentIndex < $scope.images.length - 1) ? ++$scope.currentIndex : 0;
            };

            $scope.nextSlide = function() {
                $scope.direction = 'right';
                $scope.currentIndex = ($scope.currentIndex > 0) ? --$scope.currentIndex : $scope.images.length - 1;
            };

//#endregion    

            $scope.parseUrl = function(url) {
                url = encodeURI(url);
                $scope.errors = [];
                var isValidUrl = $scope.isValidUrlEntered(url);
                if (!isValidUrl) return;
                $scope.isDataLoaded = false;
                $scope.loadStarted = true;
                resetChart();
                loadImages(url);
                loadWordReport(url, $scope.maxReportSize);
            }


            $scope.validateInputLive = function (url) {
                if (typeof url === 'undefined' || !url) {
                    return true;
                }

                if (url.length < 7) {
                    return $scope.startsWithHttp(url);
                }

                if (url.length === 7 && url.toLowerCase().substring(0, 7) === "http://") {
                    return true;
                }
                if (url.length === 8 && url.toLowerCase().substring(0, 8) === "https://") {
                    return true;
                }
                return true;
            }

            $scope.isValidUrlEntered = function (url) {
                if ($("#secondaryUrl").val() != "") return false;
                if (typeof url === 'undefined' || !url) {
                    $scope.valid = false;
                    return false;
                }

                var expression = /[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?/gi;
                var pattern = new RegExp(expression);
                if (!pattern.test(url)) {
                    $scope.valid = false;
                    $scope.invalidUrl = false;
                    return false;
                } else {
                    $scope.valid = true;
                    $scope.invalidUrl = true;
                    return true;
                }
            };

            function loadImages(url) {
                $scope.images = [];
                $scope.working.images = true;
                $http.get("/api/parser/images?url=" + url, { cache: true }).success(function(data, status, headers, config) {
                    $scope.images = data;
                    $scope.loaded.images = true;
                    $scope.working.images = false;
                    completeDataLoad();
                }).error(function(data, status, headers, config) {
                    if (status === 404) {
                        $scope.errors.push({ code: 404, message: "Unable to parse the URL. It is likely that it does not exist or has invalid markup." });
                    } else {
                        $scope.errors.push({
                            code: 500,
                            message: "Problems loading website images."
                        });
                    }
                    $scope.working.words = false;
                    $scope.loaded.images = true;
                    completeDataLoad();
                });
            }

            function resetChart() {
                $scope.chartData.labels = [];
                $scope.chartData.datasets[0].data = [];
            }

            function loadWordReport(url, maxReportSize) {
                $scope.working.words = true;
                $http.get("/api/parser/wordreport?url=" + url + "&maxReportSize=" + maxReportSize, { cache: true }).success(function(data, status, headers, config) {
                    if (typeof data !== 'undefined' && data != null) {
                        for (var i = 0; i < data.length; i++) {
                            $scope.chartData.labels.push(data[i].Word.toLowerCase());
                            $scope.chartData.datasets[0].data.push(data[i].Count);
                        }
                    };

                    $scope.loaded.words = true;
                    $scope.working.words = false;
                    completeDataLoad();
                }).error(function(data, status, headers, config) {
                    if (status === 404) {
                        $scope.errors.push({ code: 404, message: "Unable to parse the URL. It is likely that it does not exist." });
                    } else {
                        $scope.errors.push({
                            code: 500,
                            message: "Problems loading the word report."
                        });
                    }
                    $scope.working.words = false;
                    $scope.loaded.words = true;
                    completeDataLoad();
                });
            }

            function completeDataLoad() {
                if ($scope.loaded.images && $scope.loaded.words) {
                    $scope.isDataLoaded = true;
                    createErrorSummary();
                }
            }
        }
    ])
    .animation('.slide-animation', function() {
        return {
            addClass: function(element, className, done) {
                var scope = element.scope();

                if (className == 'ng-hide') {
                    var finishPoint = element.parent().width();
                    if (scope.direction !== 'right') {
                        finishPoint = -finishPoint;
                    }
                    TweenMax.to(element, 0.5, { left: finishPoint, onComplete: done });
                } else {
                    done();
                }
            },
            removeClass: function(element, className, done) {
                var scope = element.scope();

                if (className == 'ng-hide') {
                    element.removeClass('ng-hide');

                    var startPoint = element.parent().width();
                    if (scope.direction === 'right') {
                        startPoint = -startPoint;
                    }

                    TweenMax.set(element, { left: startPoint });
                    TweenMax.to(element, 0.5, { left: 0, onComplete: done });
                } else {
                    done();
                }
            }
        };
    });