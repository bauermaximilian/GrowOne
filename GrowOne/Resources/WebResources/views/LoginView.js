/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { App } from "../App.js";
import { ApiClient, ApiClientMock } from "../common/ApiClient.js";
import { html, Component } from "../common/lib/preact.htm.module.js";

export class LoginViewProps {
   /** @type {App} */
   appContext;
}

class LoginViewState {
   isBusy = false;
   lastLoginAttemptFailed = false;
   darkMode = true;
}

export class LoginView extends Component {
   props = new LoginViewProps();
   state = new LoginViewState();

   /** @type{HTMLInputElement} */
   #referencePasswordInput;

   get appContext() { return this.props.appContext; }

   componentDidMount() {
      window.matchMedia('(prefers-color-scheme: dark)')?.addEventListener('change', this.#onDarkModeChanged);
      this.#onDarkModeChanged(null);
   }

   componentWillUnmount() {
      window.matchMedia('(prefers-color-scheme: dark)')?.removeEventListener('change', this.#onDarkModeChanged);
   }

   #onDarkModeChanged = async e => {
      await this.setStateAsync({darkMode: window.matchMedia('(prefers-color-scheme: dark)')?.matches === true});
   };

   setStateAsync = async (e) => new Promise((r, _) => this.setState(e, () => r(null)));

   /**
    * 
    * @param {MouseEvent} e 
    * @returns 
    */
   #onLoginClick = async e => {
      e.preventDefault();
      if (this.state.isBusy) return;

      await this.setStateAsync({isBusy: true});

      try {
         let apiClient;
         if (e.altKey) {
            apiClient = await ApiClientMock.create(this.#referencePasswordInput.value);
         }
         else {
            apiClient = await ApiClient.create(this.#referencePasswordInput.value);
         }
         this.appContext.login(apiClient);
         await this.setStateAsync({isBusy: false, lastLoginAttemptFailed: false });            
      } catch (error) {
         this.#referencePasswordInput.value = "";
         await this.setStateAsync({isBusy: false, lastLoginAttemptFailed: true });
      }
   };

   #onPasswordChange = async () => {
      if (this.state.lastLoginAttemptFailed && !this.state.isBusy) {
         await this.setStateAsync({ lastLoginAttemptFailed: false });
      }
   }

   render() {
      let logoStyle = {
         height: "100px"
      };
      if (!this.state.darkMode) {
         logoStyle["filter"] = "invert()";
      }

      return html`
      <article class="grid">
         <div>
            <hgroup style="text-align:center;">
               <img src="/logo.svg" alt="Logo" id="logo" style=${logoStyle}/>
            </hgroup>
            <hgroup style="text-align:center">
               <h1>GrowOne</h1>
               <h2>Please authenticate yourself.</h2>
            </hgroup>
            <form>
               <input type="password" 
                      oninput=${this.#onPasswordChange}
                      aria-invalid=${this.state.lastLoginAttemptFailed ? true : null } 
                      ref=${r => this.#referencePasswordInput = r}/>
               <button onclick=${this.#onLoginClick} 
                       aria-busy=${this.state.isBusy}>
                       Login
               </button>
            </form>
         </div>
      </article>
      `;
   }
}