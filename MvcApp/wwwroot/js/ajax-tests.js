function DisplayAjaxResponse(Text, TagType = "pre") {
    let el = document.getElementById("ajax-results");
    el.innerHTML = "";

    let el2 = tp.el(el, TagType);
    el2.innerHTML = Text;
}

function PlainAjaxCall() {
 
    let XHR = new XMLHttpRequest();
 
    let Url = '/Ajax/PlainAjaxCall';
    XHR.open("POST", Url, true);        

    XHR.setRequestHeader('Content-Type', 'application/json; charset=UTF-8');
    XHR.setRequestHeader("Accept", "*/*");
    XHR.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
 
    XHR.onload = function (e) {
        if (XHR.readyState === XMLHttpRequest.DONE) {
            let Succeeded = (XHR.status === 0 || XHR.status >= 200 && XHR.status < 300);              
            if (Succeeded === true) {
                let ResponseText = XHR.responseText;
                DisplayAjaxResponse(ResponseText);
            }
        }
    };
    XHR.onerror = function (e) {
        let ResponseText = XHR.responseText;
        DisplayAjaxResponse(ResponseText);     
    };

    let Model = {
        Message: "JavaScript model from PlainAjaxCall()",
        Value: 123.45
    };

    Model = JSON.stringify(Model);

    XHR.send(Model);
}

async function Ajax_PostModelAsync() {
    let Model = {
        Message: "JavaScript model from Ajax_PostModelAsync()",
        Value: 123.45
    };

    let Url = '/Ajax/Ajax_PostModelAsync';
    let Args = await tp.Ajax.PostModelAsync(Url, Model);

    let JsonText = tp.ToJson(Args, true);
    DisplayAjaxResponse(JsonText);
}

async function AjaxRequest_OperationName() {
    let OperationName = "AjaxRequest_OperationName";

    let Params = {
        Message: "JavaScript model from AjaxRequest_OperationName()",
        Value: 123.45
    }
    let Args = await tp.AjaxRequest.Execute(OperationName, Params);
    
    let JsonText = tp.ToJson(Args, true);  
    DisplayAjaxResponse(JsonText);
}

async function AjaxRequest_Model() {
    let Model = {
        Message: "JavaScript model from AjaxRequest_Model()",
        Value: 123.45
    };

    let OperationName = "AjaxRequest_Model";
    let Request = new tp.AjaxRequest(OperationName, Model);

    let Args = await tp.AjaxRequest.Execute(Request);

    let JsonText = tp.ToJson(Args, true);      

    DisplayAjaxResponse(JsonText);
}

async function AjaxRequest_View() {
    let OperationName = "AjaxDemoView";
    let Request = new tp.AjaxRequest(OperationName);
    Request.Type = tp.AjaxRequestType.Ui;

    let Args = await tp.AjaxRequest.Execute(Request);
    let HtmlText = Args.Packet.HtmlText;

    DisplayAjaxResponse(HtmlText, "div");
}

