var fetch = require("node-fetch");
var multiparty = require('multiparty');
var fs = require('fs');
var qiniu = require("qiniu");
var fileType = require('file-type'); //判断文件类型
var http = require('http');
//var baseurl = "http://localhost/app/";
var baseurl = "http://www.xiaoweipian.com/"

/**
 * GET请求
 */
getApi = function(req, res) {
	var ipaddr = req.headers['x-forwarded-for'] || req.connection.remoteAddress || req.socket.remoteAddress || req.connection.socket.remoteAddress;
	var url = "";
	switch(req.query.action) {
		//预览文章
		case "preview":
			url = baseurl + "Back/ArticleInfo?key=" + req.query.key + "&xwp=" + req.query.xwp;
			break;
			//正式文章
		case "article":
			url = baseurl + "Home/Info?key=" + req.query.key + "&xwp=" + req.query.xwp + "&ArticlePassword=" + req.query.pwd + "&url=" + req.query.url;
			break;
			//用户文章
		case "userarticle":
			url = baseurl + "User/Article?UserNumber=" + req.query.number;
			break;
			//评论
		case "comment":
			url = baseurl + "Api/Comment/ArticleComment?page=1&rows=10&New=1&ArticleNumber=" + req.query.number;
			break;
			//文章校验密码
		case "checkpwd":
			url = baseurl + "Article/CheckPowerPwd?ArticleID=" + req.query.aid + "&ArticlePowerPwd=" + req.query.pwd;
			break;
		case "user":
			url = baseurl + "User/Info?number=" + req.query.number + "&url=" + req.query.url;
			break;
		default:
			break;
	}
	fetch(url, {
		method: 'GET',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json'
			//'Authorization': 'Bearer ' + req.query.access_token
		}
	}).then(function(response) {
		return response.text();
	}).then(function(body) {
		res.send(body);
		res.end();
	}).catch(function(ex) {
		res.end();
	})
}

//图片跨域处理
picApi = function(req, res) {
	var url = req.query.url;
	fetch(url, {
		method: 'GET'
	}).then(function(response) {
		return response.buffer();
	}).then(function(body) {
		var file = fileType(body); //file:{"ext":"jpg","mime":"image/jpeg"}
		file.sourceurl = url;
		file.url = "data:" + file.mime + ";base64," + body.toString('base64');
		res.send(file);
		res.end();
	}).catch(function(ex) {
		res.end();
	})

	/*http.get("http://www.xiaoweipian.com/Upload/Images/Article/20171010/150762494532169/201710101643237384_1.jpg", function(response) {　　
		var chunks = [];
		var size = 0;　
		response.on('data', function(chunk) {　　　　
			chunks.push(chunk);
			size += chunk.length;
		});
		response.on('end', function(err) {
			var data = Buffer.concat(chunks, size);　
			var base64Img = data.toString('base64');
			res.send(base64Img);
			res.end();
		});
	});*/
}

/**
 * POST请求
 */
commonPostApi = function(req, res) {
	var str = {}
	var name = req.query.name;
	switch(name) {
		// 晒单评价
		case "commentSpu":
			str = {
				spu_id: parseInt(req.query.spu_id, 10),
				sku_id: parseInt(req.query.sku_id, 10),
				msg: req.query.msg,
				star: parseInt(req.query.star, 10),
				orderid: parseInt(req.query.orderid, 10),
				photos: req.query.photos
			}
			break;
			// 评论回复
		case "commentReply":
			str = {
				spu_id: parseInt(req.query.spu_id, 10),
				sku_id: parseInt(req.query.sku_id, 10),
				msg: req.query.msg,
				star: parseInt(req.query.star, 10)
			}
			break;
			// 操作日志
		case "log":
			str = {
				uid: parseInt(req.query.uid, 10),
				uri: req.query.uri,
				ipaddr: req.headers['x-forwarded-for'] || req.connection.remoteAddress || req.socket.remoteAddress || req.connection.socket.remoteAddress,
				event: req.query.event
			}
			break;
			// 下单
		case "makeOrder":
			str = {
				sku_id: parseInt(req.query.sku_id, 10),
				contract_id: parseInt(req.query.contract_id, 10),
				pay_type: parseInt(req.query.pay_type, 10),
				sku_price_id: parseInt(req.query.sku_price_id, 10),
				peroid_price_id: parseInt(req.query.peroid_price_id, 10),
				access_id: [],
				coupons_id: [],
				count: parseInt(req.query.count, 10),
				invoince_typ: parseInt(req.query.invoince_typ, 10),
				invoince_title: req.query.invoince_title,
				addr_id: parseInt(req.query.addr_id, 10)
			}
			break;
		default:
			break;
	}
	fetch(req.query.url, {
		method: 'POST',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json',
			'Authorization': 'Bearer ' + req.query.access_token
		},
		body: JSON.stringify(str)
	}).then(function(response) {
		return response.text();
	}).then(function(body) {
		res.send(body);
		res.end();
	}).catch(function(ex) {
		res.end();
	})
}

/**
 * 注册
 */
registerApi = function(req, res) {
	var phone = req.query.Phone;
	var password = req.query.Password;
	var password2 = req.query.Password2;
	var username = req.query.UserName;
	var str = new Buffer('dxmall:dxmall').toString('base64')
	fetch(req.query.url, {
		method: 'POST',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json',
			'Authorization': 'Basic ' + str
		},
		body: JSON.stringify({
			Phone: phone,
			Password: password,
			Password2: password2,
			UserName: username
		})
	}).then(function(response) {
		return response.text();
	}).then(function(body) {
		res.send(body);
		res.end();
	}).catch(function(ex) {
		res.end();
	})
}

/**
 * 登录
 */
loginApi = function(req, res) {
	var username = req.query.username;
	var userpwd = req.query.userpwd;
	var str = new Buffer('dxmall:dxmall').toString('base64')
	fetch(req.query.url, {
		method: 'POST',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json',
			'Authorization': 'Basic ' + str
		},
		body: JSON.stringify({
			grant_type: 'password',
			username: username,
			password: userpwd
		})
	}).then(function(response) {
		return response.text();
	}).then(function(body) {
		res.send(body);
		res.end();
	}).catch(function(ex) {
		res.end();
	})
}

/**
 * 刷新凭证
 */
refreshApi = function(req, res) {
	var refresh_token = req.query.refreshToken;
	var str = new Buffer('dxmall:dxmall').toString('base64')
	fetch(req.query.url, {
		method: 'POST',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json',
			'Authorization': 'Basic ' + str
		},
		body: JSON.stringify({
			grant_type: 'refresh_token',
			refresh_token: refresh_token
		})
	}).then(function(response) {
		return response.text();
	}).then(function(body) {
		res.send(body);
		res.end();
	}).catch(function(ex) {
		res.end();
	})
}

/**
 * 编辑单个地址信息
 */
addressEditApi = function(req, res) {
	var str = {
		"is_def": parseInt(req.query.is_default, 10),
		"receiver": req.query.receiver,
		"recv_phone": req.query.recv_phone,
		"post_code": req.query.post_code,
		"address": req.query.address,
		"province": req.query.province,
		"province_id": parseInt(req.query.province_id, 10),
		"city": req.query.city,
		"city_id": parseInt(req.query.city_id, 10),
		"l3": req.query.l3,
		"l3_id": parseInt(req.query.l3_id, 10)
	}
	fetch(req.query.url, {
		method: req.query.method,
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json',
			'Authorization': 'Bearer ' + req.query.access_token
		},
		body: JSON.stringify(str)
	}).then(function(response) {
		return response.text();
	}).then(function(body) {
		res.send(body);
		res.end();
	}).catch(function(ex) {
		res.end();
	})
}

/**
 * 删除
 */
deleteApi = function(req, res) {
	fetch(req.query.url, {
		method: 'DELETE',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json',
			'Authorization': 'Bearer ' + req.query.access_token
		}
	}).then(function(response) {
		return response.text();
	}).then(function(body) {
		res.send(body);
		res.end();
	}).catch(function(ex) {
		res.end();
	})
}

/**
 * 上传图片
 */
uploadApi = function(req, res) {
	//生成multiparty对象，并配置上传目标路径
	var form = new multiparty.Form({
		uploadDir: './upload/'
	});
	//上传完成后处理
	form.parse(req, function(err, fields, files) {
		var urls = [];
		if(err) {
			console.log('parse error: ' + err);
		} else {
			var uploads = files.file;
			uploads.map(function(item, index) {
				var uploadedPath = item.path;
				var dstPath = './upload/' + new Date().getTime().toString() + (parseInt(Math.random() * 1000)).toString() + "." + item.originalFilename.split(".")[1];
				urls.push(dstPath.substr(1))

				//重命名为真实文件名
				fs.rename(uploadedPath, dstPath, function(err) {
					if(err) {
						console.log('rename error: ' + err);
					} else {
						console.log('rename ok');
					}
				});
			})
		}
		res.writeHead(200, {
			'Content-Type': 'application/json'
		});
		res.end(JSON.stringify(urls));
	});
}

/**
 *  七牛云存储
 *  图片路径（http://空间域名/key）
 */
uploadApi2 = function(req, res) {
	qiniu.conf.ACCESS_KEY = "AUJrRnADlgVAM-30gaIsVNWCoxVK80Qf3diYqE1X";
	qiniu.conf.SECRET_KEY = "_E_S26d5TUdr00K_D0xzeHHNhU-nv7z1US6RhFPz";
	var uptoken = new qiniu.rs.PutPolicy("dxmall").token();
	var extra = new qiniu.io.PutExtra();

	//生成multiparty对象，并配置上传目标路径
	var form = new multiparty.Form({
		uploadDir: './upload/'
	});
	//上传完成后处理
	form.parse(req, function(err, fields, files) {
		var num = 0; //文件数
		var totalnum = 0;
		var urls = [];
		if(err) {
			console.log('parse error: ' + err);
		} else {
			var uploads = files.file;
			totalnum = uploads.length;
			uploads.map(function(item, index) {
				var uploadedPath = item.path;
				//重命名文件名
				var key = new Date().getTime().toString() + (parseInt(Math.random() * 1000)).toString() + "." + item.originalFilename.split(".")[1];

				//上传本地保存路径
				var dstPath = './upload/' + key;

				//重命名为真实文件名
				fs.rename(uploadedPath, dstPath, function(err) {
					if(err) {
						console.log('rename error: ' + err);
					} else {
						console.log('rename ok');
						qiniu.io.putFile(uptoken, key, dstPath, extra, function(err, ret) {
							if(err) {
								// 上传失败， 处理返回代码
								console.log(err);
							} else {
								// 上传成功， 处理返回值
								console.log(ret.hash, ret.key);
								urls.push("http://o7lv5jbbo.bkt.clouddn.com/" + key);
							}
							res.writeHead(200, {
								'Content-Type': 'application/json'
							});
							res.end(JSON.stringify(urls));
						});
					}
				});
			})
		}
	});
};