const HtmlWebpackPlugin = require('html-webpack-plugin');
const InlineChunkHtmlPlugin = require('inline-chunk-html-plugin');
const RemovePlugin = require("remove-files-webpack-plugin");
const FaviconsWebpackPlugin = require('favicons-webpack-plugin');
const path = require('path');

module.exports = {
   mode: 'production',

   entry: ['./App.js', './common/lib/pico.min.css'],

   output: {
      path: path.resolve(''),
      filename: 'index.js'
   },

   module: {
      rules: [{
         test: /\.js$/,
         use: 'babel-loader'
      },
      {
         test: /\.css$/i,
         use: [
            'style-loader',
            'css-loader',
         ],
      }]
   },

   plugins: [
      new HtmlWebpackPlugin({
         title: "GrowOne",
         meta: {
            viewport: 'width=device-width,initial-scale=1,maximum-scale=1,user-scalable=no',
         },
         favicon: "logo.svg",
         filename: 'index.html'
      }),
      new InlineChunkHtmlPlugin(HtmlWebpackPlugin, [/index/]),
      new RemovePlugin({
         after: {
           include: [
             'index.js'
           ]
         }
       },
	  )
   ]
};