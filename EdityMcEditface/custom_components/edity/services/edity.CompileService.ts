"use strict";

import * as http from 'hr.http';

var host = "";

export function setHost(url) {
    host = url;
}

export function getStatus() {
    return http.get(host + '/edity/Compile/Status');
}