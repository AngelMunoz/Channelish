export class AuthController {
  constructor(host) {
    this.host = host;
    host.addController();
    this.authenticated = false;
  }

  /**
   *
   * @param {string} email
   * @param {string} password
   */
  async signin(email, password) {
    const req = await fetch("/api/auth/signin", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
    if (!req.ok) throw new Error(`${req.status} - ${req.statusText}`);
    const response = await req.json();
    if (response.token) {
      sessionStorage.setItem("autn_token", response.token);
      this.authenticated = true;
    }
    this.host.requestUpdate();
  }

  /**
   *
   * @param {string} name
   * @param {string} email
   * @param {string} password
   */
  async signup(name, email, password) {
    const req = await fetch("/api/auth/signup", {
      method: "POST",
      body: JSON.stringify({ name, email, password }),
    });
    if (!req.ok) throw new Error(`${req.status} - ${req.statusText}`);
    const response = await req.json();
    if (response.token) {
      sessionStorage.setItem("auth_token", response.token);
      this.authenticated = true;
    }
    this.host.requestUpdate();
  }
}
