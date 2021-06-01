'use strict';

class CameraManager {
    constructor(width, height) {
        this.streaming = false;
        this.height = 206;
        this.width = 306;
        this.canvas = document.getElementById('canvas');
        this.photo = document.getElementById('photo');
        this.video = document.getElementById('video');
    }

    startupCamera() {
        let _video = this.video;
        let _canvas = this.canvas;

        // access video stream from webcam
        navigator.mediaDevices.getUserMedia({
            video: true,
            audio: false
        })
            // on success, stream it in video tag
            .then(function (stream) {
                _video.srcObject = stream;
                _video.play();
            })
            .catch(function (err) {
                console.log("An error occurred: " + err);
            });

        _video.addEventListener('canplay', function (ev) {
            if (!this.streaming) {
                this.height = _video.videoHeight / (_video.videoWidth / this.width);

                if (isNaN(this.height)) {
                    this.height = this.width / (4 / 3);
                }

                _video.setAttribute('width', this.width);
                _video.setAttribute('height', this.height);
                _canvas.setAttribute('width', this.width);
                _canvas.setAttribute('height', this.height);
                this.streaming = true;
            }
        }, false);

        this.clearphoto();
    }

    clearphoto() {
        var context = this.canvas.getContext('2d');
        context.fillStyle = "#AAA";
        context.fillRect(0, 0, this.canvas.width, this.canvas.height);

        var data = this.canvas.toDataURL('image/png');
        this.photo.setAttribute('src', data);
    }

    openCamera(ev) {
        //$('#dial-camera').dialog({
        //    position: { my: 'left top', at: 'right top', of: $('#btnTakePhoto') },
        //    hide: true,
        //    modal: true,
        //    resizable: false,
        //    width: 350,
        //    show: true,
        //    draggable: false,
        //    closeText: "KAPAT"
        //});

        this.startupCamera();
    }

    goTakePicture(ev) {
        this.takepicture();
        ev.preventDefault();
    }

    takepicture () {
        var context = this.canvas.getContext('2d');
        if (this.width && this.height) {
            this.canvas.width = this.width;
            this.canvas.height = this.height;
            context.drawImage(this.video, 0, 0, this.width, this.height);

            var data = this.canvas.toDataURL('image/png');

            // SET DB OBJECT IMAGE VALUES
            var imgArr = this.canvas.getContext("2d").getImageData(0, 0, 306, 206).data;
            //$scope.modelObject.ImageDataBase64 = data;// $scope.arrayBufferToBase64(imgArr);
            //$scope.modelObject.ImageHeader = 'image/png';

            this.photo.setAttribute('src', data);
            this.video.srcObject.getVideoTracks().forEach(track => track.stop());
        } else {
            this.clearphoto();
        }

        //$('#dial-camera').dialog('close');
    }

    arrayBufferToBase64(buffer) {
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
}