/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { Settings } from "./Settings.js";

export class WaterFillLevelWarningSettings extends Settings {
   /** @type {boolean} */
   enabled;
   /** @type {number} */
   minimumLevel;
   /** @type {number} */
   maximumLevel;

   /**
    * @param {boolean} enabled
    * @param {number} minimumLevel
    * @param {number} maximumLevel
    */
   constructor(enabled, minimumLevel, maximumLevel) {
      super();
      this.enabled = enabled;
      this.minimumLevel = minimumLevel;
      this.maximumLevel = maximumLevel;
   }

   clone() {
      return new WaterFillLevelWarningSettings(this.enabled, this.minimumLevel, 
         this.maximumLevel);
   }
}