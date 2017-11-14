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
	var height = window.innerWidth * 0.5;
	//优酷
	if(sourceurl.indexOf(".") < 0) {
		return '<div class="media-cont audio-cont" style="width:100%;"><div id="youku_player_' + sourceurl + '" sid="' + sourceurl + '" class="youku_player" style="width:100%;height:' + height + 'px;"></div></div>';
	}
	//SWF
	if(sourceurl.toLowerCase().indexOf(".swf") > 0) {
		return '<div class="media-cont audio-cont" style="width:100%;"><embed src="' + sourceurl + '" allowfullscreen="true" quality="high" width="100%"  height="' + height + 'px" align="middle" allowscriptaccess="always" type="application/x-shockwave-flash"></embed></div>';
	}
	//本地视频 
	if(sourceurl.toLowerCase().indexOf(".") > 0) {
		return '<video class="video" style="width:100%;height:' + height + 'px;" controls autoplay poster="../Back/Images/poster.png"><source src="' + sourceurl + '" type="video/mp4" /></video>';
	}
	return "";
}