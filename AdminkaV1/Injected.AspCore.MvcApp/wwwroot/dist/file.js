"use strict";
//// <reference path="typings/jquery/jquery.d.ts" />
var Usaprl2;
(function (Usaprl2) {
    var WorkflowManager = /** @class */ (function () {
        function WorkflowManager(operationName) {
            this.logBuffer = [];
            this.operationName = operationName;
            this.correlationToken = this.NewGuid();
        }
        WorkflowManager.prototype.NewGuid = function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        };
        ;
        WorkflowManager.prototype.ProcessVoid = function (action) {
            var workflow = new Workflow(this.operationName, this.correlationToken);
            // todo: analize action like
            //var logstack = function () {
            //    var stack = (arguments != null && arguments.callee != null && (<any>arguments.callee).trace) ? (<any>arguments.callee).trace() : null;
            //    this.logBuffer.push(stack);
            //}
            try {
                //todo: get more information from f
                action(workflow);
                // todo: why not  f.call(invoc);
            }
            catch (ex) {
                var joinedInfo = this.logBuffer.join('\n');
                workflow.LogError(ex, joinedInfo);
                workflow.ShowErrorDialogJs(ex, joinedInfo);
            }
        };
        WorkflowManager.prototype.Process = function (action) {
            var workflow = new Workflow(this.operationName, this.correlationToken);
            try {
                return action(workflow);
            }
            catch (ex) {
                var joinedInfo = this.logBuffer.join('\n');
                workflow.LogError(ex, joinedInfo);
                workflow.ShowErrorDialogJs(ex, joinedInfo);
            }
        };
        return WorkflowManager;
    }());
    Usaprl2.WorkflowManager = WorkflowManager;
    //class WorkflowDeferredManager<T>{
    //    constructor() {
    //    }
    //    public when<T>(...deferreds: Array<T | JQueryPromise<T>/* as JQueryDeferred<T> */>): JQueryPromise<T> {
    //        var dfd = new WorkflowDeferred<T>();
    //        return dfd;
    //    }
    //}
    /*
    class WorkflowDeferred<T> implements JQueryDeferred<T>{
        private deffered: JQueryDeferred<T>;
        private defferedPrototype;
        constructor() {
            this.deffered = <JQueryDeferred<T>>new (<any>$).Deferred();
            var defferedPrototype = Object.getPrototypeOf(this.deffered);
        }

        public then<T>(doneFilter: (value?: T, ...values: any[]) => T | JQueryPromise<T>, failFilter?: (...reasons: any[]) => any, progressFilter?: (...progression: any[]) => any);
        public then(doneFilter: (value?: T, ...values: any[]) => void,
            failFilter?: (...reasons: any[]) => any,
            progressFilter?: (...progression: any[]) => any): JQueryPromise<void> {
            return this.defferedPrototype.then.apply(this, doneFilter, failFilter, progressFilter);
            //return this.deffered.then(doneFilter, failFilter, progressFilter);
        }

        public always(alwaysCallback1?: JQueryPromiseCallback<any> | JQueryPromiseCallback<any>[], ...alwaysCallbacksN: Array<JQueryPromiseCallback<any> | JQueryPromiseCallback<any>[]>): JQueryDeferred<T> {
            return this.deffered.always(alwaysCallback1, ...alwaysCallbacksN);
        }
        public done(doneCallback1?: JQueryPromiseCallback<T> | JQueryPromiseCallback<T>[], ...doneCallbackN: Array<JQueryPromiseCallback<T> | JQueryPromiseCallback<T>[]>): JQueryDeferred<T> {
            var d = this.deffered.done(doneCallback1, ...doneCallbackN);
            return d;
        }
        public fail(failCallback1?: JQueryPromiseCallback<any> | JQueryPromiseCallback<any>[], ...failCallbacksN: Array<JQueryPromiseCallback<any> | JQueryPromiseCallback<any>[]>): JQueryDeferred<T> {
            return this.deffered.fail(failCallback1, ...failCallbacksN);
        }

        public state(): string {
            return this.deffered.state();
        }

        public progress(progressCallback1?: JQueryPromiseCallback<any> | JQueryPromiseCallback<any>[], ...progressCallbackN: Array<JQueryPromiseCallback<any> | JQueryPromiseCallback<any>[]>): JQueryDeferred<T> {
            return this.deffered.progress(progressCallback1);
        }
        public notify(value?: any, ...args: any[]): JQueryDeferred<T> {
            return this.deffered.notify(value, args);
        }
        public notifyWith(context: any, value?: any[]): JQueryDeferred<T> {
            return this.deffered.notifyWith(context, value);
        }
        public reject(value?: any, ...args: any[]): JQueryDeferred<T> {
            return this.deffered.reject(value, args);
        }
        public rejectWith(context: any, value?: any[]): JQueryDeferred<T> {
            return this.deffered.rejectWith(context, value);
        }
        public resolve(value?: T, ...args: any[]): JQueryDeferred<T> {
            return this.deffered.rejectWith(value, args);
        }
        public resolveWith(context: any, value?: T[]): JQueryDeferred<T> {
            return this.deffered.rejectWith(context, value);
        }
        public promise(target?: any): JQueryPromise<T> {
            return this.deffered.promise(target);
        }
        public pipe(doneFilter?: (x: any) => any, failFilter?: (x: any) => any, progressFilter?: (x: any) => any): JQueryPromise<any> {
            return this.deffered.pipe(doneFilter, failFilter, progressFilter);
        }
    }
    */
    var Workflow = /** @class */ (function () {
        function Workflow(operationName, correlationToken) {
            this.logErrorUrl = '/Error/LogBrowserMessage';
            this.logPerfUrl = '/Error/WriteBrowserPerfomanceCounter';
            this.OperationName = operationName;
            this.CorrelationToken = correlationToken;
        }
        /*
        public LogAjaxError(errMessage: string, xhr: XMLHttpRequest, responseJSON: Usap.RemeltLevel2V1.Contract.ExceptionDetails, textStatus: string, errorThrown: string, stack: string) {

            if (errMessage == null) return;
            if (responseJSON != null && responseJSON.IsUser) {
                return;
            }
            var errMessagePack = errMessage + "; textStatus:" + textStatus + "; errorThrown" + errorThrown + "; xhr:" + xhr.responseText;

            var fullMessage = errMessagePack;
            if (stack != null) fullMessage += "\n  at " + (stack == "" ? "(stack unachived)" : stack);
            var logErrorUrl = this.logErrorUrl;
            var correlationToken = this.CorrelationToken;
            var operationName = this.OperationName;
            // send error message
            var ajaxSettings: JQueryAjaxSettings = {
                type: 'POST',
                url: logErrorUrl,
                cache: false,
                headers: { "X-CorrelationToken": correlationToken },
                data: { message: fullMessage, isError: true, operationName: operationName, correlationToken: correlationToken }
            }
            $.ajax(ajaxSettings);
        }*/
        Workflow.prototype.LogError = function (ex, stack) {
            //if (ex == null) return;
            //var url = ex.fileName != null ? ex.fileName : document.location;
            //if (stack == null && ex.stack != null) stack = ex.stack;
            //// format output
            //var out = ex.message != null ? ex.name + ": " + ex.message : ex;
            //out += ": at document path '" + url + "'.";
            //if (stack != null) out += "\n  at " + (stack == "" ? "(unachived)" : stack);
            //var logErrorUrl = this.logErrorUrl;
            //var correlationToken = this.CorrelationToken;
            //var operationName = this.OperationName;
            //// send error message
            //$.ajax({
            //    type: 'POST',
            //    url: logErrorUrl,
            //    cache: false,
            //    headers: { "X-CorrelationToken": correlationToken },
            //    data: { message: out, isError: true, operationName: operationName, correlationToken: correlationToken }
            //});
        };
        /*
        public LogMessage(msg: string) {
            if (msg == null)
                return;

            if (console != null)
                console.log(msg);

            var logErrorUrl = this.logErrorUrl;
            var correlationToken = this.CorrelationToken;
            var operationName = this.OperationName;

            $.ajax({
                type: 'POST',
                url: logErrorUrl,
                cache: false,
                headers: { "X-CorrelationToken": correlationToken },
                data: { message: String(msg), isError: false, operationName: operationName, correlationToken: correlationToken }
            });
        }

        public LogPerf(counterName, value) {
            if (counterName == null) return;

            var logPerfUrl = this.logPerfUrl;
            var correlationToken = this.CorrelationToken;
            var operationName = this.OperationName;

            $.ajax({
                type: 'POST',
                url: logPerfUrl,
                cache: false,
                headers: { "X-CorrelationToken": correlationToken },
                data: { 'counterName': counterName, 'value': value }
            });
        }

        public AjaxFail(title, xhr: XMLHttpRequest, responseJSON: Usap.RemeltLevel2V1.Contract.ExceptionDetails, textStatus, errorThrown): JQueryPromise<any> {
            this.LogAjaxError(title, xhr, responseJSON, textStatus, errorThrown,
                (arguments != null && arguments.callee != null && (<any>arguments.callee).trace) ? (<any>arguments.callee).trace() : null);
            if (title == null)
                title = "Error"
            return this.ShowErrorDialog(xhr, responseJSON, title);
        }

        //public ajax<T>(): WorkflowDeferredManager<T> {
        //    var workflowDeferredManager =  new WorkflowDeferredManager();
        //    return workflowDeferredManager;
        //}

        // TODO: http://www.svlada.com/override-jquery-ajax-handler/
        // TODO: http://stackoverflow.com/questions/17582239/overriding-the-jquery-ajax-success


        public ajax<T>(ajaxPromise: JQueryPromise<any>, done: AjaxCallback<T>, title: string, userError: AjaxError = null): JQueryPromise<any> {
            var promise = $.when(ajaxPromise)
                .then(
                (ajaxData: T, textStatus: string, XHR: XMLHttpRequest, responseJSON: any) => {
                    try {
                        done(ajaxData, textStatus, XHR)
                    } catch (ex) {
                        var details = (done != null) ? "Body: " + String(done).substring(0, 512) + "..." : null;
                        this.LogError(ex, details);
                        var promise = this.ShowErrorDialogJs(ex, details).done(x => { ; });
                        return promise;
                    }
                },
                (XHR: XMLHttpRequest, textStatus, errorThrown, responseJSON) => {
                    var responceJsonHandled = XHR.hasOwnProperty("responseJSON") ? XHR["responseJSON"] : responseJSON;
                    if (userError != null && responceJsonHandled && responceJsonHandled.IsUser)
                        userError(responceJsonHandled.ExceptionMessage);
                    else
                        this.AjaxFail(title, XHR, responceJsonHandled, textStatus, errorThrown);
                }
                );
            return promise;
        }

        public ajaxVanilla<T>(url: string, params: any, done: AjaxCallback<T>, title: string, useBlockUi: boolean): JQueryPromise<any> {
            var t0 = performance.now();
            if (useBlockUi)
                this.blockUi();
            var ajaxPromise = this.createAjaxLoadDataPromise<T>(url, params);
            if (useBlockUi)
                ajaxPromise.always(() => $.unblockUI());
            var uiSynchronizedPromise = $.when(ajaxPromise)
                .then(
                (ajaxData: T, textStatus: string, XHR: XMLHttpRequest, responseJSON: any) => {
                    try {
                        var tX = XHR.getResponseHeader("X-RL2Server-Duration");
                        var t1 = performance.now();
                        done(ajaxData, textStatus, XHR)
                        var t2 = performance.now();
                        var message = (tX != null ? "" : tX + " | ") + (t1 - t0) + " | " + (t2 - t0);
                        this.LogPerf(this.OperationName + ".ajaxVanilla", message);
                    } catch (ex) {
                        var details = (done != null) ? "Body: " + String(done).substring(0, 512) + "..." : null;
                        this.LogError(ex, details);
                        var promise = this.ShowErrorDialogJs(ex, details).done(x => {; });
                        return promise;
                    }
                },
                (XHR: XMLHttpRequest, textStatus, errorThrown, responseJSON) =>
                    this.AjaxFail(title, XHR, XHR.hasOwnProperty("responseJSON") ? XHR["responseJSON"] : responseJSON, textStatus, errorThrown)
                );
            return uiSynchronizedPromise;
        }

        public createAjaxLoadDataPromise<T>(url: string, params: any): JQueryPromise<any> {
            var deffered = jQuery.Deferred<any>();
            var xhr = new XMLHttpRequest();
            xhr.open("POST", url, true);
            xhr.setRequestHeader("Content-type", "application/json; charset=utf-8");
            xhr.setRequestHeader("X-CorrelationToken", this.CorrelationToken);

            xhr.onload = () => {
                if (xhr.status >= 200 && xhr.status < 400) {
                    var responseJSON = JSON.parse(xhr.response);
                    deffered.resolve(responseJSON, "", xhr);
                } else {
                    var responseJSON = JSON.parse(xhr.response);
                    deffered.reject(xhr, xhr.statusText, xhr.response, responseJSON);
                }
            };

            xhr.onerror = (ev) => {
                deffered.reject(xhr, xhr.status, xhr.statusText);
            };
            var paramsText = JSON.stringify(params);
            xhr.send(paramsText);
            return deffered;
        }

        public blockUi() {
            jQuery.blockUI({
                message: $("<div>Wait a moment..</div>"),
                css: {
                    fontSize: "400%",
                    fontWeight: "900",
                    color: "white",
                    background: "none",
                    border: "none"
                },
                overlayCSS: {
                    backgroundColor: "rgba(58, 124, 158, 1.2)"
                }
            });
        }

        // TODO: http://habrahabr.ru/post/112960/ - search for saveContact ,
        // somehihng how to organize dialog save
        private ShowErrorDialog(xhr: XMLHttpRequest, responseJSON: Usap.RemeltLevel2V1.Contract.ExceptionDetails, title: string): JQueryPromise<any> {
            var correlationToken = this.CorrelationToken;
            var code = xhr.status;
            var template =
                $('<div><div>'
                    + (responseJSON == null ?
                        '<b>Correlation Token: </b><span class="erdCorrelationToken"></span><br/>'
                        + '<b>Code: </b><span class="erdCode"></span><br/>'
                        :
                        ('<b>Server Message: </b><span class="erdMessage"></span><br />'
                            + '<b>Exception Type: </b><span class="erdExceptionType"></span><br />'
                            + '<b>Exception Message: </b><span class="erdExceptionMessage"></span><br />'
                            + '<b>Controller: </b><span class="erdController"></span><br />'
                            + '<b>Action: </b><span class="erdAction"></span><br/>'
                            + '<b>Correlation Token: </b><span class="erdCorrelationToken"></span><br/>'
                            + '<br/><b>Stack:</b>'
                            + '<pre  class="erdStackTrace"></pre>'))
                    + (responseJSON != null ? '' : '<br/><b>Serialised to string:</b>'
                        + '<pre class="erdSerialised"></pre>')
                    + '</div></div></div>');
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
                        var serialised = JSON.stringify(xhr)
                        template.find('.erdSerialised').text(serialised);
                    } else {
                        var serialised = JSON.stringify(xhr)
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
            }
            else {
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
                message = techTitle + ". " + message
            }
            var wholeTitle = '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> ' + title;
            return this.ShowExpandableDialog(wholeTitle, message, template);
        }*/
        Workflow.prototype.ShowErrorDialogJs = function (ex, bufferText) {
            //var template =
            //    $('<div><div><p>'
            //        + '<b>Exception Message: </b><span class="erdExceptionMessage"></span><br />'
            //        + '<b>Correlation Token: </b><span class="erdCorrelationToken"></span></p><div>'
            //        + '<b>Stack:</b>'
            //        + '<pre class="erdStackTrace"></pre>'
            //        + '<b>Serialised to string:</b>'
            //        + '<pre class="erdSerialised"></pre>'
            //        + '</div></div></div>');
            //template.find('.erdExceptionMessage').text(ex.description != null ? ex.description : ex);
            //template.find('.erdCorrelationToken').text(this.CorrelationToken);
            //template.find('.erdSerialised').text(String(ex) + '\n' + bufferText);
            //template.find('.erdStackTrace').text((ex.stack != null) ? ex.stack : "");
            //var message: string = "It looks as though we've not covered something in on our system. Please contact the system administrator.";
            //var title: string = '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> ' + 'JavaScript Exception';
            //return this.ShowExpandableDialog(title, message, template);
        };
        return Workflow;
    }());
    Usaprl2.Workflow = Workflow;
})(Usaprl2 || (Usaprl2 = {}));
//# sourceMappingURL=file.js.map