/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { AutomaticWateringSettings } from "./AutomaticWateringSettings.js";
import { HardwareSettings } from "./HardwareSettings.js";
import { MoistureWarningSettings } from "./MoistureWarningSettings.js";
import { Settings } from "./Settings.js";
import { TemperatureWarningSettings } from "./TemperatureWarningSettings.js";
import { WaterFillLevelWarningSettings } from "./WaterFillLevelWarningSettings.js";

export class ApplicationSettings extends Settings {
   /** @type {AutomaticWateringSettings} */
   automaticWateringSettings;
   /** @type {MoistureWarningSettings} */
   moistureWarningSettings;
   /** @type {TemperatureWarningSettings} */
   temperatureWarningSettings;
   /** @type {WaterFillLevelWarningSettings} */
   waterFillLevelWarningSettings;
   /** @type {HardwareSettings} */
   hardwareSettings;

   /**
    * @param {AutomaticWateringSettings} automaticWateringSettings
    * @param {MoistureWarningSettings} moistureWarningSettings
    * @param {TemperatureWarningSettings} temperatureWarningSettings
    * @param {WaterFillLevelWarningSettings} waterFillLevelWarningSettings
    * @param {HardwareSettings} hardwareSettings
    */
   constructor(automaticWateringSettings, moistureWarningSettings,
      temperatureWarningSettings, waterFillLevelWarningSettings, 
      hardwareSettings) {
      super();
      this.automaticWateringSettings = automaticWateringSettings;
      this.moistureWarningSettings = moistureWarningSettings;
      this.temperatureWarningSettings = temperatureWarningSettings;
      this.waterFillLevelWarningSettings = waterFillLevelWarningSettings;
      this.hardwareSettings = hardwareSettings;
   }

   clone() {
      return new ApplicationSettings(this.automaticWateringSettings, this.moistureWarningSettings,
         this.temperatureWarningSettings, this.waterFillLevelWarningSettings, 
         this.hardwareSettings);
   }
}