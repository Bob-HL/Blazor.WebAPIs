const path = require('path');
var webpack = require("webpack");

module.exports = {
    entry: {
        'All': './Scripts/All.ts',
        'Geolocation': './Scripts/Geolocation.ts',
        'IndexedDb': './Scripts/IndexedDb.ts',
        'JsAgent': './Scripts/JsAgent.ts'
    },
    mode: 'development',
    output: {
        path: path.resolve(__dirname, 'wwwroot'),
        filename: '[name].js'
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                exclude: /node_modules/,
                use: {
                    loader: "ts-loader"
                }
            }
        ]
    }
};