//var RootUrl = "http://localhost/app/";
//var RootUrl = "http://www.xiaoweipian.com/";
//var RootUrl = "http://www.xiaoweipian.com:3000/api";
var RootUrl = "http://localhost:3000/api";
var key = "";
var xwp = "";
var MusicID = 0;
var MusicUrl = "";
var ArticleID = 0;
var CurrTemplate = 0;
var CurrCover = "";
var CurrColor = "";
var Article = null;
var CurrBackground = null;
var CurrTemplateJson = null;
var TitleColor = "#2887F0";
var UserColor = "#333"
var SubTitleColor = "#8c8c8c";
var BackgroundColor = "rgba(255, 255, 255, 0.15)";
var isLoading = false;

var $header = base.Get("header");
var $wrapper = base.Get("wrapper");
var $wrapper1 = base.Get("x-cover");
var $wrapper2 = base.Get("x-bg");
var $wrapper21 = base.Get("x-bg-base");
var $cover = base.Get("x-bg-blur");
var $cover1 = base.Get("x-bg-blur1");
var $head = base.Get("x-head");
var $body = base.Get("x-body");
var $top = base.Get("x-top");
var $bottom = base.Get("x-bottom");
var $avatarbg = base.Get("x-avatar-sub");
var $avatar = base.Get("useravatar0");
var $edit = base.Get("desc");

function InitReady() {
	var topheight = base.Get("download-bar").clientHeight;
	$wrapper.style.paddingTop = $wrapper2.style.top = $wrapper21.style.top = topheight + "px";

	base.Get("music1").style.top = (topheight + 20) + "px";
	if(window.innerWidth > 750) {
		base.Get("music1").style.right = (window.innerWidth - 750) / 2 + 18 + "px";
	}

	mui("#wrapper").on('tap', '.user', function(event) {
		window.location.href = GetRootUrl() + "/u/" + this.getAttribute("userid");
	});

	if(CurrTemplateJson) {
		if(CurrTemplateJson.CoverFixed == 2) {
			window.addEventListener('scroll', function(e) {
				var top = document.documentElement.scrollTop || document.body.scrollTop;
				//var top = $wrapper.scrollTop;
				if(top >= 150) {
					$wrapper2.style.opacity = 0;
					$wrapper21.style.opacity = 1;
				} else {
					$wrapper2.style.opacity = 1 - top / 150;
					$wrapper21.style.opacity = top / 150;
				}
			});
		}
	}
}

function ChangeMusic(index) {
	$("#music0,#music1").addClass("hide");
	MusicID = "music" + (CurrTemplate == 0 ? 0 : 1);
	if(!base.IsNullOrEmpty(MusicUrl)) {
		if(Article.AutoMusic == 1) {
			document.addEventListener('touchstart', startsound);
			document.addEventListener('click', startsound);
		}
		base.Get("music" + (CurrTemplate == 0 ? 0 : 1)).classList.remove("hide");
	}
}

//修改主题色
function ChangeThemeColor(temp) {
	if(temp == null) {
		return;
	}
	base.Get("title0").style.color = temp.TitleColor;
	base.Get("nickname0").style.color = temp.UserColor;
	base.Get("nickname1").style.color = temp.UserColor;
	base.Get("createdate0").style.color = temp.TimeColor;
	base.Get("views0").style.color = temp.ViewColor;
	$avatar.style.border = "2px solid " + temp.TitleColor;
}

//背景状态切换 
function ChangeBg() {
	if(CurrTemplate == 0) {
		base.Get("title0").classList.remove("well2");
		base.Get("x-avatar").classList.add("hide");
		base.Get("x-article").classList.remove("tc");
		base.Get("x-article").classList.add("tl");
		base.Get("nickname1").classList.add("hide");
		base.Get("nickname0").classList.remove("hide");
		base.Get("x-content").classList.remove("mt10");
		$head.style.marginTop = "1rem";
	} else {
		base.Get("title0").classList.add("well2");
		base.Get("x-avatar").classList.remove("hide");
		base.Get("x-article").classList.remove("tl");
		base.Get("x-article").classList.add("tc");
		base.Get("nickname0").classList.add("hide");
		base.Get("nickname1").classList.remove("hide");
		base.Get("x-content").classList.add("mt10");
		$head.style.marginTop = "3rem";
	}

	mui.each(mui(".edit-content"), function(i, item) {
		item.classList.remove("well2");
	})

	//容器间距
	switch(CurrTemplateJson.PaddingFixed) {
		case 0:
			$body.classList.add("well2");
			$edit.classList.add("temp");
			break;
		case 1:
			$edit.classList.remove("temp");
			$body.classList.add("well2");
			break;
		case 2:
			$body.classList.remove("well2");
			$edit.classList.add("temp");
			break;
		case 3:
			$body.classList.remove("well2");
			$edit.classList.remove("temp");
			mui.each(mui(".edit-content"), function(i, item) {
				item.classList.add("well2");
			})
			break;
		default:
			$body.classList.add("well2");
			$edit.classList.add("temp");
			break;
	}
	ChangeMusic();

	$top.classList.add("hide");
	$bottom.classList.add("hide");
	$body.style.margin = "0";
	$wrapper.style.color = "#333";
	$wrapper.style.backgroundColor = "";
	$wrapper1.classList.remove("x-cover");
	$wrapper1.style.background = "";
	$cover.style.backgroundColor = "";
	$cover1.style.backgroundColor = "";
	$wrapper2.classList.add("hide");
	$wrapper21.classList.add("hide");
	$wrapper2.style.background = "";
	$wrapper2.style.opacity = "1";
	$wrapper21.style.background = "";
	$wrapper21.style.opacity = "0";
	$avatarbg.style.backgroundImage = "";
	$avatarbg.style.width = "5rem";
	$avatarbg.style.height = "5rem";
	$avatar.style.width = "100%";
	$avatar.style.left = "0";
	$avatar.style.top = "0";
	$avatar.style.borderRadius = "50%";
	//$avatar.style.border = "";
	$avatar.style.zIndex = "-1";
	$avatarbg.style.marginBottom = "1rem";
	if(CurrTemplateJson.ShowAvatar < 0) {
		base.Get("x-avatar").classList.add("hide");
	}
	if(!base.IsNullOrEmpty(CurrTemplateJson.FontColor)) {
		$wrapper.style.color = CurrTemplateJson.FontColor;
	}
	if(CurrTemplate > 1) {
		//头尾
		if(!base.IsNullOrEmpty(CurrTemplateJson.TopImage)) {
			var tophtml = [];
			for(var i = 0; i < CurrTemplateJson.TopImage.length; i++) {
				var top = CurrTemplateJson.TopImage[i];
				tophtml.push('<img src="' + top.Url + '" class="' + top.Align + '" style="width:' + top.Width + '" />');
			}
			$top.innerHTML = tophtml.join('');
			$top.classList.remove("hide");
		}
		if(!base.IsNullOrEmpty(CurrTemplateJson.BottomImage)) {
			$bottom.setAttribute("src", CurrTemplateJson.BottomImage);
			$bottom.classList.remove("hide");
		}

		//头像
		if(CurrTemplateJson.Avatar != null) {
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.BackgroundImage)) {
				$avatarbg.style.backgroundImage = "url(" + CurrTemplateJson.Avatar.BackgroundImage + ")";
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.Width)) {
				$avatarbg.style.width = CurrTemplateJson.Avatar.Width;
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.Height)) {
				$avatarbg.style.height = CurrTemplateJson.Avatar.Height;
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.MarginBottom)) {
				$avatarbg.style.marginBottom = CurrTemplateJson.Avatar.MarginBottom;
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.MarginTop)) {
				if(CurrTemplateJson.TransparencyFixed == 0) {
					$head.style.marginTop = CurrTemplateJson.Avatar.MarginTop;
				} else {
					$body.style.marginTop = CurrTemplateJson.Avatar.MarginTop;
					$head.style.marginTop = "1rem";
				}
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.SubWidth)) {
				$avatar.style.width = CurrTemplateJson.Avatar.SubWidth;
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.Top)) {
				$avatar.style.top = CurrTemplateJson.Avatar.Top;
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.Left)) {
				$avatar.style.left = CurrTemplateJson.Avatar.Left;
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.BorderRadius)) {
				$avatar.style.borderRadius = CurrTemplateJson.Avatar.BorderRadius;
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.Index)) {
				$avatar.style.zIndex = CurrTemplateJson.Avatar.Index;
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Avatar.Border)) {
				$avatar.style.border = CurrTemplateJson.Avatar.Border;
			}
		}

		if(CurrTemplateJson.TransparencyFixed == 0) {
			$cover.style.backgroundColor = CurrTemplateJson.Transparency;
		} else {
			$cover1.style.backgroundColor = CurrTemplateJson.Transparency;
		}

		if(CurrTemplateJson.CoverFixed == 0) {
			$wrapper1.classList.add("x-cover");
			if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundImage)) {
				var imglist = [];
				var imgs = CurrTemplateJson.BackgroundImage.split('?');
				for(var i = 0; i < imgs.length; i++) {
					imglist.push("url(" + imgs[i] + ")");
				}
				$wrapper1.style.backgroundImage = imglist.join(',');
				if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundPosition)) {
					$wrapper1.style.backgroundPosition = CurrTemplateJson.BackgroundPosition;
				}
				if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundRepeat)) {
					$wrapper1.style.backgroundRepeat = CurrTemplateJson.BackgroundRepeat;
				}
				if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundSize)) {
					$wrapper1.style.backgroundSize = CurrTemplateJson.BackgroundSize;
				}
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Background)) {
				$wrapper.style.background = CurrTemplateJson.Background;
				if(!CurrTemplateJson.Background.startWith("#") && !CurrTemplateJson.Background.startWith("rgba")) {
					$wrapper.style.background = base.BrowserName() + CurrTemplateJson.Background;
				}
			}
		} else {
			if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundImage)) {
				var imglist = [];
				var imgs = CurrTemplateJson.BackgroundImage.split('?');
				for(var i = 0; i < imgs.length; i++) {
					imglist.push("url(" + imgs[i] + ")");
				}
				$wrapper2.style.backgroundImage = imglist.join(',');
				if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundPosition)) {
					$wrapper2.style.backgroundPosition = CurrTemplateJson.BackgroundPosition;
				}
				if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundRepeat)) {
					$wrapper2.style.backgroundRepeat = CurrTemplateJson.BackgroundRepeat;
				}
				if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundSize)) {
					$wrapper2.style.backgroundSize = CurrTemplateJson.BackgroundSize;
				}
				$wrapper2.classList.remove("hide");
			}
			if(CurrTemplateJson.CoverFixed == 2) {
				if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundBlurImage)) {
					var imglist = [];
					var imgs = CurrTemplateJson.BackgroundBlurImage.split('?');
					for(var i = 0; i < imgs.length; i++) {
						imglist.push("url(" + imgs[i] + ")");
					}
					$wrapper21.style.backgroundImage = imglist.join(',');
					if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundPosition)) {
						$wrapper21.style.backgroundPosition = CurrTemplateJson.BackgroundPosition;
					}
					if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundRepeat)) {
						$wrapper21.style.backgroundRepeat = CurrTemplateJson.BackgroundRepeat;
					}
					if(!base.IsNullOrEmpty(CurrTemplateJson.BackgroundSize)) {
						$wrapper21.style.backgroundSize = CurrTemplateJson.BackgroundSize;
					}
					$wrapper21.classList.remove("hide");
				}
			}
			if(!base.IsNullOrEmpty(CurrTemplateJson.Background)) {
				if(CurrTemplateJson.Background.startWith("#") || CurrTemplateJson.Background.startWith("rgba")) {
					$wrapper2.style.backgroundColor = CurrTemplateJson.Background;
				} else {
					$wrapper2.style.background = CurrTemplateJson.Background;
					$wrapper2.style.background = base.BrowserName() + CurrTemplateJson.Background;
				}
				$wrapper2.classList.remove("hide");
			}
		}
	} else if(CurrTemplate == 1) {
		//自定义
		if(CurrBackground == null) {
			//全屏
			$wrapper2.style.background = "";
			$wrapper2.className = "";
		} else {
			//背景透明度
			$cover.style.backgroundColor = "RGBA(255, 255, 255, " + (100 - CurrBackground.Transparency) / 100 + ")";
			var url = CurrBackground.Url;
			if(CurrBackground.High == 0) {
				url = base.ShowThumb(url, 1);
			} else {
				url = base.ShowThumb(url, 0);
			}
			$wrapper2.className = "";
			switch(CurrBackground.Full) {
				case 0:
					//居顶 
					$wrapper2.style.background = "url(" + url + ") no-repeat top center";
					$wrapper2.style.backgroundSize = "100% 15.5rem";
					break;
				case 1:
					//全屏
					$wrapper2.style.background = "url(" + url + ") center center no-repeat";
					$wrapper2.style.backgroundSize = "cover";
					//$wrapper2.style.backgroundSize = "100% auto";
					break;
				default:
					break;
			}
			$body.classList.add("well2");
			$edit.classList.add("temp");
		}
	}
}

function AppendStr(item) {
	var div = document.createElement('div');
	div.className = 'mui-table-view-cell';
	var model = [];
	model.push('<div class="mui-slider-cell mt10 mb10"><div class="oa-contact-cell mui-table">');
	model.push('<div class="mui-table-cell oa-contact-avatar"><img src="' + base.ShowThumb(item.Avatar, 1) + '" class="user" userid="' + item.UserNumber + '" /></div>');
	model.push('<div class="mui-table-cell pb0" style="border-bottom:1px solid ' + BackgroundColor + '">');
	model.push('<p class="f12 user" style="color:' + UserColor + '" userid="' + item.UserNumber + '">' + UnUnicodeText(item.NickName) + '<span class="fr f11" style="color:' + SubTitleColor + '">' + item.CreateDateText + '</span></p>');
	model.push('<p class="f11 mt5 mb5 summary" style="line-height:1.3rem;color:' + SubTitleColor + '">' + UnUnicodeText(item.Summary) + '</p>');
	if(!base.IsNullOrEmpty(item.ParentCommentNumber)) {
		model.push('<p class="tip mb0 f11" style="line-height:1.3rem;"><span class="user blue summary" userid="' + item.ParentUserNumber + '">@' + UnUnicodeText(item.ParentNickName) + '：</span>' + UnUnicodeText(item.ParentSummary) + '</p>');
	}
	model.push('</div></div>');
	div.innerHTML = model.join('');
	return div;
}

function InitArticle(data) {
	mui.each(mui(".user"), function(i, item) {
		item.setAttribute("userid", data.CreateUserNumber);
		item.setAttribute("nickname", data.NickName);
		item.setAttribute("avatar", data.Avatar);
		item.setAttribute("cover", data.Cover);
	})

	var html = [];
	var parts = data.ArticlePart;
	if(parts != null) {
		for(var i = 0; i < parts.length; i++) {
			var part = parts[i];
			if(part.Types == 1) {
				html.push('<div class="full f13 fl mt10 ' + part.IntroExpand + '"><img onload="LazyImg(this)" src="img/default.png" data-lazyload="' + base.ShowThumb(part.Introduction, 1) + '" class="full fl" /></div>');
			} else if(part.Types == 2) {
				var txtdiv = document.createElement("div");
				txtdiv.innerHTML = UnUnicodeText(part.Introduction);
				$(txtdiv).find(".edit").addClass(part.IntroExpand);
				html.push('<div class="full f13 fl mt10 edit-content">' + txtdiv.innerHTML + '</div>');
			} else if(part.Types == 3) {
				html.push('<div class="full f13 fl mt10 hide" style="display:inline-block;">' + AppendVideo(part.Introduction) + '</div>');
			} else if(part.Types == 4) {
				html.push('<div class="full f13 fl mt10" style="display:inline-block;">' + part.Introduction + '</div>');
			}
		}
	}

	MusicUrl = data.MusicUrl;
	CurrBackground = data.BackgroundJson;
	CurrTemplate = data.Template;

	if(data.TemplateJson != null) {
		if(data.TemplateJson.ID != 1) {
			CurrTemplateJson = data.TemplateJson;
			if(data.ColorTemplateJson == null) {
				TitleColor = CurrTemplateJson.TitleColor;
				SubTitleColor = CurrTemplateJson.ViewColor;
				UserColor = CurrTemplateJson.UserColor;
				BackgroundColor = CurrTemplateJson.Transparency;
				ChangeThemeColor(CurrTemplateJson);
			}
		}
	}
	if(data.ColorTemplateJson != null) {
		TitleColor = data.ColorTemplateJson.TitleColor;
		SubTitleColor = data.ColorTemplateJson.ViewColor;
		UserColor = data.ColorTemplateJson.UserColor;
		ChangeThemeColor(data.ColorTemplateJson);
	}

	base.Get("musicname0").innerHTML = data.MusicName;
	base.Get("useravatar0").setAttribute("src", base.ShowThumb(data.Avatar, 1));
	base.InnerHtml(["title", "title0"], data.Title);
	base.InnerHtml(["nickname", "nickname0", "nickname1"], data.NickName);
	base.InnerHtml(["createdate0"], data.CreateDateText);
	base.InnerHtml(["views0"], "阅读  " + data.Views);
	base.Get("x-head").classList.remove("hide");
	base.Get("desc").innerHTML = UnUnicodeText(html.join(""));

	ChangeBg();

	$(".youku_player").each(function() {
		var $this = $(this);
		ShowVideo($this.attr("id"), $this.attr("sid"));
	});
	mui.each(document.querySelectorAll(".video"), function() {
		var $this = this;
		$this.addEventListener('canplay', function() {
			$this.parentNode.classList.remove("hide");
		});
	});

	document.getElementById("loader").classList.add("hide");
	$("#wrapper").removeClass("hide");

	base.Get("color-lump").style.background = TitleColor;
	base.Get("color-lump-text").style.color = TitleColor;
	LoadComment(key);

	if(!base.IsNullOrEmpty(data.Showy)) {
		LoadScript("min/snow.min.js", function() {
			var srcs = data.Showy.split("|");
			if(srcs.length > 2) {
				snow.init(srcs[0], srcs[1], srcs[2])
			} else {
				snow.init(data.Showy, 1, 0)
			}
		});
	}

	mui('#desc').on('tap', 'a.link', function() {
		var link = this.getAttribute("link");
		if(base.IsNullOrEmpty(link)) {
			return;
		}
		window.location.href = link;
	});

	isLoading = false;
}

function LoadComment(ArticleNumber) {
	$.ajax({
		type: "GET",
		url: RootUrl,
		data: {
			number: ArticleNumber,
			action: "comment"
		},
		dataType: "json",
		success: function(data) {
			if(data != null) {
				data = JSON.parse(data);
				if(data.result) {
					data = data.message;
					var length = data.records;
					if(length > 0) {
						document.getElementById('commentlist').classList.remove("hide");
						var table = document.getElementById('commentlist');
						for(var i = 0, len = data.list.length; i < len; i++) {
							var div = AppendStr(data.list[i]);
							table.appendChild(div);
						}
						$("#comment").removeClass("hide");
					}
				}
			}
		}
	});
}