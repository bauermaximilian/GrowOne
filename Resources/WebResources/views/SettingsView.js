/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { App } from "../App.js";
import { html, Component } from "../common/lib/preact.htm.module.js";
import { ApplicationSettings } from "../common/ApplicationSettings.js";
import { AutomaticWateringSettings } from "../common/AutomaticWateringSettings.js";
import { MoistureWarningSettings } from "../common/MoistureWarningSettings.js";
import { TemperatureWarningSettings } from "../common/TemperatureWarningSettings.js";
import { HardwareSettings } from "../common/HardwareSettings.js";
import { WaterFillLevelWarningSettings } from "../common/WaterFillLevelWarningSettings.js";

export class SettingsViewProps {
   /** @type {App} */
   appContext;
}

class SettingsViewState {
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
   /** @type {boolean} */
   unsavedChanges = false;
   /** @type {boolean} */
   isBusy = false;
}

export class SettingsView extends Component {
   props = new SettingsViewProps();
   state = new SettingsViewState();

   #settingsLoading = false;

   get appContext() { return this.props.appContext; }

   setStateAsync = async (e) => new Promise((r, _) => this.setState(e, () => r(null)));

   componentDidMount() {
      this.#pullSettingsFromServer();
   }

   #pullSettingsFromServer = async () => {
      this.#settingsLoading = true;
      this.appContext.setLoadingOverlayVisibility(true);
      try {
         let settings = await this.appContext.apiClient.getConfiguration();
         await this.setStateAsync({
            automaticWateringSettings: settings.automaticWateringSettings,
            moistureWarningSettings: settings.moistureWarningSettings,
            temperatureWarningSettings: settings.temperatureWarningSettings,
            waterFillLevelWarningSettings: settings.waterFillLevelWarningSettings,
            hardwareSettings: settings.hardwareSettings
         });
      } catch (error) {
         if (await this.appContext.showDialog("Error", 
            "The current settings couldn't be retrieved.\n" + error, ["cancel", "retry"]) === "retry") {
               await this.#pullSettingsFromServer();
         } else {
            this.appContext.logout();
         }
      }

      this.#settingsLoading = false;
      this.appContext.setLoadingOverlayVisibility(false);
   };

   #setParameter = async (event, value) => {    
      let typedValue;
      if (event.target.type === 'number') {
         typedValue = parseFloat(value);
      } else {
         typedValue = value;
      }

      let nameSegments = event.target.name.split(".");
      let updateStateObject = { unsavedChanges: true };
      
      if (nameSegments.length > 1) {
         let targetConfiguration = this.state[nameSegments[0]];
         let updateConfigurationObject = {};
         updateConfigurationObject[nameSegments[1]] = typedValue;         
         updateStateObject[nameSegments[0]] = targetConfiguration.with(updateConfigurationObject);
         this.setState(updateStateObject);
      } else {
         updateStateObject[nameSegments[0]] = typedValue;
         this.setState(updateStateObject);
      }
   }

   #onSaveSettingsButtonClick = async(event) => {
      event.preventDefault();

      if (this.#settingsLoading || this.state.isBusy) return;
      await this.setStateAsync({isBusy: true});
      
      try {
         await this.appContext.apiClient.updateConfiguration(
            new ApplicationSettings(this.state.automaticWateringSettings,
               this.state.moistureWarningSettings,
               this.state.temperatureWarningSettings,
               this.state.waterFillLevelWarningSettings,
               this.state.hardwareSettings));
         await this.setStateAsync({unsavedChanges: false});
         if ("yes" == await this.appContext.showDialog("Settings saved",
            "The settings were successfully saved.\n" + 
            "The device needs to be restarted for the changes to take effect.\nDo you want to restart now?", ["yes", "no"])) {
               this.appContext.setLoadingOverlayVisibility(true);
               await this.appContext.apiClient.restartDevice();
               this.appContext.setLoadingOverlayVisibility(false);
               this.appContext.logout();
            }
      } catch (error) {
         if (await this.appContext.showDialog("Error",
            "The settings couldn't be saved.\n" + error,
            ["cancel", "retry"]) === "retry") {
            await this.#onSaveSettingsButtonClick();
         }
      }

      await this.setStateAsync({isBusy: false});
   };

   render() {
      return html`
      <form>
         <fieldset disabled=${!this.state.automaticWateringSettings}>
            <label>
               <input type="checkbox" 
                      role="switch"
                      name="automaticWateringSettings.enabled"
                      checked=${this.state.automaticWateringSettings?.enabled}
                      onchange=${e => this.#setParameter(e, e.target.checked)}/>
                  Enable automatic watering
            </label>
            <div class="grid">            
               <label>
                  Minimum moisture (%):
                  <input type="number"
                         min="0" max="100"
                         name="automaticWateringSettings.minimumMoisture"
                         disabled=${!this.state.automaticWateringSettings?.enabled}
                         value=${Math.round(this.state.automaticWateringSettings?.minimumMoisture * 100)}
                         onchange=${e => this.#setParameter(e, parseInt(e.target.value) / 100.0)}/>
               </label>
               <label>
                  Duration (seconds):
                  <input type="number" 
                         min="0" max="60"
                         name="automaticWateringSettings.durationSeconds"
                         disabled=${!this.state.automaticWateringSettings?.enabled}
                         value=${this.state.automaticWateringSettings?.durationSeconds}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
               <label>
                  Cooldown (seconds):
                  <input type="number"
                         min="0" max="86400"
                         name="automaticWateringSettings.cooldownSeconds"
                         disabled=${!this.state.automaticWateringSettings?.enabled}
                         value=${this.state.automaticWateringSettings?.cooldownSeconds}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
            </div>
         </fieldset>
         
         <fieldset disabled=${!this.state.moistureWarningSettings}>
            <label>
               <input type="checkbox" 
                      role="switch"
                      name="moistureWarningSettings.enabled"
                      checked=${this.state.moistureWarningSettings?.enabled}
                      onchange=${e => this.#setParameter(e, e.target.checked)}/>
               Enable acoustic moisture warning
            </label>
            <div class="grid">
               <label>
                  Minimum moisture (%):
                  <input type="number"
                         min="0" max="100"
                         name="moistureWarningSettings.minimumMoisture"
                         disabled=${!this.state.moistureWarningSettings?.enabled}
                         value=${Math.round(this.state.moistureWarningSettings?.minimumMoisture * 100)}
                         oninput=${e => this.#setParameter(e, parseInt(e.target.value) / 100)}/>
               </label>
               <label>
                  Maximum moisture (%):
                  <input type="number"
                         min="0" max="100"
                         name="moistureWarningSettings.maximumMoisture"
                         disabled=${!this.state.moistureWarningSettings?.enabled}
                         value=${Math.round(this.state.moistureWarningSettings?.maximumMoisture * 100)}
                         oninput=${e => this.#setParameter(e, parseInt(e.target.value) / 100)}/>
               </label>
            </div>
         </fieldset>
         
         <fieldset disabled=${!this.state.temperatureWarningSettings}>
            <label>
               <input type="checkbox"
                      role="switch"
                      name="temperatureWarningSettings.enabled"
                      checked=${this.state.temperatureWarningSettings?.enabled}
                      onchange=${e => this.#setParameter(e, e.target.checked)}/>
               Enable acoustic temperature warning
            </label>
            <div class="grid">            
               <label>
                  Minimum temperature (°C):
                  <input type="number"
                         min="0" max="100"
                         name="temperatureWarningSettings.minimumTemperature"
                         disabled=${!this.state.temperatureWarningSettings?.enabled}
                         value=${this.state.temperatureWarningSettings?.minimumTemperature}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
               <label>
                  Maximum temperature (°C):
                  <input type="number"
                         min="0" max="100"
                         name="temperatureWarningSettings.maximumTemperature"
                         disabled=${!this.state.temperatureWarningSettings?.enabled}
                         value=${this.state.temperatureWarningSettings?.maximumTemperature}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
            </div>
         </fieldset>

         <fieldset disabled=${!this.state.waterFillLevelWarningSettings}>
            <label>
               <input type="checkbox"
                      role="switch"
                      name="waterFillLevelWarningSettings.enabled"
                      checked=${this.state.waterFillLevelWarningSettings?.enabled}
                      onchange=${e => this.#setParameter(e, e.target.checked)}/>
               Enable acoustic water fill level warning
            </label>
            <div class="grid">            
               <label>
                  Minimum water fill level (%):
                  <input type="number"
                         min="0" max="100"
                         name="waterFillLevelWarningSettings.minimumLevel"
                         disabled=${!this.state.waterFillLevelWarningSettings?.enabled}
                         value=${Math.round(this.state.waterFillLevelWarningSettings?.minimumLevel * 100)}
                         oninput=${e => this.#setParameter(e, parseInt(e.target.value) / 100)}/>
               </label>
               <label>
                  Maximum water fill level (%):
                  <input type="number"
                         min="0" max="100"
                         name="waterFillLevelWarningSettings.maximumLevel"
                         disabled=${!this.state.waterFillLevelWarningSettings?.enabled}
                         value=${Math.round(this.state.waterFillLevelWarningSettings?.maximumLevel * 100)}
                         oninput=${e => this.#setParameter(e, parseInt(e.target.value) / 100)}/>
               </label>
            </div>
         </fieldset>

         <fieldset disabled=${!this.state.hardwareSettings}>
            <div class="grid">               
               <label>
                  Buzzer pin
                  <input type="number"
                         min="1" max="39"
                         name="hardwareSettings.buzzerPin"
                         value=${this.state.hardwareSettings?.buzzerPin}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>                             
               <label>
                  <em data-tooltip="Use an ADC-compatible pin from 32 to 39.">Moisture sensor pin</em>
                  <input type="number"
                         min="1" max="39"
                         name="hardwareSettings.moistureSensorPin"
                         value=${this.state.hardwareSettings?.moistureSensorPin}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
            </div>

            <div class="grid">
               <label>
                  Irrigation switch pin
                  <input type="number"
                         min="1" max="39"
                         name="hardwareSettings.irrigatorSwitchPin"
                         value=${this.state.hardwareSettings?.irrigatorSwitchPin}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
               <label>
                  Sensor switch pin
                  <input type="number"
                         min="1" max="39"
                         name="hardwareSettings.sensorSwitchPin"
                         value=${this.state.hardwareSettings?.sensorSwitchPin}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
            </div>

            <div class="grid">
               <label>
                  HC-SR04 echo pin
                  <input type="number"
                         min="1" max="39"
                         name="hardwareSettings.hcsr04EchoPin"
                         value=${this.state.hardwareSettings?.hcsr04EchoPin}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
               <label>
               HC-SR04 trigger pin
                  <input type="number"
                         min="1" max="39"
                         name="hardwareSettings.hcsr04TriggerPin"
                         value=${this.state.hardwareSettings?.hcsr04TriggerPin}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
            </div>

            <div class="grid">
               <label>
                  DHT22 echo pin
                  <input type="number"
                         min="1" max="39"
                         name="hardwareSettings.dht22EchoPin"
                         value=${this.state.hardwareSettings?.dht22EchoPin}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
               <label>
                  DHT 22 trigger pin
                  <input type="number"
                         min="1" max="39"
                         name="hardwareSettings.dht22TriggerPin"
                         value=${this.state.hardwareSettings?.dht22TriggerPin}
                         oninput=${e => this.#setParameter(e, e.target.value)}/>
               </label>
            </div>
         </fieldset>

         <fieldset>
            <label>
               Device password
               <input type="password"
                      name="hardwareSettings.password"
                      value=${this.state.hardwareSettings?.password}
                      oninput=${e => this.#setParameter(e, e.target.value)}/>
            </label>
         </fieldset>

         <button onclick=${this.#onSaveSettingsButtonClick} 
                 disabled=${!this.state.unsavedChanges}
                 aria-busy=${this.state.isBusy}>
            Save settings
         </button>
      </form>
      `;
   }
}