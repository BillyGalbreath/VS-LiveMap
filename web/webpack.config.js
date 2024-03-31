const path = require('path');
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const TerserPlugin = require("terser-webpack-plugin");

module.exports = {
  devServer: {
    headers: {
      'Cache-Control': 'no-store',
    },
  },
  devtool: 'source-map',
  entry: [
    './src/livemap.css',
    './src/livemap.ts'
  ],
  externals: {
    "leaflet": "L"
  },
  mode: 'development',
  module: {
    rules: [
      {
        test: /\.ts$/i,
        use: 'ts-loader',
        include: [path.resolve(__dirname, 'src')]
      },
      {
        test: /\.s?css$/i,
        use: [
          MiniCssExtractPlugin.loader,
          {
            loader: "css-loader",
            options: {
              sourceMap: true,
              url: false,
            }
          }
        ],
      }
    ]
  },
  optimization: {
    minimize: true,
    minimizer: [
      `...`,
      new CssMinimizerPlugin(),
      new TerserPlugin()
    ],
  },
  output: {
    publicPath: '/',
    filename: 'livemap.js',
    path: path.resolve(__dirname, 'public')
  },
  performance: {
    maxEntrypointSize: 512000,
    maxAssetSize: 512000
  },
  resolve: {
    extensions: ['.ts', '.js', '.scss', '.css']
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: 'livemap.css'
    })
  ]
}
