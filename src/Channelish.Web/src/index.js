import "./index.css";
//@ts-ignore
import { LitElement, html, nothing } from "lit";
import { SocketController } from "./controllers.js";
/**
 * @type {(ctrl: SocketController) => any} ctrl
 */
const atLeastOneTpl = (ctrl) => {
  return html`<button
    @click=${() =>
      ctrl.sendMessageTo("general", "status-change", { message: "olv" })}
  >
    Send Status Change
  </button>`;
};

class MyApp extends LitElement {
  constructor() {
    super();
    this.socketCtrl = new SocketController(this);
  }

  render() {
    return html`
      <article>
        <h1>Hello, World!</h1>
        <button @click=${() => this.socketCtrl.connectTo()}>
          Connecto to General
        </button>
        ${this.socketCtrl.atLeastOne ? atLeastOneTpl(this.socketCtrl) : nothing}
      </article>
    `;
  }
}

//@ts-ignore
customElements.define("my-app", MyApp);
