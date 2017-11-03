var ArticleID = 0;
var ArticleNumber = ""; //文章编号
var PartID = 0;
var SelectAll = false; //是否全选
var userinfo = base.GetUserInfo();
var initWidth = 500;
var initHeight = 500;
var mask = base.CreateMask(false, function () {
    base.CloseWaiting();
});

//编辑变动缓存
var ActionCache = [];
var CurrActionCache = -1;
var $editor = base.Get("editor");

mui.ready(function () {
    var limit = initWidth / 9;
    mui.each(mui(".menu"), function (i, item) {
        item.style.width = limit + "px";
    })
    base.Get("align_first").style.marginRight = limit * 2 + "px";
    base.Get("fs_first").style.marginRight = limit * 2 + "px";
    base.Get("bold_first").style.marginRight = limit * 4 + "px";

    //全选 
    base.Get("all").addEventListener('tap', function (event) {
        objEditorBody.focus();
        if (this.classList.contains("select")) {
            SelectAll = false;
            this.classList.remove("select");
            edDoc.execCommand("Unselect");
        } else {
            SelectAll = true;
            this.classList.add("select");
            edDoc.execCommand("SelectAll");
        }
    }, false);

    //上一步
    base.Get("menuPrev").addEventListener('tap', function (event) {
        if (isLoading) {
            return
        }
        isLoading = true;
        if (CurrActionCache > 0) {
            CurrActionCache -= 1;
            objEditorBody.innerHTML = ActionCache[CurrActionCache];
        } else {
            CurrActionCache = 0;
        }
        isLoading = false;
    }, false);

    //下一步 
    base.Get("menuNext").addEventListener('tap', function (event) {
        if (isLoading) {
            return
        }
        isLoading = true;
        if (CurrActionCache < ActionCache.length - 1) {
            CurrActionCache += 1;
            objEditorBody.innerHTML = ActionCache[CurrActionCache];
        } else {
            CurrActionCache = ActionCache.length - 1;
        }
        isLoading = false;
    }, false);

    //字体大小
    base.Get("menuSize").addEventListener('tap', function (event) {
        base.AddClass(["#color", "#bgcolor", "#align", "#bold"], "hide");
        base.Get("fs").classList.toggle("hide");
    }, false);

    //颜色 
    base.Get("menuColor").addEventListener('tap', function (event) {
        base.AddClass(["#bgcolor", "#align", "#bold", "#fs"], "hide");
        base.Get("color").classList.toggle("hide");
    }, false);

    //背景色
    base.Get("menuBgColor").addEventListener('tap', function (event) {
        base.AddClass(["#color", "#align", "#bold", "#fs"], "hide");
        base.Get("bgcolor").classList.toggle("hide");
    }, false);

    //加粗
    base.Get("menuBold").addEventListener('tap', function (event) {
        objEditorBody.focus();
        var node = ShowParentNode();
        base.RemoveClass(["#btnBold", "#btnItalic", "#btnUnderline"], "curr");
        mui.each(node, function (i, item) {
            if (item == "B") {
                base.Get("btnBold").classList.add("curr");
            }
            if (item == "I") {
                base.Get("btnItalic").classList.add("curr");
            }
            if (item == "U") {
                base.Get("btnUnderline").classList.add("curr");
            }
        })
        base.AddClass(["#color", "#bgcolor", "#align", "#fs"], "hide");
        base.Get("bold").classList.toggle("hide");

    }, false);

    //颜色切换
    mui('#color').on('tap', 'div', SetStyle);

    //背景色切换
    mui('#bgcolor').on('tap', 'div', SetStyle);

    //大小切换
    mui('#fs').on('tap', 'div', SetStyle);

    //对齐
    base.Get("menuAlign").addEventListener('tap', function (event) {
        base.AddClass(["#color", "#bgcolor", "#bold", "#fs"], "hide");
        base.Get("align").classList.toggle("hide");
    }, false);

    //加粗
    base.Get("btnBold").addEventListener('tap', SetStyle, false);

    //斜体
    base.Get("btnItalic").addEventListener('tap', SetStyle, false);

    //下划线
    base.Get("btnUnderline").addEventListener('tap', SetStyle, false);

    //居左
    base.Get("btnLeft").addEventListener('tap', function () {
        edDoc.execCommand("justifyLeft");
        InitAlign(this, "tl");
    }, false);

    //居中
    base.Get("btnCenter").addEventListener('tap', function () {
        edDoc.execCommand("justifyCenter");
        InitAlign(this, "tc");
    }, false);

    //居右
    base.Get("btnRight").addEventListener('tap', function () {
        edDoc.execCommand("justifyRight");
        InitAlign(this, "tr");
    }, false);

    //超链接
    base.Get("menuLink").addEventListener('tap', SetStyle, false);
})

//初始化对齐
function InitAlign(item, name) {
    base.RemoveClass(["#btnLeft", "#btnCenter", "#btnRight"], "curr");
    item.classList.add("curr");
    objEditorBody.classList.remove("tl");
    objEditorBody.classList.remove("tc");
    objEditorBody.classList.remove("tr");

    objEditorBody.classList.add(name);
    InsertPush();
}

//打开编辑
function ShowLink() {
    base.OpenWindow("edithref", "edithref.html", {
        Link: "",
        Text: ""
    });
}

//编辑超链接
function UpdateLink(link) {
    if (!base.IsNullOrEmpty(link)) {
        ChangeLink(link);
    }
    console.log(link)
}

//撤销链接
function Undo() {
    edDoc.execCommand('undo');
}

//改变样式 
function SetStyle(e) {
    document.activeElement.blur();

    var that = e.target;
    var type = this.getAttribute("data-type") || "";
    var valueTxt = this.getAttribute("data") || "";
    if (that.id === "menuLink") {
        valueTxt = "placeholder";
        ShowLink();
    };
    edDoc.execCommand(type, false, valueTxt);
    if (this.classList.contains("fs")) {
        base.RemoveClass([".fs"], "curr");
        ChangeFontSize();
    } else {
        if (that.id !== "menuLink") {
            this.classList.toggle("curr")
        }
    }
    if (type == "BackColor") {
        this.classList.toggle("inline")
    }
    if (that.id !== "menuLink") {
        InsertPush();
    }
};

//超链接替换
function ChangeLink(link) {
    mui.each(objEditorBody.getElementsByTagName("a"), function (i, node) {
        if (node.getAttribute("href") === "placeholder") {
            node.setAttribute("href", "#");
            node.setAttribute("link", link);
            node.classList.add("link");
        };
    });
};

//字体大小替换
function ChangeFontSize() {
    var size = "";
    if (SelectAll) {
        mui.each(objEditorBody.getElementsByClassName("f12"), function (i, node) {
            node.classList.remove("f12");
        });
        mui.each(edDoc.getElementsByClassName("f14"), function (i, node) {
            node.classList.remove("f14");
        });
        mui.each(edDoc.getElementsByClassName("f20"), function (i, node) {
            node.classList.remove("f20");
        });
        mui.each(edDoc.getElementsByClassName("f24"), function (i, node) {
            node.classList.remove("f24");
        });
        mui.each(edDoc.getElementsByClassName("f36"), function (i, node) {
            node.classList.remove("f36");
        });
    }
    mui.each(objEditorBody.getElementsByTagName("font"), function (i, node) {
        if (node.hasAttribute("size")) {
            size = node.getAttribute("size");
            node.classList.remove("f12");
            node.classList.remove("f14");
            node.classList.remove("f20");
            node.classList.remove("f24");
            node.classList.remove("f36");
            switch (size) {
                case "3":
                    node.classList.add("f12");
                    break;
                case "4":
                    node.classList.add("f14");
                    break;
                case "5":
                    node.classList.add("f20");
                    break;
                case "6":
                    node.classList.add("f24");
                    break;
                case "7":
                    node.classList.add("f36");
                    break;
                default:
                    break;
            }
            node.removeAttribute("size");
        }
    })
}

//添加缓存
function InsertPush() {
    ActionCache.push(objEditorBody.outerHTML);
    CurrActionCache = ActionCache.length - 1;
}


//初始化文字编辑
function InitEditText(html) {
    layer.open({
        type: 1,
        closeBtn: 1,
        skin: 'layui-layer-rim',
        area: ['500px', '500px'],
        title: "编辑",
        content: $('#editText'),
        btn: ["保存修改", "取消保存"],
        yes: function (index, layero) {
            if (isLoading) {
                return;
            }
            isLoading = true;
            var val = objEditorBody.innerHTML;
            if (base.IsNullOrEmpty(val)) {
                isLoading = false;
                layer.msg("请编辑文字");
                return false;
            }
            layer.load();
            $.ajax({
                url: rooturl + "System/CheckDirty",
                data: { Title: escape(val) },
                type: "post",
                dataType: "json",
                success: function (data) {
                    if (data != null) {
                        if (data.result) {
                            objEditorBody.removeAttribute("contenteditable");
                            AddText((currEditPartID == 0 ? base.GetUid() : currEditPartID), escape(objEditorBody.outerHTML), currEditPartID);
                        } else {
                            layer.msg(data.message);
                        }
                    }
                    isLoading = false;
                }
            });
            layer.closeAll('loading');
            layer.close(index);
            base.AddClass(["#color", "#bgcolor", "#bold", "#fs", "#align"], "hide");
            base.RemoveClass([".curr"], "curr");
        },
        cancel: function (index, layero) {
            //取消保存
            layer.confirm('确定要取消当前修改吗？', {
                title: false,
                btn: ['确定', '取消']
            }, function () {
                layer.closeAll();
                base.AddClass(["#color", "#bgcolor", "#bold", "#fs", "#align"], "hide");
                base.RemoveClass([".curr"], "curr");
            }, function (index) {
                layer.close(index);
            });
            return false;
        }
    });
    if (base.IsNullOrEmpty(html)) {
        initEditor('');
    } else {
        var div = document.createElement("div");
        div.innerHTML = html;
        div.childNodes[0].setAttribute("contenteditable", true);
        initEditor(div.innerHTML)
    }
    InsertPush();
    objEditorBody.focus();
    if (objEditorBody.classList.contains("tl")) {
        base.Get("menuAlign").classList.add("menuLeft");
    } else if (objEditorBody.classList.contains("tc")) {
        base.Get("menuAlign").classList.add("menuCenter");
    } else if (objEditorBody.classList.contains("tr")) {
        base.Get("menuAlign").classList.add("menuRight");
    } else {
        base.Get("menuAlign").classList.add("menuLeft");
    }
    base.Get("menuAlign").classList.remove("hide");
    isloading = false;
}