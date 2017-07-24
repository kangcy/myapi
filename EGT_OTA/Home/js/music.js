var audio = null;
var timestamp = 0;

function switchsound() {
	if(audio == null) {
		audio = document.createElement('audio');
		audio.id = 'bgsound';
		//audio.src = MusicUrl;
		audio.loop = 'loop';
		audio.innerHTML = '<source src="' + MusicUrl + '" type="audio/mpeg">'
		document.body.appendChild(audio);
	}

	if(audio.paused) {
		audio.play();
		document.getElementById(MusicID).setAttribute("play", "stop");
	} else if(new Date().getTime() > timestamp + 1000) {
		audio.pause();
		document.getElementById(MusicID).setAttribute("play", "on");
	}
}

function stopsound() {
	if(audio != null) {
		audio.pause();
	}
	document.getElementById(MusicID).setAttribute("play", "on");
}

function startsound() {
	document.removeEventListener('touchstart', startsound);

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
}