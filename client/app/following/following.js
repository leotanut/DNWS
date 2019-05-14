'use strict';

angular.module('followingList', ['ngRoute'])
  .component('followingList', {
    templateUrl: 'following/following.html',
    controller: ['$http', '$rootScope', function TweetListController($http, $rootScope) {
      var self = this;

      const requestOptions = {
          headers: { 'X-session': $rootScope.x_session }
        };

        self.getfollow = function getfollow(username)
        {

            const data = "peoplefollowing=" + encodeURIComponent(username);
            $http.post('http://localhost:8080/twitterapi/following/', data, requestOptions);
        }
        self.getunfollow = function getunfollow(username)
        {
            $http.defaults.headers.delete = { 'X-session': $rootScope.x_session };
            const data = "peoplefollowing=" + encodeURIComponent(username);
            $http.delete('http://localhost:8080/twitterapi/following/?' + data);
        }

      $http.get('http://localhost:8080/twitterapi/following/', requestOptions).then(function (response) {
        self.followings = response.data;
      });
    }]
});