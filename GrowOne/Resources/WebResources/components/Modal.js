/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { Component, html } from "../common/lib/preact.htm.module.js";

/**
 * @typedef {("ok"|"cancel"|"yes"|"no"|"retry"|"ignore")} ModalButton
 */

class ModalState {
   /** @type {string} */
   title;
   /** @type {string} */
   message;
   /** @type {ModalButton[]} */
   buttons;
   /** @type {boolean} */
   isShown = false;
}

export class Modal extends Component {
   state = new ModalState();

   #currentResolve;
   #currentReject;

   /**
    * @param {string} title 
    * @param {string} message 
    * @param {ModalButton[]} buttons 
    * @returns {Promise<ModalButton?>}
    */
   async show(title, message, buttons) {
      this.#currentReject?.call(this, new Error("The modal was replaced by another one."));
      this.#currentResolve = null;
      // this.#currentReject = null;

      this.setState({
         title: title,
         message: message,
         buttons: buttons,
         isShown: true
      });

      return new Promise((resolve, reject) => {
         this.#currentResolve = resolve;
         // this.#currentReject = reject;
      });
   }

   #closeModal = (button) => {
      this.#currentResolve?.call(this, button);
      this.#currentResolve = null;
      // this.#currentReject = null;
      this.setState({isShown: false});
   };

   componentWillUnmount() {
      this.#currentResolve?.call(this, undefined);
      // this.#currentReject?.call(this, new Error("The modal was unmounted."));
      this.#currentResolve = null;
      // this.#currentReject = null;
   }

   render() {
      return html`
         <dialog open=${this.state.isShown}>
            <article>
               <header>
                  <a href="javascript:;" 
                     aria-label="Close"
                     class="close"
                     onclick=${() => this.#closeModal(null)}></a>
                  <h3 style="margin-bottom:0">${this.state.title}</h3>
               </header>
               <p>
                  ${this.state.message?.split("\n").map(row => html`${row}<br/>`)}
               </p>
               <footer>
                  ${this.state.buttons?.map(button => this.#renderButton(button))}
               </footer>
            </article>
         </dialog>
         `;
   }

   /**
    * @param {ModalButton} modalButton 
    */
   #renderButton(modalButton) {
      let className;
      let label;

      switch (modalButton) {
         case "ok": label = "OK"; break;
         case "cancel": label = "Cancel"; className = "secondary"; break;
         case "yes": label = "Yes"; break;
         case "no": label = "No"; className = "secondary"; break;
         case "ignore": label = "Ignore"; className = "secondary"; break;
         case "retry": label = "Retry"; break;
      };

      return html`
      <a href="javascript:;" 
         role="button" 
         class=${className}
         onclick=${() => this.#closeModal(modalButton)}>
         ${label}
      </a>`;
   }
}