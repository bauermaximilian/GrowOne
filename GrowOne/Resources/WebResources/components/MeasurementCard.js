/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { Measurement } from "../common/Measurement.js";
import { html, Component } from "../common/lib/preact.htm.module.js";

export class MeasurementCardProps {
   /** @type {Measurement} */
   measurement;
}

export class MeasurementCard extends Component {
   props = new MeasurementCardProps();

   render() {
      let twoColumnGridStyle = {
         "display": "grid",
         "grid-template-columns": "repeat(2, 1fr)",
      };

      let statisticsRows = this.props.measurement.statistics.split("\n");
           
      return html`
      <article style="margin-bottom:1rem;margin-top:0">
         <header>${this.props.measurement.label}</header>
         <div style=${twoColumnGridStyle}>
            <h1 style="margin:0">${this.props.measurement.currentValue}</h1>
            <div style="text-align:right">
               <sub style="top:0">
                  ${statisticsRows.map((row, index) => html`
                     ${index > 0 && html`<br/>`}
                     ${row}
                  `)}
               </sub>
            </div>
         </div>
      </article>
      `;
   }
}