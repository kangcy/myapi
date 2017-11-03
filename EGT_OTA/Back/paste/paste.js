var isIE = (navigator.appName.indexOf("Microsoft") != -1) ? true : false;
var bodyStyle = "margin:0;min-height:100px;padding:10px;word-break:break-all;";
var or, divTemp, originText, newData, objEditor, objEditorBody, edDoc, w;

function initEditor(html) {
    var ifrm = document.getElementById("editor")
    //ifrm.contentWindow.document.designMode = "On";
    if (base.IsNullOrEmpty(html)) {
        ifrm.contentWindow.document.write("<link rel=\"stylesheet\" href=\"mui.min.css\"><body style=\"" + bodyStyle + "\"><div class=\"edit f12 tl\" contenteditable=\"true\"></div><script src=\"../Scripts/base.min.js\" /></body>");
    } else {
        ifrm.contentWindow.document.write("<link rel=\"stylesheet\" href=\"mui.min.css\"><body style=\"" + bodyStyle + "\">" + html + "<script src=\"../Scripts/base.min.js\" /></body>");
    }
    ifrm.contentWindow.document.close();
    objEditor = document.getElementById("editor");
    w = objEditor.contentWindow;
    if (isIE) {
        ifrm.contentWindow.document.documentElement.attachEvent("onpaste", function (e) {
            return pasteClipboardData(ifrm.id, e);
        });
    } else {
        edDoc = objEditor.contentWindow.document;
        objEditorBody = objEditor.contentWindow.document.body.firstChild;
        ifrm.contentWindow.document.addEventListener("paste", function (e) {
            return pasteClipboardData(ifrm.id, e);
        }, false);
    }
}

initEditor("");

function getSel(w) {
    return w.getSelection ? w.getSelection() : w.document.selection;
}

function setRange(sel, r) {
    sel.removeAllRanges();
    sel.addRange(r);
}

//过滤Word内容
function filterPasteWord(str) {
    //remove link break
    str = str.replace(/\r\n|\n|\r/ig, "");
    //remove &nbsp; entities at the start of contents
    str = str.replace(/^\s*(&nbsp;)+/ig, "");
    //remove &nbsp; entities at the end of contents
    str = str.replace(/(&nbsp;|<br[^>]*>)+\s*$/ig, "");
    //Remove comments, scripts (e.g., msoShowComment), XML tag, VML content, MS Office namespaced tags, and a few other tags
    str = str.replace(/<(!|script[^>]*>.*?<\/script(?=[>\s])|\/?(\?xml(:\w+)?|xml|img|meta|link|style|\w:\w+)(?=[\s\/>]))[^>]*>/gi, "");
    //convert word headers to strong
    str = str.replace(/<p [^>]*class="?MsoHeading"?[^>]*>(.*?)<\/p>/gi, "<p><strong>$1</strong></p>");
    //remove lang attribute
    str = str.replace(/(lang)\s*=\s*([\'\"]?)[\w-]+\2/ig, "");
    //Examine all styles: delete junk, transform some, and keep the rest
    str = str.replace(/(<[a-z][^>]*)\sstyle="([^"]*)"/gi, function (str, tag, style) {
        var n = [],
			i = 0,
			s = style.trim().replace(/&quot;/gi, "'").split(";");

        for (var i = 0; i < s.length; i++) {
            v = s[i];
            var name, value,
				parts = v.split(":");

            if (parts.length == 2) {
                name = parts[0].toLowerCase();
                value = parts[1].toLowerCase();
                switch (name) {
                    case "mso-padding-alt":
                    case "mso-padding-top-alt":
                    case "mso-padding-right-alt":
                    case "mso-padding-bottom-alt":
                    case "mso-padding-left-alt":
                    case "mso-margin-alt":
                    case "mso-margin-top-alt":
                    case "mso-margin-right-alt":
                    case "mso-margin-bottom-alt":
                    case "mso-margin-left-alt":
                    case "mso-table-layout-alt":
                    case "mso-height":
                    case "mso-width":
                    case "mso-vertical-align-alt":
                        n[i++] = name.replace(/^mso-|-alt$/g, "") + ":" + ensureUnits(value);
                        continue;

                    case "horiz-align":
                        n[i++] = "text-align:" + value;
                        continue;

                    case "vert-align":
                        n[i++] = "vertical-align:" + value;
                        continue;

                    case "font-color":
                    case "mso-foreground":
                        n[i++] = "color:" + value;
                        continue;

                    case "mso-background":
                    case "mso-highlight":
                        n[i++] = "background:" + value;
                        continue;

                    case "mso-default-height":
                        n[i++] = "min-height:" + ensureUnits(value);
                        continue;

                    case "mso-default-width":
                        n[i++] = "min-width:" + ensureUnits(value);
                        continue;

                    case "mso-padding-between-alt":
                        n[i++] = "border-collapse:separate;border-spacing:" + ensureUnits(value);
                        continue;

                    case "text-line-through":
                        if ((value == "single") || (value == "double")) {
                            n[i++] = "text-decoration:line-through";
                        }
                        continue;

                    case "mso-zero-height":
                        if (value == "yes") {
                            n[i++] = "display:none";
                        }
                        continue;
                }
                if (/^(mso|column|font-emph|lang|layout|line-break|list-image|nav|panose|punct|row|ruby|sep|size|src|tab-|table-border|text-(?:align|decor|indent|trans)|top-bar|version|vnd|word-break)/.test(name)) {
                    continue;
                }
                n[i++] = name + ":" + parts[1];
            }
        }
        // If style attribute contained any valid styles the re-write it; otherwise delete style attribute.
        if (i > 0) {
            return tag + ' style="' + n.join(';') + '"';
        } else {
            return tag;
        }
    });

    return str;
}

//过滤粘贴内容
function filterPasteText(str) {
    str = str.replace(/<\/?[^>]*>/g, ''); //去除HTML
    str.value = str.replace(/[ | ]*\n/g, '\n'); //去除行尾空白
    str = str.replace(/\n[\s| | ]*\r/g, '\n'); //去除多余空行
    return str;
}

//是否从word中粘贴
function isWordDocument(strValue) {
    var re = new RegExp(/(class=\"?Mso|style=\"[^\"]*\bmso\-|w:WordDocument)/ig);
    return re.test(strValue);
}

//过滤粘贴
function filterPasteData(originalText) {
    if (isWordDocument(originalText)) {
        originalText = filterPasteWord(originalText);
    }
    return filterPasteText(originalText);
}

function block(e) {
    e.preventDefault();
}

function pasteClipboardData(editorId, e) {
    if (isIE) {
        var orRange = objEditor.contentWindow.document.selection.createRange();

        var ifmTemp = document.getElementById("ifmTemp");
        if (!ifmTemp) {
            ifmTemp = document.createElement("IFRAME");
            ifmTemp.id = "ifmTemp";
            ifmTemp.style.width = "1px";
            ifmTemp.style.height = "1px";
            ifmTemp.style.position = "absolute";
            ifmTemp.style.border = "none";
            ifmTemp.style.left = "-10000px";
            ifmTemp.src = "iframeblankpage.html";
            document.body.appendChild(ifmTemp);
            ifmTemp.contentWindow.document.designMode = "On";
            ifmTemp.contentWindow.document.open();
            ifmTemp.contentWindow.document.write("<link rel=\"stylesheet\" href=\"mui.min.css\"><body style=\"" + bodyStyle + "\"><div tabindex=\"-1\" class=\"edit f12 tl\"></div><script src=\"../Scripts/base.min.js\" /></body>");
            ifmTemp.contentWindow.document.close();
        } else {
            ifmTemp.contentWindow.document.body.firstChild.innerHTML = "";
        }

        originText = objEditorBody.innerText;

        ifmTemp.contentWindow.focus();
        ifmTemp.contentWindow.document.execCommand("Paste", false, null);
        objEditor.contentWindow.focus();

        newData = ifmTemp.contentWindow.document.body.firstChild.innerHTML;
        newData = filterPasteData(newData);
        ifmTemp.contentWindow.document.body.firstChild.innerHTML = newData;
        orRange.pasteHTML(newData);
        if (e) {
            e.returnValue = false;
            if (e.preventDefault)
                e.preventDefault();
        }
        return false;
    } else {
        enableKeyDown = false;
        var divTemp = edDoc.createElement("DIV");
        divTemp.id = 'htmleditor_tempdiv';
        divTemp.innerHTML = '\uFEFF';
        divTemp.style.left = "-10000px";
        divTemp.style.height = "1px";
        divTemp.style.width = "1px";
        divTemp.style.position = "absolute";
        divTemp.style.overflow = "hidden";
        objEditorBody.appendChild(divTemp);
        edDoc.addEventListener("mousedown", block, false);
        edDoc.addEventListener("keydown", block, false);
        enableKeyDown = false;
        or = getSel(w).getRangeAt(0);

        var docBody = divTemp.firstChild;
        rng = edDoc.createRange();
        rng.setStart(docBody, 0);
        rng.setEnd(docBody, 1);
        setRange(getSel(w), rng);

        originText = edDoc.body.textContent;
        if (originText === '\uFEFF') {
            originText = "";
        }

        window.setTimeout(function () {
            if (divTemp.innerHTML === '\uFEFF') {
                newData = "";
                objEditorBody.removeChild(divTemp);
                return;
            }

            newData = divTemp.innerHTML;
            if (or) {
                setRange(getSel(w), or);
            }
            newData = filterPasteData(newData);
            divTemp.innerHTML = newData;

            if (objEditorBody.childNodes.length <= 1) {
                objEditorBody.innerHTML = newData;
            } else {
                edDoc.execCommand('insertHtml', false, newData);
                edDoc.body.firstChild.removeChild(divTemp);
            }
        }, 0);
        enableKeyDown = true;
        edDoc.removeEventListener("mousedown", block, false);
        edDoc.removeEventListener("keydown", block, false);
        return true;
    }
}

//选中文字父节点
function ShowParentNode() {
    var tbl = [];
    var direct = (objEditor.contentWindow.document.selection && objEditor.contentWindow.document.selection.createRange) ? objEditor.contentWindow.document.selection.createRange().parentElement() : objEditor.contentWindow.getSelection().focusNode.parentNode;
    do {
        tbl.push(direct.tagName);
    }
    while ((direct = direct.parentNode) && (direct !== objEditor.contentWindow.document.documentElement));
    return tbl.reverse();
}