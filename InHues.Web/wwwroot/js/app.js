function getElementWidthByClass(className) {
    const element = document.querySelector(`.${className}`);

    if (element) {
        const elementWidth = element.offsetWidth;
        return elementWidth;
    }

    return null;
}

const DTGeneric = {
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
    PrintHTML: function (elementId,isRx)
    {
        let paperWidth, paperHeight;
        let stylesheet;
        let imageWidth;
        let printSize;
        if (isRx) {
            paperWidth = 5.5;
            paperHeight = 8.5;
            stylesheet = "css/rx_print.css";
            printSize = "5.5in 8.5in";
            imageWidth = inchesToPixels(paperWidth-0.5);
        } else {
            paperWidth = 8.26;
            paperHeight = 11.69;
            stylesheet = "css/assessment_print.css";
            printSize = "A4;";
            imageWidth = inchesToPixels(paperWidth - 0.5);
        }
        var element = document.getElementById(elementId);
        if (element) {
            printJS({
                printable: elementId,
                type: 'html',
                css: ["css/custom.css", "css/app.css", "css/bootstrap/bootstrap.min.css", stylesheet],
                scanStyles: false,
                targetStyles: "*",
                style: `@page{size:${printSize}; margin:0; padding:0;} .canvas-img {width:${imageWidth}px !important;}`
            })
        }
        console.log(`imgHeight - ${imgHeight}`);
        console.log(`paperWidth - ${inchesToPixels(paperWidth - 0.2)}px`);
        console.log(`paperHeight - ${inchesToPixels(paperHeight - 0.2)}px`);



        //var element = document.getElementById(elementId);
        //const computedRxImgHeight = (inchesToPixels(paperWidth) * 850) / 760;
        //const rxStyles = `.rx-img{width:${inchesToPixels(paperWidth)}px !important; height: ${computedRxImgHeight}px !important}`;
        //if (element) {
        //    printJS({
        //        printable: elementId,
        //        type: 'html',
        //        css: ["css/custom.css", "css/app.css", "css/bootstrap/bootstrap.min.css", "css/rx_print.css"],
        //        scanStyles: false,
        //        targetStyles: "*",
        //        style: `#${elementId}-content {width:${inchesToPixels(paperWidth)}px; height:${inchesToPixels(paperHeight)}px; color:red !important; ${rxStyles}}`
        //    })
        //}
        //console.log(`rx-img-height - ${computedRxImgHeight}px`);
        //console.log(`paperWidth - ${inchesToPixels(paperWidth)}px`);
        //console.log(`paperHeight - ${inchesToPixels(paperHeight)}px`);
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

const DTCamera = {
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
                    DTCamera.videoStream = stream;
                })
                .catch((error) => {
                    console.error('Error accessing camera:', error);
                });
        } else {
            console.error('getUserMedia not supported');
        }
    },

    StopCamera: function () {
        if (DTCamera.videoStream) {
            const tracks = DTCamera.videoStream.getTracks();
            tracks.forEach((track) => {
                track.stop(); // Stop each track in the stream
            });
            DTCamera.videoStream = null; // Reset the stream reference
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

window.DTCamera = DTCamera;
window.DTGeneric = DTGeneric;