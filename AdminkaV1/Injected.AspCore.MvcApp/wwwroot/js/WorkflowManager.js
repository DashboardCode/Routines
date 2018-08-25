function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

/// <reference path="../node_modules/@types/jquery/index.d.ts" />
var WorkflowManager =
/*#__PURE__*/
function () {
  function WorkflowManager(operationName) {
    _classCallCheck(this, WorkflowManager);

    this.logBuffer = [];
    this.operationName = operationName;
    this.correlationToken = this.NewGuid();
  }

  _createClass(WorkflowManager, [{
    key: "NewGuid",
    value: function NewGuid() {
      return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c === 'x' ? r : r & 0x3 | 0x8;
        return v.toString(16);
      });
    }
  }, {
    key: "ProcessVoid",
    value: function ProcessVoid(action) {
      var workflow = new Workflow(this.operationName, this.correlationToken); // todo: analize action like
      //var logstack = function () {
      //    var stack = (arguments != null && arguments.callee != null && (<any>arguments.callee).trace) ? (<any>arguments.callee).trace() : null;
      //    this.logBuffer.push(stack);
      //}

      try {
        //todo: get more information from f
        action(workflow); // todo: why not  f.call(invoc);
      } catch (ex) {
        var joinedInfo = this.logBuffer.join('\n');
        workflow.LogError(ex, joinedInfo);
        workflow.ShowErrorDialogJs(ex, joinedInfo);
      }
    }
  }, {
    key: "Process",
    value: function Process(action) {
      var workflow = new Workflow(this.operationName, this.correlationToken);

      try {
        return action(workflow);
      } catch (ex) {
        var joinedInfo = this.logBuffer.join('\n');
        workflow.LogError(ex, joinedInfo);
        workflow.ShowErrorDialogJs(ex, joinedInfo);
      }
    }
  }]);

  return WorkflowManager;
}();

export { WorkflowManager as default };

var ExceptionDetails = function ExceptionDetails(IsUser, Message, ExceptionType, ExceptionMessage, Controller, Action, TechInfo) {
  _classCallCheck(this, ExceptionDetails);

  this.IsUser = IsUser;
  this.Message = Message;
  this.ExceptionType = ExceptionType;
  this.ExceptionMessage = ExceptionMessage;
  this.Controller = Controller;
  this.Action = Action;
  this.TechInfo = TechInfo;
};

export var Workflow =
/*#__PURE__*/
function () {
  function Workflow(operationName, correlationToken) {
    _classCallCheck(this, Workflow);

    this.logErrorUrl = '/Error/LogBrowserMessage';
    this.logPerfUrl = '/Error/WriteBrowserPerfomanceCounter';
    this.OperationName = operationName;
    this.CorrelationToken = correlationToken;
  }

  _createClass(Workflow, [{
    key: "LogAjaxError",
    value: function LogAjaxError(errMessage, xhr, responseJSON, textStatus, errorThrown, stack) {
      if (errMessage == null) return;

      if (responseJSON != null && responseJSON.IsUser) {
        return;
      }

      var errMessagePack = errMessage + "; textStatus:" + textStatus + "; errorThrown" + errorThrown + "; xhr:" + xhr.responseText;
      var fullMessage = errMessagePack;
      if (stack != null) fullMessage += "\n  at " + (stack == "" ? "(stack unachived)" : stack);
      var logErrorUrl = this.logErrorUrl;
      var correlationToken = this.CorrelationToken;
      var operationName = this.OperationName; // send error message

      var ajaxSettings = {
        type: 'POST',
        url: logErrorUrl,
        cache: false,
        headers: {
          "X-CorrelationToken": correlationToken
        },
        data: {
          message: fullMessage,
          isError: true,
          operationName: operationName,
          correlationToken: correlationToken
        }
      };
      $.ajax(ajaxSettings);
    }
  }, {
    key: "LogError",
    value: function LogError(ex, stack) {
      if (ex == null) return;
      var url = ex.fileName != null ? ex.fileName : document.location;
      if (stack == null && ex.stack != null) stack = ex.stack; // format output

      var out = ex.message != null ? ex.name + ": " + ex.message : ex;
      out += ": at document path '" + url + "'.";
      if (stack != null) out += "\n  at " + (stack == "" ? "(unachived)" : stack);
      var logErrorUrl = this.logErrorUrl;
      var correlationToken = this.CorrelationToken;
      var operationName = this.OperationName; // send error message

      $.ajax({
        type: 'POST',
        url: logErrorUrl,
        cache: false,
        headers: {
          "X-CorrelationToken": correlationToken
        },
        data: {
          message: out,
          isError: true,
          operationName: operationName,
          correlationToken: correlationToken
        }
      });
    }
  }, {
    key: "LogMessage",
    value: function LogMessage(msg) {
      if (msg == null) return;
      if (console != null) console.log(msg);
      var logErrorUrl = this.logErrorUrl;
      var correlationToken = this.CorrelationToken;
      var operationName = this.OperationName;
      $.ajax({
        type: 'POST',
        url: logErrorUrl,
        cache: false,
        headers: {
          "X-CorrelationToken": correlationToken
        },
        data: {
          message: String(msg),
          isError: false,
          operationName: operationName,
          correlationToken: correlationToken
        }
      });
    }
  }, {
    key: "LogPerf",
    value: function LogPerf(counterName, value) {
      if (counterName == null) return;
      var logPerfUrl = this.logPerfUrl;
      var correlationToken = this.CorrelationToken;
      var operationName = this.OperationName;
      $.ajax({
        type: 'POST',
        url: logPerfUrl,
        cache: false,
        headers: {
          "X-CorrelationToken": correlationToken
        },
        data: {
          'counterName': counterName,
          'value': value
        }
      });
    }
  }, {
    key: "AjaxFail",
    value: function AjaxFail(title, xhr, responseJSON, textStatus, errorThrown) {
      this.LogAjaxError(title, xhr, responseJSON, textStatus, errorThrown, arguments != null && arguments.callee != null && arguments.callee.trace ? arguments.callee.trace() : null);
      if (title == null) title = "Error";
      return this.ShowErrorDialog(xhr, responseJSON, title);
    } //public ajax<T>(): WorkflowDeferredManager<T> {
    //    var workflowDeferredManager =  new WorkflowDeferredManager();
    //    return workflowDeferredManager;
    //}
    // TODO: http://www.svlada.com/override-jquery-ajax-handler/
    // TODO: http://stackoverflow.com/questions/17582239/overriding-the-jquery-ajax-success

  }, {
    key: "ajax",
    value: function ajax(ajaxPromise, done, title) {
      var _this = this;

      var userError = arguments.length > 3 && arguments[3] !== undefined ? arguments[3] : null;
      var promise = $.when(ajaxPromise).then(function (ajaxData, textStatus, XHR, responseJSON) {
        try {
          done(ajaxData, textStatus, XHR);
        } catch (ex) {
          var details = done != null ? "Body: " + String(done).substring(0, 512) + "..." : null;

          _this.LogError(ex, details);

          var promise = _this.ShowErrorDialogJs(ex, details).done(function (x) {
            ;
          });

          return promise;
        }
      }, function (XHR, textStatus, errorThrown, responseJSON) {
        var responceJsonHandled = XHR.hasOwnProperty("responseJSON") ? XHR["responseJSON"] : responseJSON;
        if (userError != null && responceJsonHandled && responceJsonHandled.IsUser) userError(responceJsonHandled.ExceptionMessage);else _this.AjaxFail(title, XHR, responceJsonHandled, textStatus, errorThrown);
      });
      return promise;
    }
  }, {
    key: "ajaxVanilla",
    value: function ajaxVanilla(url, params, done, title, useBlockUi) {
      var _this2 = this;

      var t0 = performance.now();
      if (useBlockUi) this.blockUi();
      var ajaxPromise = this.createAjaxLoadDataPromise(url, params); //if (useBlockUi)
      //    ajaxPromise.always(() => $.unblockUI());

      var uiSynchronizedPromise = $.when(ajaxPromise).then(function (ajaxData, textStatus, XHR, responseJSON) {
        try {
          var tX = XHR.getResponseHeader("X-RL2Server-Duration");
          var t1 = performance.now();
          done(ajaxData, textStatus, XHR);
          var t2 = performance.now();
          var message = (tX != null ? "" : tX + " | ") + (t1 - t0) + " | " + (t2 - t0);

          _this2.LogPerf(_this2.OperationName + ".ajaxVanilla", message);
        } catch (ex) {
          var details = done != null ? "Body: " + String(done).substring(0, 512) + "..." : null;

          _this2.LogError(ex, details);

          var promise = _this2.ShowErrorDialogJs(ex, details).done(function (x) {
            ;
          });

          return promise;
        }
      }, function (XHR, textStatus, errorThrown, responseJSON) {
        return _this2.AjaxFail(title, XHR, XHR.hasOwnProperty("responseJSON") ? XHR["responseJSON"] : responseJSON, textStatus, errorThrown);
      });
      return uiSynchronizedPromise;
    }
  }, {
    key: "createAjaxLoadDataPromise",
    value: function createAjaxLoadDataPromise(url, params) {
      var deffered = jQuery.Deferred();
      var xhr = new XMLHttpRequest();
      xhr.open("POST", url, true);
      xhr.setRequestHeader("Content-type", "application/json; charset=utf-8");
      xhr.setRequestHeader("X-CorrelationToken", this.CorrelationToken);

      xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 400) {
          var responseJSON = JSON.parse(xhr.response);
          deffered.resolve(responseJSON, "", xhr);
        } else {
          var responseJSON = JSON.parse(xhr.response);
          deffered.reject(xhr, xhr.statusText, xhr.response, responseJSON);
        }
      };

      xhr.onerror = function (ev) {
        deffered.reject(xhr, xhr.status, xhr.statusText);
      };

      var paramsText = JSON.stringify(params);
      xhr.send(paramsText);
      return deffered;
    }
  }, {
    key: "blockUi",
    value: function blockUi() {} //jQuery.blockUI({
    //    message: $("<div>Wait a moment..</div>"),
    //    css: {
    //        fontSize: "400%",
    //        fontWeight: "900",
    //        color: "white",
    //        background: "none",
    //        border: "none"
    //    },
    //    overlayCSS: {
    //        backgroundColor: "rgba(58, 124, 158, 1.2)"
    //    }
    //});
    // TODO: http://habrahabr.ru/post/112960/ - search for saveContact ,
    // somehihng how to organize dialog save

  }, {
    key: "ShowErrorDialog",
    value: function ShowErrorDialog(xhr, responseJSON, title) {
      var correlationToken = this.CorrelationToken;
      var code = xhr.status;
      var template = $('<div><div>' + (responseJSON == null ? '<b>Correlation Token: </b><span class="erdCorrelationToken"></span><br/>' + '<b>Code: </b><span class="erdCode"></span><br/>' : '<b>Server Message: </b><span class="erdMessage"></span><br />' + '<b>Exception Type: </b><span class="erdExceptionType"></span><br />' + '<b>Exception Message: </b><span class="erdExceptionMessage"></span><br />' + '<b>Controller: </b><span class="erdController"></span><br />' + '<b>Action: </b><span class="erdAction"></span><br/>' + '<b>Correlation Token: </b><span class="erdCorrelationToken"></span><br/>' + '<br/><b>Stack:</b>' + '<pre  class="erdStackTrace"></pre>') + (responseJSON != null ? '' : '<br/><b>Serialised to string:</b>' + '<pre class="erdSerialised"></pre>') + '</div></div></div>');
      template.find('.erdCorrelationToken').text(correlationToken);

      if (responseJSON != null) {
        template.find('.erdMessage').text(responseJSON.Message);
        template.find('.erdExceptionType').text(responseJSON.ExceptionType);
        template.find('.erdExceptionMessage').text(responseJSON.ExceptionMessage);
        template.find('.erdController').text(responseJSON.Controller);
        template.find('.erdAction').text(responseJSON.Action);
        console.log("e1");
        template.find('.erdStackTrace').text(responseJSON.TechInfo);
      } else {
        if (JSON != null && JSON.stringify != null) {
          var serialised = '';

          if (xhr.responseText != null && xhr.responseText.indexOf("<!DOCTYPE html") == 0) {
            // TODO: add indents (it is impossible to parse and get body but text can be improoved)
            var serialised = JSON.stringify(xhr);
            template.find('.erdSerialised').text(serialised);
          } else {
            var serialised = JSON.stringify(xhr);
            template.find('.erdSerialised').text(serialised);
          }

          template.find('.erdCode').text(xhr.status);
        }
      }

      var details = template.html();
      var message = "";
      var techTitle = "";

      if (responseJSON != null && responseJSON.IsUser == true) {
        //title = "Prohibited behaviour";
        message = responseJSON.ExceptionMessage;
      } else {
        var messageMap = {
          '400': "Server understood the request, but request content was invalid. 95% of the time this is because of a problem on the client system e.g. there is something unstable in the web application running the Web browser. Please contact system Administrator.",
          '401': "Access to the URL resource requires user authentication which has not yet been provided or which has been provided but failed authorization tests.",
          '403': "Forbidden resource can't be accessed. Web server gives this response without providing any reason.",
          '404': "The requested resource not found.",
          '500': "It looks as though we've not covered something in on our system. Please contact the system administrator.",
          '503': "The Web server is currently unable to handle your request due to a temporary overloading or maintenance of the server."
        };
        var titleMap = {
          '400': "Bad request error (400)",
          '401': "Unauthorized access (401)",
          '403': "Forbidden (403)",
          '404': "Not found (404)",
          '500': "Internal Server Error (500)",
          '503': "Service unavailable (503)"
        };
        techTitle = titleMap[code];
        message = messageMap[code];

        if (techTitle == null) {
          techTitle = "Programming Mistake";
          message = messageMap["500"];
        }

        message = techTitle + ". " + message;
      }

      var wholeTitle = '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> ' + title;
      return this.ShowExpandableDialog(wholeTitle, message, template);
    }
  }, {
    key: "ShowErrorDialogJs",
    value: function ShowErrorDialogJs(ex, bufferText) {
      var template = $('<div><div><p>' + '<b>Exception Message: </b><span class="erdExceptionMessage"></span><br />' + '<b>Correlation Token: </b><span class="erdCorrelationToken"></span></p><div>' + '<b>Stack:</b>' + '<pre class="erdStackTrace"></pre>' + '<b>Serialised to string:</b>' + '<pre class="erdSerialised"></pre>' + '</div></div></div>');
      template.find('.erdExceptionMessage').text(ex.description != null ? ex.description : ex);
      template.find('.erdCorrelationToken').text(this.CorrelationToken);
      template.find('.erdSerialised').text(String(ex) + '\n' + bufferText);
      template.find('.erdStackTrace').text(ex.stack != null ? ex.stack : "");
      var message = "It looks as though we've not covered something in on our system. Please contact the system administrator.";
      var title = '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> ' + 'JavaScript Exception';
      return this.ShowExpandableDialog(title, message, template);
    }
  }, {
    key: "ShowExpandableDialog",
    value: function ShowExpandableDialog(title, message, template) {
      var defered = $.Deferred();

      var x = function x(result) {
        return defered.resolve(result);
      };

      $('#errorDialogModal').modal('show'); //var dialog = BootstrapDialog.alert({
      //    type: BootstrapDialog.TYPE_DANGER,
      //    closable: true,
      //    title: title,
      //    message: message,
      //    callback: (result) => defered.resolve(result)
      //})

      var details = template.html(); //if (details == null) {
      //    dialog.getModalFooter().hide();
      //}
      //else {
      //var $q = dialog.getModalFooter().html(
      //    '<div class="span4 collapse-group">'
      //    + '<a class="btn btn-danger collapse-button" data-toggle="collapse">View details &raquo;</a>'
      //    + '<div class="collapse" style="margin-top:1em;text-align:left;">' + details + '</div>'
      //    + '</div>')
      //    .css('text-align', 'left');
      //$q.find('.collapse-button').on('click',
      //    () => {
      //        $q.find('.collapse').collapse('toggle')
      //    }
      //)
      //$q.find('.collapse')
      //    .on('shown.bs.collapse.in',
      //    (e) => {
      //        dialog.getModalFooter().closest('.modal-backdrop')
      //            .height(
      //            Math.max($('.modal-backdrop').height(),
      //                $('.bootstrap-dialog')[0].scrollHeight));
      //    })
      //    .on('hidden.bs.collapse.in',
      //    (e) => {
      //        $('.modal-backdrop').height(
      //            $(document).height());
      //    });
      //}

      return defered.promise();
    }
  }, {
    key: "ShowErrorNotify",
    value: function ShowErrorNotify(title) {
      $(document).ready(function () {
        dc_notify_error(title);
      });
    }
  }]);

  return Workflow;
}();