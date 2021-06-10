const path = require('path');
var webpack = require("webpack");

module.exports = {
    entry: {
        'IndexedDb': './Scripts/IndexedDb.ts',
        'Common': './Scripts/Common.ts'
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