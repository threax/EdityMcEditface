"use strict";

//Old html rapier http upload function, brought in as a shim

function extractData(xhr) {
    var data;
    var contentType = xhr.getResponseHeader('content-type');
    if (contentType && contentType.search(/application\/json/) !== -1) {
        try {
            data = JSON.parse(xhr.response);
        }
        catch (err) {
            data = xhr.response;
        }
    }
    else {
        data = xhr.response;
    }
    return data;
}

//Helper function to handle results
function handleResult(xhr, success, fail) {
    if (xhr.status > 199 && xhr.status < 300) {
        if (success !== undefined) {
            success(extractData(xhr));
        }
    }
    else {
        if (fail !== undefined) {
            fail(extractData(xhr));
        }
    }
}

/**
 * Upload a file to a url
 * @param {string} url - The url to upload to
 * @param {object|FormData} data - The data to upload, if this is already form data it will be used directly, otherwise
 * data will be sent directly as a file.
 */
export function upload(url: string, data: any) {
    return buildRequestPromise(url, 'POST', function () {
        var formData = null;

        if (data instanceof FormData) {
            formData = data;
        }
        else {
            formData = new FormData();
            formData.append('file', data);
        }

        return formData;
    }, function () {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url);
        return xhr;
    });
}

function buildRequestPromise(url, method, dataBuilder, xhrBuilder) {
    //Build promise for request
    return new Promise(function (resolve, reject) {
        //Common xhr setup
        var xhr = xhrBuilder();
        xhr.withCredentials = true;

        xhr.onload = function () {
            handleResult(xhr, resolve, reject);
        };

        var data = dataBuilder();

        if (data === undefined) {
            xhr.send();
        }
        else {
            xhr.send(data);
        }
    });
}