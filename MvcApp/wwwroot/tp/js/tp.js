/**
* Selects an element specified by a selector.
* If Selector is a string, then it returns the first found element in document, if any, else null.
* If Selector is already an element, returns that element.
* Else returns null.
* @param {Element|string} Selector - An element or a string selector.
* @return {Element|null} Returns an element, if any, or null.
* @class
*/
var tp = function (Selector) {
    if (typeof Selector === 'string') {
        return document.querySelector(Selector);
    } else if (Selector instanceof Element) {
        return Selector;
    }

    return null;
};
 
//#region Errors

/**
 * Throws a tripous exception
 * @param {string} Message The exception message
 */
tp.Throw = function (Message) {
    let Ex = new Error(Message);
    Ex.name = 'tp-Error';
    throw Ex;
};
 
/**
Returns a text of an error for display purposes
@param {any} e - An Error, or ErrorEvent, or a string value or any other object with error information
@returns {string} Returns the error text
*/
tp.ExceptionText = function (e) {
    let SB = new tp.StringBuilder();
    let S, o;

    //---------------------------
    function HandleEvent(e) {
        o = { Message: 'Unknown error.' };
        try {
            o = {};

            o.Name = e.error && e.error.name ? e.error.name : '';
            o.Message = e.message;

            if (tp.SysConfig.DebugMode === true) {
                o.Type = e.type ? e.type : '';
                o.Number = e.error && e.error.number ? e.error.number : null;
                o.File = e.filename;
                o.Line = e.lineno;
                o.Col = e.colno;
                o.Stack = e.error && e.error.stack ? e.error.stack : '';
            }

            if (e instanceof PromiseRejectionEvent) {
                if (!tp.IsEmpty(e.reason)) {
                    if (e.reason instanceof Error) {
                        if (tp.SysConfig.DebugMode === true)
                            o.Stack = e.reason.stack;
                    } else {
                        o.Reason = e.reason.toString();
                    }
                }
            }

        } catch (e) {
            //
        }

        for (let Prop in o) {
            let v = o[Prop];
            if (v && tp.IsSimple(v)) {
                v = v.toString();
                if (!tp.IsBlank(v)) {
                    S = tp.Format('{0}: {1}', Prop, v);
                    SB.AppendLine(S);
                }
            }
        }
    }
    //---------------------------


    if (tp.IsString(e) && !tp.IsBlank(e)) {
        SB.AppendLine(e);
    } else if (e instanceof Error) {
        SB.AppendLine(e.name + ': ' + e.message);
        SB.AppendLine(e.stack || 'Stack not available.');
    } else if (e instanceof PromiseRejectionEvent) {
        HandleEvent(e);
    } else if (e instanceof ErrorEvent) {
        HandleEvent(e);
    } else if ('ErrorText' in e && !tp.IsBlank(e.ErrorText)) {
        SB.AppendLine(e.ErrorText);
    } else if (e instanceof tp.AjaxArgs) {
        if (!tp.IsBlank(e.ErrorText))
            SB.AppendLine(e.ErrorText);
        else if (!tp.IsBlank(e.ResponseData.ErrorText))
            SB.AppendLine(e.ResponseData.ErrorText);
        else
            SB.AppendLine('Unknown ajax error.');
    } else {
        try {
            S = e.toString();
            SB.AppendLine(S);
        } catch (e) {
            SB.AppendLine('Unknown error.');
        }
    }
    return SB.ToString();
};

//#endregion
 
//#region Type checking

/** Type checking function. Returns true if the specified value is null or undefined.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsEmpty = function (v) { return v === null || v === void 0; };               // null or undefined
/** Type checking function. Returns true if the specified value is not null nor undefined.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsValid = function (v) { return !(v === null || v === void 0); };

/** Type checking function. Returns true if the specified value is an object.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsObject = function (v) { return tp.IsValid(v) && typeof v === 'object'; };
/** Type checking function. Returns true if the specified value is an array.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsArray = function (v) { return Array.isArray(v); }; //  v instanceof Array || Object.prototype.toString.call(v) === '[object Array]';
/** Type checking function. Returns true if the specified value is a function.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsFunction = function (v) { return typeof v === 'function'; };
/** Type checking function. Returns true if the specified value is an arguments object.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsArguments = function (v) { return Object.prototype.toString.call(v) === "[object Arguments]"; };

/** Type checking function. Returns true if the specified value is string.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsString = function (v) { return typeof v === 'string'; };
/** Type checking function. Returns true if the specified value is number.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsNumber = function (v) { return typeof v === 'number'; };
/** Type checking function. Returns true if the specified value is integer.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsInteger = function (v) { return typeof v === 'number' && v % 1 === 0; }
/** Type checking function. Returns true if the specified value is decimal number.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsFloat = function (v) { return typeof v === 'number' && n % 1 !== 0; }
/** Type checking function. . Returns true if the specified value is boolean.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsBoolean = function (v) { return typeof v === 'boolean'; };
/** Type checking function. . Returns true if the specified value is a date object.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsDate = function (v) { return !tp.IsEmpty(v) && Object.prototype.toString.call(v) === "[object Date]"; };
/** Type checking function. Returns true if the specified value is string, number or boolean.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsPrimitive = function (v) { return typeof v === 'string' || typeof v === 'number' || typeof v === 'boolean'; };
/** Type checking function. Returns true if the specified value is string, number, boolean or date.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsSimple = function (v) { return tp.IsPrimitive(v) || tp.IsDate(v); };

/** Type checking function. Returns true if the specified value is a promise object.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsPromise = function (v) { return v instanceof Promise; };
/** Type checking function. Returns true if the specified value is a RegExp object.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsRegExp = function (v) { return Object.prototype.toString.call(v) === "[object RegExp]"; };

/** Type checking function. Returns true if the specified value is an object but NOT a DOM element.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsPlainObject = function (v) { return tp.IsObject(v) && !v.nodeType; };
/** Type checking function. Returns true if the specified value is a DOM {@link Node}.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsNode = function (v) { return v instanceof Node; };


/** Parses a json text and returns an object, array, string, number, boolean on success, else null.
 * @param {string} JsonText The json text to parse.
 * @returns {object} Returns an object, array, string, number, boolean on success, else null.
 */
tp.ParseJson = function (JsonText) {
    let Result = null;

    if (typeof JsonText === 'string' && !tp.IsBlank(JsonText)) {
        try {
            // JSON.parse returns an Object, Array, string, number, boolean, or null value corresponding to the given JSON text
            let o = JSON.parse(JsonText);
            Result = tp.IsValid(o) ? o : null;
        } catch (e) {
            //
        }
    }

    return Result;
};
/** Tries to parse a json text. <br />
 * Returns an object of type <code>{Value: object, Result: boolean}</code> where the Result is true on success and the Value is the parsed object.
 * @param {string} JsonText The json text to parse.
 * @returns {object} Returns an object of type <code>{Value: object, Result: boolean}</code> where the Result is true on success and the Value is the parsed object.
 */
tp.TryParseJson = function (JsonText) {
    let o = tp.ParseJson(JsonText);
    return {
        Value: o,
        Result: tp.IsValid(o)
    };
};
/**
 * Returns true if a specified string is a json string.
 * @param {string} Text The text to check
 * @returns {boolean} Returns true if a specified string is a json string.
 */
tp.IsJsonText = function (Text) {
    if (typeof Text === 'string' && !tp.IsBlank(Text)) {
        try {
            let o = JSON.parse(Text);
            return tp.IsValid(o);
        } catch (e) {
            //
        }
    }

    return false;
};


/** Type checking function for a DOM Node. Returns true if the specified value is a DOM attribute Node.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsAttribute = function (v) { return !!(v && v.nodeType === Node.ATTRIBUTE_NODE); };
/** Type checking function for a DOM Node. Returns true if the specified value is a DOM {@link Element}.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsElement = function (v) { return v instanceof Element; };
/** Type checking function for a DOM Node. Returns true if the specified value is a DOM {@link HTMLElement}.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsHTMLElement = function (v) { return v instanceof HTMLElement; };
/** Type checking function for a DOM Node. Returns true if the specified value is a DOM text node.
 * @param {any} v The value to check
 * @returns {boolean} Returns true if the specified value passes the check.
 */
tp.IsText = function (v) { return !!(v && v.nodeType === Node.TEXT_NODE); };

/**
Type guard function for an HTMLElement that has a name attribute.
@param {Element} v - The element to check.
@returns {boolean} -
*/
tp.IsNamedHtmlElement = function (v) { return v instanceof HTMLElement && 'name' in v; };
/**
Type guard for an element that provides the querySelector() and querySelectorAll() methods, i.e. is document or Element
@param {Element} v - The element to check.
@returns {boolean} - Returns true if the specified element provides querySelector() and querySelectorAll() methods.
*/
tp.IsNodeSelector = function (v) { return tp.IsValid(v) && 'querySelector' in v && 'querySelectorAll' in v; };
/**
Type-guard function. An element providing checkValidity() and reportValidity() methods passes this test.
@see {@link https://developer.mozilla.org/en-US/docs/Web/Guide/HTML/HTML5/Constraint_validation|Constraint validation}
@param {Element} v - The element to check.
@returns {boolean} - Returns true if the specified element provides checkValidity() and reportValidity() methods.
*/
tp.IsValidatableElement = function (v) { return !tp.IsEmpty(v) && tp.IsFunction(v['checkValidity']) && tp.IsFunction(v['setCustomValidity']); };
/**
* Type-guard, mostly for the required property of form elements
* @param {Element} v - The element to check.
* @returns {boolean} -
*/
tp.IsFormElement = function (v) { return v instanceof HTMLInputElement || v instanceof HTMLSelectElement || v instanceof HTMLTextAreaElement; };

/**
Type guard function for the Cloneable interface
* @param {Element} v - The element to check.
* @returns {boolean} -
*/
tp.IsCloneable = function (v) { return !tp.IsEmpty(v) && tp.IsFunction(v['Clone']); };
/**
Type guard function for the Assignable interface
* @param {Element} v - The element to check.
* @returns {boolean} -
*/
tp.IsAssignable = function (v) { return !tp.IsEmpty(v) && tp.IsFunction(v['Assign']); };

/**
True if a specified value is a DOM element of a certain type (e.g div, span, etc.)
@param {any} v Any value
@param {string} NodeName The node name  (e.g div, span, etc.)
@returns {boolean} Returns true if a specified value is a DOM element of a certain type (e.g div, span, etc.)
*/
tp.ElementIs = function (v, NodeName) { return tp.IsElement(v) && tp.IsSameText(v.nodeName, NodeName); };

//#endregion

//#region Class construction and inheritance

/**
Sets BaseClass as the base class of Class.
Actually is a shortcut just to avoid writing the same code lines everywhere.
@param {function} Class The class to inherit from
@param {function} BaseClass The base class
@returns {function} Returns the base class prototype
*/
tp.SetBaseClass = function (Class, BaseClass) {
    // see: http://stackoverflow.com/questions/9959727/proto-vs-prototype-in-javascript
    Class.prototype = Object.create(BaseClass.prototype);  // the prototype to be used when creating new instances
    Class.prototype.constructor = Class;                   // the function that used as the constructor
    return BaseClass.prototype;                            // return the base prototype, it is stored in a private variable inside class closures
};
/**
 * Defines a named or accessor property in a class prototype
 * @param {string} Name The name of the property
 * @param {object} Prototype The class prototype
 * @param {function} GetFunc The getter function
 * @param {function} [SetFunc] The setter function
 */
tp.Property = function (Name, Prototype, GetFunc, SetFunc = null) {

    var o = {};
    if (typeof GetFunc === 'function') {        // it is a named accessor property
        o.get = GetFunc;
        if (typeof SetFunc === 'function') {    // if not present, it effectively creates a read-only property
            o.set = SetFunc;
        }
    } else {                            // it is a named property
        o.value = GetFunc;
        o.writable = true;
    }
    o.enumerable = true;
    o.configurable = true;
    Object.defineProperty(Prototype, Name, o);
};
/**
 * Defines a constant in a class prototype
 * @param {string} Name The name of the constant
 * @param {object} Prototype The class prototype
 * @param {any} Value The value of the constant
 */
tp.Constant = function (Name, Prototype, Value) {
    var o = {
        value: Value,
        writable: false,
        enumerable: false,
        configurable: false
    };

    Object.defineProperty(Prototype, Name, o);
};

/**
Invokes a constructor and returns the new instance
@param {function} Ctor A constructor function
@param {...args} args A rest parameter
@returns {object} Returns the new instance of the specified constructor.
*/
tp.CreateInstance = function (Ctor, ...args) {
    return new Ctor(args);
};
//#endregion

//#region Reflection

/** Contains information about a property or function 
 @class
 */
tp.PropertyInfo = function () {
    this.Name = '';
    this.Signature = '';
    this.Type = '';
    this.Args = 0;
    this.HasGetter = false;
    this.HasSetter = false;
    this.IsConstructor = false;
    this.IsFunction = false;
    this.IsProperty = false;
    this.IsConfigurable = false;
    this.IsEnumerable = false;
    this.IsWritable = false;
    this.Pointer = null;
};


/**
 * Returns the property descriptor of a specified property, if any, else null.  
 Can be used also for calling inherited property getters/setters. 
 @example
// Here is how to call a base property
// NOTE: In both of the following examples, base is the base prototype
 
return tp.GetPropertyDescriptor(base, 'Name').get.call(this);      // getter call
tp.GetPropertyDescriptor(base, 'Name').set.call(this, v);          // setter call
 
 @param {object} o An object (maybe a prototype object)
 @param {string} PropName The name of the property.
 @returns {PropertyDescriptor} Returns the property descriptor or null.
 */
tp.GetPropertyDescriptor = function (o, PropName) {
    if (o !== null) {
        return o.hasOwnProperty(PropName) ?
            Object.getOwnPropertyDescriptor(o, PropName) :
            tp.GetPropertyDescriptor(Object.getPrototypeOf(o), PropName);
    }

    return null;
};
/** Returns information about a property or function
@param {object} o - The container object
@param {string} Key - The name of the member
@returns {tp.PropertyInfo} Returns an information object.
*/
tp.GetPropertyInfo = function (o, Key) {
    var PD = tp.GetPropertyDescriptor(o, Key);

    var Result = new tp.PropertyInfo();
    Result.Name = Key;
    Result.Signature = Key;


    if (PD) {
        var Pointer = o[Key];
        var ParamList;

        Result.Name = Key;
        if (tp.IsFunction(Pointer)) {
            Result.Type = 'f';
            Result.IsFunction = true;
            Result.Args = Pointer.length || 0;
            ParamList = Result.Args > 0 ? tp.GetFunctionParams(Pointer) : [];
            Result.Signature = 'function ' + Key + '(' + ParamList.join(',') + ')';

        } else if (tp.IsArray(Pointer)) {
            Result.Type = 'a';
        } else {
            Result.Type = 'o';
        }

        Result.HasGetter = Boolean(PD.get);
        Result.HasSetter = Boolean(PD.set);
        Result.IsConstructor = tp.IsSameText('constructor', Key);
        Result.IsProperty = !Result.IsFunction && !Result.IsConstructor;
        Result.IsConfigurable = PD.configurable;
        Result.IsEnumerable = PD.enumerable;
        Result.IsWritable = PD.writable === true || Result.HasSetter === true;
        Result.Pointer = Pointer;
    }

    return Result;
};
/** Returns true if a specified property is writable (provides a setter and is not read-only)
@param {object} o - The container object
@param {string} Key - The name of the member
@returns {tp.PropertyInfo} Returns true if a specified property is writable (provides a setter and is not read-only)
*/
tp.IsWritableProperty = function (o, Key) {
    let PI = tp.GetPropertyInfo(o, Key);
    return PI.HasSetter || PI.IsWritable;
};
/** Returns information about the properties and functions of a object
@param {object} o - The container object
@returns {tp.PropertyInfo[]} Returns a list of information objects.
*/
tp.GetPropertyInfoList = function (o) {
    var A = [];

    if (o) {
        var P;
        for (var Prop in o) {
            P = tp.GetPropertyInfo(o, Prop);
            if (P)
                A.push(P);
        }
    }

    return A;
};
/** Returns a descriptive text of an object
@param {object} o - The container object
@returns {string} Returns a descriptive text of an object
*/
tp.GetReflectionText = function (o) {

    var A = tp.GetPropertyInfoList(o);

    var S;
    var f = '{0} {1} {2} {3} {4} {5} {6} {7} {8}';
    var SB = new tp.StringBuilder();

    A.forEach(function (P) {
        S = tp.Format(f,
            P.Type,
            P.Args,
            P.IsConstructor ? 'c' : '_',
            P.HasGetter ? 'g' : '_',
            P.HasSetter ? 's' : '_',

            P.IsConfigurable ? 'c' : '_',
            P.IsEnumerable ? 'e' : '_',
            P.IsWritable ? 'w' : '_',
            P.Signature
        );

        SB.AppendLine(S);
    });

    S = SB.ToString();

    return S;
};
/** Returns the definition text of an object, that is the signatures of properties and functions. 
@param {object} o - The container object
@returns {string} Returns the definition text of an object, that is the signatures of properties and functions.
*/
tp.GetObjectDefText = function (o) {
    var A = tp.GetPropertyInfoList(o);

    var Constr = '';
    var Props = [];
    var Funcs = [];

    var i, ln;
    for (i = 0, ln = A.length; i < ln; i++) {
        if (A[i].IsFunction) {
            Funcs.push(A[i].Signature);
        } else if (A[i].IsConstructor) {
            Constr = A[i].Signature;
        } else {
            Props.push(A[i].Signature);
        }
    }

    Props.sort();
    Funcs.sort();

    var SB = new tp.StringBuilder();

    if (Constr !== '') {
        SB.AppendLine('constructor ' + Constr);
    }

    if (Props.length > 0) {
        SB.AppendLine();
        SB.AppendLine('// properties');
        for (i = 0, ln = Props.length; i < ln; i++) {
            SB.AppendLine(Props[i]);
        }
    }

    if (Funcs.length > 0) {
        SB.AppendLine();
        SB.AppendLine('// methods');
        for (i = 0, ln = Funcs.length; i < ln; i++) {
            SB.AppendLine(Funcs[i]);
        }
    }

    return SB.ToString();

};
/** Returns an array of the parameter names of any function passed in. 
@param {function} func The function to operate on
@returns {string[]} Returns an array of the parameter names of any function passed in.
*/
tp.GetFunctionParams = function (func) {
    // http://stackoverflow.com/questions/1007981/how-to-get-function-parameter-names-values-dynamically-from-javascript
    return (func + '').replace(/\s+/g, '')
        .replace('/[/][*][^/*]*[*][/]/g', '')           // strip simple comments  
        .split('){', 1)[0].replace(/^[^(]*[(]/, '')   // extract the parameters  
        .replace('/=[^,]+/g', '')                       // strip any ES6 defaults  
        .split(',').filter(Boolean);                  // split & filter [""]  
};
/**
 * Returns true if a specified object has a specified property (not function).
@param {object} o - The container object
@param {string} Key - The name of the member
@returns {boolean} Returns true if a specified object has a specified property (not function).
 */
tp.HasProperty = function (o, Key) {
    let PI = tp.GetPropertyInfo(o, Key);
    return PI.IsProperty == true && PI.HasSetter === true;
};
/**
 * Returns true if a specified object has a specified writable property (not function).
@param {object} o - The container object
@param {string} Key - The name of the member
@returns {boolean} Returns true if a specified object has a specified writable property (not function).
 */
tp.HasWritableProperty = function (o, Key) {
    let PI = tp.GetPropertyInfo(o, Key);
    return PI.IsProperty == true && (PI.IsWritable === true || PI.HasSetter === true);
};

/** Returns an array with all property names of an object, walking down to its hierarchy.
 * An optional call-back may exclude any of the passed property, by name.
 * NOTE: Functions are excluded. Properties starting with __ are also excluded.
 * @param {object} o The object to operate on
 * @param {function} [CanGetPropFunc=null] Optional. A <code>function CanGetPropFunc(PropName): boolean </code>. Returning false excludes the property.
 * @returns {string[]} Returns an array with all property names of an object, walking down to its hierarchy.
 */
tp.GetPropertyNames = function (o, CanGetPropFunc = null) {
    let Result = [];
    CanGetPropFunc = CanGetPropFunc || function (Prop) { return true; };
    let List;

    do {
        List = Object.getOwnPropertyNames(o);

        List.forEach((prop) => {
            if (typeof o[prop] !== 'function' && !prop.startsWith('__') && Result.indexOf(prop) === -1 && CanGetPropFunc(prop)) {
                Result.push(prop);
            }
        });

    }
    while (o = Object.getPrototypeOf(o));

    return Result;
};
//#endregion

//#region Merging objects


/**
Merges properties ONLY of objects in the Sources array to the Dest object. 
CAUTION: No overload. All argument must have values. 
@param {object} Dest - The destination object. It is returned as the Result of the function.
@param {object|any[]} Sources - The source object or an array of source objects (or arrays)
@param {boolean} [DeepMerge=true] - When DeepMerge is true, then source properties that are objects and arrays, are deeply copied to Dest. If false then only their referencies are copied to Dest.
@returns {object|any[]} - Returns the Dest object.
*/
tp.MergeProps = function (Dest, Sources, DeepMerge = true) {

    if (tp.IsEmpty(Dest))
        return null;

    if (tp.IsValid(Sources)) {

        if (!tp.IsArray(Sources)) {
            let x = Sources;
            Sources = [];
            Sources.push(x);
        }

        Sources.forEach((Source) => {
            if (tp.IsValid(Source)) {

                let PropNameList = tp.GetPropertyNames(Source);

                PropNameList.forEach((PropName) => {
                    let v = Source[PropName];

                    if (v !== Dest) {
                        if (tp.IsSimple(v)) {
                            Dest[PropName] = v;
                        }
                        else {
                            if (!DeepMerge) {
                                Dest[PropName] = v;
                            }
                            else {
                                let DestValue = Dest[PropName];
                                if (tp.IsArray(v)) {
                                    DestValue = DestValue && tp.IsArray(DestValue) ? DestValue : [];
                                    Dest[PropName] = tp.MergeProps(DestValue, [v], DeepMerge);
                                }
                                else if (tp.IsObject(v)) {
                                    DestValue = DestValue && tp.IsPlainObject(DestValue) ? DestValue : {};
                                    Dest[PropName] = tp.MergeProps(DestValue, [v], DeepMerge);
                                }
                                else if (PropName !== 'constructor') {
                                    Dest[PropName] = v;
                                }
                            }
                        }
                    }
                });
            }

        });
    }



    return Dest;
};
/**
Merges properties ONLY of objects in the Sources array to the Dest object. Returns the Dest.
It does a deep merge, that is source properties that are objects and arrays, are deeply copied to Dest.
@param {object} Dest - The destination object. It is returned as the Result of the function.
@param {object|any[]} Sources - The source object or an array of source objects (or arrays)
@returns {object|any[]} - Returns the Dest object.
*/
tp.MergePropsDeep = function (Dest, Sources) {
    return tp.MergeProps(Dest, Sources, true);
};
/**
Merges properties ONLY of the Source object to the Dest object. Returns the Dest.
@param {object} Dest - The destination object. It is returned as the Result of the function.
@param {object|any[]} Sources - The source object or an array of source objects (or arrays)
@returns {object} - Returns the Dest object.
*/
tp.MergePropsShallow = function (Dest, Sources) {
    return tp.MergeProps(Dest, Sources, false);
};

//#endregion

//#region Format, FormatNumber, FormatDateTime

/**
Formats a string the C# way <br />
Number and Date values should be passed as already formatted strings.
@example
var S = tp.Format('String: {0}, Number: {1}, Boolean: {2}', 'tripous', 789, true);
@param {string} s - The format string  
@param {any[]} ...values - The values for the format string. Number and Date values should be passed as already formatted strings. 
@returns {string} Returns the formatted string.
*/
tp.Format = function (s, ...values) {
    if (tp.IsString(s)) {
        let i, ln, Params = [];
        for (i = 1, ln = arguments.length; i < ln; i++) {
            Params.push(arguments[i]);
        }

        for (i = 0; i < Params.length; i++) {
            let regEx = new RegExp("\\{" + (i) + "\\}", "gm");
            s = s.replace(regEx, Params[i]);
        }
    }

    return s;
};
let _F = tp.Format;
/** 
 * Formats a number into a string the C# way. <br /> 
 * It uses two types of formats: Standard and Custom. <br />
   <pre>
    ------------------------------------------------------------------------------------------                      
                                    Standard Formats                                                                
    ------------------------------------------------------------------------------------------
    Char   Name        Numbers         Groups      Decimals    Examples
    ------------------------------------------------------------------------------------------
    D      Decimal     integers only   no          no          1234 ("D") -> 1234,  -1234 ("D6") -> -001234
    C      Currency    float numbers   yes         yes         123.456 ("C", en-US) -> $123.46
    F      Fixed       float numbers   no          yes         -1234.56 ("F4", en-US) -> -1234.5600
    N      Number      float numbers   yes         yes         -1234.56 ("N3", en-US) -> -1,234.560

    See: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-string
    ------------------------------------------------------------------------------------------
                                    Custom Formats
    ------------------------------------------------------------------------------------------
    0	    Zero	    Replaces the zero with the corresponding digit if one is present; otherwise, zero appears in the result string
    #	    Digit	    Replaces the "#" symbol with the corresponding digit if one is present; otherwise, no digit appears in the result string.
    .	    Decimal	    Determines the location of the decimal separator in the result string
    ,	    Group	    Inserts a localized group separator character between each group.

    Examples
    0     1234.5678 ("00000") -> 01235    0.45678 ("0.00", en-US) -> 0.46
    #     1234.5678 ("#####") -> 1235     0.45678 ("#.##", en-US) -> 0.46
    .     0.45678 ("0.00", en-US) -> 0.46
    ,     2147483647 ("##,#", en-US) -> 2,147,483,647

    See: https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings
    ------------------------------------------------------------------------------------------
  </pre>
 * @param {number} v The number to be formatted as string.
 * @param {string} format The format to use. 
 * @returns {string} Returns the formatted string.
 */
tp.FormatNumber = (v, format) => {
    if (typeof v !== 'number')
        return '';

    let o = {
        style: 'decimal'
    };

    format = format.toUpperCase();

    // standard formats
    if (['C', 'D', 'F', 'N'].indexOf(format[0]) !== -1) {

        let Decimals = format.length > 1 ? format.substring(1) : '0';
        Decimals = parseInt(Decimals, 10);

        // currency
        if (format[0] === 'C') {
            let Culture = tp.Cultures.Find(tp.CultureCode);
            Decimals = Decimals === 0 ? Culture.CurrencyDecimals : Decimals;

            o.style = 'currency';
            o.useGrouping = true;
            o.currency = Culture.CurrencyCode;
            o.currencyDisplay = 'symbol';
        }
        else {  // all others
            if (format[0] === 'D') {
                o.minimumIntegerDigits = Decimals > 0 ? Decimals : 1;
                Decimals = 0;
            } else {
                o.minimumIntegerDigits = 1;
                Decimals = Decimals > 0 ? Decimals : 2;
            }

            o.useGrouping = format[0] === 'N';
            o.minimumFractionDigits = Decimals;
            o.maximumFractionDigits = Decimals;
        }
    }
    else { // custom formats

        let Parts = format.split('.');
        let sFormatIntPart = Parts[0];
        let sFormatDecPart = Parts.length > 1 ? Parts[1] : '';
        let UseGroups = sFormatIntPart.indexOf(',') > -1;

        if (UseGroups) {
            sFormatIntPart = sFormatIntPart.replace(',', '');
        }

        o.useGrouping = UseGroups;                  // .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
        o.minimumIntegerDigits = sFormatIntPart.startsWith('0') ? sFormatIntPart.length : 1;
        o.minimumFractionDigits = sFormatDecPart.length;
        o.maximumFractionDigits = sFormatDecPart.length;
    }

    let Result = v.toLocaleString(tp.CultureCode, o);
    return Result;

};
/** Formats a number into a string.
 Decimals is the number of decimal places into string. 
 If DecimalSep or ThousandSep are not defined, the corresponging tripous globals are used.
@param {number} v - The number to be formatted as string.
@param {number} Decimals - The number of decimal places into string.
@param {character} [DecimalSep] - The decimal separator to use
@param {character} [ThousandSep] - The thousand separator to use
@returns {string} Returns the formatted string.
*/
tp.FormatNumber2 = function (v, Decimals = 0, DecimalSep = null, ThousandSep = null) {
    if (isNaN(v) || !tp.IsValid(v))
        return '';

    DecimalSep = DecimalSep || tp.DecimalSeparator;
    ThousandSep = ThousandSep || tp.ThousandSeparator;

    var S = v.toFixed(~~Decimals);

    var Parts = S.split('.');
    var NumPart = Parts[0];
    var DecPart = Parts[1] ? DecimalSep + Parts[1] : '';

    return NumPart.replace(/(\d)(?=(?:\d{3})+$)/g, '$1' + ThousandSep) + DecPart;
};
/**
Formats a Date value based on a specified format string pattern. <br />
If no format is specified the current culture date format is used.  <br />
Adapted from: https://github.com/UziTech/js-date-format/blob/master/js-date-format.js
@param {Date} v - The Date value to format.  
@param {string} [format] - Optional. The format string pattern. If not specified the current culture date format is used.
@returns {string} Returns the formatted string
*/
tp.FormatDateTime = function (v, format = null) {
    // adapted from: https://github.com/UziTech/js-date-format/blob/master/js-date-format.js

    format = format || tp.GetDateFormat();

    let Pad = function (value, length) {
        var negative = value < 0 ? "-" : "";
        var zeros = "0";
        for (var i = 2; i < length; i++) {
            zeros += "0";
        }
        return negative + (zeros + Math.abs(value).toString()).slice(-length);
    };

    let Parts = {
        date: v,
        yyyy: function () { return Parts.date.getFullYear(); },
        yy: function () { return Parts.date.getFullYear() % 100; },
        MM: function () { return Pad(Parts.date.getMonth() + 1, 2); },
        M: function () { return Parts.date.getMonth() + 1; },
        dd: function () { return Pad(Parts.date.getDate(), 2); },
        d: function () { return Parts.date.getDate(); },
        HH: function () { return Pad(Parts.date.getHours(), 2); },
        H: function () { return Parts.date.getHours(); },
        hh: function () {
            var hour = Parts.date.getHours();
            if (hour > 12) {
                hour -= 12;
            } else if (hour < 1) {
                hour = 12;
            }
            return Pad(hour, 2);
        },
        h: function () {
            var hour = Parts.date.getHours();
            if (hour > 12) {
                hour -= 12;
            } else if (hour < 1) {
                hour = 12;
            }
            return hour;
        },
        mm: function () { return Pad(Parts.date.getMinutes(), 2); },
        m: function () { return Parts.date.getMinutes(); },
        ss: function () { return Pad(Parts.date.getSeconds(), 2); },
        s: function () { return Parts.date.getSeconds(); },
        fff: function () { return Pad(Parts.date.getMilliseconds(), 3); },
        ff: function () { return Pad(Math.floor(Parts.date.getMilliseconds() / 10), 2); },
        f: function () { return Math.floor(Parts.date.getMilliseconds() / 100); },
        zzzz: function () { return Pad(Math.floor(-Parts.date.getTimezoneOffset() / 60), 2) + ":" + Pad(-Parts.date.getTimezoneOffset() % 60, 2); },
        zzz: function () { return Math.floor(-Parts.date.getTimezoneOffset() / 60) + ":" + Pad(-Parts.date.getTimezoneOffset() % 60, 2); },
        zz: function () { return Pad(Math.floor(-Parts.date.getTimezoneOffset() / 60), 2); },
        z: function () { return Math.floor(-Parts.date.getTimezoneOffset() / 60); }
    };


    let Result = [];
    let IsMatch = false;
    let FormatPart;
    let ResultPart;

    while (format.length > 0) {

        IsMatch = false;
        FormatPart;
        for (var i = format.length; i > 0; i--) {
            FormatPart = format.substring(0, i);
            if (FormatPart in Parts) {
                ResultPart = Parts[FormatPart]();
                Result.push(ResultPart);
                format = format.substring(i);
                IsMatch = true;
                break;
            }
        }

        if (!IsMatch) {
            Result.push(format[0]);
            format = format.substring(1);
        }
    }

    return Result.join("");
};
//#endregion

//#region Strings



/**
 Returns true if a specified string is null, undefined or it just contains white space chars (space, tab, etc). <br />
 Throws an exception if the specified value is other than undefined, null or string.
@param {string} v - A string value. 
@returns {boolean}  Returns true if the string is null, undefined or it just contains white space chars (space, tab, etc)
*/
tp.IsBlank = function (v) {
    if (v === void 0 || v === null)
        return true;

    if (!tp.IsString(v)) {
        tp.Throw('Can not check for null or whitespace a non-string value');
    }

    return v.trim().length === 0; //v.replace(/\s/g, '').length < 1;
};
/**
 Returns true if a specified string is null, undefined or it just contains white space chars
@param {string} v - A string value.
@returns {boolean}  Returns true if the string is null, undefined or it just contains white space chars
*/
tp.IsNullOrWhiteSpace = function (v) { return tp.IsBlank(v); };
/**
 * Returns true if a specified string is null, undefined or it just contains white space chars (space, tab, etc). <br />
 * No exception is thrown if the specified value is other than undefined, null or string.
 * @param {string} v - A string value.
 * @returns {boolean}  Returns true if the string is null, undefined or it just contains white space chars (space, tab, etc)
 */
tp.IsBlankString = function (v) {
    return (v === void 0 || v === null) || (tp.IsString(v) && v.trim().length === 0);
};

/** True if a specified character is a white space char (space, tab, etc)  
@param {character} c - A character value. 
@returns {boolean} Returns True if a specified character is a white space char (space, tab, etc)
*/
tp.IsWhitespaceChar = function (c) { return c.charCodeAt(0) <= 32; }; // return ' \t\n\r\v'.indexOf(c) === 0;
/**
Returns true if a specified text looks like html markup.
@see {@link https://stackoverflow.com/questions/15458876/check-if-a-string-is-html-or-not|stackoverflow}
@param {string} Text - The text to test.
@returns {boolean} Returns true if a specified text looks like html markup.
*/
tp.IsHtml = function (Text) { return /<[a-z][\s\S]*>/i.test(Text); };


/**
 True if two string are of the same text, case-insensitively always
@param {string} A - The first string.
@param {string} B - The second string
@returns {boolean} Returns true if the two strings are case-insensitively identicals.
*/
tp.IsSameText = function (A, B) {
    return tp.IsString(A) && tp.IsString(B) && A.toUpperCase() === B.toUpperCase();
};
/**
 True if a  sub-string is contained by another string
@param {string} Text - The string
@param {string} SubText - The sub-string, the string to search for.
@param {boolean} [CI=true] - CI (Case-Insensitive) can be true (the default) or false 
@returns {boolean} Returns true if a substring is contained in the other string.
*/
tp.ContainsText = function (Text, SubText, CI = true) {
    CI = CI === true;
    if (tp.IsString(Text) && !tp.IsBlank(Text)) {
        return CI ? Text.toLowerCase().includes(SubText.toLowerCase()) : Text.includes(SubText);
    }

    return false;
};
/**
Inserts a sub-string in another string at a specified index and returns the new string.
@param {string} SubString - The sub-string to insert.
@param {string} Text - The string
@param {number} Index - The position at the string where the sub-string should be inserted.
@returns {string} Returns the new string.
*/
tp.InsertText = function (SubString, Text, Index) {
    return [Text.slice(0, Index), SubString, Text.slice(Index)].join('');
};

 

/**
Trims a string (removes blank characters from start and end) and returns the new string.
@param {string} v - The string .
@returns {string} Returns the new string.
*/
tp.Trim = function (v) {
    return tp.IsBlank(v) ? "" : v.trim(); //v.replace(/^\s+|\s+$/g, "");
};
/**
Trims a string by removing blank characters from the start of the string and returns the new string.
@param {String} v - The string .
@returns {string} Returns the new string.
*/
tp.TrimStart = function (v) {
    return tp.IsBlank(v) ? "" : v.trimStart(); //v.replace(/^\s+/, "");
};
/**
Trims a string by removing blank characters from the end of the string and returns the new string.
@param {string} v - The string .
@returns {string}  Returns the new string.
*/
tp.TrimEnd = function (v) {
    return tp.IsBlank(v) ? "" : v.trimEnd(); //v.replace(/\s+$/, "");
};

/**
  True if a string starts with a sub-string.
 @param {string} Text - The string to check.
 @param {string} SubText - The sub-string, the string to search for.
 @param {boolean} [CI=true] - CI (Case-Insensitive) can be true (the default) or false 
 @returns {boolean} Returns true if a string starts with a sub-string.
 */
tp.StartsWith = function (Text, SubText, CI = true) {
    if (tp.IsBlank(SubText) || tp.IsBlank(Text))
        return false;

    if (tp.IsEmpty(CI)) {
        CI = true;
    }

    let S = Text.substring(0, SubText.length);

    return CI === true ? S.toUpperCase() === SubText.toUpperCase() : S === SubText;

};
/**
 True if a string ends with a sub-string.
@param {string} Text - The string
@param {string} SubText - The sub-string, the string to search for.
@param {boolean} [CI=true] - CI (Case-Insensitive) can be true (the default) or false
@returns {boolean} Returns true if a string ends with a sub-string.
*/
tp.EndsWith = function (Text, SubText, CI = true) {
    if (tp.IsBlank(SubText) || tp.IsBlank(Text))
        return false;

    if (tp.IsEmpty(CI)) {
        CI = true;
    }

    let S = Text.substring(Text.length - SubText.length, Text.length);

    return CI === true ? S.toUpperCase() === SubText.toUpperCase() : S === SubText;
};
/**
Replaces a sub-string by another sub-string, inside a string, and returns the new string.
@param {string} v - The string .
@param {string} OldValue - The old string .
@param {string} NewValue - The new string .
@returns {string} Returns the new string.
*/
tp.Replace = function (v, OldValue, NewValue) {
    return v.replace(OldValue, NewValue);
};
/**
 Replaces all occurences of a sub-string by another sub-string, inside a string, and returns the new string.
@param {string} v - The string .
@param {string} OldValue - The old string.
@param {string} NewValue - The new string.
@param {boolean} [CI=true] - CI (Case-Insensitive) can be true (the default) or false 
@returns {string} Returns the string after the replacement.
*/
tp.ReplaceAll = function (v, OldValue, NewValue, CI = true) {
    OldValue = tp.RegExEscape(OldValue);
    var Flags = CI === true ? 'igm' : 'gm';
    return v.replace(new RegExp(OldValue, Flags), NewValue);
};
/**
Replaces a character found at a specified index inside a string, and returns the new string.
@param {string} v - The string .
@param {character} NewChar - The character that replaces the old character.
@param {number} Index - The index of the character to be replaced
@returns {string} Returns the string after the replacement.
*/
tp.ReplaceCharAt = function (v, NewChar, Index) {
    return [v.slice(0, Index), NewChar, v.slice(Index + 1)].join('');
};

/**
Places single or double quotes around a string (defaults to single quotes), and returns the new string.
@param {string} v - The string .
@param {boolean} [DoubleQuotes=true] When true then double quotes are used.
@returns {string} Returns the result string
*/
tp.Quote = function (v, DoubleQuotes = true) {
    DoubleQuotes = DoubleQuotes === true;

    if (tp.IsValid(v)) {
        if (DoubleQuotes) {
            v = v.replace(/"/gm, '\\"');
            v = '"' + v + '"';
        } else {
            v = v.replace(/'/gm, "\\'");
            v = "'" + v + "'";
        }
    }

    return v;
};
/**
Unquotes a string if it is surrounded by single or double quotes, and returns the new string.
@param {string} v - The string .
@returns {string} Returns the result string
*/
tp.Unquote = function (v) {
    if (tp.IsValid(v)) {
        if (v.charAt(0) === '"') {
            return v.replace(/(^")|("$)/g, '');
        } else if (v.charAt(0) === "'") {
            return v.replace(/(^')|('$)/g, '');
        }
    }

    return v;
};
/**
Trims a specified string and if the last character is the comma character it removes it. Returns the new string.
@param {string} v - The string
@returns {string} Returns the new string.
*/
tp.RemoveLastComma = function (v) {
    if (tp.IsBlank(v))
        return '';
    else {
        v = tp.Trim(v);
        if (v.length > 0 && tp.EndsWith(v, ','))
            v = v.substring(0, v.length - 1);

        return v;
    }
};
/**
Splits a string into chunks according to a specified chunk size and returns an array of strings.
@param {string} v - The string .
@param {number} ChunkSize - The size in characters for each chunk.
@returns {string[]} Returns Array of strings.
*/
tp.Chunk = function (v, ChunkSize) {
    var rg = new RegExp('.{1,' + ChunkSize + '}', 'g');
    var A = v.match(rg);
    return A;
};
/**
Splits a string into an array of strings by separating the string into and an array of substrings.  
The separator is treated as a string or a regular expression. 
If separator is omitted or does not occur in string, the array returned contains one element consisting of the entire string.  
If separator is an empty string, then the string is converted to an array of characters.
@param   {string}  v - The string to operate on
@param   {string} [Separator=' '] - Optional. Specifies the character(s) to use for separating the string.
@param   {boolean} [RemoveEmptyEntries=true] Optional. When true, the default, then empty entries are removed from result.
@returns  {string[]}  Returns an array of strings.
*/
tp.Split = function (v, Separator = ' ', RemoveEmptyEntries = true) {
    v = v || '';
    RemoveEmptyEntries = RemoveEmptyEntries === true;

    if (RemoveEmptyEntries) {
        var Parts = v.split(Separator);
        var A = [];
        for (var i = 0, ln = Parts.length; i < ln; i++) {
            if (!tp.IsBlank(Parts[i]))
                A.push(Parts[i]);
        }
        return A;
    } else {
        return v.split(Separator);
    }

};
/**
 * Splits a string like "ThisIsAString" into a string like "This Is A String".
 * @param {string} v The string to split.
 */
tp.SplitOnUpperCase = function (v) {
    let Result = '';
    if (tp.IsString(v)) {
        Result = v.match(/[A-Z][a-z]+/g).join(' ');
    }
    return Result;
};
/**
Splits a string of a certain format and creates and returns a javascript object.  
The input string MUST have the following format:
  Key0:Value0; Key1:Value1; KeyN:ValueN; 
Key is a string (with single or double quotes, or no quotes at all).
Value could be any value, or string (with single or double quotes, or no quotes at all).
@param  {string} v - The string to operate on
@returns {object} Returns the constructed object.
 */
tp.SplitDescriptor = function (v) {

    var Result = {};

    if (tp.IsString(v)) {
        var Lines = tp.Split(v, ";", true);
        var parts;

        var Key;
        var Value;

        if (Lines) {
            for (var i = 0; i < Lines.length; i++) {
                parts = tp.Split(Lines[i], ":");
                if (parts && parts.length === 2) {
                    Key = tp.Unquote(Trim(parts[0]));
                    Value = tp.Unquote(Trim(parts[1]));

                    if (Key.length && Value.length) {
                        Result[Key] = Value;
                    }
                }
            }
        }
    }

    return Result;
};
/**
Returns a united string by joining a list of values using an optional separator.  
@param {string} Separator - The separator to use.
@param  {...any} values - The values to join
@returns {string} Returns the result string.
 */
tp.Join = function (Separator, ...values) {

    Separator = Separator || '';

    var i, ln, Params = [];
    for (i = 1, ln = arguments.length; i < ln; i++) {
        Params.push(arguments[i]);
    }

    return Params.join(Separator);
};
/**
Returns a united string by joining a list of values using as separator a comma and a space. 
@param {...any} values The values to join
@returns {string} Returns the result string.
 */
tp.CommaText = function (...values) {

    var i, ln, Params = [];
    for (i = 0, ln = arguments.length; i < ln; i++) {
        Params.push(arguments[i]);
    }
    return Params.join(',  ');
};
/**
Returns an array of strings, by splitting a string, considering the line breaks (\n or \r\n) as separator 
@param  {string} v The string to operate on.
@returns {string[]} Returns an array of strings
 */
tp.ToLines = function (v) {
    if (!tp.IsBlank(v)) {
        var SEP = '##__##';
        v = tp.ReplaceAll(v, '\r\n', SEP);
        v = tp.ReplaceAll(v, '\n', SEP);
        return v.split(SEP);
    }
    return [];
};
/**
Returns a html string, by replacing line breaks (\n or \r\n) with <code>&lt;br /&gt;</code> elements. 
@param  {string} v The string to operate on.
@returns {string} Returns the new string.
 */
tp.LineBreaksToHtml = function (v) {
    if (!tp.IsBlank(v)) {
        var SEP = '##__##';
        v = tp.ReplaceAll(v, '\r\n', SEP);
        v = tp.ReplaceAll(v, '\n', SEP);
        v = tp.ReplaceAll(v, SEP, '');
    }

    return v;
};
/** Replaces line breaks (\r\n, \r and \n) with a specifed separator string and returns the new string.
 * @param {string} v The string to operate on
 * @param {string} sep The separator that replaces the line breaks.
 * @returns {string} Replaces line breaks (\r\n, \r and \n) with a specifed separator string and returns the new string.
 */
tp.ReplaceLineBreaks = function (v, sep) {
    if (!tp.IsBlank(v)) {
        v = tp.ReplaceAll(v, '\r\n', sep);
        v = tp.ReplaceAll(v, '\r', sep);
        v = tp.ReplaceAll(v, '\n', sep);
    }
    return v;
};
/**
Creates and returns a string by repeating a string value a certain number of times
@param  {string} v - The string to repeat.
@param {number} Count - How many times to repeat the input string
@returns {string} Returns a string by repeating a string value a certain number of times
 */
tp.Repeat = function (v, Count) {
    var Result = "";

    for (var i = 0; i < Count; i++) {
        Result += v;
    }
    return Result;
};
/**
Pads a string from left side (start) with a specified sub-string until a specified total length
@param  {string} v - The string to operate on.
@param  {string} PadText - The string to be used in padding.
@param {number} TotalLength - The desired total length of the result string
@returns {string} Returns the padded string.  
 */
tp.PadLeft = function (v, PadText, TotalLength) {
    if (!tp.IsValid(v))
        return v;

    TotalLength = ~~TotalLength;
    v = String(v);
    if (v.length < TotalLength) {
        PadText = tp.Repeat(PadText, TotalLength - v.length);
        v = PadText + v;
    }

    return v;
};
/**
Pads a string from right side (end) with a specified sub-string until a specified total length
@param  {string} v - The string to operate on.
@param  {string} PadText - The string to be used in padding.
@param {number} TotalLength - The desired total length of the result string
@returns {string} Returns the padded string.
 */
tp.PadRight = function (v, PadText, TotalLength) {
    if (!tp.IsValid(v))
        return v;

    TotalLength = ~~TotalLength;
    v = String(v);
    if (v.length < TotalLength) {
        PadText = tp.Repeat(PadText, TotalLength - v.length);
        v = v + PadText;
    }

    return v;
};
/**
Truncates a string to a specified length if string's length is greater than the specified length. Returns the truncated string.
@param  {string} v - The string to operate on.
@param {number} NewLength - The length of the result string
@returns {string} Returns the new string.
 */
tp.SetLength = function SetLength(v, NewLength) {
    if (tp.IsBlank(v))
        return "";

    v = String(v);
    NewLength = ~~NewLength;
    if (v.length > NewLength) {
        v = v.slice(0, NewLength);
    }
    return v;
};

/** Returns true if a specified string is a valid identifier name
 * @param {string} v The string to check.
 * @param {string} [PlusValidChars=''] Optional. User defined valid characters, other than the first character, e.g. '$'.
 * @returns {boolean} Returns true if a specified string is a valid table name
 */
tp.IsValidIdentifier = function (v, PlusValidChars = '') {
    if (!tp.IsString(v) || tp.IsBlank(v))
        return false;

    PlusValidChars = tp.IsString(PlusValidChars) ? PlusValidChars : '';

    let SLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    let SNumbers = "0123456789";
    let SStartLetters = SLetters + '_';
    let SValidChars = SLetters + SNumbers + PlusValidChars + '_';

    for (let i = 0, ln = v.length; i < ln; i++) {
        c = v.charAt(i);
        if ((i === 0) && !SStartLetters.includes(c))
            return false;

        if (!SValidChars.includes(c))
            return false;
    }

    return true;
}

/** Used as the return value by number convertion functions 
 */
tp.NumberConvertionResult = class {

    /**
     * Constructor
     * @param {number} Value The value after the convertion
     * @param {boolean} Result Result of the convertion 
     */
    constructor(Value = 0, Result = false) {
        this.Value = Value || 0;
        this.Result = Result === true;
    }

    /** The value after the convertion
     @type {number}
     */
    Value = 0;
    /** Result of the convertion
     @type {boolean}
     */
    Result = false;

};

/**
Tries to convert a string into an integer.   <br />
Returns a {@link tp.NumberConvertionResult} object as <code>{Value: v, Result: true}</code> where Value is the convertion result, if successful, and Result indicates success or failure.
@param  {string} v - The string to operate on.
@returns {tp.NumberConvertionResult} Returns an {@link tp.NumberConvertionResult} object as <code> {Value: v, Result: true}</code>
 */
tp.TryStrToInt = function (v) {
    let NCR = new tp.NumberConvertionResult();

    if (tp.IsNumber(v)) {
        v = tp.Truncate(v);
        NCR.Value = v;
        NCR.Result = true;
    }
    else if (!tp.IsBlankString(v)) {
        try {
            v = parseInt(v, 10);
            NCR.Value = v;
            NCR.Result = isNaN(v) ? false : true;
        } catch (e) {
            //
        }
    }

    return NCR;
};
/**
Tries to convert a string into a float.  <br />
NOTE: The decimal separator could be point or comma. <br />
Returns a {@link tp.NumberConvertionResult} object as <code>{Value: v, Result: true}</code>  where Value is the convertion result, if successful, and Result indicates success or failure.
@param  {string} v - The string to operate on.
@returns {tp.NumberConvertionResult} Returns a {@link tp.NumberConvertionResult} object as <code> {Value: v, Result: true}</code>
 */
tp.TryStrToFloat = function (v) {

    let NCR = new tp.NumberConvertionResult();

    if (tp.IsNumber(v)) {
        NCR.Value = v;
        NCR.Result = true;
    }
    else if (!tp.IsBlankString(v)) {
        try {
            v = v.replace(',', '.');
            v = parseFloat(v, 10);
            NCR.Value = v;
            NCR.Result = isNaN(v) ? false : true;
        } catch (e) {
            //
        }
    }

    return NCR;

};

/**
Converts a string (or a number) to an integer and returns that integer. <br />
Uses the "try" version to perform the convertion.
@param  {string} v - The string to operate on.
@param {number} [Default=0] - The default value to return if the convertion is not possible. Defaults to 0.
@returns {number} Returns the number.
 */
tp.StrToInt = function (v, Default = 0) {
    let NCR = tp.TryStrToInt(v);
    return NCR.Result === true ? NCR.Value : Default;
};
/**
Converts a string (or a number) to a float and returns that float. <br />
Uses the "try" version to perform the convertion.
@param  {string} v - The string to operate on.
@param {number} [Default=0] - The default value to return if the convertion is not possible. Defaults to 0.
@returns {number} Returns the number.
 */
tp.StrToFloat = function (v, Default = 0) {
    let NCR = tp.TryStrToFloat(v);
    return NCR.Result === true ? NCR.Value : Default;
};
/**
Converts a string to a boolean and returns that boolean.  
The input string must be either 'true' or 'false' regardless of case-sensitivity. 
@param  {string} v - The string to operate on.
@param {boolean} [Default=false] - The default value to return if the convertion is not possible. Defaults to false.
@returns {boolean} Returns the boolean value.
 */
tp.StrToBool = function (v, Default = false) {
    Default = Default === true;

    if (tp.IsSameText(v, "true") || tp.IsSameText(v, "yes")) {
        return true;
    } else if (tp.IsSameText(v, "false") || tp.IsSameText(v, "no")) {
        return false;
    }

    return Default;
};


/**
Converts an integer value into a hexadecimal string, and returns that string   
@param  {number} v - The value to operate on.
@returns {string} Returns the hex string.
 */
tp.ToHex = function (v) {
    if (v < 0) {
        v = 0xFFFFFFFF + v + 1; // ensure not a negative number
    }

    var S = v.toString(16).toUpperCase();
    while (S.length % 2 !== 0) {
        S = '0' + S;
    }
    return S;
};
/**
Escapes a string for use in Javascript regex and returns the escaped string   
@param  {string} v - The value to operate on.
@returns {string} Returns the escaped string.
@see {@link https://stackoverflow.com/questions/3446170/escape-string-for-use-in-javascript-regex|StackOverflow}
 */
tp.RegExEscape = function (v) {
    //return v.replace(/([.*+?\^=!:${}()|\[\]\/\\])/g, "\\$1");
    return tp.IsBlank(v) ? "" : v.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
};
/**
 Converts a dashed string to camel case, e.g. background-color to backgroundColor and -webkit-user-select to webkitUserSelect and returns the new string  
@param  {string} v - The value to operate on.
@returns {string} Returns the the camel-cased string.
 */
tp.DashToCamelCase = function (v) {
    if (!tp.IsBlank(v)) {
        if (v.length > 1 && v.charAt(0) === '-') {
            v = v.substring(1);
        }

        v = v.replace(/-([\da-z])/gi, function (match, c) {
            return c.toUpperCase();
        });
    }

    return v;
};
/**
 Combines two strings by returning a single url path. Ensures that the required slashes are in place.
@param {string} A The first string.
@param {string} B The second string.
@returns {string} The combination of the two strings.
*/
tp.UrlCombine = function (A, B) {
    if (!tp.EndsWith(A, '/') && !tp.StartsWith(B, '/'))
        A += '/';
    return A + B;
};
/**
 Combines a TableName a dot and a FieldName and returns a string. If no TableName is specified then just the FieldName is returned.
@param {string} TableName Optional. The table name
@param {string} FieldName The field name
@returns {string} Returns the combined string, e.g. Customer.Name or just Name
*/
tp.FieldPath = function (TableName, FieldName) {
    if (!tp.IsBlankString(FieldName)) {
        return !tp.IsBlankString(TableName) ? TableName + '.' + FieldName : FieldName;
    }

    return '';
};
/**
 Combines a TableName a double dash (__) and a FieldName and returns a string. If no TableName is specified then just the FieldName is returned.
@param {string} TableName The table name
@param {string} FieldName The field name
@returns {string} Returns the combined string, e.g. Customer__Name
*/
tp.FieldAlias = function (TableName, FieldName) {
    if (!tp.IsBlankString(FieldName)) {
        return !tp.IsBlankString(TableName) ? TableName + '__' + FieldName : FieldName;
    }

    return '';
};
/**
Returns a new GUID string.
@param {boolean} [UseBrackets=false] Optional. If true, then ther result string is encloded by brackets. Defaults to false.
@returns {string} Returns a GUID string.
@see {@link https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript|StackOverflow}
*/
tp.Guid = function (UseBrackets = false) {

    var d = new Date().getTime();
    if (typeof performance !== 'undefined' && typeof performance.now === 'function') {
        d += performance.now(); //use high-precision timer if available
    }

    var Result = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16).toUpperCase();
    });

    UseBrackets = UseBrackets || false;
    return !UseBrackets ? Result : "{" + Result + "}";
};
/** Creates and returns a random string of a specified length, picking characters from a specified set of characters.
 * @param {number} Length The length or the string to create
 * @param {string} CharSet The set of characters to pick from.
 * @returns {string} Returns a random string of a specified length, picking characters from a specified set of characters.
 */
tp.GenerateRandomString = function (Length, CharSet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789') {
    if (tp.IsBlank(CharSet))
        CharSet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';

    let Buffer = [];
    let Index, c;

    for (let i = 0, ln = Length; i < ln; i++) {
        Index = tp.Random(0, CharSet.length - 1);
        c = CharSet.charAt(Index);
        Buffer.push(c);
    }

    let Result = Buffer.join('');
    return Result;


};
/**
Creates and returns a function from a string
@param {string} v - The function source code
@example
var func = tp.CreateFunction('function (a, b) { return a + b; }');
@returns {function} Returns a function  
@see {@link http://stackoverflow.com/questions/7650071/is-there-a-way-to-create-a-function-from-a-string-with-javascript|StackOverflow}
*/
tp.CreateFunction = function (v) {
    var funcReg = /function *\(([^()]*)\)[ \n\t]*\{(.*)\}/gmi;
    var match = funcReg.exec(v.replace(/\n/g, ' '));

    if (match) {
        var Args = match[1].split(',');
        Args.push(match[2]);
        return Function.apply(null, Args);
    }

    return null;
};

/**
Creates a base-64 encoded ASCII string from a string value.
@param {string} v - The value to operate on
@returns {string} Returns a base-64 string.
@see {@link https://stackoverflow.com/questions/30106476/using-javascripts-atob-to-decode-base64-doesnt-properly-decode-utf-8-strings|StackOverflow}
@see {@link https://developer.mozilla.org/en-US/docs/Web/API/WindowBase64/Base64_encoding_and_decoding|MDN}
*/
tp.ToBase64 = function (v) {
    return window.btoa(encodeURIComponent(v).replace(/%([0-9A-F]{2})/g, function (match, p1) {
        return String.fromCharCode(Number('0x' + p1.toString()));
    }));
};
/**
Decodes a string of data which has been encoded using base-64 encoding.
@param {string} v - The value to operate on
@returns {string} Returns the plain string.
@see {@link https://stackoverflow.com/questions/30106476/using-javascripts-atob-to-decode-base64-doesnt-properly-decode-utf-8-strings|StackOverflow}
@see {@link https://developer.mozilla.org/en-US/docs/Web/API/WindowBase64/Base64_encoding_and_decoding|MDN}
*/
tp.FromBase64 = function (v) {
    return window.atob(v);
};

//#endregion

//#region Numbers

/** polyfill for Math.trunc */
if (!Math.trunc) {
    Math.trunc = function (v) {
        return v | 0;
    };
}


/**
* Rounds a float number (or a string representation of a float number) to specified decimal places (defaults to 2).
* @param   {number|string} v The float number or the string representation of a float number.
* @param   {number} Decimals Defaults to 2. The decimal places to round to.
  @returns {number} Returns the rounded number
*/
tp.Round = function (v, Decimals = 2) {
    Decimals = Decimals || 2;

    if (typeof v === 'string') {
        v = parseFloat(v);
    }

    return parseFloat(v.toFixed(Decimals));
};
/**
Truncates a float number to an integer
@param {number} v - The number to truncate
@returns {number} Returns an integer.
*/
tp.Truncate = function (v) {
    return Math.trunc(v);  //return v | 0;
};
/** Returns a random integer inside a specified range
@param {number} Min The minimum number in the desirable range
@param {number} Max The maximum number in the desirable range
@returns {number} Returns the random integer
*/
tp.Random = function (Min, Max) {
    return Math.floor(Math.random() * (Max - Min + 1)) + Min;
};
/** Returns a random float number inside a specified range
@param {number} Min The minimum number in the desirable range
@param {number} Max The maximum number in the desirable range
@returns {number} Returns the random float
*/
tp.RandomFloat = function (Min, Max) {
    return Math.random() * (Max - Min + 1) + Min;
};


//#endregion

//#region Dates

/** Replaces the toJSON() method of the Date object. <br />
 * The toJSON() default implementation returns a string by just calling the toISOString() method. <br />
 * That is while all other Date operations are performed in local time, the stringification results in UTC time. <br />
 * {@link https://stackoverflow.com/questions/31877535/json-stringify-changes-date|JSON.stringify changes date} <br />
 * {@link https://stackoverflow.com/questions/1486476/json-stringify-changes-time-of-date-because-of-utc|JSON Stringify changes time of date because of UTC} <br />
 * {@link https://stackoverflow.com/questions/31096130/how-to-json-stringify-a-javascript-date-and-preserve-timezone|How to JSON stringify a javascript Date and preserve timezone} <br />
 * The above described logic creates problems when the stringified date is sent to server. <br />
 * This implementation stringifies a date and preserves the time-zone, i.e. sends local time to server. <br />
 * TAKEN FROM: https://stackoverflow.com/a/36643588/1779320
 * */
Date.prototype.toJSON = function () {
    if (!tp.IsValidDate(this))
        return null;

    var timezoneOffsetInHours = -(this.getTimezoneOffset() / 60); //UTC minus local time
    var sign = timezoneOffsetInHours >= 0 ? '+' : '-';
    var leadingZero = (Math.abs(timezoneOffsetInHours) < 10) ? '0' : '';

    //It's a bit unfortunate that we need to construct a new Date instance 
    //(we don't want _this_ Date instance to be modified)
    var correctedDate = new Date(this.getFullYear(), this.getMonth(),
        this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds(),
        this.getMilliseconds());
    correctedDate.setHours(this.getHours() + timezoneOffsetInHours);
    var iso = correctedDate.toISOString().replace('Z', '');

    return iso + sign + leadingZero + Math.abs(timezoneOffsetInHours).toString() + ':00';
}

/** Enum-like class
 @class
 @enum {number}
 */
tp.Day = {
    Sunday: 0,
    Monday: 1,
    Tuesday: 2,
    Wednesday: 3,
    Thursday: 4,
    Friday: 5,
    Saturday: 6
};
Object.freeze(tp.Day);

/** Enum-like class. Indicates the pattern of a date format string.
 @class
 @enum {number}
 */
tp.DatePattern = {
    /** MM-dd-yyyy. Middle-endian (month, day, year), e.g. 04/22/96
     * @type {number} 
     */
    MDY: 0,
    /** dd-MM-yyyy. Little-endian (day, month, year), e.g. 22.04.96 or 22/04/96
     * @type {number}
     */
    DMY: 1,
    /** yyyy-MM-dd. Big-endian (year, month, day), e.g. 1996-04-22
     * @type {number}
     */
    YMD: 2
};

/** Returns true if a specified Date value is a valid date.
 * @param {Date|object} v The value to check.
 * @returns {boolean} Returns true if a specified Date value is a valid date.
 */
tp.IsValidDate = function (v) {
    return v instanceof Date && !isNaN(v.getTime());
    // return new Date(v).toString() !== 'Invalid Date';
};

/**
 * Parses a date, time or date-time string into a Date value. The string format should be in yyyy/MM/dd HH:mm:ss  
 Using the / date separator the date is parsed to local date-time.  
 Using the - date separator the date is parsed to UTC date-time. 
 @see {@link http://stackoverflow.com/questions/5619202/converting-string-to-date-in-js|stackoverflow}
 @see {@link https://stackoverflow.com/questions/2587345/why-does-date-parse-give-incorrect-results|stackoverflow}
 * @param   {string} v A date string in the format yyyy/MM/dd for local dates and yyyy-MM-dd for UTC dates.
 * @returns  {Date} A date object
 */
tp.ParseDateTime = function (v) {
    let o = tp.TryParseDateTime(v);
    if (o.Result === true) {
        return o.Value;
    }
    return new Date(Date.parse(v));
};
/**
 * Parses a date, time or date-time string into a Date value. The string format should be in yyyy/MM/dd HH:mm:ss  
 Using the / date separator the date is parsed to local date-time. 
 Using the - date separator the date is parsed to UTC date-time. 
 @see {@link http://stackoverflow.com/questions/5619202/converting-string-to-date-in-js|stackoverflow}
 *
 * @param   {string}   v   A date string in the format yyyy/MM/dd for local dates and yyyy-MM-dd for UTC dates.
 * @returns  {object}   Returns an object as { Value: Date, Result: boolean }
 */
tp.TryParseDateTime = function (v, CultureCode = null) {
    var Info = {
        Value: null,
        Result: false
    };

    if (!tp.IsString(CultureCode) || tp.IsBlank(CultureCode))
        CultureCode = tp.CultureCode;

    try {
        if (tp.IsString(v) && !tp.IsBlank(v)) {
            let i, ln;
            let Seps = ['-', '/', '.'];
            v = v.trim();

            let DateFormat = tp.GetDateFormat(CultureCode);
            let DatePattern = tp.GetDatePattern(DateFormat);

            let Sep = tp.GetDateSeparator(CultureCode);
            for (i = 0, ln = Seps.length; i < ln; i++) {
                if (v.indexOf(Seps[i]) !== -1) {
                    Sep = Seps[i];
                    break;
                }
            }

            let Today = tp.Today();
            let Year, Month, Day;

            switch (DatePattern) {
                case tp.DatePattern.DMY:
                    Year = 3;
                    Month = 2;
                    Day = 1;
                    break;
                case tp.DatePattern.MDY:
                    Year = 3;
                    Month = 1;
                    Day = 2;
                    break;
                default:
                    Year = 1;
                    Month = 2;
                    Day = 3;
                    break;
            }

            let Parts = v.split(Sep);

            if (tp.IsArray(Parts) && Parts.length > 0) {
                Year = Parts.length >= Year ? Parts[Year - 1] : Today.getFullYear();
                Month = Parts.length >= Month ? Parts[Month - 1] - 1 : Today.getMonth();
                Day = Parts.length >= Day ? Parts[Day - 1] : tp.DayOfMonth(Today);

                let D = new Date(Year, Month, Day);
                Info.Value = D;
                Info.Result = true;
                return Info;
            }
        }

    } catch (e) {
        //
    }

    try {
        var ms = Date.parse(v);
        if (!isNaN(ms)) {
            Info.Value = new Date(ms);
            Info.Result = true;
        }
    } catch (e) {
        //
    }

    return Info;
};
/**
 * Returns a specified Date value formatted as ISO Date string, i.e. yyyy-MM-dd.
 * @param   {Date} v The Date value to format
 * @returns  {string} The formatted string
 */
tp.ToISODateString = function (v) {
    return tp.FormatDateTime(v, tp.DateFormatISO);
};
/**
 * Formats a Date value to a string using local or ISO format. <br />
 * If a culture code is specified then the value is formatted according to that specified culture code, i.e. 'el-GR'.  <br />
 * If no culture code is specified, then the value is formatted according to the current culture, see: {@link tp.CultureCode}.    <br />
 * If the string 'ISO' is specified as the culture code, then the value is formatted as an ISO Date string, i.e. yyyy-MM-dd.
 * @param   {Date} v The Date value to format
 * @param   {string} [CultureCode=null] If null or empty, the default, then the date is formatted according to {@link tp.CultureCode} culture. Else a culture code, i.e. 'el-GR' or the string 'ISO' is required.
 * @returns  {string} The formatted string
 */
tp.ToDateString = function (v, CultureCode = null) {
    if (!tp.IsString(CultureCode) || tp.IsBlank(CultureCode))
        CultureCode = tp.CultureCode;

    if (CultureCode === 'ISO')
        return tp.ToISODateString(v);

    let format = tp.IsBlank(CultureCode) ? tp.DateFormat : tp.GetDateFormat(CultureCode);
    let Result = tp.FormatDateTime(v, format);
    return Result;
};
/**
 * Formats a Date value to a time string, optionally with seconds and milliseconds.
 * @param   {Date} v The Date value to format
 * @param   {boolean} [Seconds=false] Defaults to false. When true, then seconds are included in the returned string.
 * @param   {boolean} [Milliseconds=false] Defaults to false. When true, then seconds and milliseconds are included in the returned string.
 * @returns  {string} The formatted string
 */
tp.ToTimeString = function (v, Seconds = false, Milliseconds = false) {
    let format = 'HH:mm';

    if (Milliseconds == true)
        format = 'HH:mm:ss.fff';
    else if (Seconds === true)
        format = 'HH:mm:ss';

    return tp.FormatDateTime(v, format);
};
/**
 * Formats a Date value to a date-time string using local or ISO format, and optionally with seconds and milliseconds.
 * If a culture code is specified then the value is formatted according to that specified culture code, i.e. 'el-GR'.  <br />
 * If no culture code is specified, then the value is formatted according to the current culture, see: {@link tp.CultureCode}.    <br />
 * If the string 'ISO' is specified as the culture code, then the value is formatted as an ISO Date string, i.e. yyyy-MM-dd.
 * @param   {Date} v The Date value to format
 * @param   {string} [CultureCode=null] If null or empty, the default, then the date is formatted according to {@link tp.CultureCode} culture. Else a culture code, i.e. 'el-GR' or the string 'ISO' is required.
 * @param   {boolean} [Seconds=false] Defaults to false. When true, then seconds are included in the returned string.
 * @param   {boolean} [Milliseconds=false] Defaults to false. When true, then seconds and milliseconds are included in the returned string.
 * @returns  {string} The formatted string
 */
tp.ToDateTimeString = function (v, CultureCode = null, Seconds = false, Milliseconds = false) {
    return tp.ToDateString(v, CultureCode) + ' ' + tp.ToTimeString(v, Seconds, Milliseconds);
};
/**
Returns a Date value with the current date and time.
@returns {Date} The current date and time.
*/
tp.Now = function () {
    return new Date();
};
/**
Returns a Date value with the current date. Time part is zero-ed
@returns {Date} The current date
*/
tp.Today = function () {
    return tp.ClearTime(new Date());
};
/**
Returns a Date value with the current time. Time part is zero-ed
@returns {Date} The current time 
*/
tp.Time = function () {
    return tp.ClearDate(new Date());
};
/** 
 * Returns a number between 0..6 representing the day of the week.
 * @param {Date} v - The Date to operaton on.
 * @returns {number} The number of the day of the week (0..6).
 */
tp.DayOfWeek = function (v) {
    return v.getDay();
};
/** 
 * Returns  a number between 1..31 representing the day of the month.
 * @param {Date} v - The Date to operaton on.
   @returns {number} The number of day of the month (1..31)
 */
tp.DayOfMonth = function (v) {
    return v.getDate();
};
/**
 * Adds a specified number of Years to a Date value. Years could be negative.  
 Returns the modified Date.  
 CAUTION: The passed Date value is modified after this call.
 * @param {Date} v - The value to operate on
   @param {number} Years - How many years to add
   @returns {Date} The modified Date
 */
tp.AddYears = function (v, Years) {
    v.setFullYear(v.getFullYear() + Years);
    return v;
};
/**
 * Adds a specified number of Months to a Date value. Months could be negative.  
 Returns the modified Date.  
 CAUTION: The passed Date value is modified after this call.
 * @param {Date} v - The value to operate on
   @param {number} Months - How many months to add
   @returns {Date} The modified Date
 */
tp.AddMonths = function (v, Months) {
    v.setMonth(v.getMonth() + Months);
    return v;
};
/**
 * Adds a specified number of Days to a Date value. Days could be negative.  
 Returns the modified Date. 
 CAUTION: The passed Date value is modified after this call.
 * @param {Date} v - The value to operate on
   @param {number} Days - How many days to add
   @returns {Date}  The modified Date
 */
tp.AddDays = function (v, Days) {
    v.setDate(v.getDate() + Days);
    return v;
};
/**
 * Adds a specified number of Weeks to a Date value. Weeks could be negative.   
 Returns the modified Date. 
 CAUTION: The passed Date value is modified after this call.
 * @param {Date} v - The value to operate on
   @param {number} Weeks - How many weeks to add
   @returns {Date}  The modified Date
 */
tp.AddWeeks = function (v, Weeks) {
    return tp.AddDays(v, Weeks * 7);
};
/**
 * Adds a specified number of Hours to a Date value. Hours could be negative.  
 Returns the modified Date. 
 CAUTION: The passed Date value is modified after this call.
 * @param {Date} v - The value to operate on
   @param {number} Hours - How many hours to add
   @returns {Date} The modified Date
 */
tp.AddHours = function (v, Hours) {
    v.setTime(v.getTime() + Hours * 60 * 60 * 1000);
    return v;
};
/**
 * Adds a specified number of Minutes to a Date value. Minutes could be negative.  
 Returns the modified Date.  
 CAUTION: The passed Date value is modified after this call.
 * @param {Date} v - The value to operate on
   @param {number} Minutes - How many minutes to add
   @returns {Date} The modified Date
 */
tp.AddMinutes = function (v, Minutes) {
    v.setTime(v.getTime() + Minutes * 60 * 1000);
    return v;
};
/**
 * Adds a specified number of Seconds to a Date value. Seconds could be negative. 
 Returns the modified Date. 
 CAUTION: The passed Date value is modified after this call.
 * @param {Date} v - The value to operate on
   @param {number} Seconds - How many seconds to add
   @returns {Date} The modified Date
 */
tp.AddSeconds = function (v, Seconds) {
    v.setTime(v.getTime() + Seconds * 1000);
    return v;
};
/**
 * Sets the date part of Date value to zero
 * @param {Date} v - The value to operate on
   @returns {Date} The modified Date
 */
tp.ClearDate = function (v) {
    v.setFullYear(0);
    v.setMonth(0);
    v.setDate(0);
    return v;
};
/**
 * Sets the time part of Date value to zero
 * @param {Date} v - The value to operate on
   @returns {Date} The modified Date
 */
tp.ClearTime = function (v) {
    v.setHours(0);
    v.setMinutes(0);
    v.setSeconds(0);
    v.setMilliseconds(0);
    return v;
};
/**
 * Returns true if a specified Year is a leap year.
 * @param {number} Year The year to check
   @returns {boolean} True if is a leap year.
 */
tp.IsLeapYear = function (Year) {
    return Year % 4 === 0 && Year % 100 !== 0 || Year % 400 === 0;
};
/**
 * Returns the number of days in a Month of a specified Year.
 * @param {number} Year The year to check
   @param {number} Month The month to check
   @returns {number} The number of days in the month.
 */
tp.DaysInMonth = function (Year, Month) {
    return [31, (tp.IsLeapYear(Year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][Month];
};
/**
 * Compares two Date values. Returns 1 if A is greater, -1 if A is less and 0 when the values are equal.
 * @param {Date} A The first value
 * @param {Date} B The second value
   @returns {number} Returns 1 if A is greater, -1 if A is less and 0 when the values are equal.
 */
tp.DateCompare = function (A, B) {
    if (A === tp.Undefined) { A = null; }
    if (B === tp.Undefined) { B = null; }

    return A > B ? 1 : (A < B ? -1 : 0);
};
/**
Returns true if a specified Date value lays between the A and B Date values.
@param {Date} v - The date to check
@param {Date} A - The down limit
@param {Date} B - the upper limit
@returns {boolean} True if the specified date lays between the A and B Date values.
*/
tp.DateBetween = function (v, A, B) {
    if (A === tp.Undefined) { A = null; }
    if (B === tp.Undefined) { B = null; }

    return v >= A && v <= B;
};
/**
Clones a date and returns the new date
@param {Date} v - The date to clone
@returns {Date} The new date.
*/
tp.DateClone = function (v) {
    return new Date(v.getTime());
};

/**
Returns the start date-time of a specified Date value, i.e. yyyy-MM-dd 00:00:00
@param {Date} v - The date to clone
@returns {Date} Returns the start date-time of a specified Date value, i.e. yyyy-MM-dd 00:00:00
*/
tp.StartOfDay = function (v) {
    return tp.ClearTime(v);
};
/**
Returns the end date-time of a specified Date value, i.e yyyy-MM-dd 23:59:59
@param {Date} v - The date to clone
@returns {Date} Returns the end date-time of a specified Date value, i.e yyyy-MM-dd 23:59:59
*/
tp.EndOfDay = function (v) {
    v = tp.ClearTime(v);
    v = tp.AddDays(v, 1);
    v = tp.AddSeconds(v, -1);
    return v;
};



//#endregion

//#region Arrays 

/**
Type guard function. Returns true if a specified object provides a length property.
@param {any} v - The object to check.
@returns {boolean} - Returns true if a specified object provides a length property.
*/
tp.IsArrayLike = function (v) {
    return tp.IsValid(v) && 'length' in v;
};
/**
Returns true if a specified index is a inside the length of the specified array or array-like object.
@param {ArrayLike} List - The array to operate on
@param {number} Index The index to check
@returns {boolean} True if the specified index is a valid array index.
*/
tp.InRange = function (List, Index) { return Index >= 0 && Index <= List.length - 1; };
/**
Converts an array-like object, 
such as {@link https://developer.mozilla.org/en-US/docs/Web/API/NodeList|NodeList} 
or {@link https://developer.mozilla.org/en-US/docs/Web/API/HTMLCollection|HTMLCollection} 
or {@link https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormControlsCollection|HTMLFormControlsCollection}, 
into a javascript array, by copying elements from the specified source object to a result array. Returns the array. 
@param  {object} List - The array-like object to operate on.
@returns {any[]}  An array.
 */
tp.ToArray = function (List) {
    var A = [];
    for (var i = 0, ln = List.length; i < ln; i++)
        A.push(List[i]);
    return A;
};

/**
 * Returns true if the condition callback returns true for any of the elements in an array.
 *
 * @param {any[]} List The array to operate up on
 * @param {function} Func function(value, index, array), to be called on Context for each element.
 * @param {object} [Context=null] Optional. The context (this) to use when calling Func. Could be null.
 * @returns {boolean} Returns true if any of the array elements passes the check.
 */
tp.Any = function (List, Func, Context = null) { return List.some(Func, Context); };
/**
 * Returns true if all elements in the array pass the test implemented by the provided function.
 *
 * @param {any[]} List The array to operate up on
 * @param {function} Func function(value, index, array), to be called on Context for each element.
 * @param {object} [Context=null] Optional. The context (this) to use when calling Func. Could be null.
 * @returns {boolean} Returns true if all array elements pass the check.
 */
tp.All = function (List, Func, Context = null) { return List.every(Func, Context); };
/**
 * Returns a new array with the results of calling a provided function on every element in this array.
 *
 * @param {any[]} List The array to operate up on
 * @param {function} Func function(value, index, array), to be called on Context for each element.
 * @param {object} [Context=null] Optional. The context (this) to use when calling Func. Could be null.
 * @returns {any[]} Returns a new array with the results of calling a provided function on every element in this array.
 */
tp.Transform = function (List, Func, Context = null) { return List.map(Func, Context); };
/**
 *  Selects all objects found in an array having a distinct value in a specified property.
 * @param {array} List The array to operate up on
 * @param {string} Prop The name of the property to look for distinct values.
 * @returns {array} A new array with objects having a distinct value in the specified property or an empty array.
 */
tp.Distinct = function (List, Prop) {
    var Unique = {};
    var Result = [];

    var o;
    for (var i = 0, ln = List.length; i < ln; i++) {
        o = List[i];
        if (!Unique[o[Prop]]) {
            Result.push(o);
            Unique[o[Prop]] = true;
        }
    }

    return Result;
};
/**
 * Returns a new array containing any object found in a specified array having a specified property with a specified value.
 * @param {array} List The array to operate up on
 * @param {string} Prop The name of the property to match
 * @param {any} v The value the property must have. NOTE: If v is an object (that is NOT string, number, boolean, null, undefined) then a reference equality check takes place.
 * @returns {array} May be an empty array.
 */
tp.Where = function (List, Prop, v) {
    var Result = [];

    var o;
    for (var i = 0, ln = List.length; i < ln; i++) {
        o = List[i];
        if (o[Prop] === v) {
            Result.push(o);
        }
    }

    return Result;
};
/**
 * Returns a new array containing any object found in an array having the properties and the values of a specified object.
 * @param {array} List The array to operate up on
 * @param {object} Props An object with one or more properties to check for matchings
 * @returns {array} May be an empty array.
 */
tp.WhereAll = function (List, Props) {
    var Result = [];

    var o;
    for (var i = 0, ln = List.length; i < ln; i++) {
        o = List[i];
        if (tp.Equals(o, Props)) {
            Result.push(o);
        }
    }

    return Result;
};
/**
 * Returns the first element of the sequence that satisfies the Func condition or null if no such element is found.
 * This method it is actually FirstOrNull()
 * @param {any[]} List The array to operate up on
 * @param {function} Func function(value, index, array), to be called on Context for each element.
 * @param {object} [Context=null] Optional. The context (this) to use when calling Func. Could be null.
 * @returns {any} The first element in the array that passes the condition, if any, or a null value.
 */
tp.FirstOrDefault = function (List, Func, Context = null) {
    var Result = null;

    for (var i = 0, ln = List.length; i < ln; i++) {
        if (Func.call(Context, List[i], i, List) === true) {
            Result = List[i];
            break;
        }
    }

    return Result;
};

//#endregion

//#region Units


/**
* list of all units and their identifying string
@see {@link http://www.w3schools.com/cssref/css_units.asp|w3schools}
@class
@static
*/
tp.UnitMap = {
    pixel: "px",
    percent: "%",
    inch: "in",
    cm: "cm",
    mm: "mm",
    point: "pt",
    pica: "pc",
    em: "em",
    ex: "ex"
};
Object.freeze(tp.UnitMap);


/**
 Extracts and returns the unit suffix of a string value, i.e. px, %, em, etc. 
 taken from: http://upshots.org/javascript/javascript-get-current-style-as-any-unit
 
 @param {string} v The value, i.e. 2px, 100%, etc.
 @returns {string} Returns the unit suffix of a string value, i.e. px, %, em, etc.
 */
tp.ExtractUnit = function (v) {
    if (typeof v === 'string') {
        var unit = v.match(/\D+$/);                           // get the existing unit
        var s = unit === null ? tp.UnitMap.pixel : unit[0];   // if its not set, assume px - otherwise grab string
        return s;
    }

    return '';
};
/**
 * Returns the number out of a string value, that is extracts the number found in a string value like 2px or 100%
 * taken from: http://stackoverflow.com/questions/3530127/convert-css-width-string-to-regular-number
 * @param   {string} v The value, i.e. 2px, 100%, etc.
   @returns {number} Returns the number out of a string value
 */
tp.ExtractNumber = function (v) {
    if (typeof v === 'number')
        return v;
    return !tp.IsBlank(v) ? Number(v.replace(/[^\d\.\-]/g, '')) : 0;
};

/**
Returns true if a specified string value is a pixel value, e.g. 10px
@param {string} v - The value to check, e.g. 10px
@returns {boolean} Returns true if the specified string passes the test.
*/
tp.IsPixel = function (v) { return tp.UnitMap.pixel === tp.ExtractUnit(v); };
/**
Returns true if a specified string value is a em value, e.g. 2em
@param {string} v - The value to check, e.g. 2em
@returns {boolean} Returns true if the specified string passes the test.
*/
tp.IsEm = function (v) { return tp.UnitMap.em === tp.ExtractUnit(v); };
/**
Returns true if a specified string value is a percent value, e.g. 10%
@param {string} v - The value to check, e.g. 10%
@returns {boolean} Returns true if the specified string passes the test.
*/
tp.IsPercent = function (v) { return tp.UnitMap.percent === tp.ExtractUnit(v); };
/** Converts a number or string into a pixel unit value 
@param {number|string} v - The to convert
@returns {string} Returns true if the specified string passes the test.
*/
tp.px = function (v) { return v.toString() + "px"; };


//#endregion

//#region Url handling
/**
 * Navigates to a specified url
 * @param {string} Url Url to navigate to
 */
tp.NavigateTo = function (Url) {
    if (!tp.IsBlank(Url))
        window.location.href = Url;
};
/**
Returns the base url, e.g http://server.com/
@returns {string} Returns the base url
*/
tp.GetBaseUrl = function () { return window.location.protocol + "//" + window.location.host + "/"; };
/**
 * Returns a query string parameter by name, if any, else null
 * @param {string} Name - The name of the parameter
 * @param {string} [Url] - Optional. If not specified then the current url is used
   @returns {string} Returns a query string parameter by name, if any, else null
 */
tp.ParamByName = function (Name, Url = null) {
    if (!Url)
        Url = window.location.href;

    Name = Name.replace(/[\[\]]/g, "\\$&");

    var regex = new RegExp("[?&]" + Name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(Url);

    if (!results)
        return null;

    if (!results[2])
        return '';

    return decodeURIComponent(results[2].replace(/\+/g, " "));
};

/**
 * Returns a plain object where each property is a query string parameter.
 * @param {string} [Url] - Optional. If not specified then the current url is used
   @returns {object} Returns a plain object where each property is a query string parameter.
 */
tp.GetParams = function (Url = null) {
    if (!Url)
        Url = window.location.href;

    var Result = {};

    var Index = Url.indexOf('?');

    if (Index !== -1) {
        var Parts,
            List = Url.slice(Index + 1).split('&');

        for (var i = 0; i < List.length; i++) {
            Parts = List[i].split('=');

            Result[Parts[0]] = decodeURIComponent(Parts[1]);
        }
    }

    return Result;
};

//#endregion

//#region Selecting and finding elements

/**
Selects and returns a direct or non-direct child element, if any, or null, in a specified parent or the document. <br />
NOTE: If only a single parameter is passed then it is considered as the element selector to select in the whole document.
@param {string|Node} ParentElementOrSelector - The parent element where the element is a direct or non-direct child. If not specified (i.e. passed as null) then the document is used.
@param {string|Node} ElementOrSelector - The child element to select.
@returns {Element} Returns a child element, if any, or null.
*/
tp.Select = function (ParentElementOrSelector, ElementOrSelector) {
    let Parent = null,
        el = null;

    if (arguments.length === 2) {
        Parent = typeof arguments[0] === 'string' ? document.querySelector(arguments[0]) : arguments[0];
        el = arguments[1];
    } else if (arguments.length === 1) {
        Parent = document;
        el = arguments[0];
    }

    if (tp.IsNodeSelector(Parent) && typeof el === 'string')
        el = Parent.querySelector(el);

    if (el instanceof HTMLElement)
        return el;

    return null;

};
/**
Selects and returns a NodeList of all direct or non-direct child elements, in a specified parent, or an empty NodeList.<br />
NOTE: If only a single parameter is passed then it is considered as the element selectors to select in the whole document.
@param {string|Node} ParentElementOrSelector -  Optional. The parent element where the elements are direct or non-direct children. If not specified the document is used.
@param {string} Selectors - A comma separated list of selectors, e.g. input, select, textarea, button
@returns {NodeList} - Returns a NodeList of all direct or non-direct child elements, or an empty array.
*/
tp.SelectAll = function (ParentElementOrSelector, Selectors) {
    let Parent = null,
        sSelectors = null;

    if (arguments.length === 2) {
        Parent = tp.IsString(arguments[0]) ? document.querySelector(arguments[0]) : arguments[0];
        sSelectors = arguments[1];
    } else if (arguments.length === 1) {
        Parent = document;
        sSelectors = arguments[0];
    }

    if (tp.IsNodeSelector(Parent) && tp.IsString(sSelectors))
        return Parent.querySelectorAll(sSelectors);

    return [];
};
/**
Returns the closest ancestor (parent node) of a specified element which matches a specified selector. 
If there isn't such an ancestor, it returns null.
@param {Element} el - The element the closest ancestor is to be found.
@param {string} Selector - A selector for the closest ancestor
@returns {HTMLElement} - 
@see {@link https://developer.mozilla.org/en-US/docs/Web/API/Element/closest}
*/
tp.Closest = function (el, Selector) {
    var Result = el.closest(Selector);
    return Result instanceof HTMLElement ? Result : null;
};
/**
Returns the FIRST text node of an element, if any, else null.
@param {Element} el - The element.
@returns {any} - Returns the FIRST Text node or null
*/
tp.FindTextNode = function (el) {
    if (tp.IsElement(el) && el.hasChildNodes()) {
        var List = el.childNodes;
        for (let i = 0, ln = List.length; i < ln; i++) {
            if (List[i].nodeType === Node.TEXT_NODE) {
                return List[i];
            }
        }
    }

    return null;
};
/**
* Returns the index of an element in its parent's children collection, if any, else -1.
* @param {Element} elParent - The parent element  
* @param {Element} el - The element to find.
* @returns {number} Returns the index of the element in its parent, or -1.
*/
tp.ChildIndex = function (elParent, el) {
    if (tp.IsElement(el) && tp.IsElement(elParent)) {
        var List = elParent.children; // children is an HTMLCollection, it provides no methods at all
        for (var i = 0, ln = List.length; i < ln; i++) {
            if (List[i] === el)
                return i;
        }
    }

    return -1;
};
/**
Returns ONLY the direct HTMLElement children of a specified element.
NOTE: HTMLElement.children property returns an HTMLCollection which is a collection of Element elements.
That is it filters out any non-Element nodes such as #text or #comment nodes, etc.
But there is at least one Element, the svg, which is not HTMLElement.
So it is not always safe to assume that the HTMLElement.children will contain just HTMLElement elements.
 
@param {Element|string} ElementOrSelector - The parent dom element
@return {HTMLElement[]} Returns an array with the direct HTMLElement children of a specified element
*/
tp.ChildHTMLElements = function (ElementOrSelector) {
    var el = tp.Select(ElementOrSelector);
    var Result = [];

    if (el instanceof HTMLElement) {

        /*
        NOTE: HTMLElement.children property returns an HTMLCollection which is a collection of Element elements.
        That is it filters out any non-Element nodes such as #text or #comment nodes, etc.
        But there is at least one Element, the svg, which is not HTMLElement.
        So it is not always safe to assume that the HTMLElement.children will contain just HTMLElement elements.
        */
        let List = el.children;

        for (var i = 0, ln = List.length; i < ln; i++) {
            if (List[i] instanceof HTMLElement) {
                Result.push(List[i]);
            }
        }

    }

    return Result;
};
/**
Returns true when an element is contained directly or indirectly by a parent element.
@param {Element} Parent - The parent DOM element
@param {Element} el - The element to check.
@returns {boolean} -
*/
tp.ContainsElement = function (Parent, el) {
    if (tp.IsValid(Parent) && 'contains' in Parent) {
        return Parent.contains(el);
    } else if (tp.IsValid(el)) {
        var Node = el.parentNode;
        while (!tp.IsEmpty(Node)) {
            if (Node === Parent) {
                return true;
            }
            Node = Node.parentNode;
        }
    }

    return false;
};
/**
Returns true if the target (sender) of an event is a specified element or any other element contained by the specified element as direct or nested child.
@param {HTMLElement} el - The container element to check
@param {EventTarget} target - The sender of the event
@returns {boolean} Returns a boolean value indicating whether the target is the specified element or is contained by the specified element.
*/
tp.ContainsEventTarget = function (el, target) {
    return el === target || target instanceof HTMLElement && tp.ContainsElement(el, target);
};

/**
Returns an element if the id of that element ends width a specified id, else null.
@param {string} IdEnding - The ending of the Id 
@param {Element} [ParentElement=null] - Optional. If null, document is used as parent.
@returns {any} Returns the found html element or null.
*/
tp.FindElementWithIdEnding = function (IdEnding, ParentElement = null) {
    ParentElement = ParentElement || document;

    var NodeList, Result = [], i, len, rgx;

    NodeList = ParentElement.getElementsByTagName('*');
    len = NodeList.length;
    rgx = new RegExp(IdEnding + '$');
    var el;
    for (i = 0; i < len; i++) {
        if (NodeList[i] instanceof HTMLElement) {
            el = NodeList[i];
            if (rgx.test(el.id)) {
                Result.push(el);
            }
        }
    }

    if (Result.length > 0) {
        return Result[0];
    }

    return null;
};


//#endregion

//#region Misc Functions

/**
Calls a function, if specified, using a context if not null, passing the specified arguments.
Returns whatever the called function returns
@param {Function} Func - A reference to a function, e.g. the function name 
@param {any} [Context] - The context to be used when calling the function 
@param {any} [Args] - The arguments to the function. 
@returns {any} Returns whatever the called function returns 
*/
tp.Call = function (Func, Context, ...Args) {
    if (typeof Func === 'function') {
        if (Args.length > 0) {
            return Func.apply(Context, Args);
        } else {
            return Func.call(Context);
        }
    }

    return null;
};
/**
Returns true when B properties exist in A and have the same values in both
@param {object} A - The first object
@param {object} B - The second object
@returns {boolean} Returns true when B properties exist in A and have the same values in both
*/
tp.Equals = function (A, B) {
    if (A === B)
        return true;

    for (var Key in B) {
        if (B[Key] !== A[Key])
            return false;
    }

    return true;
};
/**
 * Waits for a specified number of milli-seconds and then calls a specified function, if passed.
 * @param {number} MSecsToWait Milli-seconds to wait
 * @param {Function} [FuncToCall=null] Function to call
 */
tp.WaitAsync = async function (MSecsToWait, FuncToCall = null) {
    FuncToCall = tp.IsFunction(FuncToCall) ? FuncToCall : () => { };
    return new Promise((Resolve, Reject) => {
        setTimeout(() => {
            try {
                FuncToCall();
                Resolve();
            } catch (e) {
                Reject(e);
            }
        }, MSecsToWait);
    })
};
/** Returns true if an instance implements an interface
 @param {object} Instance The instance to check.
 @param {string|string[]} MemberNames The names of methods, properties, fields, etc, of the interface.
 @returns {boolean} True if the specified instance implements all interface members
 */
tp.ImplementsInterface = function (Instance, MemberNames) {

    if (tp.IsValid(Instance)) {

        if (tp.IsString(MemberNames)) {
            let x = MemberNames;
            MemberNames = [];
            MemberNames.push(x);
        }

        if (tp.IsArray(MemberNames)) {

            for (let i = 0, ln = MemberNames.length; i < ln; i++) {
                if (!(MemberNames[i] in Instance)) {
                    return false;
                }
            }

            return true;
        }

    }

    return false;
};

/**
Returns the name (string) of an enumeration value of an enum type.
@example
    var Color = { Red: 1, Green: 2, Blue: 4 };
    var S = tp.EnumNameOf(Color, Color.Green); // returns 'Green'
@param {object} EnumType - The enumeration type, that is any plain javascript object with numeric constants.
@param {number} v - The integer value of an enum constant
@returns {string} Returns the name of an enum constant if found, else empty string.
*/
tp.EnumNameOf = function (EnumType, v) {
    if (typeof v === 'number') {
        for (var Key in EnumType) {
            if (EnumType[Key] === v)
                return Key;
        }
    }

    return '';
};
/**
 * Handles a drop-down operation by toggling the visibility and positioning of a specified element that plays the role of a drop-down list. <br />
 * The list element could be in absolute or fixed position.
 * @param {string|Element} Button Selector or Element. The element which displays the drop-down when is clicked
 * @param {string|Element} List Selector or Element. The drop-down list
 * @param {string} CssClass The css class to toggle to the drop-down list
 */
tp.DropDownHandler = function (Button, List, CssClass = 'tp-Visible') {
    Button = tp(Button);
    List = tp(List);

    Button.addEventListener('click', (ev) => {
        let Position = tp.GetComputedStyle(List).position;
        let R = Button.getBoundingClientRect();
        let P; // tp.Point

        // toggle() returns true if adds the class
        if (List.classList.toggle(CssClass)) {
            if (tp.IsSameText('absolute', Position)) {
                P = tp.ToParent(Button);
                List.style.left = tp.px(P.X);
                List.style.top = tp.px(P.Y + R.height);
            } else if (tp.IsSameText('fixed', Position)) {
                P = tp.ToViewport(Button);
                List.style.left = tp.px(P.X);
                List.style.top = tp.px(P.Y + R.height);
            }
        }
    });

    window.addEventListener('click', (ev) => {
        if (List.classList.contains(CssClass) && !tp.ContainsEventTarget(Button, ev.target)) {
            List.classList.remove(CssClass);
        }
    });

    window.addEventListener('scroll', (ev) => {
        //if (List.classList.contains(CssClass) && !tp.ContainsEventTarget(Button, ev.target)) {
        List.classList.remove(CssClass);
        //}
    });
};

/** Serializes a specified object to json text. 
 * The result text can be formatted, according to a specified flag.
 * @param {object} o The object to serialize.
 * @param {boolean} [Formatted=true] Optional. When true the json text is formatted.
 */
tp.ToJson = function (o, Formatted = true) {
    return Formatted === true ? JSON.stringify(o, null, 4) : JSON.stringify(o);
}
/**
Displays a message in an alert box.
@param {string} MessageText - The message text
*/
tp.ShowMessage = function (MessageText) {
   alert(MessageText);
};

//#endregion

//#region DOM handling

/**
Adds an element of a specified node type to a parent element and returns the newly created element.  
@param {string|Element} ParentOrSelector Selector or element.
@param {string} [TagName='div'] The tag type name. Defaults to 'div'.
@returns {HTMLElement} Returns the newly created element.
*/
tp.el = function (ParentOrSelector, TagName = 'div') {
    ParentOrSelector = tp.Select(ParentOrSelector);

    if (tp.IsHTMLElement(ParentOrSelector)) {
        TagName = TagName || 'div';
        var Result = ParentOrSelector instanceof Document ? ParentOrSelector.createElement(TagName) : ParentOrSelector.ownerDocument.createElement(TagName);
        ParentOrSelector.appendChild(Result);
        return Result;
    }

    return null;
};
/**
Adds a div to a parent element and returns the div element.
@param {string|Element} ParentOrSelector Selector or element.
@returns {HTMLDivElement} Returns the newly created element.
*/
tp.Div = function (ParentOrSelector) { return tp.el(ParentOrSelector, 'div'); };
/**
Adds a span to a parent element and returns the span element.
@param {string|Element} ParentOrSelector Selector or element.
@returns {HTMLSpanElement} Returns the newly created element.
*/
tp.Span = function (ParentOrSelector) { return tp.el(ParentOrSelector, 'span'); };
/**
Adds a specified text as a paragraph to a parent element and returns the paragraph element.
@param {string|Element} ParentOrSelector Selector or element.
@param {string} [Text=''] The text of the paragraph. Defaults to empty string. 
@returns {HTMLParagraphElement} Returns the newly created element.
*/
tp.Paragraph = function (ParentOrSelector, Text = '') {
    let el = tp.el(ParentOrSelector, 'p');
    if (el && !tp.IsBlank(Text)) {
        el.innerText = Text;
    }
    return el;
};
/**
Adds a text break to an element.
@param {string|Element} ParentOrSelector Selector or element.
*/
tp.Break = function (ParentOrSelector) {
    ParentOrSelector = tp.Select(ParentOrSelector);
    if (tp.IsHTMLElement(ParentOrSelector))
        tp.Append(ParentOrSelector, '');
};

/** Creates an Ace Editor object inside a specified parent element.
 * Returns the Ace Editor element. The '__Editor' property of the element points to Ace Editor object.
 * @see {@link https://ace.c9.io|Ace Editor}
 * @param {string|Node} ParentElementOrSelector - The parent element where the element is a direct or non-direct child. If not specified (i.e. passed as null) then the document is used.
 * @param {string} EditorMode Optional. A string indicating the editor mode, e.g. sql or javascript or css or html or csharp, etc.
 * @param {string} SourceCode Optional. The source code to display.
 * @returns {HTMLElement} Returns the Ace Editor element. The '__Editor' property of the element points to Ace Editor object.
 */
tp.CreateSourceCodeEditor = function (ParentElementOrSelector, EditorMode = 'sql', SourceCode = '') {    

    if (typeof ace == "undefined")
        tp.Throw('Ace Editor not found.');

    let Id = tp.SafeId('AceEditor');

    EditorMode = `ace/mode/${EditorMode}`;

    let Css = `
    position: absolute;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    font-size: 14px;
    margin: 2px 0;
`;

    let elEditor = tp.el(ParentElementOrSelector, 'pre');
    elEditor.id = Id;
    tp.StyleText(elEditor, Css);

    let Editor = ace.edit(elEditor);
    Editor.setTheme("ace/theme/twilight");
    Editor.session.setMode(EditorMode);

    if (tp.IsString(SourceCode) && !tp.IsBlank(SourceCode)) {
        Editor.setValue(SourceCode, -1);
    }

    elEditor.__Editor = Editor;
    return elEditor;
};

/** Indicates what is the value attribute of an element according to its node type: 
 * value, checked, innerHTML, selectedIndex or textContent */
tp.ElementValueType = {
    Unknown: 0,
    Value: 1,
    Checked: 2,
    InnerHtml: 4,
    TextContent: 8,
    SelectedIndex: 0x10,

};
/** Returns one of the {@link tp.ElementValueType} indicating what is the value attribute of an element according to its node type: 
 * value, checked, innerHTML, selectedIndex or textContent
 * @param {string|Element}  el - A selector or an element
 * @returns {number} Returns one of the {@link tp.ElementValueType} indicating what is the value attribute of an element according to its node type: 
 * value, checked, innerHTML, selectedIndex or textContent
 */
tp.GetElementValueType = function (el) {
    let Result = tp.ElementValueType.Unknown;

    el = tp.Select(el);

    if (el) {
        let NodeName = el.nodeName.toLowerCase();

        if (NodeName === 'input')
            Result = el.type === 'checkbox' || el.type === 'radio' ? tp.ElementValueType.Checked : tp.ElementValueType.Value;
        else if (NodeName === 'textarea')
            Result = tp.ElementValueType.Value;
        else if (NodeName === 'button')
            Result = tp.ElementValueType.InnerHtml;
        else if (NodeName === 'select')
            Result = tp.ElementValueType.SelectedIndex;
        else if ('textContent' in el)
            Result = tp.ElementValueType.TextContent;
        else
            Result = tp.ElementValueType.InnerHtml;
    }

    return Result;
};

/**
Gets or sets the value of an element. 
To be used mainly with input, select and textarea elements.
NOTE: Passing both arguments, sets the value of an element.
@param {string|Element}  el - A selector or an element
@param {any} [v=null] - The value to set to the element. If null then the function returns the element's value
@returns {any} If a value is not specified then the function returns the element's value, else returns the passed value.
*/
tp.val = function (el, v = null) {
    let ValueType = tp.GetElementValueType(el);

    if (ValueType !== tp.ElementValueType.Unknown) {

        el = tp.Select(el);

        if (el) {

            // get
            if (tp.IsEmpty(v)) {
                switch (ValueType) {
                    case tp.ElementValueType.Value: return el.value;
                    case tp.ElementValueType.Checked: return el.checked;
                    case tp.ElementValueType.InnerHtml: return el.innerHTML;
                    case tp.ElementValueType.TextContent: return el.textContent;
                    case tp.ElementValueType.SelectedIndex: return tp.InRange(el.options, el.selectedIndex) ? el.options[el.selectedIndex].value : null;
                }
            }
            // set
            else {

                switch (ValueType) {
                    case tp.ElementValueType.Value:
                        el.value = v;
                        break;
                    case tp.ElementValueType.Checked:
                        el.checked = Boolean(v);
                        break;
                    case tp.ElementValueType.InnerHtml:
                        el.innerHTML = v;
                        break;
                    case tp.ElementValueType.TextContent:
                        el.textContent = v;
                        break;
                    case tp.ElementValueType.SelectedIndex:
                        let Flag = false;
                        for (i = 0, ln = el.options.length; i < ln; i++) {
                            if (el.options[i].value === v) {
                                el.selectedIndex = i;
                                Flag = true;
                                break;
                            }
                        }

                        if (!Flag && tp.IsNumber(v) && tp.InRange(el.options, v)) {
                            el.selectedIndex = v;
                        }
                        break;
                }

                return v;
            }
        }
    }

    return null;
};
/**
Clears the value of an element. 
To be used mainly with input, select and textarea elements.
 @param   {string|Element}  el - A selector or an element
*/
tp.ClearValue = function (el) {
    let ValueType = tp.GetElementValueType(el);

    if (ValueType !== tp.ElementValueType.Unknown) {
        el = tp.Select(el);
        if (el) {
            switch (ValueType) {
                case tp.ElementValueType.Value:
                    el.value = '';
                    break;
                case tp.ElementValueType.Checked:
                    el.checked = false;
                    break;
                case tp.ElementValueType.InnerHtml:
                    el.innerHTML = '';
                    break;
                case tp.ElementValueType.TextContent:
                    el.textContent = '';
                    break;
                case tp.ElementValueType.SelectedIndex:
                    el.selectedIndex = -1;
                    break;
            }
        }
    }
};
 

/**
 Gets or sets the inner html of an element.  
 NOTE: Passing both arguments, sets the value of an element.
 @param   {string|Element}  el - A selector or an element
 @param   {string} [v=''] - The value to set to the element. If null then the function returns the element's value
 @returns  {string} If a value is not specified then the function returns the element's inner html, else returns the passed value.
 */
tp.Html = function (el, v = '') {
    el = tp.Select(el);

    if (el instanceof HTMLElement) {
        if (typeof v === 'string') {
            el.innerHTML = v;
        } else {
            return el.innerHTML;
        }
    }

    return '';
};

/**
 * Returns an HTMLElement based on a specified Selector or HtmlText. Throws an exception on failure. <br />
 * Used when he have to display a content div inside another div, such as in dialog boxes.
 * @param {HTMLElement|string} ElementOrSelectorOrHtmlText Could be an HTMLElement, a selector or just plain HTML text.
 * @returns {HTMLElement} Returns an HTMLElement based on a specified Selector or HtmlText
 */
tp.HtmlToElement = function (ElementOrSelectorOrHtmlText) {
    let Result = null;

    if (tp.IsElement(ElementOrSelectorOrHtmlText)) {
        Result = ElementOrSelectorOrHtmlText;
    }

    if (Result === null && !tp.IsBlankString(ElementOrSelectorOrHtmlText)) {
        if (tp.IsHtml(ElementOrSelectorOrHtmlText)) {
            // create a temp div
            let div = tp.Div(tp.Doc.body);
            div.innerHTML = ElementOrSelectorOrHtmlText.trim();
            Result = div.firstChild;
            div.parentNode.removeChild(div);
        }
        else {
            Result = tp(ElementOrSelectorOrHtmlText);
        }
    }

    if (Result === null) {
        tp.Throw('Can not extract the Content element');
    }

    return Result;

};

/** Returns an array of all direct HTMLElement children of a specified parent HTMLElement
 * @param {HTMLElement|string} ElementOrSelector The parent element.
 * @returns {HTMLElement[]} Returns an array of all direct HTMLElement children of a specified parent HTMLElement
 */
tp.GetElementList = function (ElementOrSelector) {
    let Result = [];
    let el = tp(ElementOrSelector);
    if (tp.IsHTMLElement(el)) {
        for (let i = 0, ln = el.children.length; i < ln; i++) {
            if (tp.IsHTMLElement(el.children[i]))
                Result.push(el.children[i]);
        }
    }
    return Result;
};

/**
Safely removes an HTMLElement from the DOM, that is from its parent node, if any.
@param {string|Node} ElementOrSelector The element to remove from DOM
*/
tp.Remove = function (ElementOrSelector) {
    let el = tp.Select(ElementOrSelector);
    if (tp.IsHTMLElement(el) && el.parentNode) {
        el.parentNode.removeChild(el);
    }
};
/**
Removes all child nodes/elements from a parent element.
@param {string|Node} ParentOrSelector The element to operate on
*/
tp.RemoveChildren = function (ParentOrSelector) {
    ParentOrSelector = tp.Select(ParentOrSelector);
    if (ParentOrSelector instanceof HTMLElement) {
        while (ParentOrSelector.firstChild) {
            ParentOrSelector.removeChild(ParentOrSelector.lastChild);
        }
    }
};

/**
Appends an element or html markup to a parent element as its last child. Returns the last node of the specified parent.
@param {string|Element} ParentOrSelector - Selector or element
@param {string|Element} ElementOrHtml - The element or the html markup text that should be added
@returns {Node} Returns the last node of the specified parent.
 */
tp.Append = function (ParentOrSelector, ElementOrHtml) {
    ParentOrSelector = tp.Select(ParentOrSelector);

    if (ParentOrSelector instanceof HTMLElement || ParentOrSelector instanceof Document) {
        var v = ElementOrHtml;
        if (tp.IsNode(v)) {
            return ParentOrSelector.appendChild(v);
        } else if (tp.IsString(v) && !tp.IsBlank(v)) {
            if ('insertAdjacentHTML' in ParentOrSelector) {
                ParentOrSelector.insertAdjacentHTML('beforeend', v);
            } else {
                ParentOrSelector.innerHTML += v;
            }

            return ParentOrSelector.childNodes[ParentOrSelector.childNodes.length - 1];
        }
    }

    return null;
};
/**
Inserts an element or html markup to a parent element as its first child. Returns the first node of the specified parent.
@param {string|Element}  ParentOrSelector - Selector or element
@param {string|Element}  ElementOrHtml - The element or the html markup text that should be added
@returns {Node} Returns the first node of the specified parent.
 */
tp.Prepend = function (ParentOrSelector, ElementOrHtml) {
    ParentOrSelector = tp.Select(ParentOrSelector);

    if (ParentOrSelector instanceof HTMLElement) {
        var v = ElementOrHtml;
        if (tp.IsNode(v)) {
            if (ParentOrSelector.childNodes.length === 0)
                return ParentOrSelector.appendChild(v);
            else
                return ParentOrSelector.insertBefore(v, ParentOrSelector.childNodes[0]);
        } else if (tp.IsString(v) && !tp.IsBlank(v)) {
            if ('insertAdjacentHTML' in ParentOrSelector) {
                ParentOrSelector.insertAdjacentHTML('afterbegin', v);
            } else {
                ParentOrSelector.innerHTML = v + ParentOrSelector.innerHTML;
            }

            return ParentOrSelector.childNodes[0];
        }
    }

    return null;
};

/**
Creates an element of a specified type, appends that element to a parent element, and returns the newly created element. 
@param {string|Element}  ParentOrSelector - Selector or element
@param {string} TagName - The node type, e.g. div, span, input etc.
@returns {Element} Returns the newly created element, or null if any of the passed arguments is not valid.
*/
tp.AppendElement = function (ParentOrSelector, TagName) {
    ParentOrSelector = tp.Select(ParentOrSelector);
    if (tp.IsNode(ParentOrSelector) && tp.IsString(TagName)) {
        var Result = ParentOrSelector.ownerDocument.createElement(TagName);
        ParentOrSelector.appendChild(Result);
        return Result;
    }
    return null;
};
/**
Creates an element of a specified type, insert that element to a parent element at a specified index, and returns the newly created element. 
@param {string|Element}  ParentOrSelector - Selector or element
@param {number} Index The index position, among child elements, where to place the newly create element
@param {string} TagName - The node type, e.g. div, span, input etc.
@returns {Element} Returns the newly created element, or null if any of the passed arguments is not valid.
*/
tp.InsertElement = function (ParentOrSelector, Index, TagName) {
    ParentOrSelector = tp.Select(ParentOrSelector);
    if (ParentOrSelector instanceof HTMLElement) {
        let Result = ParentOrSelector.ownerDocument.createElement(TagName);
        let List = tp.ChildHTMLElements(ParentOrSelector);
        if (List.length === 0 || Index === List.length - 1)
            ParentOrSelector.appendChild(Result);
        else
            ParentOrSelector.insertBefore(Result, List[Index]);
        return Result;
    }
    return null;
};

/**
Appends a specified node as the last node of a parent.
@param {string|Node} ParentOrSelector Selector or Node. The parent where to append the specified Node.
@param {Node} Node The Node to be appended
*/
tp.AppendNode = function (ParentOrSelector, Node) {
    ParentOrSelector = tp.Select(ParentOrSelector);
    if (tp.IsNode(ParentOrSelector)) {
        ParentOrSelector.appendChild(Node);
    }
};
/**
Inserts a specified node at a specified index in the child nodes of a parent.
@param {string|Node} ParentOrSelector Selector or Node. The parent where to append the specified Node.
@param {number} Index The index position, among child nodes, where to place the specified Node.
@param {Node} Node The Node to be inserted.
*/
tp.InsertNode = function (ParentOrSelector, Index, Node) {
    ParentOrSelector = tp.Select(ParentOrSelector);
    if (tp.IsNode(ParentOrSelector)) {
        let List = ParentOrSelector.childNodes;
        if (List.length === 0 || Index === List.length - 1)
            ParentOrSelector.appendChild(Node);
        else
            ParentOrSelector.insertBefore(Node, List[Index]);
    }
};

/**
 * Adds a list of items as options to a list control (HTMLSelectElement).
 * @param {string|HTMLSelectElement} ListControl Selector or HTMLSelectElement
 * @param {object[]} List The list of items to add as options. Could be a list of <code>{ Text: , Value: }</code> items. In such a case no call-back is needed.
 * @param {string} [SelectedValue=null] Optional. The combobox selected value.
 */
tp.AddOptionList = function (ListControl, List, SelectedValue = null) {

    ListControl = tp(ListControl);

    let i, ln, Item, Data;

    ListControl.options.length = 0;

    function AddOption(Text, Value) {
        let o = ListControl.ownerDocument.createElement('option');
        o.text = Text;
        o.value = Value;
        ListControl.add(o);
    }

    for (i = 0, ln = List.length; i < ln; i++) {
        Item = List[i];
        AddOption(Item.Text, Item.Value);
    }

    if (tp.IsValid(SelectedValue)) {
        ListControl.value = SelectedValue;
    }
};

/**
Sets multiple mutliple attributes of an element, at once, based on a specified object
@example
tp.SetAttributes(el, { id: 'img0', src: 'image.jpg' );   // set multiple attributes at once
@param {string|Element}  el -  Selector or element
@param {object} o - An object with key/value pairs where key may be a string
*/
tp.SetAttributes = function (el, o) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el) && tp.IsPlainObject(o)) {
        for (var Prop in o) {
            if (!tp.IsFunction(o[Prop]))
                tp.Attribute(el, Prop, o[Prop]);
        }
    }
};
/**
Gets or sets the value of an attribute
NOTE: Passing both arguments, sets the value.
@example
// get
var v = tp.Attribute(el, 'id');
@example
// set a single attribute
tp.Attribute(el, 'id', 'div0');
@param {string|Element} el Selector or element
@param {string} Name Denotes the attribute name.  
@param {object} [v=null] The value of the attribute to set.
@returns {any} When acts as a get then it returns the value of the attribute. When acts as a set, it always returns null.
 */
tp.Attribute = function (el, Name, v = null) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el)) {

        if (tp.IsString(Name)) {

            if (tp.IsEmpty(v)) {                        // get
                return el.getAttribute(Name);
            } else {                                    // set  
                if (Name in el.style) {
                    el[Name] = v;
                } else {
                    el.setAttribute(Name, v);
                }

                return v;
            }

        }

    }

    return null;
};
/**
Removes an attribute from an element.
@param {string|Element} el Selector or element
@param {string} Name Denotes the attribute name.
*/
tp.RemoveAttribute = function (el, Name) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el) && el.hasAttribute(Name))
        el.removeAttribute(Name);
};
/**
Returns true if a specified element has an attribute
@param {string|Element} el Selector or element
@param {string} Name Denotes the attribute name.
@returns {boolean} Returns true if a specified element has an attribute
*/
tp.HasAttribute = function (el, Name) {
    el = tp.Select(el);
    return tp.IsHTMLElement(el) && el.hasAttribute(Name);
};

/**
Sets multiple properties of the style property of an element, at once, based on a specified object
@example
tp.SetStyle(el, {'width': '100px', height: '100px', 'background-color', 'yellow', backgroundColor: 'red' });   // set multiple style properties at once
@param {string|Element} el Selector or element
@param {object} o - An object with key/value pairs where key may be a string
*/
tp.SetStyle = function (el, o) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el) && tp.IsPlainObject(o)) {
        for (var Prop in o) {
            if (!tp.IsFunction(o[Prop]))
                tp.StyleProp(el, Prop, o[Prop]);
        }
    }
};
/**
Gets or sets the value of a style property    
NOTE: Passing both arguments, sets the value.
@example
// get
var v = tp.StyleProp(el, 'width')
@example
// set a single style property
tp.StyleProp(el, 'width', '100px');
@param {string|Element} el Selector or element
@param {string} Name Denotes the property name 
@param {oject} [v=null] The value of the property to set.
@returns {any} When acts as a get then it returns the value of the property. When acts as a set, it always returns null.
 */
tp.StyleProp = function (el, Name, v = null) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el) && tp.IsString(Name)) {
        if (tp.IsEmpty(v)) {                        // get
            let Style = tp.GetComputedStyle(el);
            return Style.getPropertyValue(Name);
        } else {                                    // set  
            if (Name in el.style) {
                el.style[Name] = v;
            } else {
                el.style.setProperty(Name, v, null);
            }
            return v;
        }
    }

    return null;

};
/**
 Gets or sets the css style text of an element. 
 @param {string|Element}  el - A selector or an element
 @param {string} [v=''] - The value to set to the element. If null or empty then the function returns the element's value
 @returns {string} When acts as a get then it returns the the css style text of an element. When acts as a set, it always returns empty string.
*/
tp.StyleText = function (el, v = '') {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el)) {
        if (tp.IsBlank(v))
            return el.style.cssText;
        else
            el.style.cssText = v;
    }

    return '';
};
/**
Returns the currently active style of an element.
@param {string|Element} el - A selector or an element
@returns {CSSStyleDeclaration} - Returns the style object property of an element, which updates itself automatically when the element's style is changed.
*/
tp.GetComputedStyle = function (el) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el))
        return el.ownerDocument.defaultView.getComputedStyle(el, '');

    return null;
};

/**
Gets or sets the value of a data-* attribute or sets the values of multiple data-* attributes. 
To get the value of a data-* attribute pass 1. the element and 2. the attribute name.
To set the value of a single data-* attribute pass 1. the element 2. the attribute name and 3. the value.
To set the value of multiple data-* attributes pass 1. the element and 2. a plain object.
@example
// get
var v = tp.Data(el, 'field');

@example
// set a single data-* attribute
tp.Data(el, 'field', 'UserName');

// set multiple data-* attributes at once
tp.Data(el, {'field': 'UserName', level: 'guest', points: '456', 'color', 'yellow', index: '0' });
 
@param {HTMLElement|string} el Selector or element
@param {string|object} o When string denotes the attribute name. Else it's an object with key/value pairs where key may be a string
@param {object} [v=null] The value to set.
@returns {string} When acts as a get then it returns the value of the data-* attribute. When acts as a set, it always returns empty string.
 */
tp.Data = function (el, o, v = null) {

    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        if (tp.IsEmpty(v) && tp.IsString(o)) {        // get
            return el.getAttribute('data-' + o);
        } else {
            if (tp.IsString(o)) {                    // set  
                el.setAttribute('data-' + o, v);
            } else if (tp.IsPlainObject(o)) {
                for (var Prop in o) {
                    if (!tp.IsFunction(o[Prop]))
                        el.setAttribute('data-' + Prop, o[Prop]);
                }
            }
        }
    }

    return '';

};

/**
 * Returns the value of the data-setup attribute of a specified element if any, else empty string.
 * @param {HTMLElement|string} el The element to operate on.
 */
tp.GetDataSetup = function (el) { return tp.Data(el, 'setup'); };
/**
 * Returns the data setup script object associated to an element, if any, else null. <br />
 * This function returns the __DataSetup script object, if already exists as a property to the specified element. <br />
 * Else tries to get the value of the data-setup attribute, if exists, and creates the object.  <br /> 
 * Finally associates the object to the element and returns the object. <br />
 * Returns null if no attribute exists on element.
 * @param {HTMLElement|string} el The element to operate on.
 * @returns {object} Returns the javascript object created using the data-setup attribute, or null, if no attribute exists on element.
 */
tp.GetDataSetupObject = function (el) {
    el = tp(el);

    // return the object if already there
    if ('__DataSetup' in el)
        return el['__DataSetup'];

    // no object, so get the data-setup attribute, if exists, and create the object
    let Result = null;
    let S = tp.GetDataSetup(el);
    if (tp.IsString(S) && !tp.IsBlank(S)) {
        Result = eval("(" + S + ")");

        if ('ClassType' in Result)
            Result.ClassType = tp.StrToClass(Result['ClassType']);
 
        if (tp.SysConfig.DebugMode === false)
            tp.RemoveAttribute(el, 'data-setup');

        el['__DataSetup'] = Result;
    }

    return Result;
};
 
/**
Gets or sets the value of a data-field attribute of an element  
NOTE: Passing both arguments, sets the value
@param {string|Element} el Selector or element
@param {string} [v=null] The value to set.
@returns {string} When acts as a get then it returns the value of the data-field attribute. When acts as a set, it always returns empty string.
*/
tp.Field = function (el, v = null) { return tp.Data(el, 'field', v); };
/**
Gets or sets the value of the display style property of an element  
NOTE: Passing both arguments, sets the value.
@param {string|Element} el Selector or element
@param {string} [v=null] The value to set.
@returns {string} When acts as a get then it returns the value. When acts as a set, it always returns empty string.
*/
tp.Display = function (el, v = null) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        if (tp.IsEmpty(v)) {        // get
            return tp.GetComputedStyle(el).display;
        } else {                    // set
            el.style.display = v;
            return v;
        }
    }

    return '';
};

/**
 * Type guard function. Returns true if a specified element provides a disabled property.
 * @param {string|Element} el Selector or element
 * @returns {boolean} Returns true if a specified element provides a disabled property.
 */
tp.HasDisabledProperty = function (el) {
    el = tp.Select(el);
    return el instanceof HTMLButtonElement
        || el instanceof HTMLInputElement
        || el instanceof HTMLTextAreaElement
        || el instanceof HTMLSelectElement
        || el instanceof HTMLOptionElement
        || el instanceof HTMLOptGroupElement
        || el instanceof HTMLFieldSetElement
        || el instanceof HTMLLinkElement
        ;
};


/**
Enables or disables an element by setting a proper value to the disabled attribute.  
NOTE: For the function to act as a get, just let the last argument unspecified.  
@param {string|Element} el Selector or element
@param {boolean} [v=null] True enables the element, false disables the element. Leave it unspecified to get the current state.
@returns {boolean} When acts as a get then it returns a value indicating whether the element is enabled.  
*/
tp.Enabled = function (el, v = null) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        if (tp.IsEmpty(v)) {
            if (tp.HasDisabledProperty(el))
                return !el.disabled;

            return !tp.HasClass(el, 'tp-Disabled');
        } else {
            if (tp.HasDisabledProperty(el)) {
                el.disabled = v === false;
                return el.disabled;
            } else {
                if (v === true) {
                    tp.RemoveClass(el, 'tp-Disabled');
                } else {
                    tp.AddClass(el, 'tp-Disabled');
                }

                return v;
            }
        }
    }
};
/**
Get or sets the visibility state of an element according to a specified flag, by settting a proper value to the visibility style property 
NOTE: For the function to act as a get, just let the last argument unspecified. 
@param {string|Element} el Selector or element
@param {boolean} [v=null] True shows the element where false hides the element. Let it unspecified to get the current state.
@returns {boolean} When acts as a get then it returns a value indicating whether the element is visible. When acts as a set, it always returns false.
*/
tp.Visibility = function (el, v = null) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        if (tp.IsEmpty(v)) {        // get
            return tp.IsSameText(tp.StyleProp(el, 'visibility'), 'visible');
        } else {                    // set
            el.style.visibility = v ? "visible" : "hidden";
            return v;
        }
    }

    return true;
};
/**
Shows or hides a specified element by setting the display CSS property.
Gets or sets the display CSS property of an element.
NOTE: When setting it uses the inline style display property. It sets display to none (for non-visible) or empty string (for visible).
@param {string|Element} el Selector or element
@param {boolean} [v=null] True shows the element where false hides the element. Let it unspecified to get the current state.
@returns {boolean} When acts as a get then it returns a value indicating whether the element is visible. When acts as a set, it always returns false.
*/
tp.Visible = function (el, v = null) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        if (tp.IsEmpty(v)) {        // get
            return tp.GetComputedStyle(el).display !== 'none';
        } else {                    // set
            el.style.display = v === true ? '' : 'none';
        }
    }

    return v;
};
/**
Gets or sets the readonly attribute of an input or textarea element.
@param {string|Element} el Selector or element
@param {boolean} [v=null] True sets the element to read-only state. Let it unspecified to get the current state.
@returns {boolean} Returns true if the element is read-only.
*/
tp.ReadOnly = function (el, v = null) {
    el = tp.Select(el);

    if (el instanceof HTMLInputElement && el.type === 'checkbox') {
        if (tp.IsEmpty(v)) { // get
            return el.disabled;
        } else {
            el.disabled = v === true;
        }
    }
    else if (el instanceof HTMLInputElement || el instanceof HTMLTextAreaElement) {
        if (tp.IsEmpty(v)) { // get
            return el.readOnly;
        } else {
            el.readOnly = v === true;
        }
    }

    return v === true;
};



/**
Returns true if an element has a specified css class.
@param {string|Element} el  Selector or element
@param {string} Name - The css class name
@returns {boolean} Returns true if an element has a specified css class.
*/
tp.HasClass = function (el, Name) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        return tp.IsBlank(Name) ? false : el.classList.contains(Name);
    }

    return false;
};
/**
Adds a specified css class to an element, if not already there.
@param {string|Element}  el   Selector or element
@param {string} Name - The css class name
*/
tp.AddClass = function (el, Name) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el) && !tp.IsBlankString(Name)) {

        // we may passed something like 'fa fa-xxxx'
        let StringList = tp.Split(Name, ' ', true);
        if (tp.IsArray(StringList)) {
            StringList.forEach((item) => {
                if (!el.classList.contains(item))
                    el.classList.add(item);
            });
        }

    }
};
/**
Removes a specified css class from an element.
@param {string|Element}  el   Selector or element
@param {string} Name - The css class name
*/
tp.RemoveClass = function (el, Name) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el) && tp.IsString(Name) && !tp.IsBlank(Name) && el.classList.contains(Name)) {
        el.classList.remove(Name);
    }
};
/**
Toggles a specified css class from an element, i.e. adds the class if not there, removes the class if there.
@param {string|Element}  el   Selector or element
@param {string} Name - The css class name
*/
tp.ToggleClass = function (el, Name) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el) && !tp.IsBlank(Name)) {
        el.classList.toggle(Name);
    }
};
/**
Adds one or more css classes to an element.
@param {string|Element}  el   Selector or element
@param {string[]} Names - Rest parameter. One or more css class names.
*/
tp.AddClasses = function (el, ...Names) {

    el = tp.Select(el);

    let Add = (Name) => {
        if (tp.IsArray(Name)) {
            Name.forEach(item => {
                if (!tp.IsBlankString(item))
                    Add(item)
            });
        }
        else if (!tp.IsBlankString(Name)) {
            tp.AddClass(el, Name);
        }

    };

    Add(Names);

};
/**
Removes one or more css classes from an element.
@param {string|Element} el Selector or element
@param {string[]} Names - Rest parameter. One or more css class names.
*/
tp.RemoveClasses = function (el, ...Names) {
    el = tp.Select(el);

    let Remove = (Name) => {
        if (tp.IsArray(Name)) {
            Name.forEach(item => {
                if (!tp.IsBlankString(item))
                    Remove(item)
            });
        }
        else if (!tp.IsBlankString(Name)) {
            tp.RemoveClass(el, Name);
        }
    };

    Remove(Names);


};
/**
Clears all csss classes from an element
@param {string|Element} el Selector or element
*/
tp.ClearClasses = function (el) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el))
        el.className = '';
};
/**
Concatenates css class names into a single string.
Each argument could be just a single class name, or more names space separated
@param {string[]} Names - Rest parameter. One or more css class names.
@returns {string} A string with class names delimited with spaces.
*/
tp.ConcatClasses = function (...Names) {

    var A = [];
    var Parts = null;
    var S, i, ln;

    for (i = 0, ln = Names.length; i < ln; i++) {
        S = Names[i];
        if (!tp.IsBlank(S)) {
            Parts = tp.Split(S, ' ', true);
            A = A.concat(Parts);
        }
    }

    return A.join(' ');
};

/**
Returns true if a specified element is the focused element in the document
@param {string|Element} el Selector or element
@returns {boolean} Returns true if a specified element is the focused element in the document
*/
tp.IsFocused = function (el) {
    el = tp.Select(el);
    return tp.IsHTMLElement(el) && tp.Doc.activeElement === el;
};
/**
Returns true if a specified element is the focused element in the document OR contains the focused element
@param {string|Element} el Selector or element
@returns {boolean} Returns true if a specified element is the focused element in the document OR contains the focused element
*/
tp.HasFocused = function (el) {
    el = tp.Select(el);
    return tp.IsHTMLElement(el) && (tp.Doc.activeElement === el || tp.ContainsElement(el, tp.Doc.activeElement));
};
/**
Gets or sets the tabIndex attribute of a specified element. Returns the tabIndex or NaN if the specified element is not a HTMLElement.
Tab index   < 0         - can be focused either by a mouse click, or the focus() function
Tab index   >= 0        - can be focused either by a mouse click, or the focus() function, or the tab key, according to its tab-order
Tab index   unspecifed  - IE sets it to 0 and can be focused either by a mouse click, or the focus() function
                          Chrome and Firefox set it to -1 and it can NOT be focused at all

Clicking on a nested div (element) with unspecified tab-index
    IE gives focus always to the child
    Chrome and Firefox give focus only to the parent, and only if the parent has its tab-index specified explicitly

Conclusion:
    It seems that setting tab-index explicitly to -1 is the best choice when the purpose is to control the focus
    either by mouse clicks or by the focus() function
@see {@link https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex|mdn}
@see {@link https://developer.mozilla.org/en-US/docs/Web/Accessibility/Keyboard-navigable_JavaScript_widgets|mdn: Keyboar navigatable widgets}
@param {string|Element} el Selector or element
@param {number} [v=null] - When specified then the function is a setter, else it is a getter
@returns {number} Returns the tabIndex or NaN if the specified element is not a HTMLElement.
*/
tp.TabIndex = function (el, v = null) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el)) {
        if (tp.IsNumber(v)) {
            el.tabIndex = v;
            return v;
        } else {
            return el.tabIndex;
        }
    }

    return NaN;
};

/**
Returns a point with the location of an element, relative to the Top/Left of the fully rendered page (document)
@param {string|Element} el Selector or element
@returns {tp.Point} Returns the location of the element, i.e { X: number, Y: number }.
*/
tp.ToDocument = function (el) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el)) {
        var R = el.getBoundingClientRect();
        var po = tp.Viewport.GetPageOffset();
        var clientPoint = tp.ToViewport(el);

        var X = Math.round(R.left + po.X - clientPoint.X);
        var Y = Math.round(R.top + po.Y - clientPoint.Y);
    }

    return new tp.Point(0, 0);
};
/**
Returns a point with the location of an element, relative to the Top/Left of the browser window (viewport)
@param {string|Element} el Selector or element
@returns {tp.Point} Returns the location of the element, i.e { X: number, Y: number }.
*/
tp.ToViewport = function (el) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        var R = el.getBoundingClientRect();
        var X = Math.round(R.left);
        var Y = Math.round(R.top);
        return new tp.Point(X, Y);
    }

    return new tp.Point(0, 0);
};
/**
Returns a point with the location of a block (NOT inline) element, relative to the Top/Left of its immediate parent element
@param {string|Element} el Selector or element
@returns {tp.Point} Returns the location of the element, i.e { X: number, Y: number }.
*/
tp.ToParent = function (el) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        var X = Math.round(el.offsetLeft);
        var Y = Math.round(el.offsetTop);
        return new tp.Point(X, Y);
    }

    return new tp.Point(0, 0);
};

/**
Returns the size of an element. <br />
It also measures the size of an {@link HTMLElement} without a parent.
If the argument is an {@link HTMLElement} without a parent, then the function temporarily adds a DIV to the document and the element to that DIV, and then measures its size. 
@param {string|Element} el Selector or element
@returns {tp.Size} Returns the size of the element, i.e { Width: number, Height: number }.
*/
tp.SizeOf = function (el) {
    var Result = new tp.Size(0, 0);

    if (el) {
        let DIV = null;

        if (tp.IsString(el)) {
            el = tp.Select(el);
        }

        if (tp.IsHTMLElement(el)) {

            if (tp.IsEmpty(el.parentElement)) {
                DIV = el.ownerDocument.createElement("div");
                el.ownerDocument.body.appendChild(DIV);
                DIV.appendChild(el);
            }

            var R = el.getBoundingClientRect();
            var W = Math.round(R.width);
            var H = Math.round(R.height);

            Result = new tp.Size(W, H);

            if (DIV) {
                DIV.removeChild(el);
                DIV.parentElement.removeChild(DIV);
            }
        }
    }


    return Result;

};
/**
Returns the rectangle of an element in viewport, that is relative to the Top/Left of the viewport (uses the getBoundingClientRect).
@param {string|Element} el Selector or element
@returns {tp.Rect} Returns the rectangle of the element, i.e { X: number, Y: number, Width: number, Height: number }.
*/
tp.BoundingRect = function (el) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        var R = el.getBoundingClientRect();
        var X = Math.round(R.left);
        var Y = Math.round(R.top);
        var W = Math.round(R.width);
        var H = Math.round(R.height);
        return new tp.Rect(X, Y, W, H);
    }

    return new tp.Rect(0, 0, 0, 0);
};
/**
Returns the rectangle of an element relative to the Top/Left of its parent element.
@param {string|Element} el Selector or element
@returns {tp.Rect} Returns the rectangle of the element, i.e { X: number, Y: number, Width: number, Height: number }.
*/
tp.OffsetRect = function (el) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        var Pos = tp.ToParent(el);
        var Size = tp.SizeOf(el);

        var X = Pos.X;
        var Y = Pos.Y;
        var W = Size.Width;
        var H = Size.Height;

        return new tp.Rect(X, Y, W, H);
    }

    return new tp.Rect(0, 0, 0, 0);

};
 


/**
Gets or sets the z-index of an element.  
NOTE: Passing both arguments, sets the value.
@see {@link http://philipwalton.com/articles/what-no-one-told-you-about-z-index/|article}
@see {@link http://www.w3.org/TR/CSS2/zindex.html|w3.org}
@param {string|Element} el Selector or element
@param {string|number} [v=null] - A numberic string or a number
@returns {number} When acts as a get, then returns the numeric value of the z-index of an element, else 0
*/
tp.ZIndex = function (el, v = null) {
    /* z-Index - integer 
        see:    http://philipwalton.com/articles/what-no-one-told-you-about-z-index/
        http://www.w3.org/TR/CSS2/zindex.html        */

    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        if (tp.IsEmpty(v)) {
            var Result = tp.GetComputedStyle(el).zIndex;
            return tp.StrToInt(Result, 0);
            //return Number(isNaN(Result) ? '0' : Result);
        } else {
            el.style.zIndex = v.toString();
        }
    }

    return 0;

};
/**
Returns the max z-index in a container element.
@param {string|Element} [Container=null] - Optional. Selector or element or null. Defaults to document.
@returns {number} Returns the maximum z-index.
*/
tp.MaxZIndexOf = function (Container = null) {
    Container = tp.IsString(Container) ? tp.Select(Container) : Container || document;

    var Result, List, el, i, ln, v;

    Result = 0;

    List = Container.querySelectorAll('*');
    ln = List.length;

    for (i = 0; i < ln; i++) {
        el = List[i];
        v = el.ownerDocument.defaultView.getComputedStyle(el, '').getPropertyValue('z-index');
        if (v === 'auto') {
            v = i;
        }

        v = tp.ExtractNumber(v);
        Result = Math.max(Result, v);
    }

    return Result;
};
/**
Returns the min z-index in a container element.
@param {string|Element} [Container=null] - Optional. Selector or element or null. Defaults to document.
@returns {number} Returns the minimum z-index.
*/
tp.MinZIndexOf = function (Container = null) {
    Container = tp.IsString(Container) ? tp.Select(Container) : Container || document;

    var Result, List, el, i, ln, v;

    Result = 0;

    List = Container.querySelectorAll('*');
    ln = List.length;

    for (i = 0; i < ln; i++) {
        el = List[i];
        v = el.ownerDocument.defaultView.getComputedStyle(el, '').getPropertyValue('z-index');
        if (v === 'auto') {
            v = i;
        }
        if (v && tp.IsNumber(v)) {
            if (v < Result) {
                Result = v;
            }
        }
    }

    return Result;
};
/**
Brings an element in front of all of its siblings (child elements in the same parent element).  
Returns the z-index of the element after the placement.
@param {string|Element} el - Selector or element
@returns {number} Returns the z-index of the element after the placement.
*/
tp.BringToFront = function (el) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el)) {
        let v = tp.MaxZIndexOf(el.parentNode);
        let v2 = tp.ZIndex(el);
        if (v2 < v) {
            v++;
            el.style.zIndex = v.toString();
        } else {
            v = v2;
        }

        return v;
    }
    return 0;
};
/**
Sends an element to back of all of its siblings (child elements in the same parent element).  
Returns the z-index of the element after the placement.
@param {string|Element} el - Selector or element
@returns {number} Returns the z-index of the element after the placement.
*/
tp.SendToBack = function (el) {
    el = tp.Select(el);

    if (tp.IsHTMLElement(el)) {
        if (!el) return null;

        var v = tp.MinZIndexOf(el.parentNode);
        v--;
        el.style.zIndex = v.toString();
        return v;
    }

    return 0;
};


/**
Sets the start and end positions of the current text selection in an input (mostly textbox) element
@param {string|Element} el - Selector or element. The input (textbox) element
@param {number} Start - The index of the first selected character
@param {number} End - The index of the character after the last selected character
*/
tp.TextBoxSelectText = function (el, Start, End) {
    el = tp.Select(el);

    if (el) {

        if (tp.IsEmpty(End)) {
            End = el.value.length;
        }

        if ('setSelectionRange' in el) {
            el.setSelectionRange(Start, End);
        } else if ('createTextRange' in el) {
            var Range = el.createTextRange();
            Range.moveStart("character", Start);
            Range.moveEnd("character", -el.value.length + End);
            Range.select();
        }

        //el.focus();
    }

};
/**
Replaces the selected text in an input (textbox) element with a specified text.
@param {string|Element} el - Selector or element. The input (textbox) element.
@param {string} Text - The text to use in replacing the currently selected text in the element.
*/
tp.TextBoxReplaceSelectedText = function (el, Text) {
    el = tp.Select(el);
    if (el) {
        if ('setSelectionRange' in el) {
            var Start = el.selectionStart;
            el.value = el.value.substring(0, Start) + Text + el.value.substring(el.selectionEnd, el.value.length);
            el.setSelectionRange(Start + Text.length, Start + Text.length);
        } else if ('selection' in el.ownerDocument) {
            var Range = el.ownerDocument.selection.createRange();
            Range.text = Text;
            Range.collapse(true);
            Range.select();
        }

        //el.focus();
    }

};

/**
Selects the text of an element
@see {@link http://stackoverflow.com/questions/985272/selecting-text-in-an-element-akin-to-highlighting-with-your-mouse|stackoverflow}
@param {string|Element} el - Selector or element.
*/
tp.ElementSelectText = function (el) {
    el = tp.Select(el);

    if (el) {
        var win = el.ownerDocument.defaultView; //win || window;
        var doc = win.document, sel, range;
        if (win.getSelection && doc.createRange) {
            sel = win.getSelection();
            range = doc.createRange();
            range.selectNodeContents(el);
            sel.removeAllRanges();
            sel.addRange(range);
        } else if (doc.body.createTextRange) {
            range = doc.body.createTextRange();
            range.moveToElementText(el);
            range.select();
        }
    }
};


/**
Turns a block element into a flex container panel that centers its childrent both in the x and y axis
@param {string|Element} el - Selector or element
*/
tp.MakeCenterChildren = function (el) {
    el = tp.Select(el);
    if (tp.IsHTMLElement(el)) {
        var S = "position: relative; height:100%; width:100%; display: flex; justify-content: center; align-items: center; flex-wrap: wrap;";
        el.style.cssText = S;
    }
};
 

/**
A full static class for comparing the position of a node against another node.
@class
@static
*/
tp.DocumentPosition = {

    /** Elements are identical. */
    Identical: 0,
    /** The nodes are in different documents (or one is outside of a document). */
    Disconnected: 1,
    /** B is before A (could be its ancestor too, though) */
    Preceding: 2,
    /** A is before B (could be its ancestor too, though)  */
    Following: 4,
    /** Ancestor */
    Ancestor: 8,
    /** Descendant */
    Descendant: 16,
    /** For private use by the browser. */
    ImplementationSpecific: 32,

    /**
    Compares the position of a node against another node in any other document and returns a bit-mask. 
    Return values 
        tp.DocumentPosition.Identical = 0;                // Elements are identical.
        tp.DocumentPosition.Disconnected = 1;             // The nodes are in different documents (or one is outside of a document).
        tp.DocumentPosition.Preceding = 2;                // B is before A (could be its ancestor too, though)
        tp.DocumentPosition.Following = 4;                // A is before B (could be its ancestor too, though)
        tp.DocumentPosition.Ancestor = 8;                 //
        tp.DocumentPosition.Descendant = 16;              //
        tp.DocumentPosition.ImplementationSpecific = 32;  // For private use by the browser.
    @see The <a href="https://developer.mozilla.org/en-US/docs/Web/API/Node/compareDocumentPosition">Node.compareDocumentPosition() in MDN</a>.
    @see <a href="http://ejohn.org/blog/comparing-document-position/">John Resig blog</a>

    @param {Node} elA The element to be used as base.
    @param {Node} elB The element to compare against base element.
    @returns {number} The return value is a bitmask
    @static
    */
    Compare: function (elA, elB) {
        return elA.compareDocumentPosition(elB);
    },

    /* methods */
    /** True if A contains B  
    @param {Node} elA The element to be used as base.
    @param {Node} elB The element to compare against base element.
    @returns {boolean} True if A contains B
    @static
    */
    Contains: function (elA, elB) {
        var Res = tp.DocumentPosition.Compare(elA, elB);
        return tp.Bf.In(tp.DocumentPosition.Ancestor, Res);
    },
    /** True if B contains A 
    @param {Node} elA The element to be used as base.
    @param {Node} elB The element to compare against base element.
    @returns {boolean} True if B contains A
    @static
    */
    ContainedBy: function (elA, elB) {
        var Res = tp.DocumentPosition.Compare(elA, elB);
        return tp.Bf.In(tp.DocumentPosition.Descendant, Res);
    },
    /** True if B is before A (but B could be an ancestor of A at the same time too, though)
    @param {Node} elA The element to be used as base.
    @param {Node} elB The element to compare against base element.
    @returns {boolean}  True if B is before A (but B could be an ancestor of A at the same time too, though)
    @static
    */
    IsBefore: function (elA, elB) {
        var Res = tp.DocumentPosition.Compare(elA, elB);
        return tp.Bf.In(tp.DocumentPosition.Preceding, Res);
    },
    /** True if A is before B (but A could be an ancestor of B at the same time too, though)
    @param {Node} elA The element to be used as base.
    @param {Node} elB The element to compare against base element.
    @returns {boolean} True if A is before B (but A could be an ancestor of B at the same time too, though)
    @static
    */
    IsAfter: function (elA, elB) {
        var Res = tp.DocumentPosition.Compare(elA, elB);
        return tp.Bf.In(tp.DocumentPosition.Following, Res);
    }
};


/**
 * Alignment helper.
 * @class
 * @static
 * @hideconstructor
 * */
tp.Alignment = {

    /** Near
     @const 
     @returns {number} 1
     */
    Near: 1,
    /** Mid
     @const
     @returns {number} 2
     */
    Mid: 2,
    /** Far
     @const
     @returns {number} 4
     */
    Far: 4,

    /** 
     @readonly
     @returns {tp.Alignment.Mid} -
     */
    get Justify() { return tp.Alignment.Mid; },

    /** Top
    @readonly
    @returns {tp.Alignment.Near} -
    */
    get Top() { return tp.Alignment.Near; },
    /** Center
    @readonly
    @returns {tp.Alignment.Mid} -
    */
    get Center() { return tp.Alignment.Mid; },
    /** Bottom
     @readonly
    @returns {Alignment.Far} -
    */
    get Bottom() { return tp.Alignment.Far; },

    /** Left
    @readonly
    @returns {tp.Alignment.Near} -
    */
    get Left() { return tp.Alignment.Near; },
    /** Right
    @readonly
    @returns {tp.Alignment.Far} -
    */
    get Right() { return tp.Alignment.Far; },

    /**
    Converts n <code>tp.Alignment</code> constant value and returns <code>flex</code> property (<code>justify-content</code>, <code>align-items</code> or <code>align-content</code>) value. <br />
    Returns <br />
    <ul>
        <li><code>flex-start</code> for <code>tp.Alignment.Near</code>  </li>
        <li><code>flex-end</code> for <code>tp.Alignment.Far</code> </li>
        <li><code>center</code> in all other cases. </li>
    </ul>

    @param {number} v - One of the <code>tp.Alignment</code> constant values
    @param {boolean} [Reverse=false] - Defaults to false. True makes the function to return the opposite value, e.g. <code>flex-end</code> instead of <code>flex-start</code>.
    @returns {string} A <code>flex</code> property (<code>justify-content</code>, <code>align-items</code> or <code>align-content</code>) value.
    */
    ToFlex: function (v, Reverse = false) {
        if (v === this.Near) {
            return Reverse === true ? 'flex-end' : 'flex-start';
        } else if (v === this.Far) {
            return Reverse === true ? 'flex-start' : 'flex-end';
        } else {
            return 'center';
        }
    },
    /**
    Converts n <code>tp.Alignment</code> constant value and returns a <code>text-align</code> css property value  <br />
    Returns <br />
    <ul>
        <li><code>left</code> for <code>tp.Alignment.Near</code>  </li>
        <li><code>right</code> for <code>tp.Alignment.Far</code> </li>
        <li><code>center</code> in all other cases. </li>
    </ul>
    @param {number} v - One of the <code>tp.Alignment</code> constant values.
    @param {boolean} [Reverse=false] - Defaults to false. True makes the function to return the opposite value, e.g. <code>right</code> instead of <code>left</code>.
    @returns {string} A css <code>text-align</code> property value
    */
    ToText: function (v, Reverse = false) {
        // to text-align css property
        if (v === this.Near) {
            return Reverse === true ? 'right' : 'left';
        } else if (v === this.Far) {
            return Reverse === true ? 'left' : 'right';
        } else {
            return 'center';
        }
    }

};

/**
Text metrics helper static class
@class
@static
*/
tp.TextSizeInfo = {
    /**
    Measures the size of an element after setting its innerHTML. Returns a { Width: xxx, Height: xxx } object.  
    Instantly creates a hidden element, passes all css font properties affecting the size of the rendered text from
    a specified source element to the hidden element, and records the size of the hidden element.
     @param {string} Text The text to measure.
     @param {Element} SourceElement The element to be used as source. Provides css font properties affecting the size of the rendered text
     @returns {tp.Size} A { Width: xxx, Height: xxx }  object
     */
    MeasureText: function (Text, SourceElement) {
        var el = tp.TextSizeInfo.CreateRulerElement(SourceElement);
        var Result = tp.TextSizeInfo.SizeOf(Text, el);
        el.parentNode.removeChild(el);
        return Result;
    },
    /**
     Creates and returns a hidden copy of a source element to be used in measuring text.         
     @param {Element} SourceElement The element to be used as source. Provides css font properties affecting the size of the rendered text
     @returns {Element} A copy of a source element to be used in measuring text.
     */
    CreateRulerElement: function (SourceElement) {
        var div = document.createElement('div');

        div.style.position = 'absolute';
        div.style.visibility = 'hidden';
        div.style.height = 'auto';
        div.style.width = 'auto';

        document.body.appendChild(div);

        var Style = div.ownerDocument.defaultView.getComputedStyle(SourceElement, ''); // get the computed style

        if (SourceElement !== document.body) {
            div.style.marginTop = Style.marginTop;
            div.style.marginRight = Style.marginRight;
            div.style.marginBottom = Style.marginBottom;
            div.style.marginLeft = Style.marginLeft;

            div.style.borderTop = Style.borderTop;
            div.style.borderRight = Style.borderRight;
            div.style.borderBottom = Style.borderBottom;
            div.style.borderLeft = Style.borderLeft;

            div.style.paddingTop = Style.paddingTop;
            div.style.paddingRight = Style.paddingRight;
            div.style.paddingBottom = Style.paddingBottom;
            div.style.paddingLeft = Style.paddingLeft;
        }

        var v;
        var FontProps = ['font-size', 'font-style', 'font-weight', 'font-family', 'line-height', 'text-transform', 'letter-spacing'];
        for (var i = 0, ln = FontProps.length; i < ln; i++) {
            v = Style.getPropertyValue(FontProps[i]);
            div.style.setProperty(FontProps[i], v);
        }
        return div;
    },

    /**
     Measures the size of an element after setting its innerHTML. Returns a { Width: xxx, Height: xxx } object.
     @param {string} Text The text to measure.
     @param {Element} el The element where the text is going to be displayed.
     @returns {tp.Size} A  { Width: xxx, Height: xxx }  object
     */
    SizeOf: function (Text, el) {
        el.innerHTML = Text;

        return new tp.Size(el.offsetWidth, el.offsetHeight);
    },
    /**
     Measures the width of an element after setting its innerHTML.  
     @param {string} Text The text to measure.
     @param {Element} el The element where the text is going to be displayed.
     @returns {number} Returns the width of an element after setting its innerHTML.
     */
    WidthOf: function (Text, el) {
        el.innerHTML = Text;
        return el.offsetWidth;
    },
    /**
     Measures the height of an element after setting its innerHTML.  
     @param {string} Text The text to measure.
     @param {Element} el The element where the text is going to be displayed.
     @returns {number} Returns the height of an element after setting its innerHTML.
     */
    HeightOf: function (Text, el) {
        el.innerHTML = Text;
        return el.offsetHeight;
    }
};







//#endregion


//---------------------------------------------------------------------------------------

//#region Encoding

/**
Encodes an argument (a Key/Value pair) for use with GET/POST ajax operations and returns the encoded string. 
@param {string} Key The key of the argument
@param {any} Value The value of the argument
@returns {string} Returns the encoded string.
*/
tp.EncodeArg = function (Key, Value) {
    Value = tp.IsEmpty(Value) ? "" : Value;
    var Result = encodeURIComponent(Key) + "=" + encodeURIComponent(Value);
    Result = Result.replace(/%20/g, '+');
    return Result;
};
/**
Encodes arguments for use with GET/POST ajax operations and returns the encoded string. 
@param  {object|object[]|HTMLElement[]} v - The value to operate on. Could be 
1. a plain object, 
2. array of values, 
3. array of DOM elements
@returns {string} Returns the encoded string.
 */
tp.EncodeArgs = function (v) {

    var i, ln, Name, Value, Data = [];

    if (v instanceof Array && v.length > 0) {
        if (tp.IsHTMLElement(v[0])) {                       // is an array of HTMLElement
            for (i = 0, ln = v.length; i < ln; i++) {
                if ('name' in v[i] && 'value' in v[i]) {
                    Name = v[i]['name'];
                    Value = v[i]['value'];
                    Data[Data.length] = tp.EncodeArg(Name, Value);
                }
            }
        } else {                                            // is an array of values
            for (i = 0, ln = v.length; i < ln; i++) {
                Data[Data.length] = tp.EncodeArg("v" + i.toString(), v);
            }
        }
    } else if (tp.IsPlainObject(v)) {                       // is a plain object
        for (var Prop in v) {
            Data[Data.length] = tp.EncodeArg(Prop, v[Prop]);
        }
    }

    var S = Data.join('&');
    S = S.replace(/%20/g, '+');

    return S;
};
 
//#endregion

//#region Serialization

/**
Reads the value of an element (input, select, textarea, button) and adds a property to a specified plain javascript object. 
The new property is named after element's name or element's id (in this order).

That is for an element such as 
    &lt;input type='text' id='UserName' value='John' /&gt;
a property/value is added as 
    { UserName: 'John' }

WARNING: input elements of type file or image, are IGNORED.
NOTE: A select element of type select-multiple generates an array property.
@param {HTMLElement} el - The element to get the value from. Must have a name attribute defined (or at least an id defined).
@param {object} Model - A plain object where the new property/value is added. Caller code is responsible to provide this object.
*/
tp.ElementToProperty = function (el, Model) {
    if (el.name || el.id) {
        var A, j, jln, Name = el.name || el.id;

        if (!tp.IsBlank(Name)) {
            var NodeName = el.nodeName.toLowerCase();
            var Type = el.type ? el.type.toLowerCase() : '';

            switch (NodeName) {
                case 'input':
                    switch (Type) {
                        case 'hidden':
                        case 'text':
                        case 'password':
                        case 'color':
                        case 'date':
                        case 'datetime-local':
                        case 'email':
                        case 'month':
                        case 'number':
                        case 'range':
                        case 'search':
                        case 'tel':
                        case 'time':
                        case 'url':
                        case 'week':
                            Model[Name] = el.value;
                            break;
                        case 'checkbox':
                            Model[Name] = el.checked ? true : false;
                            break;
                        case 'radio':
                            if (el.checked) {
                                Model[Name] = el.value;
                            }
                            break;
                        case 'button':
                        case 'submit':
                        case 'reset':
                            Model[Name] = el.value;
                            break;
                        case 'file':
                            break;
                        case 'image':
                            break;
                    }
                    break;
                case 'button':
                    switch (Type) {
                        case 'button':
                        case 'submit':
                        case 'reset':
                            Model[Name] = el.value;
                            break;
                    }
                    break;
                case 'select':
                    switch (Type) {
                        case 'select-one':
                            Model[Name] = el.value;
                            break;
                        case 'select-multiple':
                            A = [];
                            for (j = 0, jln = el.options.length; j < jln; j++) {
                                if (el.options[j].selected) {
                                    A.push(el.options[j].value);
                                }
                            }
                            Model[Name] = A;
                            break;
                    }
                    break;
                case 'textarea':
                    Model[Name] = el.value;
                    break;
            }
        }

    }

};
/**
Serializes a form, or any other container, into a javascript object, by adding a property for each input, select, textarea or button child element, to that object.  
The new property is named after child element's name or id (in this order). 

That is for an element such as
    <input type='text' id='UserName' value='John' />
a property/value is added as
    { UserName: 'John' }

WARNING: input elements of type file or image, are IGNORED.
NOTE: A select element of type select-multiple generates an array property.
@param {Element|string} ElementOrSelector - A selector or n html form or any other container element, that contains input, select, textarea and button elements.  
@param {object} [Model=null] - Optional. A plain object where the new properties/values are added.
@returns {object} Returns the model where the new properties/values are added.
*/
tp.ContainerToModel = function (ElementOrSelector, Model = null) {
    if (!Model) {
        Model = {};
    }

    var parent = tp.Select(ElementOrSelector);

    if (parent instanceof HTMLElement) {
        var i, ln, el, elements = parent.nodeName.toLowerCase() === 'form' ? parent.elements : tp.SelectAll(parent, 'input, select, textarea, button');
        for (i = 0, ln = elements.length; i < ln; i++) {
            el = elements[i];
            if (!tp.IsBlank(el.name || el.id))
                tp.ElementToProperty(el, Model);
        }
    }

    return Model;
};
/**
Serializes a form, or any other container, into a javascript object, by adding a property for each input, select, textarea or button child element, to that object.  
The new property is named after child element's name or id (in this order).

That is for an element such as
    <input type='text' id='UserName' value='John' />
a property/value is added as
    { UserName: 'John' }

WARNING: input elements of type file or image, are INCLUDED.
NOTE: A select element of type select-multiple generates an array property.
@param {boolean} ShowSpinner - True to show the global spinner while processing files.
@param {Element|String} ElementOrSelector - A selector or n html form or any other container element, that contains input, select, textarea and button elements.
@returns {Promise} Returns a promise
*/
tp.ContainerToModelAsync = async function (ShowSpinner, ElementOrSelector) {

    let Model = {};
    let Result = Promise.resolve(Model);
    let parent = tp.Select(ElementOrSelector);

    if (parent instanceof HTMLElement) {

        // collect the elements in two lists, one for the input[type='file'] and one for the rest of the elements
        let i, ln, el, elements = parent.nodeName.toLowerCase() === 'form' ? parent.elements : tp.SelectAll(parent, 'input, select, textarea, button');
        let IsInputFileElement;
        let FileElementList = [];
        let ElementList = [];
        let PromiseList = [];

        for (i = 0, ln = elements.length; i < ln; i++) {
            el = elements[i];
            if (!tp.IsBlank(el.name || el.id)) {
                IsInputFileElement = el instanceof HTMLInputElement && tp.IsSameText(el.type, 'file');

                if (IsInputFileElement) {
                    FileElementList.push(el);
                } else {
                    ElementList.push(el);
                }
            }
        }


        // input[type='file'] elements first    
        FileElementList.forEach(function (el) {

            let P = new Promise((resolve, reject) => {
                tp.ReadFiles(true, el.files)
                    .then(function (FileList) {
                        Model[el.name || el.id] = FileList; // HttpFile[]
                        resolve();
                    });
            });

            PromiseList.push(P);
        });

        // the rest elements
        let P2 = new Promise((resolve, reject) => {
            for (i = 0, ln = ElementList.length; i < ln; i++) {
                el = ElementList[i];
                tp.ElementToProperty(el, Model);
            }
            resolve();
        });
        PromiseList.push(P2);


        // nested function
        let Spinner = function (Flag) {
            if (ShowSpinner) {
                tp.ShowSpinner(Flag);
            }
        };

        Spinner(true);

        // create the result promise
        Result = Promise.all(PromiseList)
            .then(function () {
                Spinner(false);
                return Model;
            }).catch(function (e) {
                tp.ForceHideSpinner();
                tp.Throw(e ? e.toString() : 'Unknown error');
            });

    }

    return Result;
};

/**
Converts a specified ArrayBuffer to a Hex string
@param {ArrayBuffer} Buffer The ArrayBuffer to convert to Hex string
@returns {string} Returns the Hex string
*/
tp.ArrayBufferToHex = function (Buffer) {

    var UA = new Uint8Array(Buffer);
    var A = new Array(UA.length);
    var i = UA.length;
    while (i--) {
        A[i] = (UA[i] < 16 ? '0' : '') + UA[i].toString(16);  // map to hex
    }

    UA = null; // free memory
    return A.join('');
};


/**
 * A file sent by a POST action to the server, perhaps by an ajax call.
 * @class
 * */
tp.HttpFile = function () {
    this.FileName = '';
    this.Size = 0;
    this.MimeType = '';
    this.Data = '';
};
/** The file name  */
tp.HttpFile.prototype.FileName = '';
/** The size of the file */
tp.HttpFile.prototype.Size = 0;
/** The mime type of the file content */
tp.HttpFile.prototype.MimeType = '';
/** The file content as a base64 string */
tp.HttpFile.prototype.Data = '';

/**
Loads file data from disk using a system dialog. It is passed a list of File objects to load. 

Returns a Promise with a resolve(ResultFileList) where each entry in the ResultFileList is an object of 
    { FileName:, Size:, MimeType:, Data:,}
where Data is a base64 string.  

IMPORTANT: For increasing the allowed maximub POST size, see: {@link http://stackoverflow.com/questions/3853767/maximum-request-length-exceeded|stackoverflow}
@example
var el = tp.Select('#FileData');

tp.ReadFiles(true, el.files)
.then(function (FileList) {
    // handle file list here
}).catch(function (Error) {
    throw Error;
})

@param {boolean} ShowSpinner - True to show the global spinner while processing files.
@param {string|any[]|FileList} FileListOrSelector - Either an input[type="file"] element, or a selector to such an element, or a list of File objects 
(see File API FileList and File classes at {@link https://developer.mozilla.org/en-US/docs/Web/API/FileList|FileList} )
@param {function} [OnDone=null] - Optional. A function(List: HttpFile[]) to call when done and all files are loaded.
It is passed a list of { FileName:, Size:, MimeType:, Data:,} where Data is a base64 string.
@param {function} [OnError=null] - Optional. A function(e: Error, File: File) to call on error.
It is passed the error event and the File that caused the error.
@param {object} [Context=null] - Optional. Defaults to null. The context (this) to use when calling the provided call-back functions.
@param {boolean} [AsHex=false] - Optional. Defaults to false. If true then the Data of the file is converted to a Hex string. Else to a base64 string.
@returns {Promise} Returns a promise
*/
tp.ReadFiles = function (ShowSpinner, FileListOrSelector, OnDone = null, OnError = null, Context = null, AsHex = false) {

    let ReadAsBase64 = function (ResultList, File, ReadNext, Resolve, Reject) {
        let Reader = new FileReader();

        Reader.onload = function () {

            let Data = Reader.result;
            let Parts = Data.split('base64,');
            if (Parts.length === 2) {
                Data = Parts[1];
            }

            // { FileName:,Size:, MimeType:, Data:,}
            let o = new tp.HttpFile();
            o.FileName = File.name;
            o.Size = File.size;
            o.MimeType = File.type;
            o.Data = Data;

            ResultList.push(o);
            ReadNext();
        };
        Reader.onerror = function (e) {
            Reject(e);
            if (OnError)
                tp.Call(OnError, Context, e);
        };
        Reader.onabort = Reader.onerror;

        Reader.readAsDataURL(File);
    };
    let ReadAsHex = function (ResultList, File, ReadNext, Resolve, Reject) {
        let Reader = new FileReader();

        Reader.onload = function () {

            let Data = Reader.result;

            // { FileName:,Size:, MimeType:, Data:,}
            let o = new tp.HttpFile();
            o.FileName = File.name;
            o.Size = File.size;
            o.MimeType = File.type;
            o.Data = tp.ArrayBufferToHex(Data);

            ResultList.push(o);
            ReadNext();
        };
        Reader.onerror = function (e) {
            Reject(e);
            if (OnError)
                tp.Call(OnError, Context, e);
        };
        Reader.onabort = Reader.onerror;

        Reader.readAsArrayBuffer(File);
    };


    return new Promise(function (Resolve, Reject) {
        let el;
        let FileList = null;
        try {
            if (tp.IsArrayLike(FileListOrSelector)) {
                FileList = FileListOrSelector;
            } else {
                el = tp.Select(FileListOrSelector);
                if (el instanceof HTMLInputElement) {
                    FileList = el.files;
                }
            }
        } catch (e) {
            Reject(e);

            if (OnError)
                tp.Call(OnError, Context, e);
        }

        if (ShowSpinner) {
            tp.ShowSpinner(true);
        }

        var Index = 0;
        var ResultList = [];

        var ReadNext = function () {
            if (Index < FileList.length) {
                var File = FileList[Index++];

                if (AsHex === true) {
                    ReadAsHex(ResultList, File, ReadNext, Resolve, Reject);
                } else {
                    ReadAsBase64(ResultList, File, ReadNext, Resolve, Reject);
                }

            } else {
                if (ShowSpinner) {
                    tp.ShowSpinner(false);
                }
                Resolve(ResultList);
                if (OnDone)
                    tp.Call(OnDone, Context, ResultList);
            }
        };

        ReadNext();


    });
};
 
//#endregion

//#region  tp.PostModelAsForm()
/**
Creates an html form and submits that form to a url using POST method. 
It accepts a model parameter (a plain object) whose properties become form's input elements. 
Model properties that are arrays are posted as name[0]=value name[1]=value etc. 
Model properties that are other than primitives and dates are stringified using JSON.
@param {string} Url The url where the form is submitted.  
@param {object} Model A plain object whose properties become input elements in the submitted form. 
 */
tp.PostModelAsForm = function (Url, Model) {
    var form, el, i, ln, PropName, v, Data = {};

    for (PropName in Model) {
        v = Model[PropName];
        if (!tp.IsEmpty(v) && !tp.IsFunction(v)) {
            if (v instanceof Date) {
                v = v.toISOString();
            }
            Data[PropName] = v;
        }
    }

    form = document.createElement("form");
    form.action = Url;
    form.method = 'post';

    var NormalizeValue = function (Value) {
        return tp.IsSimple(Value) ? Value : JSON.stringify(Value);
    };

    for (PropName in Data) {
        v = Data[PropName];

        if (tp.IsArray(v)) {
            for (i = 0, ln = v.length; i < ln; i++) {
                el = document.createElement("input");
                el.setAttribute("type", "hidden");
                el.setAttribute("name", PropName + '[' + i + ']');
                el.setAttribute("value", NormalizeValue(v[i]));
                form.appendChild(el);
            }
        } else {
            el = document.createElement("input");
            el.setAttribute("type", "hidden");
            el.setAttribute("name", PropName);
            el.setAttribute("value", NormalizeValue(v));
            form.appendChild(el);
        }
    }

    document.body.appendChild(form);
    form.submit();

    setTimeout(() => { tp.Remove(form); }, 1000 * 3);
};
//#endregion

//---------------------------------------------------------------------------------------

//#region tp.Names
/** A static class for constructing element names
 @class
 @static
 */
tp.Names = (function () {
    let items = {};
    let counter = 2000; // do not collide with Asp.Net Core auto Ids

    return {
        /**
         * Constructs and returns a string based on Prefix and an auto-inc counter associated to Prefix.
         * It stores passed prefixes in order to auto-increment the associated counter.
         * It Prefix is null or empty, it just returns an auto-inc number as string.
         * WARNING: NOT case-sensitive.
         * @param {string} [Prefix=''] The prefix to prepend in the returned name.
         * @returns {string} Returns the new name.
         * @memberof tp.Names
         * @static
        */
        Next: function (Prefix = '') {

            if (!tp.IsNullOrWhiteSpace(Prefix)) {
                var ucPrefix = Prefix.toUpperCase();
                if (!(ucPrefix in items)) {
                    items[ucPrefix] = 2000; // do not collide with Asp.Net Core auto Ids
                }
                var V = items[ucPrefix]++;
                return Prefix + V.toString();
            }

            counter++;
            return counter.toString();
        }
    };
})();

/**
Constructs and returns a string based on a specified prefix and an internal auto-inc counter associated to thant prefix. 
It stores passed prefixes in order to auto-increment the associated counter. 
It the prefix is null or empty, it just returns an auto-inc number as string. 
WARNING: NOT case-sensitive.
@param {string} [Prefix=''] The prefix to prepend in the returned name.
@returns {string} Returns the new name.
*/
tp.NextName = function (Prefix = '') {
    return tp.Names.Next(Prefix);
};
/**
 Constructs and returns an Id, based on a specified prefix. If prefix is null or empty, tp.Prefix is used.  
 WARNING: NOT case-sensitive.
@param {string} [Prefix=tp.Prefix] The prefix to prepend in the returned Id.
@returns {string} Returns the new Id.
 */
tp.SafeId = function (Prefix = tp.Prefix) {
    if (tp.IsBlank(Prefix))
        Prefix = tp.Prefix;

    var S = tp.NextName(Prefix);
    S = tp.ReplaceAll(S, '.', '-');

    return S;
};
//#endregion

//#region  tp.Point
/**
 * A point class
 */
tp.Point = class {

    /**
     * constructor
     * @param {number} [X=0] - The left of the point
     * @param {number} [Y=0] - The top of the point
     */
    constructor(X = 0, Y = 0) {
        this.X = tp.Truncate(X || 0);
        this.Y = tp.Truncate(Y || 0);
    }

    /** Field */
    //X: number;
    /** Field */
    //Y: number;

    /* public */

    /**
     * Clears this instance
     */
    Clear() {
        this.X = 0;
        this.Y = 0;
    }
    /**
     * Adds to this instance
     * @param {number} X -
     * @param {number} Y -
     */
    Add(X, Y) {
        this.X += tp.Truncate(X);
        this.Y += tp.Truncate(Y);
    }
    /**
     * Subtracts from this instance
     * @param {number} X -
     * @param {number} Y -
     */
    Subtract(X, Y) {
        this.X -= tp.Truncate(X);
        this.Y -= tp.Truncate(Y);
    }
    /**
     * Returns true if this instance equals to the specified values
     * @param {number} X -
     * @param {number} Y -
     * @returns {boolean}  Returns true if this instance equals to the specified values
     */
    Equals(X, Y) {
        X = tp.Truncate(X);
        Y = tp.Truncate(Y);

        return this.X === X && this.Y === Y;
    }
    /**
    * Returns true if this instance is greater than a specified point, in both axis
    * @param {number} X -
    * @param {number} Y -
    * @returns {boolean}  Returns true if this instance is greater than a specified point, in both axis
    */
    Greater(X, Y) { return this.X >= X && this.Y >= Y; }
    /**
    * Returns true if this instance is less than a specified point, in both axis
    * @param {number} X -
    * @param {number} Y -
    * @returns {boolean}  Returns true if this instance is less than a specified point, in both axis
    */
    Less(X, Y) { return this.X <= X && this.Y <= Y; }
    /**
     True if the passed arguments represent two points and the point this instance represents is greater from the first passed point and is lesser from the second passed point.
     @param {number} X1 - The X of the first point
     @param {number} Y1 - The Y of the first point
     @param {number} X2 - The X of the second point
     @param {number} Y2 - The Y of the second point
     @returns {boolean} Returns true if this point is between the two specified points
     */
    IsInBetween(X1, Y1, X2, Y2) { return this.Greater(X1, Y1) && this.Less(X2, Y2); }
    /** 
     *  @return {string} Returns a string representation of this instance 
     * */
    toString() { return tp.Format("x={0}, y={1}", this.X, this.Y); }


};
/** Returns true if a point is contained by a rectangle
 * @param {tp.Point} P - The point to test
 * @param {tp.Rect} R - The rectangle to check
 * @returns {boolean} Returns true if a point is contained by a rectangle
 */
tp.Point.PointInRect = function (P, R) {
    return P.X >= R.X &&
        P.X <= R.X + R.Width &&
        P.Y >= R.Y &&
        P.Y <= R.Y + R.Height;
};
//#endregion

//#region  tp.Rect
/**
 * A rectangle class
 */
tp.Rect = class {
    /**
     * constructor
     * @param {number} [X=0] - The left of the rectangle
     * @param {number} [Y=0] - The top of the rectangle
     * @param {number} [Width=0] - The width of the rectangle 
     * @param {number} [Height=0] - The height of the rectangle
     */
    constructor(X = 0, Y = 0, Width = 0, Height = 0) {
        this.X = tp.Truncate(X || 0);
        this.Y = tp.Truncate(Y || 0);
        this.Width = tp.Truncate(Width || 0);
        this.Height = tp.Truncate(Height || 0);
    }

    /* fields and poperties
    X: number;
    Y: number;
    Width: number;
    Height: number;
   */

    get Right() { return this.X + this.Width; }
    set Right(v) {
        v = tp.Truncate(v || 0);
        this.Width = v - this.X;
    }

    get Bottom() { return this.Y + this.Height; }
    set Bottom(v) {
        v = tp.Truncate(v || 0);
        this.Height = v - this.Y;
    }


    /* public */
    /**
     * Clears this instance
     */
    Clear() {
        this.X = 0;
        this.Y = 0;
        this.Width = 0;
        this.Height = 0;
    }
    /**
     * Returns true if this instance equals to the specified values
     * @param {number} X -
     * @param {number} Y -
     * @param {number} Width -
     * @param {number} Height -
       @returns {boolean} Returns true if this instance equals to the specified values
     */
    Equals(X, Y, Width, Height) {
        X = tp.Truncate(X);
        Y = tp.Truncate(Y);
        Width = tp.Truncate(Width);
        Height = tp.Truncate(Height);

        return this.X === X && this.Y === Y && this.Width === Width && this.Height === Height;
    }
    /**
     * Returns true if this instance contains a specified point or rectangle
     * @param {any} Args Could be one of the following
     * 1. tp.Point
     * 2. tp.Rect
     * 3. X, Y
     * 4. X, Y, Width, Height
     * @returns {boolean}  Returns true if this instance contains a specified point or rectangle
     */
    Contains(Args) {
        var X, Y, Width, Height, o;
        if (arguments.length === 1) {
            o = arguments[0];

            if ("Width" in o) {
                let R = o;

                return this.X <= R.X
                    && this.Y <= R.Y
                    && this.Width <= R.Width
                    && this.Height <= R.Height;
            } else {
                let P = o;

                return P.X >= this.X
                    && P.X <= this.X + this.Width
                    && P.Y >= this.Y
                    && P.Y <= this.Y + this.Height;
            }

        } else if (arguments.length === 2) {

            X = tp.Truncate(arguments[0]);
            Y = tp.Truncate(arguments[1]);

            return X >= this.X
                && X <= this.X + this.Width
                && Y >= this.Y
                && Y <= this.Y + this.Height;

        } else if (arguments.length === 4) {

            X = tp.Truncate(arguments[0]);
            Y = tp.Truncate(arguments[1]);
            Width = tp.Truncate(arguments[2]);
            Height = tp.Truncate(arguments[3]);

            return this.X <= X
                && this.Y <= Y
                && this.Width <= Width
                && this.Height <= Height;
        }

        return false;
    }
    /**
     * Inflates this instance.
     * This method enlarges this rectangle, not a copy of it. 
     * The rectangle is enlarged in both directions along an axis. 
     * For example, if a 50 by 50 rectangle is enlarged by 50 in the x-axis, the resultant rectangle will be 150 units long (the original 50, the 50 in the minus direction, and the 50 in the plus direction) maintaining the rectangle's geometric center.
     * @param {number} Width - The width to inflate
     * @param {number} Height - the height to inflate
     */
    Inflate(Width, Height) {
        this.X = this.X - Width;
        this.Y = this.Y - Height;
        this.Width = this.Width + (2 * Width);
        this.Height = this.Height + (2 * Height);
    }
    /**
     * True if this instance and a specified rectangle have at least one common point.
     * @param {any} Args Could be one of the following
     * 1. tp.Rect
     * 2. X, Y, Width, Height
     * @returns {boolean} True if this instance and a specified rectangle have at least one common point.
     */
    IntersectsWith(Args) {

        if (arguments.length === 1) {
            let R = arguments[0];

            return R.X < this.X + this.Width
                && this.X < R.X + R.Width
                && R.Y < this.Y + this.Height
                && this.Y < R.Y + R.Height;

        } else {
            var X = tp.Truncate(arguments[0]);
            var Y = tp.Truncate(arguments[1]);
            var Width = tp.Truncate(arguments[2]);
            var Height = tp.Truncate(arguments[3]);

            return X < this.X + this.Width
                && this.X < X + Width
                && Y < this.Y + this.Height
                && this.Y < Y + Height;
        }
    }
    /**
     * Makes this rectangle to be the result of the intersection between this rectangle and a specified rectangle. 
     * If there is no intersection between the two rectangles, nothing happens.
     * @param {any} Args Could be one of the following
     * 1. tp.Rect
     * 2. X, Y, Width, Height
     */
    Intersect(Args) {

        var X1, X2, Y1, Y2;

        if (arguments.length === 1) {
            let R = arguments[0];

            X1 = Math.max(this.X, R.X);
            X2 = Math.min(this.X + this.Width, R.X + R.Width);
            Y1 = Math.max(this.Y, R.Y);
            Y2 = Math.min(this.Y + this.Height, R.Y + R.Height);


        } else {

            var X = tp.Truncate(arguments[0]);
            var Y = tp.Truncate(arguments[1]);
            var Width = tp.Truncate(arguments[2]);
            var Height = tp.Truncate(arguments[3]);

            X1 = Math.max(this.X, X);
            X2 = Math.min(this.X + this.Width, X + Width);
            Y1 = Math.max(this.Y, Y);
            Y2 = Math.min(this.Y + this.Height, Y + Height);
        }

        if (X2 >= X1 && Y2 >= Y1) {
            this.X = X1;
            this.Y = Y1;
            this.Width = X2 - X1;
            this.Height = Y2 - Y1;
        }
    }
    /**
     * Moves the location (X, Y) of this instance to a specified location
     * @param {any} Args Could be one of the following
     * 1. tp.Point
     * 2. X, Y
     */
    Offset(Args) {
        if (arguments.length === 2) {
            this.X = arguments[0];
            this.Y = arguments[1];
        } else {
            this.X = arguments[0].X;
            this.Y = arguments[0].Y;
        }
    }
    /**
     * Makes this rectangle to be the result of the union between this rectangle and a specified rectangle.
     * @param {any} Args Could be one of the following
     * 1. tp.Rect
     * 2. X, Y, Width, Height
     */
    Union(Args) {

        var X1, X2, Y1, Y2;

        if (arguments.length === 1) {
            let R = arguments[0];

            X1 = Math.max(this.X, R.X);
            X2 = Math.min(this.X + this.Width, R.X + R.Width);
            Y1 = Math.max(this.Y, R.Y);
            Y2 = Math.min(this.Y + this.Height, R.Y + R.Height);

        } else {

            var X = tp.Truncate(arguments[0]);
            var Y = tp.Truncate(arguments[1]);
            var Width = tp.Truncate(arguments[2]);
            var Height = tp.Truncate(arguments[3]);

            X1 = Math.max(this.X, X);
            X2 = Math.min(this.X + this.Width, X + Width);
            Y1 = Math.max(this.Y, Y);
            Y2 = Math.min(this.Y + this.Height, Y + Height);
        }

        this.X = X1;
        this.Y = Y1;
        this.Width = X2 - X1;
        this.Height = Y2 - Y1;
    }

    /**
    Returns a string representation of this instance
    @returns {string} Returns a string representation of this instance
    */
    toString() { return tp.Format("x={0}, y={1}, width={2}, height={3}", this.X, this.Y, this.Width, this.Height); }
};
/**
 Creates and returns a tp.Rect based on the bounding client rectangle of a specified element or based on a specified DOM ClientRect.
 The specified argument could be either a DOM ClientRect instance or a HTMLElement instance.
 @param {DOMRect|Element} ElementOrClientRect HTMLElement or DOM ClientRect
 @returns {tp.Rect} Returns a tp.Rect based on the specified argument.
 */
tp.Rect.FromClientRect = function (ElementOrClientRect) {
    if (ElementOrClientRect instanceof HTMLElement) {
        let R = ElementOrClientRect.getBoundingClientRect();
        return new tp.Rect(R.left, R.top, R.width, R.height);
    }

    if (tp.ImplementsInterface(ElementOrClientRect, ['left', 'top', 'width', 'height'])) {
        return new tp.Rect(ElementOrClientRect.left, ElementOrClientRect.top, ElementOrClientRect.width, ElementOrClientRect.height);
    }

    return new tp.Rect();
};
//#endregion

//#region  tp.Size
/**
 * A size class
 */
tp.Size = class {

    /**
    * constructor
    * @param {number} [Width=0] The width of the size
    * @param {number} [Height=0] The height of the size
    */
    constructor(Width = 0, Height = 0) {
        this.Width = tp.Truncate(Width || 0);
        this.Height = tp.Truncate(Height || 0);
    }

    /** Field */
    //Width: number;
    /** Field */
    // Height: number;


    /* public */

    /**
     * Clears this instance
     */
    Clear() {
        this.Width = 0;
        this.Height = 0;
    }
    /**
     * Adds to this instance
     * @param {number} Width - The width to add
     * @param {number} Height - The height to add
     */
    Add(Width, Height) {
        this.Width += tp.Truncate(Width);
        this.Height += tp.Truncate(Height);
    }
    /**
     * Subtracts from this instance
     * @param {number} Width - The width to subtract
     * @param {number} Height - The height to subtract
     */
    Subtract(Width, Height) {
        this.Width -= tp.Truncate(Width);
        this.Height -= tp.Truncate(Height);
    }
    /**
     * Returns true if this instance equals to the specified values
     * @param {number} Width - The width to compare
     * @param {number} Height - The height to compare
       @returns {boolean} Returns true if this instance equals to the specified values
     */
    Equals(Width, Height) {
        Width = tp.Truncate(Width);
        Height = tp.Truncate(Height);

        return this.Width === Width && this.Height === Height;
    }
    /**
    Returns a string representation of this instance
    @returns {string} Returns a string representation of this instance
    */
    toString() { return tp.Format("width={0}, height={1}", this.Width, this.Height); }
};
//#endregion

//#region Screen - tp.Viewport

/*=======================================================================================
                               12-column grid system
  ---------------------------------------------------------------------------------------
  Name                    Prefix          From             To
  ---------------------------------------------------------------------------------------
  Extra Small             xs                 0             576
  Small                   sm               577             768             
  Medium                  md               769             992
  Large                   lg               993            1200
  Extra Large             xl              1201            1400
  Double Extra Large      xxl             1401            ~
=======================================================================================*/

/**
Indicates the screen size mode (xsmall, small, medium, large, xlarge, xxlarge) 
@class
@enum
*/
tp.ScreenMode = {
    None: 0,
    XSmall: 1,      //    0 ...  576
    Small: 2,       //  577 ...  768
    Medium: 4,      //  769 ...  992
    Large: 8,       //  993 ... 1200
    XLarge: 16,     // 1201 ... 1400
    XXLarge: 32     // 1401 ... ~
};
Object.freeze(tp.ScreenMode);

tp.ScreenWidthsMax = {
    XSmall: 576,
    Small: 768,
    Medium: 992,
    Large: 1200,
    XLarge: 1400
};

/**
A static class helper for the viewport size and the screen mode (xsmall, small, medium, large) <br />
CAUTION: There are two viewports. <br />
Layout Viewport: What is available to be seen
Visual Viewport: What is currently visible
@see {@link https://developer.mozilla.org/en-US/docs/Glossary/layout_viewport}
@see {@link https://developer.mozilla.org/en-US/docs/Glossary/visual_viewport}
@see {@link https://www.quirksmode.org/mobile/viewports.html}
@class
*/
tp.Viewport = {
    Initialized: false,
    OldMode: tp.ScreenMode.None,
    Listeners: [],

    /**
    Initializes this class
    */
    Initialize() {
        if (!tp.Viewport.Initialized) {
            tp.Viewport.Initialized = true;

            tp.Viewport.OldMode = tp.Viewport.Mode;

            window.addEventListener("resize", function (ev) {
                tp.Viewport.ScreenSizeChanged();
            }, false);
        }
    },
    /**
    Returns the size of the viewport.
    @returns {tp.Size}  Returns the size of the viewport
    */
    GetSize() {
        //var w = Math.max(document.documentElement.clientWidth, window.innerWidth || 0);
        //var h = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);
        //return new tp.Size(w, h);
        return new tp.Size(this.Width, this.Height);
    },
    /**
    Returns the the Top/Left of the viewport regarding the fully rendered document.
    @returns {tp.Point}  Returns the the Top/Left of the viewport regarding the fully rendered document.
    */
    GetPageOffset() {
        var Y = window.scrollY || document.documentElement.scrollTop || document.body.scrollTop;
        var X = window.scrollX || document.documentElement.scrollLeft || document.body.scrollLeft;
        return new tp.Point(X, Y);
    },

    /**
     * Centers an element (i.e. a dialog box) in the window. <br />
     * The element's position should be fixed or absolute.
     * @param {string|Element} el Element or selector of the element to center in the window
     */
    CenterInWindow(el) {
        el = tp(el);
        if (tp.IsHTMLElement(el)) {

            let R = tp.BoundingRect(el);
            let L = (this.Width / 2) - (R.Width / 2);
            let T = (this.Height / 2) - (R.Height / 2);
            let Style = tp.GetComputedStyle(el);
            if (Style.position === 'absolute')
                T += window.scrollY;

            el.style.top = tp.px(T);
            el.style.left = tp.px(L);
        }
    },

    /**
    Called when the screen (viewport) size changes and informs the listeners.
    */
    ScreenSizeChanged() {
        let NewMode = tp.Viewport.Mode;
        let ModeFlag = NewMode !== tp.Viewport.OldMode;
        if (ModeFlag) {
            tp.Viewport.OldMode = NewMode;
        }

        // inform listeners
        let L;
        for (let i = 0, ln = tp.Viewport.Listeners.length; i < ln; i++) {
            L = tp.Viewport.Listeners[i];
            L.Func.call(L.Context, ModeFlag);
        }
    },

    /**
    Adds a listener. The listener will get notified when the screen (viewport) size changes.
    @param  {function} Func - A callback function, as function (ScreenModeFlag: boolean), to call when  the screen (viewport) size changes. 
    The ScreenModeFlag is true when the screen mode is changed as well.
    @param  {Context} [Context=null] - Optional. The context (this) to use when calling the callback function.
    @returns {tp.Listener} - Returns the newly create listener object.
    */
    AddListener(Func, Context = null) {
        let L = new tp.Listener(Func, Context);
        tp.Viewport.Listeners.push(L);
        return L;
    },
    /**
    Removes a listener added by a previous call to AddListener().
    @param {tp.Listener} Listener The listener to remove
    */
    RemoveListener(Listener) {
        var Index = this.Listeners.indexOf(Listener);

        if (Index !== -1) {
            this.Listeners.splice(Index, 1);
        }
    },

    get Mode() {
        var VS = tp.Viewport.GetSize();
        if (VS.Width <= tp.ScreenWidthsMax.XSmall) {
            return tp.ScreenMode.XSmall;
        } else if (VS.Width <= tp.ScreenWidthsMax.Small) {
            return tp.ScreenMode.Small;
        } else if (VS.Width <= tp.ScreenWidthsMax.Medium) {
            return tp.ScreenMode.Medium;
        } else if (VS.Width <= tp.ScreenWidthsMax.Large) {
            return tp.ScreenMode.Large;
        } else if (VS.Width <= tp.ScreenWidthsMax.XLarge) {
            return tp.ScreenMode.XLarge;
        } else {
            return tp.ScreenMode.XXLarge;
        }
    },
    get IsXSmall() { return tp.Viewport.Mode === tp.ScreenMode.XSmall; },
    get IsSmall() { return tp.Viewport.Mode === tp.ScreenMode.Small; },
    get IsMedium() { return tp.Viewport.Mode === tp.ScreenMode.Medium; },
    get IsLarge() { return tp.Viewport.Mode === tp.ScreenMode.Large; },
    get IsXLarge() { return tp.Viewport.Mode === tp.ScreenMode.XLarge; },
    get IsXXLarge() { return tp.Viewport.Mode === tp.ScreenMode.XXLarge; },

    get Width() {
        return window.innerWidth && document.documentElement.clientWidth ?
            Math.min(window.innerWidth, document.documentElement.clientWidth) :
            window.innerWidth ||
            document.documentElement.clientWidth ||
            document.getElementsByTagName('body')[0].clientWidth;
    },

    get Height() {
        return window.innerHeight && document.documentElement.clientHeight ?
            Math.min(window.innerHeight, document.documentElement.clientHeight) :
            window.innerHeight ||
            document.documentElement.clientHeight ||
            document.getElementsByTagName('body')[0].clientHeight;
    }
};

//#endregion

//#region Overlay - tp.ScreenOverlay

/** A global (screen) ovelay DIV. Occupies the whole viewport and becomes the top-most element. */
tp.ScreenOverlay = class {

    /** Constructor 
     * This class creates a DIV that occupies the whole viewport and  becomes the top-most element.
     * @param {HTMLElement} [Parent=null] Optional. The parent of the DIV overlay.
     */
    constructor(Parent = null) {

        Parent = tp.IsHTMLElement(Parent) ? Parent : tp.Doc.body;
        this.Handle = tp.Div(Parent); // Parent.ownerDocument.createElement('div');

        this.Handle.id = tp.SafeId('tp-ScreenOverlay');

        let OverlayStyle = `display: flex;    
position: absolute;
top: 2px;
left: 2px;
right: 2px;
bottom: 2px;
justify-content: center;
align-items: center;
background: rgba(0, 0, 0, 0.07);
`;

        tp.StyleText(this.Handle, OverlayStyle);

        Parent.appendChild(this.Handle);
        tp.BringToFront(this.Handle);
    }

    /**
    Returns the z-index of the overlay
    */
    get ZIndex() {
        return Number(this.Handle.style.zIndex);
    }
    /**
    Gets or sets a boolean value indicating whether the overlay is visible
    */
    get Visible() {
        return this.Handle.style.display !== 'none';
    }
    set Visible(v) {
        v === true;

        if (this.Visible !== v) {
            if (v) {
                this.Handle.style.display = 'flex';
                tp.BringToFront(this.Handle);
            } else {
                this.Handle.style.display = 'none';
            }
        }
    }


    /** Creates, shows and returns the DIV. 
     @returns {HTMLDivElement} Returns the DIV.
     */
    Show() {
        this.Visible = true;
        return this.Handle;
    }
    Hide() {
        this.Visible = false;
        return this.Handle;
    }
    /**
     * Hides and destroys the DIV.
     * @returns {any} It always returns null.
     * */
    Dispose() {
        if (this.Handle && this.Handle.parentNode) {
            this.Handle.parentNode.removeChild(this.Handle);
        }
        this.Handle = null;
        return null;
    }
};
tp.ScreenOverlay.prototype.Handle = null;

//#endregion

//#region Spinner

/**
Static class for displaying a global spinner to the user while waiting for a lengthy operation to be completed.
The global spinner creates a DIV that occupies the whole viewport and  becomes the top-most element.
Above that DIV displays a snake-like spinner.
@class
*/
tp.Spinner = (function () {

    let SpinnerContainerStyle = `
    position: relative;
    width: auto;
    height: auto;
    background-color: transparent;
`;

    let SpinnerStyle = `
    height: 60px;
    width: 60px;
    border: 16px solid #034F84;
    border-right-color: transparent;
    border-radius: 50%;
    animation: tripous-global-spinner 1.5s infinite linear;
`;

    let KeyFrames = `
@keyframes tripous-global-spinner {
    0% { transform: rotate(0); }
    100% { transform: rotate(360deg); }
}
`;


    let DefaultSpinner = {
        Initialized: false,
        Overlay: null,
        divContainer: null,
        divSpinner: null,

        Initialize: function () {
            if (!this.Initialized) {
                this.Initialized = true;
                var style = document.createElement('style');
                style.type = 'text/css';
                style.innerHTML = KeyFrames;
                document.getElementsByTagName('head')[0].appendChild(style);
            }
        },

        Show: function () {
            this.Initialize();

            if (!this.Overlay) {
                this.Overlay = new tp.ScreenOverlay();
            }

            this.Overlay.Show();

            this.divContainer = tp.Div(this.Overlay.Handle);
            tp.StyleText(this.divContainer, SpinnerContainerStyle);

            this.divSpinner = tp.Div(this.divContainer);
            tp.StyleText(this.divSpinner, SpinnerStyle);
        },
        Hide: function () {
            this.Overlay.Dispose();
            this.Overlay = null;
            this.divSpinner = null;
            this.divContainer = null;
            this.divOverlay = null;
        }
    };



    let fCounter = 0;
    let fInstance = null;

    let DoShow = function () {
        if (fCounter >= 0) {
            fCounter++;

            if (fCounter === 1) {
                if (!tp.ImplementsInterface(fInstance, ['Show', 'Hide'])) {
                    fInstance = DefaultSpinner;
                }
                fInstance.Show();
            }
        }
    };
    let DoHide = function () {
        if (fCounter > 0) {
            fCounter--;
        }

        if (fCounter === 0) {
            if (tp.ImplementsInterface(fInstance, ['Show', 'Hide'])) {
                fInstance.Hide();
            }
            fInstance = null;
        }
    };


    return {
        /**
        Shows or hides the spinner, according to a specified flag. 
        Calling this method with the flag set to true, it shows the spinner in the first call, and the it just increases a counter.
        Calling with the flag set to false, hides the spinner.
        @param {boolean} Flag  True to show, false to hide.
        @memberof tp.Spinner
        @static
        */
        Show: function (Flag) {
            if (Flag === true)
                DoShow();
            else
                DoHide();
        },
        /**
        Forces the spinner to hide
        @memberof tp.Spinner
        @static
        */
        ForceHide: function () {
            fCounter = 0;
            DoHide();
        },
        /**
         * Sets the object that functions as a spinner.
         * @param {object} Implementation An instance that provides a Show, Hide, and Dispose methods.
         * @memberof tp.Spinner
         * @static
         */
        SetSpinnerImplementation: function (Implementation) {
            if (tp.ImplementsInterface(Implementation, ['Show', 'Hide', 'Dispose'])) {
                fInstance = Implementation;
            }
        },
        /**
        Returns true while the spinner is visible
        @memberof tp.Spinner
        @static
        */
        get IsShowing() { return !tp.IsEmpty(fInstance); },
        /**
        Returns a number indicating how many times the Show() method is called with its Flag set to true, before a call with the Flag set to fasle.
        @memberof tp.Spinner
        @static
        */
        get ShowingCounter() { return fCounter; }
    };

})();
/**
Shows or hides the global spinner, according to a specified flag.  
The global spinner creates a DIV that occupies the whole viewport and  becomes the top-most element.
Above that DIV displays a snake-like spinner.
Calling this method with the flag set to true, it shows the global spinner in the first call, and the it just increases a counter.
Calling with the flag set to false, hides the global spinner.
@param {boolean} Flag True to show, false to hide.
*/
tp.ShowSpinner = function (Flag) {
    tp.Spinner.Show(Flag);
};
/**
Forces the global spinner to hide
*/
tp.ForceHideSpinner = function () { tp.Spinner.ForceHide(); };
//#endregion

//---------------------------------------------------------------------------------------

//#region tp.Async (Promise)

/** Promise static class/function
* Executes a plain function inside a promise and returns the promise.
* @param {function} Func - A callback function(Info = null): any to promisify.
* @param {any} [Info=null] - Optional. An argument to pass to the callback function.
* @param {object} [Context=null] - Optional. The context (this) to use when calling the callback function.
* @returns {Promise} Returns a Promise.
*/
tp.Async = async function (Func, Info = null, Context = null) {
    let ExecutorFunc = (Resolve, Reject) => {
        try {
            tp.Call(Func, Context, Info);
            Resolve(Info);
        } catch (e) {
            Reject(e);
        }
    };

    var Result = new Promise(ExecutorFunc);
    return Result;
};
/**
Executes multiple promise calls sequentially, where the next call is called ONLY if the previous call is completed.  
Returns a promise with the last item when all items are done, or in the first rejection.
@example
// Info = { Index: number, Flag: boolean }

let Counter = 0;
let ArgsList = [{ Id: 0 }, { Id: 1 }, { Id: 2 }];

let Func = function (Info) {
    return new Promise((resolve, reject) => {
        let timeout = tp.Random(500, 5000);
        setTimeout(() => {
            Info.Index = Counter++;
            log('Id: ' + Info.Id + ' Index: ' + Info.Index + ' Timeout: ' + timeout);
            //if (Info.Index === 2)
            //    throw 'I dont like 2';
            resolve(Info);
        }, timeout);

    });
};

let BreakFunc = function (Info: IInfo): boolean {
    return false;
}

let Result = tp.Async.Chain(true, ArgsList, Func, BreakFunc);
@see {@link https://www.reddit.com/r/typescript/comments/54qe6w/use_reduce_and_promises_to_execute_multiple_async/|promises and reduce}
@param {boolean} ShowSpinner - If true then a spinner overlay div is displayed while the operation executes.
@param {any[]} List - An array of values. Each value is passed, in turn, in a provided callback function.
@param {function} Func - A callback function(v: T): Promise which gets one of the array values and executes inside a promise.
@param {function} [BreakFunc=null] - Optional. A callback function(v: U): boolean called just before every iteration and if returns true the whole operation terminates.
@param {Object} [Context=null] - Optional. The context (this) to use when calling the callback functions.
@returns {Promise} Returns a promise with the last item when all items are done, or in the first rejection.
*/
tp.Async.Chain = async function (ShowSpinner, List, Func, BreakFunc = null, Context = null) {

    let Spinner = function (Flag) {
        if (ShowSpinner) {
            tp.ShowSpinner(Flag);
        }
    };

    Spinner(true);

    let ReduceFunc = (promise, current) => {
        return promise.then((value) => {
            if (tp.IsFunction(BreakFunc) && tp.Call(BreakFunc, Context, value))
                return promise;
            return tp.Call(Func, Context, current);
        });
    };

    let InitialValue = Promise.resolve(null);

    let Result = List.reduce(ReduceFunc, InitialValue);

    if (ShowSpinner) {
        Result.then((value) => {
            Spinner(false);
            return value;
        });
    }

    return Result;
};
/**
Executes multiple promise calls simultaneously, using Promise.all() and returns a promise when all items are done, or in the first rejection.   
@example
// Info = { Index: number, Flag: boolean }

let Counter = 0;
let ArgsList: IInfo[] = [{ Id: 0 }, { Id: 1 }, { Id: 2 }];

let Func = function (Info) {
    return new Promise<IInfo>((resolve, reject) => {
        let timeout = tp.Random(500, 5000);
        setTimeout(() => {
            Info.Index = Counter++;
            log('Id: ' + Info.Id + ' Index: ' + Info.Index + ' Timeout: ' + timeout);
            //if (Info.Index === 2)
            //    throw 'I dont like 2';
            resolve(Info);
        }, timeout);

    });
}; 

let Result = tp.Async.All(true, ArgsList, Func);
@param {boolean} ShowSpinner - True to show the global spinner while processing.
@param {any[]} List - An array of values. Each value is passed in the provided callback function.
@param {function} Func - A callback <code>function(v: T): Promise<U></code> which gets one of the array values and executes inside a promise.
@param {object} Context=null - Optional. The context (this) to use when calling the callback functions.
@returns {Promise} Returns a promise after all items are processed succesfully, or in the first rejection.
*/
tp.Async.All = async function (ShowSpinner, List, Func, Context = null) {

    let Spinner = function (Flag) {
        if (ShowSpinner) {
            tp.ShowSpinner(Flag);
        }
    };

    Spinner(true);

    var A = List.map((Item) => {
        return tp.Call(Func, Context, Item);
    });

    var Result = Promise.all(A);

    if (ShowSpinner) {
        Result.then((value) => {
            Spinner(false);
            return value;
        });
    }

    return Result;
};


//#endregion

//---------------------------------------------------------------------------------------

//#region Classes

//#region tp.StringBuilder

/** A class for constructing strings. The default line break is set to '\n' */
tp.StringBuilder = class StringBuilder {
    /**
     * A class for constructing strings. The default line break is set to '\n'
     * @param {string} LineBreak - Optional.  The line break to use. Defaults to \n .
     */
    constructor(LineBreak = '\n') {
        this.fData = '';
        this.fLB = LineBreak || '\n';
    }

    /* properties */
    /** Returns the length of the internal string 
     */
    get Length() { return this.fData.length; }
    /** True if the internal string is empty 
    */
    get IsEmpty() { return this.fData.length === 0; }
    /**
    Gets or sets the line break. Defaults to '\n'
    */
    get LineBreak() { return this.fLB; }
    set LineBreak(v) { this.fLB = v; }

    /* public */
    /**  
    Sets the internal string to an empty string.
    */
    Clear() {
        this.fData = '';
    }
    /**
    Appends a value
    @param {any} v - The value to append. 
    */
    Append(v) {
        if (tp.IsString(v))
            this.fData += v.toString();
    }
    /**
    Appends a value and a line break
    @param {any} v - Optional. The value to append. If not specified a line break is added.
    */
    AppendLine(v) {
        if (tp.IsString(v)) {
            this.fData += v.toString();
        }

        this.fData += this.LineBreak;
    }
    /**
    Inserts a value at a specified index in the internal string
    @param {number} Index - The index in the internal string.
    @param {any} v - The value to append. 
    */
    Insert(Index, v) {
        if (tp.IsValid(v)) {
            this.fData = tp.InsertText(v.toString(), this.fData, Index);
        }
    }
    /**
    Replaces a value with another value in the internal string
    @param {string} OldValue - The string to be replace.
    @param {string} NewValue - The replacer string.
    @param {boolean} CI - CI (Case-Insensitive) can be true (the default) or false
    */
    Replace(OldValue, NewValue, CI = true) {
        this.fData = tp.ReplaceAll(this.fData, OldValue, NewValue, CI);
    }
    /** 
    Returns the internal string 
    @returns {string} - Returns the internal string.
    */
    ToString() {
        return this.fData;
    }
};
//#endregion

//#region tp.Listener
/** A listener class. A listener requires a callback function, at least, and perhaps a context (this) object for the call. */
tp.Listener = class {

    /**
    Constructor
    @param {function} [Func=null] - The callback function
    @param {object} [Context=null] - The context (this) of the callback function
    */
    constructor(Func = null, Context = null) {
        this.Func = Func;
        this.Context = Context;
    }

    // NOTE: Firefox and Edge do not support fields yet. 
};
/** The callback function 
 * @type {function}
 */
tp.Listener.prototype.Func = null;
/** The context (this) of the callback function 
 * @type {object}
 */
tp.Listener.prototype.Context = null;
//#endregion

//#endregion

//---------------------------------------------------------------------------------------

//#region Culture related methods

/**
 * Returns the decimal separator of a specified culture, i.e. en-US
 * @param {string} [CultureCode] The culture code, i.e. en-US. If not specified then the current culture is used.
 * @return {string} Returns the decimal separator of a specified culture, i.e. en-US
 */
tp.GetDecimalSeparator = (CultureCode) => {
    if (!tp.IsString(CultureCode) || tp.IsBlank(CultureCode))
        CultureCode = tp.CultureCode;

    let n = 1.1;
    let Result = n.toLocaleString(CultureCode).substring(1, 2);
    return Result;
};
/**
 * Returns the thousand separator of a specified culture, i.e. en-US
 * @param {string} [CultureCode] The culture code, i.e. en-US. If not specified then the current culture is used.
 * @return {string} Returns the thousand separator of a specified culture, i.e. en-US
 */
tp.GetThousandSeparator = (CultureCode) => {
    if (!tp.IsString(CultureCode) || tp.IsBlank(CultureCode))
        CultureCode = tp.CultureCode;

    let n = 1000;
    let Result = n.toLocaleString(CultureCode).substring(1, 2);
    return Result;
};
/**
 * Returns the date separator of a specified culture, i.e. en-US
 * @param {string} [CultureCode] The culture code, i.e. en-US. If not specified then the current culture is used.
 * @returns {string} Returns the date separator of a specified culture, i.e. en-US
 */
tp.GetDateSeparator = (CultureCode) => {
    if (!tp.IsString(CultureCode) || tp.IsBlank(CultureCode))
        CultureCode = tp.CultureCode;

    let S = new Date().toLocaleDateString(CultureCode);

    if (S.indexOf('/') !== -1) {
        return '/';
    } else if (S.indexOf('.') !== -1) {
        return '.';
    }

    return '-';
};
/**
 * Returns the date format, i.e. dd/MM/yyyy or MM/dd/YYYY, of a specified culture, i.e. en-US
 * @param {string} [CultureCode] The culture code, i.e. en-US. If not specified then the current culture is used.
 * @returns {string} Returns the date format, i.e. dd/MM/yyyy or MM/dd/YYYY, of a specified culture, i.e. en-US
 */
tp.GetDateFormat = (CultureCode) => {
    if (CultureCode === 'ISO')
        return tp.DateFormatISO;

    if (!tp.IsString(CultureCode) || tp.IsBlank(CultureCode))
        CultureCode = tp.CultureCode;

    let i, ln;
    let DateSeparator = tp.GetDateSeparator(CultureCode);

    let DT = new Date('2000-10-15');
    let S = DT.toLocaleDateString(CultureCode, { year: 'numeric', month: '2-digit', day: '2-digit' });

    let Parts = S.split(DateSeparator);

    for (i = 0, ln = Parts.length; i < ln; i++)
        Parts[i] = Parts[i].trim();

    for (i = 0, ln = Parts.length; i < ln; i++) {
        if (Parts[i] === '2000') {
            Parts[i] = 'yyyy';
        }
        if (Parts[i] === '10') {
            Parts[i] = 'MM';
        }
        if (Parts[i] === '15') {
            Parts[i] = 'dd';
        }
    }

    return Parts.join(DateSeparator);

};
/**
 *  Returns a {@link tp.DatePattern} constant value by analyzing the DateFormat.
 * @param {string} DateFormat The date format string to analyze, e.g. yyyy-MM-dd or MM/dd/YYYY
 * @returns {number} Returns a {@link tp.DatePattern} constant value by analyzing the DateFormat.
 */
tp.GetDatePattern = function (DateFormat) {
    let Result = tp.DatePattern.DMY;
    DateFormat = DateFormat.trim();
    let C = DateFormat.charAt(0).toUpperCase();

    if (C === 'Y')
        Result = tp.DatePattern.YMD;
    else if (C === 'M')
        Result = tp.DatePattern.MDY;
    else
        Result = tp.DatePattern.DMY;

    return Result;
};


//#endregion

//#region tp.CultureCode

/** Gets or sets the current culture, i.e. locale. By default returns 'en-US'. <br />
 * The initial value of this property comes from lang attribute of the html element, e.g. <html lang="en-US"> <br />
@type {string}
*/
tp.CultureCode = null;
 

//#endregion

//---------------------------------------------------------------------------------------

//#region tp Properties and Constants

/** Line Break    
 * WARNING: .Net line break = \r\n */
tp.LB = '\n';
tp.SPACE = ' ';

/** The undefined constant as a tp constant.
 @see {@link http://stackoverflow.com/questions/7452341/what-does-void-0-mean}
 @type {undefined}
 */
tp.Undefined = void 0;
Object.defineProperty(tp, 'Undefined', {
    get() { return void 0; }
});

/** The document the script operates on */
tp.Doc = window.frameElement ? window.top.document : window.document;

/** The currently active element 
 @type {Element}
 */
tp.ActiveElement = null;
Object.defineProperty(tp, 'ActiveElement', {
    get() { return tp.Doc.activeElement; }
});


/** A global object for keeping the urls used by a javascript application in ajax and other calls. */
tp.Urls = {};
tp.Urls.AjaxExecute = '/Ajax/Execute';
tp.Urls.AjaxRequest = '/Ajax/Request';

/**
The system configuration global object
@class
*/
tp.SysConfig = {};
tp.SysConfig.DebugMode = false;
tp.SysConfig.GlobalErrorHandling = false;

Object.defineProperty(tp, 'DebugMode', {
    get() { return tp.SysConfig.DebugMode === true; }
});

//#endregion

//---------------------------------------------------------------------------------------

//#region Document Ready State

/** Returns true if the document is loaded and ready (readyState === 'complete') */
tp.Property('IsReady', tp, () => tp.Doc.readyState === "complete");
/** For internal use. A list of {@link tp.Listener} objects to be called when the document is loaded and ready. 
 * @type {tp.Listener[]}
 */
tp.ReadyListeners = [];
/**
Adds a listener to the document.onreadystatechange event.
@param {function} Func - The callback function
@param {object} Context - The context (this) of the callback function
*/
tp.AddReadyListener = function (Func, Context = null) {
    var Listener = new tp.Listener(Func, Context);
    tp.ReadyListeners.push(Listener);
};
/**
Executes a specified callback function when the document is loaded and ready.
@param {function} Func - The function to call when document is loaded and ready.
*/
tp.Ready = function (Func) {
    tp.AddReadyListener(Func);
};
/** Just a placeholder. Client code may re-assign this property. 
 * NOTE: It is executed before any ready listeners.
 */
tp.AppInitializeBefore = function () { };
/** Just a placeholder. Client code may re-assign this property.
 * NOTE: It is executed after any ready listeners.
 * */
tp.AppInitializeAfter = function () { };
/** Just a placeholder. Client code may re-assign this property.
 * NOTE: It is executed after the AppInitializeAfter()
 * */
tp.Main = function () { };

/** Just a placeholder for a function that adds languages. */
tp.AddLanguagesFunc = null;

(function () {

    let InitializeCulture = () => {
        let CultureCode = document.querySelector('html').getAttribute('lang');

        if (!tp.IsString(CultureCode) || tp.IsBlank(CultureCode))
            CultureCode = 'en-US';
 
        if (CultureCode.length < 5 || CultureCode[2] !== '-')
            tp.Throw(`Invalid culture code: ${CultureCode} \nPlease define a Culture in the html lang attribute. Example lang="en-US"`);

        tp.CultureCode = CultureCode;
    };

    let ReadyFunc = async () => {
        let List;

        InitializeCulture();
        tp.Viewport.Initialize();

        if (tp.IsFunction(tp.AppInitializeBefore))
            tp.Call(tp.AppInitializeBefore);

        // call "ready listeners"
        List = tp.ReadyListeners;
        let listener;
        for (var i = 0, ln = List.length; i < ln; i++) {
            listener = List[i];
            listener.Func.call(listener.Context);
        }

        if (typeof tp.AppInitializeAfter === 'function')
            tp.Call(tp.AppInitializeAfter);

        // call Main()
        if (typeof tp.Main === 'function')
            tp.Call(tp.Main);
    };

    if (tp.IsReady) {
        ReadyFunc();
    } else {
        tp.Doc.addEventListener('readystatechange', async (e) => {
            if (tp.IsReady) {
                ReadyFunc();
            }
        }, false);
    }

})();






//#endregion











 