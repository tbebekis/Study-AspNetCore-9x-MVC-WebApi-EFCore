var site = site || {};

/**
 * Merges a key-value pair into the existing query string and immediately navigates to the current url with that query string
 * @param {string} Key The key
 * @param {any} Value The value
 */
site.MergeToQuery = function (Key, Value) {
    var Data = tp.GetParams() || {};

    if (tp.IsPlainObject(Key)) {
        for (var Prop in Key)
            Data[Prop] = Key[Prop];
    }
    else {
        if (tp.IsEmpty(Value)) {
            if (Key in Data)
                delete Data[Key];
        } else {
            Data[Key] = Value;
        }

        // remove the page-number when the filter changes
        if (Key !== 'pagenumber') {
            if ('pagenumber' in Data)
                delete Data['pagenumber'];
        }
    }

    var Params = [];
    for (var Prop in Data)
        Params.push(Prop + '=' + Data[Prop]);

    var S = Params.length > 0 ? '?' + Params.join('&') : null;

    var Url = window.location.pathname;
    if (S)
        Url += S;

    window.location = Url;
};