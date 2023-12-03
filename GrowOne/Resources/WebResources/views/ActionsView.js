/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { App } from "../App.js";
import { html, Component } from "../common/lib/preact.htm.module.js";

export class ActionsViewProps {
   /** @type {App} */
   appContext;
}

class ActionsViewState {
   waterManuallyBusy = false;
   resetStatisticsBusy = false;
   testNotificationSoundBusy = false;
   resetDeviceBusy = false;
   waterManuallyDuration = 1;
}

export class ActionsView extends Component {
   props = new ActionsViewProps();
   state = new ActionsViewState();

   get appContext() { return this.props.appContext; }

   setStateAsync = async (e) => new Promise((r, _) => this.setState(e, () => r(null)));

   #onWaterManually = async () => {
      if (this.state.waterManuallyBusy) return;

      await this.setStateAsync({ waterManuallyBusy: true });
      try {
         await this.props.appContext.apiClient.waterManually(
            this.state.waterManuallyDuration);
      } catch (error) {
         await this.#showError(error);
      }
      await this.setStateAsync({ waterManuallyBusy: false });
   };

   #onResetStatistics = async () => {
      if (this.state.resetStatisticsBusy) return;

      if (await this.appContext.showDialog("Confirmation",
         "Do you really want to reset the statistics and the current minimum and maximum values?",
         ["no", "yes"]) === "yes") {
         await this.setStateAsync({ resetStatisticsBusy: true });
         try {
            await this.appContext.apiClient.resetMeasurementStatistics();
         } catch (error) {
            await this.#showError(error);
         }

         await this.setStateAsync({ resetStatisticsBusy: false });
      }
   };

   #onTestNotificationSound = async () => {
      if (this.state.testNotificationSoundBusy) return;

      await this.setStateAsync({testNotificationSoundBusy: true});
      try {
         await this.appContext.apiClient.testNotificationSound();
      } catch (error) {
         this.#showError(error);
      }
      await this.setStateAsync({testNotificationSoundBusy: false});
   };

   #onResetDevice = async () => {
      if (this.state.resetDeviceBusy) return;

      await this.setStateAsync({resetDeviceBusy: true});
      if (await this.appContext.showDialog("Confirmation",
         "Do you really want to restart the device?\nThis will also reset the statistics and log you out.",
         ["no", "yes"]) === "yes") {
         try {
            await this.appContext.apiClient.restartDevice();
         } catch (error) {
            this.#showError(error);
         }
         this.appContext.logout();
      }
      await this.setStateAsync({resetDeviceBusy: false});
   };

   #showError = async (error) => {
      await this.props.appContext.showDialog("Error", 
         "The specified request couldn't be executed.\n" + error, ["ok"]);
   };

   render() {
      return html`         
         <fieldset>
            <h6>Manual watering</h6>
            <label>
               Watering duration (seconds)
               <input type="number"
                      min="1" max="60"
                      value=${this.state.waterManuallyDuration}
                      oninput=${async e => await this.setStateAsync({waterManuallyDuration: parseInt(e.target.value)})}/>
            </label>
            <button onclick=${this.#onWaterManually}
                    aria-busy=${this.state.waterManuallyBusy}>
                    Water for specified amount of time
            </button>
         </fieldset>

         <fieldset>
            <h6>Statistics</h6>
            <button onclick=${this.#onResetStatistics}
                    aria-busy=${this.state.resetStatisticsBusy}>
                    Reset statistics
            </button>
         </fieldset>

         <fieldset>
            <h6>Device</h6>
            <button onclick=${this.#onTestNotificationSound}
                    aria-busy=${this.state.testNotificationSoundBusy}>
                    Test notification sound
            </button>
            <button onclick=${this.#onResetDevice}
                    aria-busy=${this.state.resetDeviceBusy}>
               Restart device
            </button>
         </fieldset>
      `;
   }
}