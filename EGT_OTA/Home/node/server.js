var express = require('express');
var proxy = require('http-proxy-middleware'); //代理服务，跨域
require("./api")
var app = new(express)()
var port = 3000

app.all('*', function(req, res, next) {
	res.header("Access-Control-Allow-Origin", "*");
	res.header("Access-Control-Allow-Headers", "X-Requested-With");
	res.header("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS");
	res.header("X-Powered-By", ' 3.2.1')
	res.header("Content-Type", "application/json;charset=utf-8");
	next();
});

// 缓存静态文件
app.use(express.static(__dirname))

//服务器代理
/*app.use('/api', proxy({
	target: "http://localhost/app/Back/ArticleInfo",
	changeOrigin: true
}));

app.use('/commentapi', proxy({
	target: "http://localhost/app/Api/Comment/ArticleComment",
	changeOrigin: true
}));*/

/*app.use('*', proxy({
	target: "http://localhost/app/Back/ArticleInfo",
	changeOrigin: true
}));*/

app.post("/upload", uploadApi2)
app.get("/api", getApi)
app.get("/picApi", picApi)
app.get("/postApi", commonPostApi)
app.get("/registerApi", registerApi)
app.get("/loginApi", loginApi)
app.get("/refreshApi", refreshApi)
app.get("/addressEditApi", addressEditApi)
app.get("/deleteApi", deleteApi)

app.get("*", function(req, res) {
	res.sendFile(__dirname + '/index.html')
})

app.listen(port, function(error) {
	if(error) {
		console.error(error)
	} else {
		console.info("==> 🌎  Listening on port %s. Open up http://localhost:%s/ in your browser.", port, port)
	}
})