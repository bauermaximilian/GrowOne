/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
// @ts-check

import { html, Component } from "../common/lib/preact.htm.module.js";

export class NavbarItem {
   /** @type{string} */
   id;
   /** @type{string} */
   label;
   /** @type{boolean} */
   isButton;

   /**
    * 
    * @param {string} id 
    * @param {string} label 
    * @param {boolean} isButton 
    */
   constructor(id, label, isButton = false) {
      this.id = id;
      this.label = label;
      this.isButton = isButton;
   }
}

export class NavbarProps {
   /** @type{string} */
   title;
   /** @type{NavbarItem[]} */
   items;
   /** @type{number} */
   activeItemIndex;
   /** @type{function(NavbarItem, number):void} */
   onItemClick;
}

class NavbarState {
   /** @type{boolean} */
   darkMode = true;
}

export class Navbar extends Component {
   props = new NavbarProps();
   state = new NavbarState();

   componentDidMount() {
      window.matchMedia('(prefers-color-scheme: dark)')?.addEventListener('change', this.#onDarkModeChanged);
      this.#onDarkModeChanged(null);
   }

   componentWillUnmount() {
      window.matchMedia('(prefers-color-scheme: dark)')?.removeEventListener('change', this.#onDarkModeChanged);
   }

   #onDarkModeChanged = async e => {
      this.setState({darkMode: window.matchMedia('(prefers-color-scheme: dark)')?.matches === true});
   };

   render() {
      let logoStyle = {
         height: "1.9rem"
      };
      if (!this.state.darkMode) {
         logoStyle["filter"] = "invert()";
      }

      return html`
      <nav class="container-fluid">
         <ul>
            <li><img src="/logo.svg" style=${logoStyle} alt="Logo"/></li>
            <!--<li><strong>${this.props.title}</strong></li>-->
         </ul>
         <ul>
            ${this.props.items?.map((value, index) => html`
            <li>
               <a href="javascript:;"
                  onclick=${() => this.props.onItemClick(value, index)}
                  role=${value.isButton ? "button" : "link"}
                  style=${index === this.props.activeItemIndex ? "font-weight:700" : ""}>
                  ${value.label}
               </a>
            </li>`)}
         </ul>
      </nav>
      `;
    }
}