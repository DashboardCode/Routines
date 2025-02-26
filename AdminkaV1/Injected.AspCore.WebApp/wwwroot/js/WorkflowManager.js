import "core-js/modules/es.symbol.description.js";
import "core-js/modules/es.regexp.exec.js";
import "core-js/modules/es.string.replace.js";
//import * as $ from 'jquery';

export default class WorkflowManager {
  constructor(workflowName) {
    this.workflowName = workflowName;
    this.ErrorDialog = new ErrorDialog($('#modalAppliactionErrorDialog'));
    this.BlockDialog = new BlockDialog($('#blockAppliactionDialog'));
    this.ToastFactory = new ToastFactory($('footer'), $('.toast'));
  }
  ProcessVoid(operationName, action) {
    var workflow = new Workflow(this.workflowName + "." + operationName, this.ErrorDialog);
    var logBuffer = [];
    // todo: analize action like
    //var logstack = function () {
    //    var stack = (arguments != null && arguments.callee != null && (<any>arguments.callee).trace) ? (<any>arguments.callee).trace() : null;
    //    this.logBuffer.push(stack);
    //}

    try {
      //todo: get more information from f
      action(workflow, () => {
        $("body").css("cursor", "progress");
        this.BlockDialog.Block();
      }, () => {
        $("body").css("cursor", "default");
        this.BlockDialog.Unblock();
      });
      // todo: why not  f.call(invoc);
    } catch (ex) {
      //workflow.LogError(ex, joinedInfo);
      var erorDetails = ErorDetailsTools.ConvertExceptionErorDetails(ex, logBuffer /**/, workflow.CorrelationToken);
      this.ErrorDialog.Show(erorDetails);
    }
  }

  //public Process<T>(action: Action<T>){
  //    var workflow = new Workflow(this.operationName, this.correlationToken);
  //    try {

  //        return action(workflow);
  //    } catch (ex) {
  //        var joinedInfo = this.logBuffer.join('\n');
  //        //workflow.LogError(ex, joinedInfo);
  //        workflow.ShowErrorDialogJs(ex, joinedInfo);
  //    }
  //}
}
class ErorDetails {
  constructor(showAdvanced, title, message, htmlMessage, correlationToken, aspRequestId) {
    this.correlationToken = correlationToken;
    this.title = title;
    this.showAdvanced = showAdvanced;
    this.htmlMessage = htmlMessage;
    this.message = message;
    this.aspRequestId = aspRequestId;
  }
}

//class ExceptionDetails {

//    public isAdminPrivilege: boolean;
//    public aspRequestId: string;
//    public htmlMessage: string; 
//    public message: string;

//    constructor(isAdminPrivilege: boolean, aspRequestId: string, htmlMessage: string, message: string) {
//        this.isAdminPrivilege = isAdminPrivilege;
//        this.htmlMessage = htmlMessage;
//        this.message = message;
//        this.aspRequestId = aspRequestId;
//    }
//}

export class Workflow {
  //private logErrorUrl = '/Error/LogBrowserMessage';
  //private logPerfUrl  = '/Error/WriteBrowserPerfomanceCounter';

  constructor(operationName, errorDialog) {
    this.OperationName = operationName;
    this.CorrelationToken = this.NewGuid();
    this.ErrorDialog = errorDialog;
  }
  NewGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = Math.random() * 16 | 0,
        v = c === 'x' ? r : r & 0x3 | 0x8;
      return v.toString(16);
    });
  }
  //public LogAjaxError(errMessage: string, xhr: XMLHttpRequest, responseJSON: ExceptionDetails, textStatus: string, errorThrown: string, stack: string) {

  //    if (errMessage == null) return;
  //    if (responseJSON != null && responseJSON.IsUser) {
  //        return;
  //    }
  //    var errMessagePack = errMessage + "; textStatus:" + textStatus + "; errorThrown" + errorThrown + "; xhr:" + xhr.responseText;

  //    var fullMessage = errMessagePack;
  //    if (stack != null) fullMessage += "\n  at " + (stack == "" ? "(stack unachived)" : stack);
  //    var logErrorUrl = this.logErrorUrl;
  //    var correlationToken = this.CorrelationToken;
  //    var operationName = this.OperationName;
  //    // send error message
  //    var ajaxSettings: JQueryAjaxSettings = {
  //        type: 'POST',
  //        url: logErrorUrl,
  //        cache: false,
  //        headers: { "X-CorrelationToken": correlationToken },
  //        data: { message: fullMessage, isError: true, operationName: operationName, correlationToken: correlationToken }
  //    }
  //    $.ajax(ajaxSettings);
  //}

  //public LogError(ex: any, stack: string|null) {
  //    if (ex == null) return;

  //    var url = ex.fileName != null ? ex.fileName : document.location;
  //    if (stack == null && ex.stack != null) stack = ex.stack;

  //    // format output
  //    var out = ex.message != null ? ex.name + ": " + ex.message : ex;
  //    out += ": at document path '" + url + "'.";
  //    if (stack != null) out += "\n  at " + (stack == "" ? "(unachived)" : stack);
  //    var logErrorUrl = this.logErrorUrl;
  //    var correlationToken = this.CorrelationToken;
  //    var operationName = this.OperationName;
  //    // send error message
  //    $.ajax({
  //        type: 'POST',
  //        url: logErrorUrl,
  //        cache: false,
  //        headers: { "X-CorrelationToken": correlationToken },
  //        data: { message: out, isError: true, operationName: operationName, correlationToken: correlationToken }
  //    });
  //}

  //public LogMessage(msg: string) {
  //    if (msg == null)
  //        return;

  //    if (console != null)
  //        console.log(msg);

  //    var logErrorUrl = this.logErrorUrl;
  //    var correlationToken = this.CorrelationToken;
  //    var operationName = this.OperationName;

  //    $.ajax({
  //        type: 'POST',
  //        url: logErrorUrl,
  //        cache: false,
  //        headers: { "X-CorrelationToken": correlationToken },
  //        data: { message: String(msg), isError: false, operationName: operationName, correlationToken: correlationToken }
  //    });
  //}

  //public LogPerf(counterName, value) {
  //    if (counterName == null) return;
  //    var logPerfUrl = this.logPerfUrl;
  //    var correlationToken = this.CorrelationToken;
  //    var operationName = this.OperationName;

  //    $.ajax({
  //        type: 'POST',
  //        url: logPerfUrl,
  //        cache: false,
  //        headers: { "X-CorrelationToken": correlationToken },
  //        data: { 'counterName': counterName, 'value': value }
  //    });
  //}

  //public AjaxFail(title, xhr: XMLHttpRequest, responseJSON: ExceptionDetails, textStatus, errorThrown): JQuery.Promise<any> {
  //    //this.LogAjaxError(title, xhr, responseJSON, textStatus, errorThrown,
  //    //    (arguments != null && arguments.callee != null && (<any>arguments.callee).trace) ? (<any>arguments.callee).trace() : null);
  //    if (title == null)
  //        title = "Error"
  //    return this.ShowErrorDialog(xhr, title);
  //}

  //public ajax<T>(): WorkflowDeferredManager<T> {
  //    var workflowDeferredManager =  new WorkflowDeferredManager();
  //    return workflowDeferredManager;
  //}

  // TODO: http://www.svlada.com/override-jquery-ajax-handler/
  // TODO: http://stackoverflow.com/questions/17582239/overriding-the-jquery-ajax-success

  HandlePromise(ajaxPromise, done, title, userError) {
    if (userError === void 0) {
      userError = null;
    }
    var promise = $.when(ajaxPromise).then((ajaxData, textStatus, XHR, responseJSON) => {
      try {
        done(ajaxData, textStatus, XHR);
      } catch (ex) {
        var details = done != null ? "Body: " + String(done).substring(0, 512) + "..." : "";
        var logBuffer = [details];
        //this.LogError(ex, details);
        var erorDetails = ErorDetailsTools.ConvertExceptionErorDetails(ex, logBuffer, this.CorrelationToken);
        this.ErrorDialog.Show(erorDetails);
      }
    }, (XHR, textStatus, errorThrown, responseJSON) => {
      var responceJsonHandled = XHR.hasOwnProperty("responseJSON") ? XHR["responseJSON"] : responseJSON;
      if (userError != null && responceJsonHandled && responceJsonHandled.IsUser) userError(responceJsonHandled.ExceptionMessage);else {
        var erorDetails = ErorDetailsTools.ConvertXhrToErorDetails(XHR, title, this.CorrelationToken);
        this.ErrorDialog.Show(erorDetails);
      }
    });
    return promise;
  }

  //public ajaxVanilla<T>(url: string, params: any, done: AjaxCallback<T>, title: string, useBlockUi: boolean): JQuery.Promise<any> {
  //    var t0 = performance.now();
  //    if (useBlockUi)
  //        this.blockUi();
  //    var ajaxPromise = this.createAjaxLoadDataPromise<T>(url, params);
  //    //if (useBlockUi)
  //    //    ajaxPromise.always(() => $.unblockUI());
  //    var uiSynchronizedPromise = $.when(ajaxPromise)
  //        .then(
  //        (ajaxData: T, textStatus: string, XHR: XMLHttpRequest, responseJSON: any) => {
  //            try {
  //                var tX = XHR.getResponseHeader("X-RL2Server-Duration");
  //                var t1 = performance.now();
  //                done(ajaxData, textStatus, XHR)
  //                var t2 = performance.now();
  //                var message = (tX != null ? "" : tX + " | ") + (t1 - t0) + " | " + (t2 - t0);
  //                //this.LogPerf(this.OperationName + ".ajaxVanilla", message);
  //            } catch (ex) {
  //                var details = (done != null) ? "Body: " + String(done).substring(0, 512) + "..." : null;
  //                //this.LogError(ex, details);
  //                var promise = this.ShowErrorDialogJs(ex, details).done(x => {; });
  //                return promise;
  //            }
  //        },
  //        (XHR: XMLHttpRequest, textStatus, errorThrown, responseJSON) =>
  //            this.AjaxFail(title, XHR, XHR.hasOwnProperty("responseJSON") ? XHR["responseJSON"] : responseJSON, textStatus, errorThrown)
  //        );
  //    return uiSynchronizedPromise;
  //}

  //public createAjaxLoadDataPromise<T>(url: string, params: any): JQuery.Deferred<any> {
  //    var deffered = jQuery.Deferred<any>();
  //    var xhr = new XMLHttpRequest();
  //    xhr.open("POST", url, true);
  //    xhr.setRequestHeader("Content-type", "application/json; charset=utf-8");
  //    xhr.setRequestHeader("X-CorrelationToken", this.CorrelationToken);

  //    xhr.onload = () => {
  //        if (xhr.status >= 200 && xhr.status < 400) {
  //            var responseJSON = JSON.parse(xhr.response);
  //            deffered.resolve(responseJSON, "", xhr);
  //        } else {
  //            var responseJSON = JSON.parse(xhr.response);
  //            deffered.reject(xhr, xhr.statusText, xhr.response, responseJSON);
  //        }
  //    };

  //    xhr.onerror = (ev) => {
  //        deffered.reject(xhr, xhr.status, xhr.statusText);
  //    };
  //    var paramsText = JSON.stringify(params);
  //    xhr.send(paramsText);
  //    return deffered;
  //}

  //public blockUi() {
  //    //jQuery.blockUI({
  //    //    message: $("<div>Wait a moment..</div>"),
  //    //    css: {
  //    //        fontSize: "400%",
  //    //        fontWeight: "900",
  //    //        color: "white",
  //    //        background: "none",
  //    //        border: "none"
  //    //    },
  //    //    overlayCSS: {
  //    //        backgroundColor: "rgba(58, 124, 158, 1.2)"
  //    //    }
  //    //});
  //}

  // TODO: http://habrahabr.ru/post/112960/ - search for saveContact ,
  // somehihng how to organize dialog save
  //private ShowErrorDialog(xhr: XMLHttpRequest, responseJSON: ExceptionDetails, title: string): JQuery.Promise<any> {
  //    var correlationToken = this.CorrelationToken;
  //    var code = xhr.status;
  //    var template =
  //        $('<div><div>'
  //            + (responseJSON == null ?
  //                '<b>Correlation Token: </b><span class="erdCorrelationToken"></span><br/>'
  //                + '<b>Code: </b><span class="erdCode"></span><br/>'
  //                :
  //                ('<b>Server Message: </b><span class="erdMessage"></span><br />'
  //                    + '<b>Exception Type: </b><span class="erdExceptionType"></span><br />'
  //                    + '<b>Exception Message: </b><span class="erdExceptionMessage"></span><br />'
  //                    + '<b>Controller: </b><span class="erdController"></span><br />'
  //                    + '<b>Action: </b><span class="erdAction"></span><br/>'
  //                    + '<b>Correlation Token: </b><span class="erdCorrelationToken"></span><br/>'
  //                    + '<br/><b>Stack:</b>'
  //                    + '<pre  class="erdStackTrace"></pre>'))
  //            + (responseJSON != null ? '' : '<br/><b>Serialised to string:</b>'
  //                + '<pre class="erdSerialised"></pre>')
  //            + '</div></div></div>');
  //    template.find('.erdCorrelationToken').text(correlationToken);
  //    if (responseJSON != null) {
  //        template.find('.erdMessage').text(responseJSON.Message);
  //        template.find('.erdExceptionType').text(responseJSON.ExceptionType);
  //        template.find('.erdExceptionMessage').text(responseJSON.ExceptionMessage);
  //        template.find('.erdController').text(responseJSON.Controller);
  //        template.find('.erdAction').text(responseJSON.Action);
  //        console.log("e1");
  //        template.find('.erdStackTrace').text(responseJSON.TechInfo);

  //    } else {
  //        if (JSON != null && JSON.stringify != null) {
  //            var serialised = '';
  //            if (xhr.responseText != null && xhr.responseText.indexOf("<!DOCTYPE html") == 0) {
  //                // TODO: add indents (it is impossible to parse and get body but text can be improoved)
  //                var serialised = JSON.stringify(xhr)
  //                template.find('.erdSerialised').text(serialised);
  //            } else {
  //                var serialised = JSON.stringify(xhr)
  //                template.find('.erdSerialised').text(serialised);
  //            }

  //            template.find('.erdCode').text(xhr.status);
  //        }
  //    }
  //    var details = template.html();

  //    var message = "";
  //    var techTitle = "";
  //    if (responseJSON != null && responseJSON.IsUser == true) {
  //        //title = "Prohibited behaviour";
  //        message = responseJSON.ExceptionMessage;
  //    }
  //    else {
  //        var messageMap = {
  //            '400': "Server understood the request, but request content was invalid. 95% of the time this is because of a problem on the client system e.g. there is something unstable in the web application running the Web browser. Please contact system Administrator.",
  //            '401': "Access to the URL resource requires user authentication which has not yet been provided or which has been provided but failed authorization tests.",
  //            '403': "Forbidden resource can't be accessed. Web server gives this response without providing any reason.",
  //            '404': "The requested resource not found.",
  //            '500': "It looks as though we've not covered something in on our system. Please contact the system administrator.",
  //            '503': "The Web server is currently unable to handle your request due to a temporary overloading or maintenance of the server."
  //        };

  //        var titleMap = {
  //            '400': "Bad request error (400)",
  //            '401': "Unauthorized access (401)",
  //            '403': "Forbidden (403)",
  //            '404': "Not found (404)",
  //            '500': "Internal Server Error (500)",
  //            '503': "Service unavailable (503)"
  //        };

  //        techTitle = titleMap[code];
  //        message = messageMap[code];
  //        if (techTitle == null) {
  //            techTitle = "Programming Mistake";
  //            message = messageMap["500"];
  //        }
  //        message = techTitle + ". " + message
  //    }
  //    var wholeTitle = '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> ' + title;
  //    return this.ShowExpandableDialog(wholeTitle, message, template);
  //}

  //private ShowExpandableDialog(title: string, message: string, template: JQuery): JQuery.Promise<any> {
  //    var defered = $.Deferred();
  //    var x = (result) => defered.resolve(result);

  //    $('#errorDialogModal').modal('show');
  //    //var dialog = BootstrapDialog.alert({
  //    //    type: BootstrapDialog.TYPE_DANGER,
  //    //    closable: true,
  //    //    title: title,
  //    //    message: message,
  //    //    callback: (result) => defered.resolve(result)
  //    //})

  //    var details = template.html();
  //    //if (details == null) {
  //    //    dialog.getModalFooter().hide();
  //    //}
  //    //else {
  //        //var $q = dialog.getModalFooter().html(
  //        //    '<div class="span4 collapse-group">'
  //        //    + '<a class="btn btn-danger collapse-button" data-toggle="collapse">View details &raquo;</a>'
  //        //    + '<div class="collapse" style="margin-top:1em;text-align:left;">' + details + '</div>'
  //        //    + '</div>')
  //        //    .css('text-align', 'left');
  //        //$q.find('.collapse-button').on('click',
  //        //    () => {
  //        //        $q.find('.collapse').collapse('toggle')
  //        //    }
  //        //)
  //        //$q.find('.collapse')
  //        //    .on('shown.bs.collapse.in',
  //        //    (e) => {
  //        //        dialog.getModalFooter().closest('.modal-backdrop')
  //        //            .height(
  //        //            Math.max($('.modal-backdrop').height(),
  //        //                $('.bootstrap-dialog')[0].scrollHeight));
  //        //    })
  //        //    .on('hidden.bs.collapse.in',
  //        //    (e) => {
  //        //        $('.modal-backdrop').height(
  //        //            $(document).height());
  //        //    });
  //    //}

  //    return defered.promise();
  //}

  //private ShowErrorNotify(title: string) {
  //    $(document).ready(function () { dc_notify_error(title) });
  //}

  //public ShowErrorDialogDefered(xhr, title): JQuery.Promise<any> {
  //    var erorDetails = ErorDetailsTools.ConvertXhrToErorDetails(xhr, title, this.CorrelationToken);
  //    return this.ShowErrorDialogInternalDefered(erorDetails);
  //}

  //public ShowErrorDialogInternalDefered(erorDetails: ErorDetails): JQuery.Promise<any> {
  //    var defered = $.Deferred();
  //    this.ErrorDialog.Show(erorDetails);
  //    this.ErrorDialog.OnClosed(defered);
  //    return defered.promise();
  //}
}
class ErrorDialog {
  constructor($errodDialog) {
    this.$errodDialog = $errodDialog;
  }
  Show(erorDetails) {
    this.$errodDialog.find('.adminka-error-title').text(erorDetails.title);
    if (erorDetails.correlationToken) {
      this.$errodDialog.find('.adminka-error-text-field-correlation-token').text(erorDetails.correlationToken);
      this.$errodDialog.find('.adminka-error-text-field-correlation-token').parent().show();
    } else this.$errodDialog.find('.adminka-error-text-field-correlation-token').parent().hide();
    if (erorDetails.showAdvanced) {
      this.$errodDialog.find('.adminka-error-advanced').show();
      this.$errodDialog.find('.adminka-error-text-field-asp-request').text(erorDetails.aspRequestId || "");
      this.$errodDialog.find('.adminka-error-text-field-message').hide();
      this.$errodDialog.find('.adminka-error-details')[0].innerHTML = erorDetails.htmlMessage || "";
    } else {
      this.$errodDialog.find('.adminka-error-advanced').hide();
      this.$errodDialog.find('.adminka-error-text-field-message').show();
      this.$errodDialog.find('.adminka-error-text-field-message').text(erorDetails.message || "");
    }
    this.$errodDialog.modal('show');
  }
  OnClosed(defered) {
    this.$errodDialog.on('hidden.bs.modal', function (e) {
      defered.resolve(e);
    });
  }
}
class ToastFactory {
  constructor($footer, $toast) {
    this.$footer = $footer;
    this.$toast = $toast;
  }
  Show(message) {
    this.$toast.find('.toast-header strong').text(message);
    this.$toast.toast('show');
  }
  CreateToast() {
    var template = '<div class="toast" data-delay="1000" data-autohide="false" role="alert" aria-live="assertive" aria-atomic="true">' + '<div class="toast-header">' +
    //'<img src="..." class="rounded mr-2" alt="...">'+
    //'<strong class="mr-auto">Bootstrap</strong>'+
    '<small class="text-muted">just now</small>' + '<button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">' + '<span aria-hidden="true">&times;</span>' + '</button>' + '</div>' + '<div class="toast-body">See? Just like this.</div>' + '</div>';
  }
}
class BlockDialog {
  constructor($blockDialog) {
    this.$blockDialog = $blockDialog;
  }
  Block() {
    this.$blockDialog.modal({
      backdrop: 'static',
      keyboard: false
    });
  }
  Unblock() {
    this.$blockDialog.modal('hide');
  }
  OnClosed(defered) {
    this.$blockDialog.on('hidden.bs.modal', function (e) {
      defered.resolve(e);
    });
  }
}
const StatusConstants = {
  MessageMap: {
    '400': "Server understood the request, but request content was invalid. 95% of the time this is because of a problem on the client system e.g. there is something unstable in the web application running the Web browser. Please contact system Administrator.",
    '401': "Access to the URL resource requires user authentication which has not yet been provided or which has been provided but failed authorization tests.",
    '403': "Forbidden resource can't be accessed. Web server gives this response without providing any reason.",
    '404': "The requested resource not found.",
    '500': "It looks as though we've not covered something in on our system. Please contact the system administrator.",
    '503': "The Web server is currently unable to handle your request due to a temporary overloading or maintenance of the server."
  },
  TitleMap: {
    '400': "Bad request error (400)",
    '401': "Unauthorized access (401)",
    '403': "Forbidden (403)",
    '404': "Not found (404)",
    '500': "Internal Server Error (500)",
    '503': "Service unavailable (503)"
  }
};
export const ErorDetailsTools = {
  ConvertXhrToErorDetails(xhr, title, localCorrelationToken) {
    console.log(xhr);
    var isUserMessage = false;
    if (xhr.status === 403) isUserMessage = true;
    var showAdvanced = false;
    if (xhr.responseJSON && xhr.responseJSON.isAdminPrivilege && !isUserMessage) {
      showAdvanced = true;
    }
    var message = xhr.responseText;
    if (xhr.responseJSON == null && (xhr.status === 400 || xhr.status === 401 || xhr.status === 403 || xhr.status === 404 || xhr.status === 500 || xhr.status === 503)) {
      message = StatusConstants.TitleMap[xhr.status] + ". " + StatusConstants.MessageMap[xhr.status];
    }
    if (xhr.responseJSON && xhr.responseJSON.message) message = xhr.responseJSON.message;
    var aspRequestId = null;
    if (xhr.responseJSON && xhr.responseJSON.aspRequestId) aspRequestId = xhr.responseJSON.aspRequestId;
    var correlationToken = xhr.getResponseHeader('X-CorrelationToken') || localCorrelationToken;
    var htmlMessage = "";
    if (xhr.responseJSON && xhr.responseJSON.htmlMessage) htmlMessage = xhr.responseJSON.htmlMessage;
    var erorDetails = new ErorDetails(showAdvanced, title, message, htmlMessage, correlationToken, aspRequestId);
    return erorDetails;
  },
  ConvertExceptionErorDetails(ex, logBuffer, localCorrelationToken) {
    var showAdvanced = true;
    var title = 'JavaScript Exception';
    var details = ex.description != null ? ex.description : ex;
    var bufferText = "";
    if (logBuffer) {
      bufferText = logBuffer.join('\n');
    }
    details = details + '\n\n' + String(ex) + '\n' + bufferText;
    details = details + '\n\n' + (ex.stack != null) ? ex.stack : "";
    var htmlMessage = $('span').text(details)[0].innerHTML;
    var erorDetails = new ErorDetails(showAdvanced, title, null, htmlMessage, localCorrelationToken, null);
    return erorDetails;
  }
};