/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { Settings } from "./Settings.js";

export class MoistureWarningSettings extends Settings {
   /** @type {boolean} */
   enabled;
   /** @type {number} */
   minimumMoisture;
   /** @type {number} */
   maximumMoisture;

   /**
    * @param {boolean} enabled
    * @param {number} minimumMoisture
    * @param {number} maximumMoisture
    */
   constructor(enabled, minimumMoisture, maximumMoisture) {
      super();
      this.enabled = enabled;
      this.minimumMoisture = minimumMoisture;
      this.maximumMoisture = maximumMoisture;
   }

   clone() {
      return new MoistureWarningSettings(this.enabled, this.minimumMoisture,
         this.maximumMoisture);
   }
}