(function(doc, win) {
	var docEl = doc.documentElement,
		resizeEvt = 'orientationchange' in window ? 'orientationchange' : 'resize',
		recalc = function() {
			var clientWidth = document.getElementsByTagName("html")[0].clientWidth - document.getElementsByTagName("html")[0].offsetLeft * 2;
			if(!clientWidth) return;
			docEl.style.fontSize = 16 * (clientWidth / 320) + 'px';
			docEl.style.width = '100%';
			docEl.style.height = '100%';
		};
	if(!doc.addEventListener) {
		return;
	}
	win.addEventListener(resizeEvt, recalc, false);
	doc.addEventListener('DOMContentLoaded', recalc, false);
	(function() {
		return;
		var dpr = scale = 1;
		var isIPhone = win.navigator.appVersion.match(/iphone/gi);
		var devicePixelRatio = win.devicePixelRatio;
		if(isIPhone) {
			// iOS下，对于2和3的屏，用2倍的方案，其余的用1倍方案
			if(devicePixelRatio >= 3 && (!dpr || dpr >= 3)) {
				dpr = 3;
			} else if(devicePixelRatio >= 2 && (!dpr || dpr >= 2)) {
				dpr = 2;
			} else {
				dpr = 1;
			}
		} else {
			dpr = 1;
		}
		scale = 1 / dpr;
		var metaEl = "";
		metaEl = doc.createElement('meta');
		metaEl.setAttribute('name', 'viewport');
		metaEl.setAttribute('content', 'initial-scale=' + scale + ', maximum-scale=' + scale + ', minimum-scale=' + scale + ', user-scalable=no');
		if(docEl.firstElementChild) {
			docEl.firstElementChild.appendChild(metaEl);
		} else {
			var wrap = doc.createElement('div');
			wrap.appendChild(metaEl);
			doc.write(wrap.innerHTML);
		}
	})();

})(document, window);

function GetParameter(param) {
	var query = window.location.search;
	query = doFilter(query);
	var iLen = param.length;
	var iStart = query.indexOf(param);
	if(iStart == -1) {
		return "";
	}
	iStart += iLen + 1;
	var iEnd = query.indexOf("&", iStart);
	if(iEnd == -1) {
		return query.substring(iStart);
	}
	return query.substring(iStart, iEnd);
};

function doFilter(query) {
	var queryParam = query.split('?');
	if(queryParam.length > 1) {
		var _index = query.indexOf("?") + 1;
		var queryBefore = query.substr(0, _index);
		var queryAfter = query.substr(_index);
		queryAfter = queryAfter.replace("?", "&");
		query = queryBefore + queryAfter;
	}
	return query;
};

function Download() {
	window.location.href = "http://app.qq.com/#id=detail&appid=1106027124";
};

function CheckPowerPwd(articleid, pwd, callback, errorcallback) {
	mui.getJSON(RootUrl, {
		aid: articleid,
		pwd: pwd,
		action: "checkpwd"
	}, function(data) {
		if(data != null) {
			if(data.result) {
				if(callback) {
					callback();
				}
				return;
			}
		}
		mui.toast("解锁失败");
		if(errorcallback) {
			errorcallback();
		}
	})
};

function GetRootUrl() {
	var pathName = window.location.pathname.substring(1);
	var webName = pathName == '' ? '' : pathName.substring(0, pathName.indexOf('/'));
	if(webName == "" || webName.toLowerCase() == "home") {
		return window.location.protocol + '//' + window.location.host;
	} else {
		return window.location.protocol + '//' + window.location.host + '/' + webName;
	}
}

function UnicodeText(str) {
	return escape(str).toLocaleLowerCase().replace(/%u/gi, '\\u');
};

function UnUnicodeText(str) {
	if(base.IsNullOrEmpty(str)) {
		return "";
	}
	return unescape(str.replace(/\\u/gi, '%u'));
};

/**
 * 动态加载JS 
 * url：JS地址
 * callback：回调方法
 */
function LoadScript(url, callback) {
	var script = document.createElement("script");
	script.type = "text/javascript";
	script.src = url;
	document.body.appendChild(script);
	if(typeof(callback) != "undefined") {
		if(script.readyState) {
			script.onreadystatechange = function() {
				if(script.readyState == "loaded" || script.readyState == "complete") {
					script.onreadystatechange = null;
					callback();
				}
			};
		} else {
			script.onload = function() {
				callback();
			};
		}
	}
};

//播放器
var audio = null;
var timestamp = 0;

function switchsound() {
	if(audio == null) {
		audio = document.createElement('audio');
		audio.id = 'bgsound';
		audio.loop = 'loop';
		audio.innerHTML = '<source src="' + (MusicUrl + "?rn=" + new Date().getTime().toString()) + '" type="audio/mpeg">';
		document.body.appendChild(audio);
	}

	if(audio.paused) {
		audio.play();
		document.getElementById(MusicID).setAttribute("play", "stop");
	} else if(new Date().getTime() > timestamp + 1000) {
		audio.pause();
		document.getElementById(MusicID).setAttribute("play", "on");
	}
};

function stopsound() {
	if(audio != null) {
		audio.pause();
	}
	document.getElementById(MusicID).setAttribute("play", "on");
};

function startsound() {
	document.removeEventListener('touchstart', startsound);
	document.removeEventListener('click', startsound);
	if(audio == null) {
		audio = document.createElement('audio');
		audio.id = 'bgsound';
		audio.src = MusicUrl;
		audio.loop = 'loop';
		document.body.appendChild(audio);
	}
	audio.play();
	document.getElementById(MusicID).setAttribute('play', 'stop');
	timestamp = new Date().getTime();
};

//懒加载图片
function LazyImg(item) {
	item.removeAttribute("onload");
	var img = new Image();
	var src = item.getAttribute("data-lazyload");
	img.src = src;
	if(img.complete) {
		item.setAttribute("src", src);
	} else {
		img.onload = function() {
			item.setAttribute("src", src);
		};
	}
};

//图片预加载
function LazyCover(url, callback) {
	var img = new Image();
	img.src = url;
	if(img.complete) {
		callback.call(img);
		return;
	}
	img.onload = function() {
		callback.call(img);
	};
};

var base = new function() {
	this.Get = function(name) {
		if(name.indexOf('.') < 0) {
			return mui("#" + name)[0];
		} else {
			return mui(name);
		}
	};
	this.ShowThumb = function(url, thumb) {
		if(base.IsNullOrEmpty(url)) {
			return "img/avatar.png";
		}
		if(url.indexOf('_0') < 0) {
			return url;
		}
		url = url.replace("_0", "_" + thumb);
		if(url.toString().toLowerCase().indexOf("http://") < 0) {
			url = "http://www.xiaoweipian.com:1010/" + url;
			//url = "http://localhost/app/" + url;
		}
		return url;
	};
	this.IsNullOrEmpty = function(str) {
		if(!str) {
			return true;
		}
		if(str == "" || str == null) {
			return true;
		}
		if(str.toString().toLowerCase() == "null") {
			return true;
		}
		return false;
	};
	this.InnerHtml = function(ids, html) {
		var model = null;
		mui.each(ids, function(index, item) {
			model = base.Get(item);
			if(model) {
				model.innerHTML = html;
			}
		});
	};
};

String.prototype.startWith = function(str) {
	var reg = new RegExp("^" + str);
	return reg.test(this);
}

String.prototype.endWith = function(str) {
	var reg = new RegExp(str + "$");
	return reg.test(this);
}

function ShowVideo(domid, vid) {
	player = new YKU.Player(domid, {
		client_id: "59bec9b0be1fc34b",
		styleid: '0',
		vid: vid,
		autoplay: false,
		show_related: false,
		newPlayer: true
	});
}

function AppendVideo(sourceurl) {
	var html = [];
	var height = window.innerWidth * 0.666;
	if(sourceurl.indexOf(".") < 0) {
		return '<div class="media-cont audio-cont" style="width:100%;"><div id="youku_player_' + sourceurl + '" sid="' + sourceurl + '" class="youku_player" style="width:100%;height:' + height + 'px;"></div></div>';
	}
	if(sourceurl.toLowerCase().indexOf(".swf") > 0) {
		return '<div class="media-cont audio-cont" style="width:100%;"><embed src="' + sourceurl + '" allowfullscreen="true" quality="high" width="100%"  height="' + height + 'px" align="middle" allowscriptaccess="always" type="application/x-shockwave-flash"></embed></div>';
	}
	if(sourceurl.toLowerCase().indexOf(".") > 0) {
		return '<video class="video" style="width:100%;height:' + height + 'px;" autoplay="autoplay" controls="controls"><source src="' + sourceurl + '" type="video/mp4" /></video>';
	}
	return "";
}