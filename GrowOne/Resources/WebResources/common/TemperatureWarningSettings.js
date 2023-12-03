/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { Settings } from "./Settings.js";

export class TemperatureWarningSettings extends Settings {
   /** @type {boolean} */
   enabled;
   /** @type {number} */
   minimumTemperature;
   /** @type {number} */
   maximumTemperature;

   /**
    * @param {boolean} enabled
    * @param {number} minimumTemperature
    * @param {number} maximumTemperature
    */
   constructor(enabled, minimumTemperature, maximumTemperature) {
      super();
      this.enabled = enabled;
      this.minimumTemperature = minimumTemperature;
      this.maximumTemperature = maximumTemperature;
   }

   clone() {
      return new TemperatureWarningSettings(this.enabled, this.minimumTemperature, 
         this.maximumTemperature);
   }
}