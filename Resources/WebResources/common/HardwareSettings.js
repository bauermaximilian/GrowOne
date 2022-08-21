/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { Settings } from "./Settings.js";

export class HardwareSettings extends Settings {
   /** @type {number} */
   moistureSensorPin;
   /** @type {number} */
   dht22EchoPin;
   /** @type {number} */
   dht22TriggerPin;
   /** @type {number} */
   irrigatorSwitchPin;
   /** @type {number} */
   sensorSwitchPin;
   /** @type {number} */
   buzzerPin;
   /** @type {number} */
   hcsr04EchoPin;
   /** @type {number} */
   hcsr04TriggerPin;
   /** @type {number} */
   hcsr04MinimumValueCm;
   /** @type {number} */
   hcsr04MaximumValueCm;   
   /** @type {string} */
   password; 

   constructor(moistureSensorPin, dht22EchoPin, dht22TriggerPin, irrigatorSwitchPin, sensorSwitchPin, 
      buzzerPin, hcsr04EchoPin, hcsr04TriggerPin, hcsr04MinimumValueCm, hcsr04MaximumValueCm, password) {
      super();
      this.moistureSensorPin = moistureSensorPin;
      this.dht22EchoPin = dht22EchoPin;
      this.dht22TriggerPin = dht22TriggerPin;
      this.irrigatorSwitchPin = irrigatorSwitchPin;
      this.sensorSwitchPin = sensorSwitchPin;
      this.buzzerPin = buzzerPin;
      this.hcsr04EchoPin = hcsr04EchoPin;
      this.hcsr04TriggerPin = hcsr04TriggerPin;
      this.hcsr04MinimumValueCm = hcsr04MinimumValueCm;
      this.hcsr04MaximumValueCm = hcsr04MaximumValueCm;
      this.password = password;
   }

   clone() {
      return new HardwareSettings(this.moistureSensorPin, this.dht22EchoPin,
         this.dht22TriggerPin, this.irrigatorSwitchPin, this.sensorSwitchPin, 
         this.buzzerPin, this.hcsr04EchoPin, this.hcsr04TriggerPin,
         this.hcsr04MinimumValueCm, this.hcsr04MaximumValueCm, this.password);
   }
}