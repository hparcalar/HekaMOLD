﻿function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function refreshArray(arrObj) {
    try {
        arrObj.push({});
        setTimeout(function () { arrObj.splice(arrObj.length - 1, 1); }, 150);
    } catch (e) {

    }
}

function groupArrayOfObjects(list, key) {
    return list.reduce(function (rv, x) {
        (rv[x[key]] = rv[x[key]] || []).push(x);
        return rv;
    }, {});
};