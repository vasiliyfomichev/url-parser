﻿angular.module('UrlParser', ['tc.chartjs'])
    .controller('UrlController', function($scope, $http) {
        $scope.valid = true;
        $scope.url = null;
        $scope.title = null;
        $scope.images = [];
        $scope.words = [];
        $scope.loaded = {
            images: false,
            words: false,
        }
        $scope.working = {
            images: false,
            words: false,
            all: false
        }

        // Chart options
        $scope.chartData = {
            labels: [],
            datasets: [
                {
                    label: '10 Most used words',
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

        $scope.currentIndex = 0;

        $scope.isWorking = function() {
            return $scope.working.words || $scope.working.images;
        }

        $scope.hasDataBeenLoaded = function() {
            return $scope.loaded.all;
        }

        $scope.setCurrentSlideIndex = function(index) {
            $scope.currentIndex = index;
        };

        $scope.isCurrentSlideIndex = function(index) {
            return $scope.currentIndex === index;
        };

        $scope.prevSlide = function() {
            $scope.currentIndex = ($scope.currentIndex < $scope.images.length - 1) ? ++$scope.currentIndex : 0;
        };

        $scope.nextSlide = function() {
            $scope.currentIndex = ($scope.currentIndex > 0) ? --$scope.currentIndex : $scope.images.length - 1;
        };

        $scope.parseUrl = function(url) {
            var isValidUrl = validateUrl(url);
            if (!isValidUrl) return;
            $scope.title = "loading reports...";
            getImages(url);
            var maxResultsToShow = 10;
            getWordReport(url, maxResultsToShow);
        }

        function validateUrl(url) {
            if (typeof url === 'undefined' || !url) return false;
            return $scope.valid = true;
        };

        function getImages(url) {
            $scope.working.images = true;
            $http.get("/api/parser/images?url=" + url).success(function(data, status, headers, config) {
                $scope.images = data;
                $scope.working = false;
                $scope.loaded.images = true;
                completeDataLoad();
            }).error(function(data, status, headers, config) {
                $scope.title = "Oops... something went wrong";
                $scope.working.words = false;
            });
        }

        function getWordReport(url, maxReportSize) {
            $scope.working.words = true;
            $http.get("/api/parser/wordreport?url=" + url + "&maxReportSize=" + maxReportSize).success(function(data, status, headers, config) {
                //$scope.words = data;
                if (typeof data !== 'undefined' && data != null) {
                    for (var i = 0; i < data.length; i++) {
                        $scope.chartData.labels.push(data[i].Word.toLowerCase());
                        $scope.chartData.datasets[0].data.push(data[i].Count);
                    }
                }

                $scope.title = data.title;
                $scope.working = false;
                $scope.loaded.words = true;
                completeDataLoad();
            }).error(function(data, status, headers, config) {
                $scope.title = "Oops... something went wrong";
                $scope.working.words = false;
            });
        }

        function completeDataLoad() {
            if ($scope.working.images && $scope.working.words) {
                $scope.title = "";
            }
            if ($scope.loaded.images && $scope.loaded.words) {
                $scope.loaded.all = true;
            }
        }
    })
    .animation('.slide-animation', function() {
        return {
            addClass: function(element, className, done) {
                if (className == 'ng-hide') {
                    TweenMax.to(element, 0.5, { left: -element.parent().width(), onComplete: done });
                } else {
                    done();
                }
            },
            removeClass: function(element, className, done) {
                if (className == 'ng-hide') {
                    element.removeClass('ng-hide');

                    TweenMax.set(element, { left: element.parent().width() });
                    TweenMax.to(element, 0.5, { left: 0, onComplete: done });
                } else {
                    done();
                }
            }
        };
    });