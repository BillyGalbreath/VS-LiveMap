// noinspection all

/**
 * Copyright 2014 JD Fergason
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// https://github.com/jdfergason/Leaflet.Ellipse

import * as L from 'leaflet';
import {Point} from '../data/Point';

L.SVG.include({
  _updateEllipse: function (layer) {
    const rx = layer._radiusX,
      ry = layer._radiusY,
      phi = layer._tiltDeg,
      endPoint = layer._endPointParams;

    const d = 'M' + endPoint.x0 + ',' + endPoint.y0 +
      'A' + rx + ',' + ry + ',' + phi + ',' +
      endPoint.largeArc + ',' + endPoint.sweep + ',' +
      endPoint.x1 + ',' + endPoint.y1 + ' z';
    this._setPath(layer, d);
  }
});

L.Canvas.include({
  _updateEllipse: function (layer) {
    if (layer._empty()) {
      return;
    }

    const p = layer._point,
      ctx = this._ctx,
      r = layer._radiusX,
      s = (layer._radiusY || r) / r;

    // https://github.com/jdfergason/Leaflet.Ellipse/commit/900d2904902da10248c23fe8d4fc022d3f68d70c
    if (this.hasOwnProperty('_drawnLayers')) {
      this._drawnlayers[layer._leaflet_id] = layer;
    } else if (this.hasOwnProperty('_layers')) {
      this._layers[layer._leaflet_id] = layer;
    } else {
      throw new Error('Cannot find property _drawnLayers or _layers');
    }

    ctx.save();

    ctx.translate(p.x, p.y);
    if (layer._tilt !== 0) {
      ctx.rotate(layer._tilt);
    }
    if (s !== 1) {
      ctx.scale(1, s);
    }

    ctx.beginPath();
    ctx.arc(0, 0, r, 0, Math.PI * 2);
    ctx.restore();

    this._fillStroke(ctx, layer);
  },
});

L.Ellipse = L.Path.extend({

  options: {
    fill: true,
    startAngle: 0,
    endAngle: 359.9
  },

  initialize: function (latlng, options) {
    L.setOptions(this, options);
    this._latlng = L.latLng(latlng);

    if (options.tilt) {
      this._tiltDeg = options.tilt;
    } else {
      this._tiltDeg = 0;
    }

    const point = Point.of(options.radii ?? [10, 10]);
    this._mRadiusX = point.x;
    this._mRadiusY = point.z;
  },

  setRadius: function (radii) {
    this._mRadiusX = radii[0];
    this._mRadiusY = radii[1];
    return this.redraw();
  },

  getRadius: function () {
    return new L.point(this._mRadiusX, this._mRadiusY);
  },

  setTilt: function (tilt) {
    this._tiltDeg = tilt;
    return this.redraw();
  },

  getBounds: function () {
    var lngRadius = this._getLngRadius(),
      latRadius = this._getLatRadius(),
      latlng = this._latlng;

    return new L.LatLngBounds(
      [latlng.lat - latRadius, latlng.lng - lngRadius],
      [latlng.lat + latRadius, latlng.lng + lngRadius]);
  },

  // @method setLatLng(latLng: LatLng): this
  // Sets the position of a circle marker to a new location.
  setLatLng: function (latlng) {
    this._latlng = L.latLng(latlng);
    this.redraw();
    return this.fire('move', {latlng: this._latlng});
  },

  // @method getLatLng(): LatLng
  // Returns the current geographical position of the circle marker
  getLatLng: function () {
    return this._latlng;
  },

  setStyle: L.Path.prototype.setStyle,

  _project: function () {
    const lngRadius = this._getLngRadius(),
      latRadius = this._getLatRadius(),
      latlng = this._latlng,
      pointLeft = this._map.latLngToLayerPoint([latlng.lat, latlng.lng - lngRadius]),
      pointBelow = this._map.latLngToLayerPoint([latlng.lat - latRadius, latlng.lng]);

    this._point = this._map.latLngToLayerPoint(latlng);

    // https://github.com/jdfergason/Leaflet.Ellipse/issues/10#issuecomment-348963516
    this._radiusX = Point.pixelsToMeters(Math.abs(this._point.x - pointLeft.x));
    this._radiusY = Point.pixelsToMeters(Math.abs(pointBelow.y - this._point.y));

    this._tilt = Math.PI * this._tiltDeg / 180;
    this._endPointParams = this._centerPointToEndPoint();
    this._updateBounds();
  },

  _updateBounds: function () {
    // http://math.stackexchange.com/questions/91132/how-to-get-the-limits-of-rotated-ellipse
    const sin = Math.sin(this._tilt);
    const cos = Math.cos(this._tilt);
    const sinSquare = sin * sin;
    const cosSquare = cos * cos;
    const aSquare = this._radiusX * this._radiusX;
    const bSquare = this._radiusY * this._radiusY;
    const halfWidth = Math.sqrt(aSquare * cosSquare + bSquare * sinSquare);
    const halfHeight = Math.sqrt(aSquare * sinSquare + bSquare * cosSquare);
    const w = this._clickTolerance();
    const p = [halfWidth + w, halfHeight + w];
    this._pxBounds = new L.Bounds(this._point.subtract(p), this._point.add(p));
  },

  _update: function () {
    if (this._map) {
      this._updatePath();
    }
  },

  _updatePath: function () {
    this._renderer._updateEllipse(this);
  },

  _getLatRadius: function () {
    // https://github.com/SlidEnergy/Leaflet.Ellipse/commit/11890f2ec425ec7f7755b991fd31016bd58f59be
    if (!!this._map.options.crs.infinite) {
      return this._mRadiusY;
    }
    return (this._mRadiusY / 40075017) * 360;
  },

  _getLngRadius: function () {
    // https://github.com/SlidEnergy/Leaflet.Ellipse/commit/11890f2ec425ec7f7755b991fd31016bd58f59be
    if (!!this._map.options.crs.infinite) {
      return this._mRadiusX;
    }
    return ((this._mRadiusX / 40075017) * 360) / Math.cos((Math.PI / 180) * this._latlng.lat);
  },

  _centerPointToEndPoint: function () {
    // Convert between center point parameterization of an ellipse
    // too SVG's end-point and sweep parameters.  This is an
    // adaptation of the perl code found here:
    // http://commons.oreilly.com/wiki/index.php/SVG_Essentials/Paths
    const c = this._point,
      rx = this._radiusX,
      ry = this._radiusY,
      theta2 = (this.options.startAngle + this.options.endAngle) * (Math.PI / 180),
      theta1 = this.options.startAngle * (Math.PI / 180),
      delta = this.options.endAngle,
      phi = this._tiltDeg * (Math.PI / 180);

    // Determine start and end-point coordinates
    const x0 = c.x + Math.cos(phi) * rx * Math.cos(theta1) +
      Math.sin(-phi) * ry * Math.sin(theta1);
    const y0 = c.y + Math.sin(phi) * rx * Math.cos(theta1) +
      Math.cos(phi) * ry * Math.sin(theta1);

    const x1 = c.x + Math.cos(phi) * rx * Math.cos(theta2) +
      Math.sin(-phi) * ry * Math.sin(theta2);
    const y1 = c.y + Math.sin(phi) * rx * Math.cos(theta2) +
      Math.cos(phi) * ry * Math.sin(theta2);

    const largeArc = (delta > 180) ? 1 : 0;
    const sweep = (delta > 0) ? 1 : 0;

    return {
      'x0': x0, 'y0': y0, 'tilt': phi, 'largeArc': largeArc,
      'sweep': sweep, 'x1': x1, 'y1': y1
    };
  },

  _empty: function () {
    return this._radiusX && this._radiusY && !this._renderer._bounds.intersects(this._pxBounds);
  },

  _containsPoint: function (p) {
    // http://stackoverflow.com/questions/7946187/point-and-ellipse-rotated-position-test-algorithm
    const sin = Math.sin(this._tilt);
    const cos = Math.cos(this._tilt);
    const dx = p.x - this._point.x;
    const dy = p.y - this._point.y;
    const sumA = cos * dx + sin * dy;
    const sumB = sin * dx - cos * dy;

    // if there is no fill, only use points where the stroke is
    if (this.options.fill === false) {
      const x = this._radiusX - this.options.weight;
      const y = this._radiusY - this.options.weight;
      if (sumA * sumA / (x * x) + sumB * sumB / (y * y) <= 1) {
        return false;
      }
    }

    return sumA * sumA / (this._radiusX * this._radiusX) + sumB * sumB / (this._radiusY * this._radiusY) <= 1;
  }
});

L.ellipse = function (latlng, options) {
  return new L.Ellipse(latlng, options);
};
