﻿var $url = '/pages/settings/adminPassword';
var $pageTypeAdmin = 'admin';
var $pageTypeUser = 'user';

var data = utils.initData({
  pageType: utils.getQueryString('pageType'),
  userId: parseInt(utils.getQueryString('userId') || '0'),
  adminInfo: null,
  password: null,
  confirmPassword: null
});

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url + '?userId=' + $this.userId).then(function (response) {
      var res = response.data;

      $this.adminInfo = res.value;

      if (!$this.pageType && $this.userId === 0) {
        $this.pageAlert = {
          type: 'warning',
          html: '您的密码已过期，请更改登录密码'
        };
      }
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  submit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '?userId=' + this.userId, {
      password: this.password
    }).then(function (response) {
      var res = response.data;

      swal({
        toast: true,
        type: 'success',
        title: "密码更改成功！",
        showConfirmButton: false,
        timer: 3000
      }).then(function () {
        if ($this.pageType == $pageTypeAdmin) {
          $this.btnReturnClick();
        } else if ($this.pageType == $pageTypeUser) {
          top.location.reload(true);
        } else {
          top.location.href = '../';
        }
      });
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    this.pageAlert = null;

    var $this = this;
    if (this.adminInfo.id > 0 && this.password != this.confirmPassword) {
      return;
    }
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  },

  btnReturnClick: function () {
    location.href = 'admin.cshtml';
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});