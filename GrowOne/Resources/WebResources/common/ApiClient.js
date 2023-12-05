/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { Measurement } from "./Measurement.js";
import { AutomaticWateringSettings } from "./AutomaticWateringSettings.js";
import { MoistureWarningSettings } from "./MoistureWarningSettings.js";
import { TemperatureWarningSettings } from "./TemperatureWarningSettings.js";
import { ApplicationSettings } from "./ApplicationSettings.js";
import { Utils } from "./Utils.js";
import { HardwareSettings } from "./HardwareSettings.js";
import { WaterFillLevelWarningSettings } from "./WaterFillLevelWarningSettings.js";

export class ApiClient {
   /** @type {string} */
   static #defaultUsername = "GrowOne";
   /** @type {string} */
   #password;

   /**
    * @param {string} password 
    */
   constructor(password) {
      this.#password = password;
   }

   /**
    * @param {string} url
    * @param {string?} body
    * @param {boolean} throws
    * @returns {Promise<any>}
    */
   async #fetchAuthenticated(url, method = "GET", body = null, throws = true) {
      let headers = new Headers();
      let authorisationString = window.btoa(ApiClient.#defaultUsername + ":" + this.#password);
      headers.set('Authorization', 'Basic ' + authorisationString);
      let response = await fetch(url, { method: method, headers: headers, body: body });
      if (response.ok || !throws) {
         return response;
      } else {
         throw new Error(`The request failed (code ${response.status}).`);
      }
   }

   /**
    * @param {string} password 
    * @returns {Promise<ApiClient>}
    */
   static async create(password) {
      let api = new ApiClient(password);
      let response = await api.#fetchAuthenticated("/api/", "GET", null, false);
      if (response.ok) {
         return api;
      } else if (response.status === 401) {
         throw new Error("The specified password is invalid.");
      } else {
         throw new Error(`The request failed (code ${response.status}).`);
      }
   }
   
   /**
    * Retrieves the current measurements and statistics.
    * REST: GET /api/measurements
    * @returns {Promise<Measurement[]>}
    */
   async getCurrentMeasurements() {
      let response = await this.#fetchAuthenticated("/api/measurements");
      let responseData = await response.json();
      let measurements = [];
      if (Array.isArray(responseData)) {
         for (let measurementData of responseData) {
            measurements.push(new Measurement(measurementData.label,
               measurementData.currentValue, measurementData.statistics));
         }
      }
      return measurements;
   }

   /**
    * Starts a manual watering for a specific amount of time.
    * REST: POST /api/action/water/{seconds}
    * @param {number} seconds 
    * @returns {Promise<void>}
    */
   async waterManually(seconds) {
      await this.#fetchAuthenticated(`/api/action/water/${seconds}`, "POST");
   }

   /**
    * Resets the measurement statistics.
    * REST: DELETE /api/measurements
    * @returns {Promise<void>}
    */
   async resetMeasurementStatistics() {
      await this.#fetchAuthenticated("/api/measurements", "DELETE");
   }

   /**
    * Plays a test notification sound on the device.
    * REST: POST /api/action/soundtest
    * @returns {Promise<void>}
    */
   async testNotificationSound() {
      await this.#fetchAuthenticated("/api/action/soundtest", "POST");
   }

   /**
    * Restarts the device, resetting any measurement statistics.
    * REST: POST /api/action/restart
    * @returns {Promise<void>}
    */
   async restartDevice() {
      await this.#fetchAuthenticated("/api/action/restart", "POST");
   }

   /**
    * Updates the device configuration with the specified settings.
    * REST: PUT /api/configuration
    * @param {ApplicationSettings} userSettings
    * @returns {Promise<void>} 
    */
   async updateConfiguration(userSettings) {
      let currentSegment = "";
      try {
         currentSegment = "automaticWateringSettings";
         await this.#fetchAuthenticated("/api/configuration/automaticwatering", "PUT",
            JSON.stringify({
               on: userSettings.automaticWateringSettings.enabled,
               min: userSettings.automaticWateringSettings.minimumMoisture,
               dur: userSettings.automaticWateringSettings.durationSeconds,
               cd: userSettings.automaticWateringSettings.cooldownSeconds
            }));
         currentSegment = "moistureWarningSettings";
         await this.#fetchAuthenticated("/api/configuration/moisturewarning", "PUT",
            JSON.stringify({
               on: userSettings.moistureWarningSettings.enabled,
               min: userSettings.moistureWarningSettings.minimumMoisture,
               max: userSettings.moistureWarningSettings.maximumMoisture
            }));
         currentSegment = "temperatureWarningSettings";
         await this.#fetchAuthenticated("/api/configuration/temperaturewarning", "PUT",
            JSON.stringify({
               on: userSettings.temperatureWarningSettings.enabled,
               min: userSettings.temperatureWarningSettings.minimumTemperature,
               max: userSettings.temperatureWarningSettings.maximumTemperature
            }));
         currentSegment = "waterFillLevelWarningSettings";
         await this.#fetchAuthenticated("/api/configuration/waterfilllevelwarning", "PUT",
            JSON.stringify({
               on: userSettings.waterFillLevelWarningSettings.enabled,
               min: userSettings.waterFillLevelWarningSettings.minimumLevel,
               max: userSettings.waterFillLevelWarningSettings.maximumLevel
            }));
         currentSegment = "hardwareSettings";
         await this.#fetchAuthenticated("/api/configuration/hardware", "PUT",
            JSON.stringify({
               msp: userSettings.hardwareSettings.moistureSensorPin,
               dep: userSettings.hardwareSettings.dht22EchoPin,
               dtp: userSettings.hardwareSettings.dht22TriggerPin,
               isp: userSettings.hardwareSettings.irrigatorSwitchPin,
               ssp: userSettings.hardwareSettings.sensorSwitchPin,
               bp: userSettings.hardwareSettings.buzzerPin,
               hep: userSettings.hardwareSettings.hcsr04EchoPin,
               htp: userSettings.hardwareSettings.hcsr04TriggerPin,
               hmin: userSettings.hardwareSettings.hcsr04MinimumValueCm,
               hmax: userSettings.hardwareSettings.hcsr04MaximumValueCm,
               pass: userSettings.hardwareSettings.password
            }));
      } catch (error) {
         throw new Error(`Saving \"${currentSegment}\" failed (${error}).`);
      }
   }

   /**
    * Gets the current device settings.
    * REST: GET /api/configuration
    * @returns {Promise<ApplicationSettings>}
    */
   async getConfiguration() {
      let response = await this.#fetchAuthenticated("/api/configuration", "GET");
      let responseData = await response.json();

      let automaticWateringSettings = new AutomaticWateringSettings(
         responseData.automaticWateringSettings?.on,
         responseData.automaticWateringSettings?.min,
         responseData.automaticWateringSettings?.dur,
         responseData.automaticWateringSettings?.cd
      );
      let moistureWarningSettings = new MoistureWarningSettings(
         responseData.moistureWarningSettings?.on,
         responseData.moistureWarningSettings?.min,
         responseData.moistureWarningSettings?.max
      );
      let temperatureWarningSettings = new TemperatureWarningSettings(
         responseData.temperatureWarningSettings?.on,
         responseData.temperatureWarningSettings?.min,
         responseData.temperatureWarningSettings?.max
      );
      let waterFillLevelWarningSettings = new WaterFillLevelWarningSettings(
         responseData.waterFillLevelWarningSettings?.on,
         responseData.waterFillLevelWarningSettings?.min,
         responseData.waterFillLevelWarningSettings?.max
      )
      let hardwareSettings = new HardwareSettings(
         responseData.hardwareSettings?.msp,
         responseData.hardwareSettings?.dep,
         responseData.hardwareSettings?.dtp,
         responseData.hardwareSettings?.isp,
         responseData.hardwareSettings?.ssp,
         responseData.hardwareSettings?.bp,
         responseData.hardwareSettings?.hep,
         responseData.hardwareSettings?.htp,
         responseData.hardwareSettings?.hmin,
         responseData.hardwareSettings?.hmax,
         responseData.hardwareSettings?.pass
      );
      
      return new ApplicationSettings(automaticWateringSettings,
         moistureWarningSettings,
         temperatureWarningSettings,
         waterFillLevelWarningSettings,
         hardwareSettings);
   }
}

export class ApiClientMock extends ApiClient {
   /** @type {string} */
   #password;
   
   /**
    * @param {string} password 
    */
   constructor(password) {
      super(password);
      this.#password = password;
   }   

   /**
    * @param {string} password 
    * @returns {Promise<ApiClientMock>}
    */
   static async create(password) {
      let api = new ApiClientMock(password);
      let authenticationSuccessful = await api.#authenticate();
      if (authenticationSuccessful) {
         return api;
      } else {
         throw new Error("The specified password was invalid.");
      }
   }

   /**
    * @returns {Promise<boolean>}
    */
   async #authenticate() {
      await Utils.sleep(500);
      return this.#password.length > 0;
   }
   
   /**
    * @returns {Promise<Measurement[]>}
    */
   async getCurrentMeasurements() {
      await Utils.sleep(800);

      //if (Math.random() < 0.5) throw new Error("Random error.");

      let mockTemperature = Math.round(20 + (Math.random() * 6 - 3));
      let mockHumidity = Math.round(60 + (Math.random() * 6 - 3));
      let mockMoisture = Math.round(55 + (Math.random() * 6 - 3));
      let mockWaterFill = Math.round(70 + (Math.random() * 4 - 2));

      return [
         new Measurement("Temperature", mockTemperature + "°C", "17°C min.\n23°C max."),
         new Measurement("Humidity", mockHumidity + "%", "40% min.\n70% max."),
         new Measurement("Soil moisture", mockMoisture + "%", "48% min.\n80% max."),
         new Measurement("Water fill level", mockWaterFill + "%", "10% min.\n90% max.")
      ];
   }

   /**
    * @param {number} seconds 
    * @returns {Promise<void>}
    */
   async waterManually(seconds) {
      await Utils.sleep(1000);
      throw new Error("I don't want to water for " + seconds + " seconds.");
   }

   /**
    * @returns {Promise<void>}
    */
   async resetMeasurementStatistics() {
      await Utils.sleep(600);
   }

   /**
    * @returns {Promise<void>}
    */
   async testNotificationSound() {
      await Utils.sleep(700);
   }

   /**
    * @returns {Promise<void>}
    */
   async restartDevice() {
      await Utils.sleep(600);
   }

   /**
    * @param {ApplicationSettings} userSettings 
    */
   async updateConfiguration(userSettings) {
      await Utils.sleep(1000);
   }

   /**
    * @returns {Promise<ApplicationSettings>}
    */
   async getConfiguration() {
      await Utils.sleep(700);
      return new ApplicationSettings(
         new AutomaticWateringSettings(true, 0.50, 10, 1750),
         new MoistureWarningSettings(true, 0.45, 1),
         new TemperatureWarningSettings(false, 10, 28),
         new WaterFillLevelWarningSettings(false, 0.1, 1),
         new HardwareSettings(34, 12, 14, 22, 23, 18, 25, 26, 2, 17, "test"));
   }
}