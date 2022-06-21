import { LitElement, html, nothing } from "lit";

/**
 *
 * @param {Page} page
 * @param {(page: Page) => void} setPage
 */
function renderContent(page = "Home", setPage) {
  switch (page) {
    case "Home":
      return html`<home-page></home-page>`;
    default:
      return html`
        <section>
          Not Found
          <sl-button @click=${() => setPage("Home")}
            >Go Home Perhaps?</sl-button
          >
        </section>
      `;
  }
}

class MyApp extends LitElement {
  static get properties() {
    return { page: { state: true, type: String } };
  }
  constructor() {
    super();
    /**
     * @type {Page}
     */
    this.page = "Home";
  }

  render() {
    return html`
      <article>
        ${renderContent(this.page, (page) => (this.page = page))}
      </article>
    `;
  }
}

//@ts-ignore
customElements.define("my-app", MyApp);
