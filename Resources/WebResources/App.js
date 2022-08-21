/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { html, Component, render } from "./common/lib/preact.htm.module.js";
import { Modal } from "./components/Modal.js";
import { Navbar, NavbarItem } from "./components/Navbar.js";
import { ActionsView } from "./views/ActionsView.js";
import { LoginView } from "./views/LoginView.js";
import { SettingsView } from "./views/SettingsView.js";
import { StatisticsView } from "./views/StatisticsView.js";

class AppState {
    currentNavigationIndex = 0;
    apiClient;
    showLoadingOverlay = false;
}

export class App extends Component {   
   state = new AppState();

   #navbarItems = [
    new NavbarItem("stats", "Stats"),
    new NavbarItem("actions", "Actions"),
    new NavbarItem("settings", "Settings"),
    new NavbarItem("logout", "Logout", true)
   ];

   /** @type {Modal} */
   #referenceModal;

   get apiClient() { return this.state.apiClient; }

   setStateAsync = async (e) => new Promise((r, _) => this.setState(e, () => r(null)));

   componentDidMount() {
      window.addEventListener("hashchange", this.#onHashChange);
      this.#onHashChange(null);
   }

   componentWillUnmount() {
      window.removeEventListener("hashchange", this.#onHashChange);
   }

   #onHashChange = e => {
      let hash = window.location.hash;
      hash = hash.substring(Math.max(hash.indexOf("#") + 1, 0)).toLowerCase();
      let navigationIndex = 0;
      for (let i = 0; i < this.#navbarItems.length; i++) {
         if (hash === this.#navbarItems[i].id) {
            navigationIndex = i;
            break;
         }
      }

      if (this.state.currentNavigationIndex !== navigationIndex) {
         this.setState({currentNavigationIndex: navigationIndex});
      }
   };

   /**
    * @param {NavbarItem} value 
    * @param {number} index 
    */
   #onNavbarItemClick = (value, index) => {
      if (value.id !== "logout") {
         window.location.hash = value.id;
      } else {
         this.logout();
      }
   };

   /**
    * @param {string} title 
    * @param {string} message 
    * @param {import("./components/Modal.js").ModalButton[]} buttons 
    * @returns {Promise<import("./components/Modal.js").ModalButton?>}
    */
    showDialog = async (title, message, buttons) => {
      return await this.#referenceModal.show(title, message, buttons);
    };

    setLoadingOverlayVisibility = (isVisible) => {
      this.setState(state => {
         if (state.showLoadingOverlay !== isVisible) {
            return {showLoadingOverlay: isVisible};
         }
      });
    };

    logout = async () => {
      this.setStateAsync({apiClient: null});
    };

    login = async (apiClient) => {
      await this.setStateAsync({apiClient: apiClient});
    }

   render() {
      // Passwort verschlüsselt "Keyfile", das - mit GUID der Ressource - SHA256-gehasht wird, 
      // Ergebnis ist AES-Passwort für Ressource
      // -> Rückrechnen von "Hash-Passwort" auf Keyfile deutlich schwerer als rückrechnen auf Passwort
      // -> Selbst wenn Keyfile gehackt wird, ist das Passwort trotzdem nicht geleakt
      // -> Keyfile auf Server? 
      // -> Keyfile ohne Dateistruktur: Damit weiß man nicht, ob man erfolgreich entschlüsselt hat :D
      // -> Backup als QR-Code

      let navigationIndex = this.state.currentNavigationIndex;

      let loginStyle = {
         "display": "flex",
         "flex-direction": "column",
         "justify-content": "center",
         "min-height": "100vh"
      };

      let overlayStyle = {
         "position" : "fixed",
         "z-index": "998",
         "background-color": "var(--background-color)",
         "opacity": "75%",
         "width": "100vw",
         "line-height": "100vh"
      };
      if (!this.state.showLoadingOverlay) {
         overlayStyle["display"] = "none";
      }

      if (this.state.apiClient) {
         return html`
         <div style=${overlayStyle} aria-busy=true></div>         
         <${Modal} ref=${r => this.#referenceModal = r}/>
         <${Navbar} items=${this.#navbarItems}
                    onItemClick=${this.#onNavbarItemClick}
                    activeItemIndex=${navigationIndex}
                    title="GrowOne"/>
         <main class="container">            
            ${navigationIndex === 0 && html`<${StatisticsView} appContext=${this}/>`}
            ${navigationIndex === 1 && html`<${ActionsView} appContext=${this}/>`}
            ${navigationIndex === 2 && html`<${SettingsView} appContext=${this}/>`}
         </main>
         `;
      } else {
         return html `
         <${Modal} ref=${r => this.#referenceModal = r}/>
         <main class="container" style=${loginStyle}>            
            <${LoginView} appContext=${this}/>
         </main>
         `;
      }
   }
}

document.addEventListener('DOMContentLoaded', 
   () => render(html`<${App}/>`, document.body));