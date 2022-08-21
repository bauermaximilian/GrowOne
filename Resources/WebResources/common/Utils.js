/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

export class Utils {
   /**
    * @param {number} interval 
    * @returns {Promise<void>}
    */
   static async sleep(interval) {
      await new Promise(r => setTimeout(r, interval));
   }
}