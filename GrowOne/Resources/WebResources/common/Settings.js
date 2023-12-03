/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

export class Settings {
   clone() {
      return new Settings();
   }

   with(values) {
      let newInstance = this.clone();
      let fields = Object.keys(newInstance);
      for (let key of Object.keys(values)) {
         if (fields.indexOf(key) >= 0) {
            newInstance[key] = values[key];
         }
      }
      return newInstance;
   }
}