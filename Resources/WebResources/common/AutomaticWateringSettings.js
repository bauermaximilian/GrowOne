/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check
import { Settings } from "./Settings.js";

export class AutomaticWateringSettings extends Settings {
   /** @type {boolean} */
   enabled;
   /** @type {number} */
   minimumMoisture;
   /** @type {number} */
   durationSeconds;
   /** @type {number} */
   cooldownSeconds;

   /**
    * @param {boolean} enabled
    * @param {number} minimumMoisture
    * @param {number} durationSeconds
    * @param {number} cooldownSeconds
    */
   constructor(enabled, minimumMoisture, durationSeconds, cooldownSeconds) {
      super();
      this.enabled = enabled;
      this.minimumMoisture = minimumMoisture;
      this.durationSeconds = durationSeconds;
      this.cooldownSeconds = cooldownSeconds;
   }
   
   clone() {
      return new AutomaticWateringSettings(this.enabled,
         this.minimumMoisture, this.durationSeconds, this.cooldownSeconds);
   }
}