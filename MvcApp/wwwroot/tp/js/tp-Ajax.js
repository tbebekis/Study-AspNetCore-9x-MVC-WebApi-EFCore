
//#region tp.AjaxContentType

/** Ajax content types
 @class 
 */
tp.AjaxContentType = {
    /** Json content
     * @type {string}
     */
    Json: 'application/json; charset=UTF-8',
    /** This is the default format used by HTML forms.
     * In this format, the data is encoded as key-value pairs separated by &amp; characters, with the key and value separated by = characters. 
     * For example, key1=value1&amp;key2=value2 .
     * This format is simple and efficient, but it has limitations in terms of the types of data that can be sent.
     * @type {string}
     */
    FormUrlEncoded: 'application/x-www-form-urlencoded; charset=UTF-8',
    /** This is a more flexible format, used by HTML forms, and can be used to send binary data, such as files, as well as text data.
     * In this format, the data is divided into multiple parts, each with its own set of headers.
     * Each part is separated by a boundary string, which is specified in the Content-Type header.
     * This format is more complex than application/x-www-form-urlencoded, but it allows for more types of data to be sent.
     * @type {string}
     */
    MultipartFormData: 'multipart/form-data; charset=UTF-8'
};
Object.freeze(tp.AjaxContentType);

//#endregion  

//#region tp.AjaxArgs

/**
Arguments class for the {@link tp.Ajax} class methods.
*/
tp.AjaxArgs = class {

    /**
     * Constructor.
     * Creates an arguments object for use with the {@link tp.Ajax} class methods
     * @param {object|tp.AjaxArgs} SourceArgs - Optional. A source arguments object to copy property values from. Could be a {@link tp.AjaxArgs} instance.
     */
    constructor(SourceArgs = null) {

        // default initialization
        this.Method = "POST";
        this.Url = '';

        this.Data = null;                       // the data to send
        this.UriEncodeData = true;
        this.Timeout = 0;
        this.ContentType = 'application/x-www-form-urlencoded; charset=UTF-8';
        this.Context = null;                                                    // context for calling the two callbacks
        this.AntiForgeryToken = '';                                             // used when POST-ing an html form in Microsoft MVC framework
        this.IsCrossDomain = false;
        this.OnSuccess = null;                                                  // function(Args: tp.AjaxArgs)
        this.OnFailure = null;                                                  // function(Args: tp.AjaxArgs)
        this.OnRequestHeaders = tp.AjaxOnRequestHeadersDefaultHandler;          // function(Args: tp.AjaxArgs)
        this.ResponseHandlerFunc = tp.AjaxResponseDefaultHandler;

        this.XHR = null;                        // XMLHttpRequest
        this.ErrorText = '';                    // the XMLHttpRequest.statusText in case of an error
        this.Result = false;                    // true if ajax call succeeded

        this.ResponseData = {                   // server response  
            IsSuccess: false,
            ErrorText: '',
            Packet: {}
        };

        this.Tag = null;                        // a user defined value

        // apply the specified parameteter
        SourceArgs = SourceArgs || {};

        for (var Prop in SourceArgs) {
            this[Prop] = SourceArgs[Prop];
        }
    }

    /* properties */
    /** Returns true if a POST method is specified. 
     @type {boolean}
     */
    get IsPost() {
        return tp.IsSameText('POST', this.Method);
    }
    /** Returns true if a GET method is specified.
    @type {boolean} */
    get IsGet() {
        return tp.IsSameText('GET', this.Method);
    }

    /** The XMLHttpRequest.responseText string in any case (could be null though in case of an error). <br />
     * <strong>Valid only after response from server</strong>
     @type {string}
     */
    get ResponseText() {
        return this.XHR ? this.XHR.responseText : '';
    }


    /** Returns a string representation of this instance 
     @returns {string} Returns a string representation of this instance
     */
    toString() {
        let S = `
Method: "${this.Method}"
Url: "${this.Url}"
AjaxResult: "${this.Result}"
ErrorText: "${this.ErrorText}"
ResponseText: "${this.ResponseText}"
ResponseResult: "${this.ResponseData.IsSuccess}"
ResponseErrorText: "${this.ResponseData.ErrorText}" `;

        return S;
    }
};
/** The Http method to use. Defaults to POST. 
@default POST
@type {string} */
tp.AjaxArgs.prototype.Method = "POST";
/** The url.  
 @type {string} */
tp.AjaxArgs.prototype.Url = '';
/** Represents the data to send. Defaults to null. <br />
 * In POSTs it is a plain object with one or more key/value pairs. <br />
 * In GETs can be null or empty, or a string with query parameters, e.g. <code>param1=value1&paramN=valueN</code>
 * @default null
 * @type {object} */
tp.AjaxArgs.prototype.Data = null;
/** When true, then Data is Uri-encoded. Defaults to true.
 * @default true
 * @see {@link http://stackoverflow.com/questions/18381770/does-ajax-post-data-need-to-be-uri-encoded|stackoverflow}
*/
tp.AjaxArgs.prototype.UriEncodeData = true;
/** The timeout in milliseconds. <br />
 * Defaults to zero, meaning no timeout. <br />
 * When set to a non-zero value will cause fetching to terminate after the given time has passed. 
 @default 0
 @type {number}
 */
tp.AjaxArgs.prototype.Timeout = 0;
/** The content type. Defaults to <code>application/x-www-form-urlencoded; charset=UTF-8</code>
 @default application/x-www-form-urlencoded; charset=UTF-8
 @type {string}
 */
tp.AjaxArgs.prototype.ContentType = 'application/x-www-form-urlencoded; charset=UTF-8';
/** context for calling the two callbacks  
 @default null
 @type {object}
 */
tp.AjaxArgs.prototype.Context = null;
/** A string used when POST-ing an html form in Microsoft MVC framework. Defaults to empty string.  
 @default ''
 */
tp.AjaxArgs.prototype.AntiForgeryToken = '';
/** Should be set to true when this is a cross-domain call
 * @default false
 * @type {boolean}
 */
tp.AjaxArgs.prototype.IsCrossDomain = false;
/** A <code>function(Args: tp.AjaxArgs)</code> callback function to call on success  
 @default null
 @type {function}
 */
tp.AjaxArgs.prototype.OnSuccess = null;
/** A <code>function(Args: tp.AjaxArgs)</code> callback function to call on failure
 @default null
 @type {function}
 */
tp.AjaxArgs.prototype.OnFailure = null;
/** A function(Args: tp.AjaxArgs) callback function to call in order to give the caller code a chance to add additional request headers.
 @default null
 @type {function}
 */
tp.AjaxArgs.prototype.OnRequestHeaders = null;
/**
 * A function(Args: tp.AjaxArgs) callback function. It is called just before the OnSuccess() call-back. <br />
 * Processes the response after an ajax call returns. <br />
 * The default response handler deserializes the Args.ResponseText into an object and assigns the Args.ResponseData object.
 * It assumes that the ResponseText is a json text containing an object with at least two properties: <code> { IsSuccess: boolean, ErrorText: string } </code>. <br />
 * Further on, if the Args.ResponseData contains a Packet property and that Packet property is a json text, deserializes it into an object.
 @default null
 @type {function}
 */
tp.AjaxArgs.prototype.ResponseHandlerFunc = null;

/** The {@link https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest|XMLHttpRequest} object. <br />
 * <strong>Valid only after response from server</strong>
 @type {XMLHttpRequest}
 */
tp.AjaxArgs.prototype.XHR = null;
/** The XMLHttpRequest.statusText string in case of an error. <br />
 * <strong>Valid only after response from server</strong>
 @type {string}
 */
tp.AjaxArgs.prototype.ErrorText = '';
/** True when ajax call succeeds 
 @type {boolean}
 */
tp.AjaxArgs.prototype.Result = false;
/** The response from the server it is always packaged as a C# HttpPacketResult instance. <br />
 * That is it comes as an object <code>{ IsSuccess: false, ErrorText: '', Packet: {} }</code>. <br />
 * The default response handler places the parsed json object in the Packet property of the ResponseData on success. <br />
 * <strong>Valid only after response from server</strong>
 @type {object}
 */
tp.AjaxArgs.prototype.ResponseData = { IsSuccess: false, ErrorText: '', Packet: {} };
/** The default response handler places the parsed json object in the Packet on success.
 * @type {object}
 * */
tp.AjaxArgs.prototype.Packet = null;
/** A user defined value.  
 @default null
 @type {any}
 */
tp.AjaxArgs.prototype.Tag = null;

//#endregion
 
//#region tp.Ajax Default handlers

/**
 * The default handler after an ajax call returns. <br />
 * Deserializes the Args.ResponseText into an object and assigns the Args.ResponseData object.
 * It assumes that the ResponseText is a json text containing an object with at least two properties: <code> { IsSuccess: boolean, ErrorText: string } </code>. <br />
 * Further on, if the Args.ResponseData  contains a Packet property and that Packet property is a json text, deserializes it into an object.
 * @param {tp.AjaxArgs} Args  A {@link tp.AjaxArgs} instance
 */
tp.AjaxResponseDefaultHandler = function (Args) {

    function ErrorText(Text) {
        return tp.IsString(Text) && !tp.IsBlank(Text) ? Text : "Unknown error";
    }

    if (Args.Result !== true)
        throw `Ajax network error: ${ErrorText(Args.ErrorText)}`;

    let o = JSON.parse(Args.ResponseText);
    Args.ResponseData = o;

    if (!tp.IsEmpty(o) && "IsSuccess" in o && o.IsSuccess !== true && "ErrorText" in o &&!tp.IsBlank(o.ErrorText))
        throw `Ajax operation error: ${ErrorText(o.ErrorText)}`;

    if (tp.IsValid(Args.ResponseData)) {

        // packet is a json string
        if (tp.IsString(Args.ResponseData.Packet) && !tp.IsBlank(Args.ResponseData.Packet)) {
            let JsonResult = tp.TryParseJson(Args.ResponseData.Packet);
            if (JsonResult.Result === true) {
                Args.ResponseData.Packet = JsonResult.Value;
                Args.Packet = JsonResult.Value;
            }
        }
        // packet is already an object
        else if (tp.IsValid(Args.ResponseData.Packet)) {
            Args.Packet = Args.ResponseData.Packet;
        }
        // packet is the ResponseData
        else if (tp.IsPlainObject(Args.ResponseData)) {
            Args.Packet = Args.ResponseData;
        }
    }

};

/**
 * The default handler for the OnRequestHeaders event. <br />
 * Does nothing
 * @param {any} Args
 */
tp.AjaxOnRequestHeadersDefaultHandler = function (Args) {
};

//#endregion

//#region tp.Ajax

/**
Ajax static function.
Executes ajax requests.
@param {tp.AjaxArgs} Args The passed arguments object. Should be a {@link tp.AjaxArgs} instance.
*/
tp.Ajax = function (Args) {

    /**
    Returns true if the ajax request is successful, by examining the status property.
    @see {@link https://developer.mozilla.org/en-US/docs/Web/HTTP/Status|Http status codes}
    @param {XMLHttpRequest} XHR The XMLHttpRequest object
    @returns {boolean} Returns true if the request is successful.
    */
    let Succeeded = function (XHR) { return !tp.IsEmpty(XHR) && (XHR.status === 0 || XHR.status >= 200 && XHR.status < 300); };

    let Context = Args.Context;
    let Data = Args.Data;
    let Url = window.encodeURI(Args.Url.toLowerCase());
    let Async = true;

    // ● encode data
    // see: http://stackoverflow.com/questions/18381770/does-ajax-post-data-need-to-be-uri-encoded
    if (!tp.IsEmpty(Data) && !tp.IsString(Data) && Args.UriEncodeData === true) {
        Data = tp.EncodeArgs(Data);
    }

    // ● when is a get
    if (Args.IsGet && !tp.IsEmpty(Data)) {
        Url += '?' + Data;
    }
 
    // ● create XMLHttpRequest
    let XHR = new XMLHttpRequest();
    Args.XHR = XHR;

    // ● error handler
    let OnError = function (e) {
        Args.ResponseText = XHR.responseText;

        var List = ['Ajax call failed. Url: ' + Url];

        if (tp.IsEmpty(e)) {
            List.push('Status Text: ' + XHR.statusText);
        } else if (e instanceof ProgressEvent) {
            List.push('Ajax call failed because of a failure on the network level');
        } else {
            List.push('Error Text: ' + tp.ExceptionText(e));
        }
        Args.ErrorText = List.join('\n');

        if (tp.IsFunction(Args.OnFailure))
            tp.Call(Args.OnFailure, Context, Args);
        else
            tp.Throw(Args.ErrorText);
    };


    // ● onload event
    XHR.onload = function (e) {
        if (XHR.readyState === XMLHttpRequest.DONE) {
            Args.ResponseText = XHR.responseText;
            if (Succeeded(XHR)) {
                Args.Result = true;
                tp.Call(Args.ResponseHandlerFunc, null, Args);
                tp.Call(Args.OnSuccess, Context, Args);
            } else {
                OnError(e);
            }
        }
    };

    // ● onerror event
    XHR.onerror = function (e) {
        OnError(e);
    };

    // ● execution
    try {
        XHR.open(Args.Method, Url, Async);

        if (Async) {
            XHR.timeout = Args.Timeout;
        }

        // headers
        XHR.setRequestHeader('Content-Type', Args.ContentType);
        XHR.setRequestHeader("Accept", "*/*");

        /*
        Only the following headers are allowed across origins:
        Accept
        Accept-Language
        Content-Language
        Last-Event-ID
        Content-Type
        */
        if (Args.IsCrossDomain === false)
            XHR.setRequestHeader('X-Requested-With', 'XMLHttpRequest'); // invalid in cross-domain call
   
        // this.setRequestHeader('x-my-custom-header', 'some value');
        if (!tp.IsBlank(Args.AntiForgeryToken)) {
            XHR.setRequestHeader("__RequestVerificationToken", Args.AntiForgeryToken);
        }

        tp.Call(Args.OnRequestHeaders, Context, Args);

        Data = Args.IsPost ? Data : null;

        // send         
        XHR.send(Data);
    } catch (e) {
        OnError(e);
    }
};

//#endregion

//#region tp.Ajax.Async()

/**
Executes an ajax request inside a promise.
@param {tp.AjaxArgs} Args The passed arguments object.
@returns {tp.AjaxArgs} Returns a {@link tp.AjaxArgs} {@link Promise}.
*/
tp.Ajax.Async = async function (Args) {

    let Context = Args.Context || null;
    let OnSuccess = Args.OnSuccess || null;
    let OnFailure = Args.OnFailure || null;

    // ------------------------------------------
    let ExecutorFunc = function (Resolve, Reject) {
        Args.Context = null;

        /** Called on success
         * @param  {tp.AjaxArgs} Args The passed arguments object.
         */
        Args.OnSuccess = function SuccessFunc(Args) {

            if (tp.IsFunction(OnSuccess)) {

                // if there is an onsuccess callback, then call it and check the results
                tp.Call(OnSuccess, Context, Args);

                if (!tp.IsEmpty(Args.ResponseData) && Args.ResponseData.IsSuccess === false && !tp.IsBlank(Args.ResponseData.ErrorText)) {
                    Reject(tp.ExceptionText(Args));
                } else {
                    Resolve(Args);
                }
            } else {
                Resolve(Args);
            }
        };
        Args.OnFailure = function FailureFunc(Args) {
            if (tp.IsFunction(OnFailure))
                tp.Call(OnFailure, Context, Args);
            Reject(tp.ExceptionText(Args));
        };

        tp.Ajax(Args);
    };
    // ------------------------------------------
    let Result = new Promise(ExecutorFunc);
    return Result;
};

//#endregion

//#region tp.AjaxArgs Helpers

/**
Creates and returns a tp.AjaxArgs instance for a POST communication
@param {string} Url - The url to call
@param {object} [Data=null] - Optional. The data to sent. Could be null
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
@returns {tp.AjaxArgs} Returns a tp.AjaxArgs object.
*/
tp.Ajax.PostArgs = function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    let Args = new tp.AjaxArgs();
    Args.Url = Url;
    Args.Method = 'POST';
    Args.ContentType = tp.AjaxContentType.FormUrlEncoded;   //  'application/x-www-form-urlencoded; charset=UTF-8'
    Args.Data = Data;

    Args.Context = Context;
    Args.OnSuccess = OnSuccess;
    Args.OnFailure = OnFailure;

    return Args;
};
/**
Creates and returns a tp.AjaxArgs instance for a GET communication
@param {string} Url - The url to call
@param {object} [Data=null] - Optional. The data to sent. Could be null
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
@returns {tp.AjaxArgs} Returns a tp.AjaxArgs object.
*/
tp.Ajax.GetArgs = function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    let Args = new tp.AjaxArgs();
    Args.Url = Url;
    Args.Method = 'GET';
    Args.Data = Data;

    Args.Context = Context;
    Args.OnSuccess = OnSuccess;
    Args.OnFailure = OnFailure;

    return Args;
};
/**
Creates and returns a tp.AjaxArgs instance for a POST-ing a model. The function serializes the model by calling JSON.stringify(). 
It also adjusts the Content-Type header as application/json; charset=utf-8
@param {string} Url - The url to call
@param {object} Model - The model, a plain object, to sent. The function serializes the model by calling JSON.stringify().
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
@returns {tp.AjaxArgs} Returns a tp.AjaxArgs object.
*/
tp.Ajax.ModelArgs = function (Url, Model, OnSuccess = null, OnFailure = null, Context = null) {
    let Args = new tp.AjaxArgs();
    Args.Url = Url;
    Args.Method = 'POST';
    Args.ContentType = tp.AjaxContentType.Json;     // 'application/json; charset=utf-8';

    if (!tp.IsEmpty(Model)) {
        if ('__RequestVerificationToken' in Model) {
            Args.AntiForgeryToken = Model['__RequestVerificationToken'];
            delete Model['__RequestVerificationToken'];
        }
        Args.Data = tp.IsString(Model) ? Model : JSON.stringify(Model);
    }

    Args.Context = Context;
    Args.OnSuccess = OnSuccess;
    Args.OnFailure = OnFailure;

    return Args;
};
/**
Merges a specified object with the Data part (tp.AjaxArgs.Data) of the ajax arguments. 
@param {tp.AjaxArgs} Args The tp.AjaxArgs object to use.
@param {Object} ExtraData - The object to merge with tp.AjaxArgs.Data
@returns {tp.AjaxArgs} Returns a tp.AjaxArgs object.
*/
tp.Ajax.AddExtraData = function (Args, ExtraData) {
    Args.Data = Args.Data || {};

    if (!tp.IsEmpty(ExtraData)) {
        for (var Prop in ExtraData) {
            Args.Data[Prop] = ExtraData[Prop];
        }
    }

    return Args;
};

//#endregion

//#region tp.Ajax Standard calls

/**
Executes a GET ajax request
@param {string} Url - The url to call
@param {object} [Data=null] - Optional. The data to sent. Could be null
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
*/
tp.Ajax.Get = function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    let Args = this.GetArgs(Url, Data, OnSuccess, OnFailure, Context);
    tp.Ajax(Args);
};
/**
Executes a POST ajax request
@param {string} Url - The url to call
@param {object} [Data=null] - Optional. The data to sent. Could be null
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
*/
tp.Ajax.Post = function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    let Args = tp.Ajax.PostArgs(Url, Data, OnSuccess, OnFailure, Context);
    tp.Ajax(Args);
};
/**
Executes a POST ajax request and sends a model to the server. The function serializes the model by calling JSON.stringify(). 
It also adjusts the Content-Type header as application/json; charset=utf-8
@param {string} Url - The url to call
@param {object} Model - The model, a plain object, to sent. The function serializes the model by calling JSON.stringify().
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
*/
tp.Ajax.PostModel = function (Url, Model, OnSuccess = null, OnFailure = null, Context = null) {
    let Args = tp.Ajax.ModelArgs(Url, Model, OnSuccess, OnFailure, Context);
    tp.Ajax(Args);
};

//#endregion

//#region tp.Ajax Async calls
/**
Executes a GET ajax request inside a promise
@param {string} Url - The url to call
@param {object} [Data=null] - Optional. The data to sent. Could be null
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
@returns {tp.AjaxArgs} Returns a {@link tp.AjaxArgs} {@link Promise}.
*/
tp.Ajax.GetAsync = async function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    let Args = tp.Ajax.GetArgs(Url, Data, OnSuccess, OnFailure, Context);
    return tp.Ajax.Async(Args);
};
/**
Executes a POST ajax request inside a promise
@param {string} Url - The url to call
@param {object} [Data=null] - Optional. The data to sent. Could be null
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
@returns {tp.AjaxArgs} Returns a {@link tp.AjaxArgs} {@link Promise}.
*/
tp.Ajax.PostAsync = async function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    let Args = tp.Ajax.PostArgs(Url, Data, OnSuccess, OnFailure, Context);
    return tp.Ajax.Async(Args);
};
/**
Executes a POST ajax request inside a promise and sends a model to the server. 
The function serializes the model by calling JSON.stringify(). 
It also adjusts the Content-Type header as application/json; charset=utf-8
@param {string} Url - The url to call
@param {object} Model - The model, a plain object, to sent. The function serializes the model by calling JSON.stringify().
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
@returns {tp.AjaxArgs} Returns a {@link tp.AjaxArgs} {@link Promise}.
*/
tp.Ajax.PostModelAsync = async function (Url, Model, OnSuccess = null, OnFailure = null, Context = null) {
    let Args = tp.Ajax.ModelArgs(Url, Model, OnSuccess, OnFailure, Context);
    return tp.Ajax.Async(Args);
};
/**
Executes a POST ajax request inside a promise, sending a container element as a model.
If the container element is a form and has a valid action, that action is used as Url.

This method serializes a form, or any other container, into a javascript object, by adding a property for each input, select, textarea or button child element, to that object. 
Each added property is named after child element's name or id (in this order) 

That is for a child element such as 
    &lt;input type='text' id='UserName' value='John' /&gt; 
 a property/value is added as 
    { UserName: 'John' } 

NOTE: input elements of type file or image, are INCLUDED.  
NOTE: A select element of type select-multiple generates an array property. 
@param {Element|string} ElementOrSelector - A selector or n html form or any other container element, that contains input, select, textarea and button elements.
@param {string} Url - The url to call. If it is null or empty and the container is a html form with a defined action url, then that url is used.
@param {function} [OnSuccess=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on success.
@param {function} [OnFailure=null] - Optional. A callback function(Args: tp.AjaxArgs): void to call on failure.
@param {object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
@returns {tp.AjaxArgs} Returns a {@link tp.AjaxArgs} {@link Promise}.
*/
tp.Ajax.PostContainerAsync = async function (ElementOrSelector, Url, OnSuccess = null, OnFailure = null, Context = null) {
    var el = tp.Select(ElementOrSelector);
    if (tp.IsBlank(Url) && el instanceof HTMLFormElement) {
        Url = el.action;
    }

    return tp.ContainerToModelAsync(true, el)
        .then(function (Model) {
            return tp.Ajax.PostModelAsync(Url, Model, OnSuccess, OnFailure, Context);
        });
};
/**
Executes a list of tp.AjaxArgs simultaneously, using Promise.all() and returns a promise when all items are done, or in the first rejection.
@param {boolean} ShowSpinner - True to show the global spinner while processing.
@param {tp.AjaxArgs[]} ArgsList An array of tp.AjaxArgs objects
@returns {Promise} Returns a promise after all items are processed succesfully, or in the first rejection.
*/
tp.Ajax.AllAsync = async function (ShowSpinner, ArgsList) {
    let Result = tp.Async.All(ShowSpinner, ArgsList, tp.Ajax.Async);
    return Result;
};

/**
Executes a list of tp.AjaxArgs sequentially, one by one. The next item executes only when the previous is done executing. 
A user provided break callback function can be used to interrupt the execution, just like a break statement in a loop. 
Returns a promise with the last item when all items are done, or in the first rejection.
@param {boolean} ShowSpinner - True to show the global spinner while processing.
@param {tp.AjaxArgs[]} ArgsList An array of tp.AjaxArgs objects
@param {function} [BreakFunc=null] - Optional. A callback function(Args: tp.AjaxArgs): boolean.
Returning true cancels any further execution, just like a break statement in a loop.
@returns {Promise} Returns a promise with the last item when all items are done, or in the first rejection.
*/
tp.Ajax.ChainAsync = async function (ShowSpinner, ArgsList, BreakFunc = false) {
    let Result = tp.Async.Chain(ShowSpinner, ArgsList, tp.Ajax.Async, BreakFunc);
    return Result;
};
//#endregion

//#region tp.AjaxRequest

/** Ajax content types
 @class 
 */
tp.AjaxRequestType = {
    /** A request that executes an operation on the server and may or may not return a packet.
     * @type {string}
     */
    Proc: 'Proc',
    /** A request that requests a user interface, that is HTML content.
     * A Ui request may set the an IsSingleInstance flag indicating that the Ui may exist only once.
     * @type {string}
     */
    Ui: 'Ui' 
};
Object.freeze(tp.AjaxRequestType);

/** Represents an ajax request. 
 * An AJAX request could be either a Ui or a Proc request.  
 * A Ui request may set the an IsSingleInstance flag indicating that the Ui may exist only once.
 * A Proc request may or may not return a packet.  
 * A requester may optionally set the CommandId and/or CommandName properties.
 * */
tp.AjaxRequest = class {

    /**
     * Constructor
     * @param {string} OperationName The name of the request operation.
     * @param {object} Params Optional. A user defined key-pair object with parameters.
     */
    constructor(OperationName, Params = null) {
        this.OperationName = OperationName;
        this.Params = Params || {};

        tp.AjaxRequest.Counter++;
        this.Id = tp.AjaxRequest.Counter.toString();
    }

    /** Optional. The id of the request.
     * @type {string}
     */
    Id = '';
    /** Required. The name of the request operation.
     * @type {string}
     */
    OperationName = '';
    /** Optional. A user defined key-pair object with parameters.
     * @type {object}
     */
    Params = {};

    /** The request type. Proc or Ui.
     * A Proc request, the default, may or may not return a packet.
     * A Ui request returns HTML.
     * @type {string}
     */
    Type = tp.AjaxRequestType.Proc;
    /** True when this is a single instance Ui request.
     @type {boolean}
     */
    IsSingleInstance = false;

    /**  A requester may optionally set the command Id of this request
     @type {string}
     */
    CommandId = '';
    /**  A requester may optionally set the command name of this request
     @type {string}
     */
    CommandName = '';
};
/** Id counter.
 * @param {number}  
 */
tp.AjaxRequest.Counter = 0;



/**
 * Executes an ajax request to the server and returns the Packet as it comes from server.
 * @param {tp.AjaxRequest|object|string} RequestOrOperationName Required. A {@link tp.AjaxRequest} object, or a plain object with an OperationName property, or a string denoting the operation name to execute.
 * @param {object} Params Optional. Used only when the first parameter is a string in order to create a {@link tp.AjaxRequest} object.
 * @returns {tp.AjaxArgs} Returns a {@link tp.AjaxArgs} {@link Promise}. The Packet is a property of the returned object.
 */
tp.AjaxRequest.Execute = async function (RequestOrOperationName, Params = null) {
    let Result = null;

    /** @type {tp.AjaxRequest} */
    let Request = null;
    let Url = tp.Urls.AjaxRequest;

    if (tp.IsString(RequestOrOperationName)) {
        Request = new tp.AjaxRequest(RequestOrOperationName, Params);
    }
    else if (tp.IsObject(RequestOrOperationName) && !tp.IsBlankString(RequestOrOperationName.OperationName)) {
        Request = RequestOrOperationName;
    }
    else {
        tp.Throw('Cannot execute ajax request. Invalid parameters.');
    }

    let Args = await tp.Ajax.PostModelAsync(Url, Request);
    Result = Args;

    return Result;
};
tp.AjaxRequest.ExecuteAsync = tp.AjaxRequest.Execute;


//#endregion