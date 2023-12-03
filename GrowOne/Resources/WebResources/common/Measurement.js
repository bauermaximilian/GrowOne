/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

export class Measurement {
   /** @type {string} */
   label;
   /** @type {string} */
   currentValue;
   /** @type {string} */
   statistics;

   /**
    * @param {string} label
    * @param {string} currentValue
    * @param {string} statistics
    */
   constructor(label, currentValue, statistics) {
      this.label = label;
      this.currentValue = currentValue;
      this.statistics = statistics;
   }
}