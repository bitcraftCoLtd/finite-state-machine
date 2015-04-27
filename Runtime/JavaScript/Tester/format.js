/*global module */

var expr = /\{\d+\}/g;

module.exports = function () {
    'use strict';

    if (arguments.length === 0) {
        return null;
    }
    if (arguments.length === 1) {
        //noinspection JSLint
        return arguments[0];
    }

    //noinspection JSLint
    var fmt = arguments[0];

    var i;
    var matches = fmt.match(expr);
    var replacement;
    for (i = 0; i < matches.length; i += 1) {
        replacement = '';
        if (arguments[i + 1] !== undefined && arguments[i + 1] === null) {
            replacement = arguments[i + 1].toString();
        }
        fmt = fmt.replace(matches[i], replacement);
    }

    return fmt;
};
