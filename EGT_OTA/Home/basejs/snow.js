var snow = new function() {
	var snowinterval = 0;
	this.snowimg = "";
	this.snowcount = 1;
	this.init = function(src, count, type) {
		this.snowimg = src;
		this.snowcount = parseInt(count);

		switch(parseInt(type)) {
			//通用
			case 0:
				this.snow0();
				break;
				//气球
			case 1:
				this.snow1();
				break;
				//心形
			case 2:
				this.snow2();
				break;
				//泡泡 
			case 3:
				this.snow3();
				break;
				//黑泡泡
			case 4:
				this.snow4();
				break;
				//小雪
			case 5:
				this.snow5();
				break;
				//小雨
			case 6:
				this.snow6();
				break;
				//暴雨
			case 7:
				this.snow7();
				break;
				//暴雪
			case 8:
				this.snow8();
				break;
				//红包
			case 9:
				this.snow9();
				break;
				//爆竹
			case 10:
				this.snow10();
				break;
				//元宝
			case 11:
				this.snow11();
				break;
			default:
				break;
		}
	};

	//清除
	this.clear = function() {
		clearInterval(snowinterval);
		document.getElementById("snowwrapper").innerHTML = "";
		var canvas = document.getElementById("snowcanvas");
		var cxt = canvas.getContext("2d");
		cxt.clearRect(0, 0, canvas.width, canvas.height);
	};
	//通用
	this.snow0 = function() {
		var c = null,
			d = null,
			img = null,
			e = window.innerWidth,
			f = window.innerHeight,
			g = 0,
			j = 0;

		function a() {
			for(var a = 0; g > a; a++) {
				var c = h[a];
				var img = mui(".flowimg")[a];
				if(img) {
					img.style.left = c.x + "px";
					img.style.top = c.y + "px";
				}
			}
			b();
		};

		function b() {
			j += .01;
			for(var a = 0; g > a; a++) {
				var b = h[a];
				b.y += Math.cos(j + b.d) + 1 + b.r / 2, b.x += 2 * Math.sin(j), (b.x > e + 5 || b.x < -100 || b.y > f) && (a % 3 > 0 ? h[a] = {
					x: Math.random() * e,
					y: -10,
					r: b.r,
					d: b.d
				} : Math.sin(j) > 0 ? h[a] = {
					x: -5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				} : h[a] = {
					x: e + 5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				})
			}
		};
		img = new Image();
		img.src = this.snowimg;
		img.style.position = "fixed";
		img.classList.add("flowimg");
		img.style.zIndex = 300;
		img.onload = function() {
			c = document.getElementById("snowwrapper");
			if(c) {
				c.width = e, c.height = f;
				for(g = 8, h = [], i = 0; g > i; i++) {
					var imgclone = img.cloneNode();
					imgclone.style.width = (Math.random() * 5 + 20) + "px";
					c.appendChild(imgclone);
					h.push({
						x: Math.random() * e,
						y: Math.random() * f,
						r: 4 * Math.random() + 1,
						d: Math.random() * g,
						img: imgclone
					});
				}
				snowinterval = setInterval(a, 25);
			}
		};
	};
	//气球
	this.snow1 = function() {
		var snowimg = this.snowimg;
		var snowcount = this.snowcount;

		function a(a) {
			var b = snowimg;
			if(snowcount > 1) {
				b = b.replace(".png", (d > snowcount ? d = 1 : d++) + ".png");
			}
			var g = 30 * Math.random() + 40,
				h = 0,
				j = document.createElement("div"),
				k = document.createElement("img");
			k.src = b;
			k.style.width = "100%";
			k.style.height = "100%";
			k.style.opacity = 1;
			k.style.transform = "rotate(" + h + "deg)";
			k.style.webkitAnimationDuration = a + "s";
			k.style.animationDuration = a + "s";
			k.style.webkitAnimationIterationCount = "infinite";
			k.style.animationIterationCount = "infinite";
			k.style.webkitAnimationTimingFunction = "ease-in-out";
			k.style.animationTimingFunction = "ease-in-out";
			k.style.animationName = "ballon_i";
			k.style.webkitAnimationName = "ballon_i";
			j.style.width = g + "px";
			j.style.height = 1 * g + "px";
			j.style.position = "absolute";
			j.style.left = -0 + "vw";
			j.appendChild(k);
			return j;
		};
		var d = 1;
		var c = document.getElementById("snowwrapper");
		snowinterval = setInterval(function() {
			var b = 15 + 25 * Math.random();
			var d = a(b);
			d.style.left = 90 * Math.random() - 10 + "px";
			d.style.top = -40 * Math.random() + 140 + "vh";
			d.style.webkitAnimationDuration = b + "s";
			d.style.animationDuration = b + "s";
			d.style.webkitAnimationIterationCount = "infinite";
			d.style.animationIterationCount = "infinite";
			d.style.webkitAnimationTimingFunction = "linear";
			d.style.animationTimingFunction = "linear";
			d.style.zIndex = 300;
			d.style.position = "fixed";
			d.style.animationName = "ballon_d";
			d.style.webkitAnimationName = "ballon_d";
			c.appendChild(d);
			c.children.length > 10 && clearInterval(snowinterval);
		}, 200);
	};
	//心形
	this.snow2 = function() {
		var snowimg = this.snowimg;
		var snowcount = this.snowcount;

		function a(a) {
			var b = snowimg;
			if(snowcount > 1) {
				b = b.replace(".png", (d > snowcount ? d = 1 : d++) + ".png");
			}
			var g = 30 * Math.random() + 20,
				h = 240 * Math.random() + -120,
				j = document.createElement("div"),
				k = document.createElement("img");
			k.src = b;
			k.style.width = "100%";
			k.style.height = "100%";
			k.style.opacity = 1;
			k.style.transform = "rotate(" + h + "deg)";
			k.style.webkitAnimationDuration = a + "s";
			k.style.animationDuration = a + "s";
			k.style.webkitAnimationIterationCount = "infinite";
			k.style.animationIterationCount = "infinite";
			k.style.webkitAnimationTimingFunction = "cubic-bezier(0.69, 0.10, 0.61, 0.92)";
			k.style.animationTimingFunction = "cubic-bezier(0.69, 0.10, 0.61, 0.92)";
			k.style.animationName = "dandelion_i";
			k.style.webkitAnimationName = "dandelion_i";
			j.style.width = g + "px";
			j.style.height = 1 * g + "px";
			j.style.position = "absolute";
			j.style.left = "-20vw";
			j.appendChild(k);
			return j;
		};
		var d = 1;
		var c = document.getElementById("snowwrapper");
		snowinterval = setInterval(function() {
			var b = 5 + 15 * Math.random();
			var d = a(b);
			d.style.left = "-50px";
			d.style.top = 700 * Math.random() + 100 + "px";
			d.style.webkitAnimationDuration = b + "s";
			d.style.animationDuration = b + "s";
			d.style.webkitAnimationIterationCount = "infinite";
			d.style.animationIterationCount = "infinite";
			d.style.webkitAnimationTimingFunction = "linear";
			d.style.animationTimingFunction = "linear";
			d.style.zIndex = 300;
			d.style.position = "fixed";
			d.style.animationName = "dandelion_d";
			d.style.webkitAnimationName = "dandelion_d";
			c.appendChild(d);
			c.children.length > 15 && clearInterval(snowinterval);
		}, 200);
	};
	//泡泡
	this.snow3 = function() {
		var snowimg = this.snowimg;
		var snowcount = this.snowcount;

		function a(a) {
			var b = snowimg;
			if(snowcount > 1) {
				b = b.replace(".png", (d > snowcount ? d = 1 : d++) + ".png");
			}
			var f = 40 * Math.random() + 10,
				g = 120 * Math.random() + -60,
				h = document.createElement("div"),
				j = document.createElement("img");
			j.src = b;
			j.style.width = "100%";
			j.style.opacity = 1;
			j.style.transform = "rotate(" + g + "deg)";
			j.style.webkitAnimationDuration = a + "s";
			j.style.animationDuration = a + "s";
			j.style.webkitAnimationIterationCount = "infinite";
			j.style.animationIterationCount = "infinite";
			j.style.webkitAnimationTimingFunction = "cubic-bezier(0.69, 0.10, 0.61, 0.92)";
			j.style.animationTimingFunction = "cubic-bezier(0.69, 0.10, 0.61, 0.92)";
			j.style.animationName = "dandelion_i";
			j.style.webkitAnimationName = "dandelion_i";
			h.style.width = f + "px";
			h.style.height = 1 * f + "px";
			h.style.position = "absolute";
			h.style.left = "-20vw";
			h.appendChild(j);
			return h;
		};
		var c = document.getElementById("snowwrapper");
		snowinterval = setInterval(function() {
			var b = 5 + 15 * Math.random();
			var e = a(b);
			e.style.left = "-20px";
			e.style.top = 700 * Math.random() + 100 + "px";
			e.style.webkitAnimationDuration = b + "s";
			e.style.animationDuration = b + "s";
			e.style.webkitAnimationIterationCount = "infinite";
			e.style.animationIterationCount = "infinite";
			e.style.webkitAnimationTimingFunction = "linear";
			e.style.animationTimingFunction = "linear", e.style.zIndex = 300;
			e.style.position = "fixed", e.style.animationName = "dandelion_d";
			e.style.webkitAnimationName = "dandelion_d";
			c.appendChild(e);
			c.children.length > 10 && clearInterval(snowinterval);
		}, 200);
	};
	//黑泡泡
	this.snow4 = function() {
		var snowimg = this.snowimg;
		var snowcount = this.snowcount;

		function a(a) {
			var b = snowimg;
			if(snowcount > 1) {
				b = b.replace(".png", (d > snowcount ? d = 1 : d++) + ".png");
			}
			var f = 40 * Math.random() + 10,
				g = 120 * Math.random() + -60,
				h = document.createElement("div"),
				j = document.createElement("img");
			j.src = b;
			j.style.width = "100%";
			j.style.height = "100%";
			j.style.opacity = 1;
			j.style.transform = "rotate(" + g + "deg)";
			j.style.webkitAnimationDuration = a + "s";
			j.style.animationDuration = a + "s";
			j.style.webkitAnimationIterationCount = "infinite";
			j.style.animationIterationCount = "infinite";
			j.style.webkitAnimationTimingFunction = "cubic-bezier(0.69, 0.10, 0.61, 0.92)";
			j.style.animationTimingFunction = "cubic-bezier(0.69, 0.10, 0.61, 0.92)";
			j.style.animationName = "dandelion_i";
			j.style.webkitAnimationName = "dandelion_i";
			h.style.width = f + "px";
			h.style.height = 1 * f + "px";
			h.style.position = "absolute";
			h.style.left = 0;
			h.appendChild(j);
			return h;
		};
		var c = document.getElementById("snowwrapper");
		snowinterval = setInterval(function() {
			var b = 5 + 15 * Math.random(),
				e = a(b);
			e.style.left = "-20vw";
			e.style.top = 700 * Math.random() + 100 + "px";
			e.style.webkitAnimationDuration = b + "s";
			e.style.animationDuration = b + "s";
			e.style.webkitAnimationIterationCount = "infinite";
			e.style.animationIterationCount = "infinite";
			e.style.webkitAnimationTimingFunction = "linear";
			e.style.animationTimingFunction = "linear";
			e.style.zIndex = 300, e.style.position = "fixed";
			e.style.animationName = "dandelion_d";
			e.style.webkitAnimationName = "dandelion_d";
			c.appendChild(e);
			c.children.length > 15 && clearInterval(snowinterval);
		}, 200);
	};
	//小雪
	this.snow5 = function() {
		function a() {
			d.clearRect(0, 0, e, f), d.fillStyle = "rgba(255, 255, 255, .7)", d.beginPath();
			for(var a = 0; g > a; a++) {
				var c = h[a];
				d.moveTo(c.x, c.y), d.arc(c.x, c.y, c.r, 0, 2 * Math.PI, !0), d.shadowOffsetX = e, d.shadowOffsetY = 0, d.shadowColor = "#000", d.shadowBlur = 100;
			}
			d.fill(), b();
		}

		function b() {
			j += .01;
			for(var a = 0; g > a; a++) {
				var b = h[a];
				b.y += Math.cos(j + b.d) + 1 + b.r / 2, b.x += 2 * Math.sin(j), (b.x > e + 5 || b.x < -5 || b.y > f) && (a % 3 > 0 ? h[a] = {
					x: Math.random() * e,
					y: -10,
					r: b.r,
					d: b.d
				} : Math.sin(j) > 0 ? h[a] = {
					x: -5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				} : h[a] = {
					x: e + 5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				})
			}
		}
		var c = document.getElementById("snowcanvas");
		if(c) {
			var d = c.getContext("2d"),
				e = window.innerWidth,
				f = window.innerHeight;
			c.width = e, c.height = f;
			for(var g = 25, h = [], i = 0; g > i; i++) h.push({
				x: Math.random() * e,
				y: Math.random() * f,
				r: 4 * Math.random() + 1,
				d: Math.random() * g
			});
			var j = 0;
			snowinterval = setInterval(a, 33);
		}
	};
	//小雨
	this.snow6 = function() {
		function a() {
			d.clearRect(0, 0, e, f), d.fillStyle = "rgba(255, 255, 255, .5)", d.beginPath();
			for(var a = 0; g > a; a++) {
				var c = h[a];
				d.moveTo(c.x, c.y), d.rect(c.x, c.y, .5, c.r);
			}
			d.fill(), b();
		}

		function b() {
			j += .01;
			for(var a = 0; g > a; a++) {
				var b = h[a];
				b.y += Math.cos(j + b.d) + 1 + b.r / 2, b.x += 2 * Math.sin(j), (b.x > e + 5 || b.x < -5 || b.y > f) && (a % 3 > 0 ? h[a] = {
					x: Math.random() * e,
					y: -10,
					r: b.r,
					d: b.d
				} : Math.sin(j) > 0 ? h[a] = {
					x: -5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				} : h[a] = {
					x: e + 5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				})
			}
		};
		var c = document.getElementById("snowcanvas");
		if(c) {
			var d = c.getContext("2d"),
				e = window.innerWidth,
				f = window.innerHeight;
			c.width = e, c.height = f;
			for(var g = 50, h = [], i = 0; g > i; i++) {
				h.push({
					x: Math.random() * e,
					y: Math.random() * f,
					r: 25 * Math.random() + 10,
					d: Math.random() * g
				});
			}
			var j = 0;
			snowinterval = setInterval(a, 33);
		}
	};
	//暴雨
	this.snow7 = function() {
		function a() {
			d.clearRect(0, 0, e, f), d.fillStyle = "rgba(255, 255, 255, .6)", d.beginPath();
			for(var a = 0; g > a; a++) {
				var c = h[a];
				d.moveTo(c.x, c.y), d.rect(c.x, c.y, .5, c.r);
			}
			d.fill(), b();
		}

		function b() {
			j += 0;
			for(var a = 0; g > a; a++) {
				var b = h[a];
				b.y += Math.cos(j + b.d) + 1 + b.r / 2, b.x += 2 * Math.sin(j), (b.x > e + 5 || b.x < -5 || b.y > f) && (a % 3 > 0 ? h[a] = {
					x: Math.random() * e,
					y: -10,
					r: b.r,
					d: b.d
				} : Math.sin(j) > 0 ? h[a] = {
					x: -5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				} : h[a] = {
					x: e + 5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				})
			}
		};
		var c = document.getElementById("snowcanvas");
		if(c) {
			var d = c.getContext("2d"),
				e = window.innerWidth,
				f = window.innerHeight;
			c.width = e, c.height = f;
			for(var g = 50, h = [], i = 0; g > i; i++) {
				h.push({
					x: Math.random() * e,
					y: Math.random() * f,
					r: 20 * Math.random() + 30,
					d: Math.random() * g
				});
			}
			var j = 0;
			snowinterval = setInterval(a, 20);
		}
	};
	//暴雪
	this.snow8 = function() {
		function a() {
			d.clearRect(0, 0, e, f), d.fillStyle = "rgba(255, 255, 255, .8)", d.beginPath();
			for(var a = 0; g > a; a++) {
				var c = h[a];
				d.moveTo(c.x, c.y), d.arc(c.x, c.y, c.r, 0, 2 * Math.PI, !0), d.shadowOffsetX = e, d.shadowOffsetY = 0, d.shadowColor = "#000", d.shadowBlur = 100;
			}
			d.fill(), b();
		}

		function b() {
			j += .001;
			for(var a = 0; g > a; a++) {
				var b = h[a];
				b.y += Math.cos(j + b.d) + 1 + b.r / 2, b.x += 2 * Math.sin(j), (b.x > e + 5 || b.x < -5 || b.y > f) && (a % 3 > 0 ? h[a] = {
					x: Math.random() * e,
					y: -10,
					r: b.r,
					d: b.d
				} : Math.sin(j) > 0 ? h[a] = {
					x: -5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				} : h[a] = {
					x: e + 5,
					y: Math.random() * f,
					r: b.r,
					d: b.d
				})
			}
		};
		var c = document.getElementById("snowcanvas");
		if(c) {
			var d = c.getContext("2d"),
				e = window.innerWidth,
				f = window.innerHeight;
			c.width = e, c.height = f;
			for(var g = 65, h = [], i = 0; g > i; i++) {
				h.push({
					x: Math.random() * e,
					y: Math.random() * f,
					r: 6 * Math.random() + 1,
					d: Math.random() * g
				});
			}
			var j = 0;
			snowinterval = setInterval(a, 6);
		}
	};
	//红包
	this.snow9 = function() {
		var snowimg = this.snowimg;

		function a() {
			var a = snowimg;
			var f = 30 * Math.random() + 30,
				g = 720 * Math.random() + -360,
				h = document.createElement("div"),
				j = document.createElement("img");
			j.src = a;
			j.style.width = "100%";
			j.style.height = "100%";
			j.style.transform = "rotate(" + g + "deg)";
			j.style.opacity = 1;
			h.style.width = f + "px";
			h.style.height = 1.19 * f + "px";
			h.style.position = "absolute";
			h.style.left = Math.random() * (d - f) + "px";
			h.appendChild(j);
			return h;
		};
		var d = window.innerWidth;
		var c = document.getElementById("snowwrapper");
		snowinterval = setInterval(function() {
			var b = a();
			b.style.top = "-" + 30 * Math.random() + "px";
			b.style.webkitAnimationDuration = 2 + 1 * Math.random() + "s";
			b.style.animationDuration = 2 + 1 * Math.random() + "s";
			b.style.webkitAnimationIterationCount = "infinite";
			b.style.animationIterationCount = "infinite";
			b.style.webkitAnimationTimingFunction = "linear";
			b.style.animationTimingFunction = "linear", b.style.zIndex = 300;
			b.style.position = "fixed";
			b.classList.add("particles_class"), c.appendChild(b);
			c.children.length > 15 && clearInterval(snowinterval);
		}, 200);
	};
	//爆竹
	this.snow10 = function() {
		var snowimg = this.snowimg;

		function a() {
			var a = snowimg;
			var f = 10 * Math.random() + 10,
				g = 120 * Math.random() + -60,
				h = document.createElement("div"),
				j = document.createElement("img");
			return j.src = a, j.style.width = "100%", j.style.height = "100%", j.style.transform = "rotate(" + g + "deg)", j.style.opacity = 1, h.style.width = f + "px", h.style.height = 2.46 * f + "px", h.style.position = "absolute", h.style.left = Math.random() * (d - f) + "px", h.appendChild(j), h;
		};
		var d = window.innerWidth;
		var c = document.getElementById("snowwrapper");
		snowinterval = setInterval(function() {
			var b = a();
			b.style.top = "-20vh";
			b.style.webkitAnimationDuration = 2 + 1 * Math.random() + "s";
			b.style.animationDuration = 2 + 1 * Math.random() + "s";
			b.style.webkitAnimationIterationCount = "infinite";
			b.style.animationIterationCount = "infinite";
			b.style.webkitAnimationTimingFunction = "ease-in";
			b.style.animationTimingFunction = "ease-in";
			b.style.zIndex = 300;
			b.style.position = "fixed";
			b.classList.add("particles_class");
			c.appendChild(b);
			c.children.length > 30 && clearInterval(snowinterval);
		}, 200);
	};
	//元宝
	this.snow11 = function() {
		var snowimg = this.snowimg;

		function a() {
			var a = snowimg;
			var f = 20 * Math.random() + 30,
				g = Math.random(),
				h = document.createElement("div"),
				j = document.createElement("img");
			return j.src = a, j.style.width = "100%", j.style.height = "100%", j.style.transform = "rotate(" + g + "deg)", j.style.opacity = 1, h.style.width = f + "px", h.style.height = .77 * f + "px", h.style.position = "absolute", h.style.left = Math.random() * (d - f) + "px", h.appendChild(j), h;
		};
		var c = document.getElementById("snowwrapper");
		var d = window.innerWidth;
		snowinterval = setInterval(function() {
			var b = a();
			b.style.top = "-" + 30 * Math.random() + "px";
			b.style.webkitAnimationDuration = 1 + 1 * Math.random() + "s";
			b.style.animationDuration = 2 + 1 * Math.random() + "s";
			b.style.webkitAnimationIterationCount = "infinite";
			b.style.animationIterationCount = "infinite";
			b.style.webkitAnimationTimingFunction = "linear";
			b.style.animationTimingFunction = "linear";
			b.style.zIndex = 300;
			b.style.position = "fixed";
			b.classList.add("particles_class");
			c.appendChild(b);
			c.children.length > 15 && clearInterval(snowinterval);
		}, 200);
	};
}