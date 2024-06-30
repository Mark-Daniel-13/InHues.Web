function getElementWidthByClass(className) {
    const element = document.querySelector(`.${className}`);

    if (element) {
        const elementWidth = element.offsetWidth;
        return elementWidth;
    }

    return null;
}

const Generic = {
    LayoutDotnetRef: null,
    OpenFile: function (elementId) {
        document.getElementById(elementId).click();
    },
    SelectInputFieldRadzen: function (elementId) {
        var inputField = document.getElementById(elementId);
        if (inputField) {
            var inputElement = inputField.querySelector('input');
            inputElement.select();
        }
    },
    SetLayoutDotNetObject: function (e) {
        LayoutDotnetRef = e;
        window.onresize = function () {
            clearTimeout(window.resizedFinished);
            window.resizedFinished = setTimeout(function () {
                LayoutDotnetRef.invokeMethodAsync('OnWindowResize', window.innerWidth,window.innerHeight);
            }, 250);
        };
    },
}
function inchesToPixels(inches) {
    // Create a temporary element to calculate DPI
    const dpiElement = document.createElement('div');
    dpiElement.style.width = '1in';
    document.body.appendChild(dpiElement);

    // Get the calculated DPI
    const dpi = dpiElement.offsetWidth;

    // Remove the temporary element
    document.body.removeChild(dpiElement);

    // Calculate pixels based on the given inches and detected DPI
    const pixels = inches * dpi;

    return pixels;
}

const Camera = {
    videoStream: null,

    CaptureImage: function (videoId,canvasId) {
        var canvas = document.getElementById(canvasId);
        var video = document.getElementById(videoId);

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        const context = canvas.getContext('2d');
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        const imageData = canvas.toDataURL('image/png');
        return imageData;
    },

    StartCamera: function (videoId) {
        var video = document.getElementById(videoId);

        if (navigator.mediaDevices.getUserMedia) {
            navigator.mediaDevices.enumerateDevices()
                .then((devices) => {
                    const videoDevices = devices.filter(device => device.kind === 'videoinput');
                    const constraints = {
                        audio: false
                    };

                    if (videoDevices.length > 0) {
                        // Check if rear camera is available
                        const rearCamera = videoDevices.find(device => device.label.toLowerCase().includes('rear'));
                        if (rearCamera) {
                            constraints.video = {
                                deviceId: rearCamera.deviceId
                            };
                        } else {
                            // No rear camera, use any available camera
                            constraints.video = true;
                        }
                    } else {
                        console.error('No video devices available');
                        return;
                    }

                    return navigator.mediaDevices.getUserMedia(constraints);
                })
                .then((stream) => {
                    video.srcObject = stream;
                    Camera.videoStream = stream;
                })
                .catch((error) => {
                    console.error('Error accessing camera:', error);
                });
        } else {
            console.error('getUserMedia not supported');
        }
    },

    StopCamera: function () {
        if (Camera.videoStream) {
            const tracks = Camera.videoStream.getTracks();
            tracks.forEach((track) => {
                track.stop(); // Stop each track in the stream
            });
            Camera.videoStream = null; // Reset the stream reference
        }
    },

    CheckCameraPermission: function () {
        return navigator.permissions.query({ name: 'camera' })
            .then((result) => {
                if (result.state === 'granted') {
                    return true; // Camera permission is granted
                } else {
                    return false; // Camera permission is denied or not yet granted
                }
            })
            .catch((error) => {
                console.error('Error checking camera permission:', error);
                return false;
            });
    },
    PromptCameraPermission: function () {
        return new Promise((resolve, reject) => {
            if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
                navigator.mediaDevices.getUserMedia({ video: true })
                    .then(() => {
                        resolve(true); // User granted camera permission
                    })
                    .catch((error) => {
                        console.error('Error accessing camera:', error);
                        resolve(false); // User denied camera permission or an error occurred
                    });
            } else {
                console.error('getUserMedia not supported');
                resolve(false); // getUserMedia not supported by the browser
            }
        });
    }
    
}

window.Camera = Camera;
window.Generic = Generic;