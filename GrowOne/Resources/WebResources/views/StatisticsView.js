/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { html, Component } from "../common/lib/preact.htm.module.js";
import { App } from "../App.js";
import { Measurement } from "../common/Measurement.js";
import { MeasurementCard } from "../components/MeasurementCard.js";

export class StatisticsViewProps {
   /** @type {App} */
   appContext;
}

class StatisticsViewState {
   /** @type {Measurement[]} */
   measurements;
}

export class StatisticsView extends Component {
   props = new StatisticsViewProps();
   state = new StatisticsViewState();

   #unmounted = false;
   #refreshIntervalMs = 10000;

   get appContext() { return this.props.appContext; }

   setStateAsync = async (e) => new Promise((r, _) => this.setState(e, () => r(null)));

   componentDidMount() {
      this.#refreshMeasurements();
   }

   componentWillUnmount() {
      this.#unmounted = true;
   }

   #refreshMeasurements = async () => {
      let requestedLoadingOverlay = false;
      if (!this.state.measurements) {
         this.appContext.setLoadingOverlayVisibility(true);
         requestedLoadingOverlay = true;
      }

      try {
         let measurements = await this.appContext.apiClient.getCurrentMeasurements();
         if (!this.#unmounted) {
            await this.setStateAsync({measurements: measurements});
            setTimeout(this.#refreshMeasurements, this.#refreshIntervalMs);
         }
      } catch (error) {
         if (!this.#unmounted) {
            if (await this.appContext.showDialog("Error", 
               "The statistics data couldn't be retrieved.\n" + error, 
               ["cancel", "retry"]) === "retry") {
               await this.#refreshMeasurements();
            } else {
               this.appContext.logout();
            }
         }
      }

      if (requestedLoadingOverlay) {
         this.appContext.setLoadingOverlayVisibility(false);
      }
   }

   render() {
      let twoColumnGridStyle = {
         "display": "grid", 
         "grid-template-columns": "repeat(2, 1fr)",
         "grid-column-gap": "1rem",
         "text-align": "center"
      };

      return html`
      <div style=${twoColumnGridStyle}>
         ${this.state.measurements?.map(measurement => html`
         <${MeasurementCard} measurement=${measurement}/>
         `)}
      </div>
      `;
    }
}